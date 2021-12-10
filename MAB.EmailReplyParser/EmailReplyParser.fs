namespace MAB.EmailReplyParser

module EmailReplyParser =
    open System.Text.RegularExpressions

    let multiLineQuoteHeaderRx = new Regex(@"(?!On.*On\s.+?wrote:)(On\s(.+?)wrote:)", RegexOptions.Singleline)

    let gmailQuoteHeaderRx = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2} GMT.?\d{2}:\d{2} .*:)$", RegexOptions.Singleline)

    let lineSeparatorRx = new Regex(@"([^\n])((?=\n_{7}_+)|(?=\n-{7}-+))$")

    let signatureDelimiterRx = new Regex(@"(?m)(--\s*$|__\s*$|—\s*$|\w-$)|(^Sent from my (\w+\s*){1,3}$)|(From:.*)")

    let quoteHeaderRx = new Regex(@"^On.*wrote:$")

    let quoteRx = new Regex(@"^>+")

    let isQuoteHeader s = quoteHeaderRx.IsMatch s

    let isSignatureDelimiter s = signatureDelimiterRx.IsMatch s

    let isQuoted s = quoteRx.IsMatch s

    let isPartOfQuote s = s |> isQuoted || s |> isQuoteHeader

    let isNotPartOfQuote s = not (s |> isPartOfQuote)

    let isNonQuotedSignatureDelimiter ln = ln.Content |> isSignatureDelimiter && ln.Content |> isNotPartOfQuote

    let isNonEmptyContent ln = ln.Type = Content && ln.Content <> ""

    let private replaceNewLinesInQuoteHeader (s: string) =
        multiLineQuoteHeaderRx.Replace(s, (fun m -> m.Value.Replace("\n", " ") |> String.replace @" {2,}" " "))

    let private addSpaceBeforeLineSeparator (s: string) =
        lineSeparatorRx.Replace(s, (fun m -> m.Value + "\n"), 1)

    let private signatureBefore idx lines =
        lines |> List.take idx |> List.exists isNonQuotedSignatureDelimiter

    let private nonEmptyContentAfter idx lines =
        lines |> List.skip idx |> List.exists isNonEmptyContent

    let private toLines i ln =
        { Index=i; Visibility=Visible; Type=Content; Content=ln }

    let private classify lines ln =
        let typ = match (ln.Content |> isPartOfQuote) with
                  | true -> Quoted
                  | false -> match (ln.Content |> isSignatureDelimiter || lines |> signatureBefore ln.Index) with
                             | true -> Signature
                             | false -> Content
        { ln with Type=typ; }

    let private setVisible lines ln =
        let vis = match ln.Type with
                  | Quoted -> match (lines |> nonEmptyContentAfter ln.Index) with
                              | true -> Visible
                              | false -> Hidden
                  | Signature -> Hidden
                  | Content -> Visible
        { ln with Visibility=vis; }

    let getLinesPlainText messageBody =
        messageBody
        |> String.replace "\r\n" "\n"
        |> replaceNewLinesInQuoteHeader
        |> addSpaceBeforeLineSeparator
        |> String.split '\n'

    let getLines messageBody =
        let lines =
            messageBody
            |> getLinesPlainText
            |> Array.toList
            |> List.mapi toLines

        let classified = lines |> List.map (classify lines)

        let setVisible' = setVisible classified

        classified
        |> List.map setVisible'
        |> List.toArray

    let getReply messageBody =
        messageBody
        |> getLines
        |> Array.filter (fun ln -> ln.Visibility = Visible)
        |> Array.map (fun ln -> ln.Content)
        |> String.concat "\n"
        |> String.trim
