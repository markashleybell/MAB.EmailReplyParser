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

    let isEmpty ln = ln = ""

    let private replaceNewLinesInQuoteHeader (s: string) =
        multiLineQuoteHeaderRx.Replace(s, (fun m -> m.Value.Replace("\n", " ") |> String.replace @" {2,}" " "))

    let private addSpaceBeforeLineSeparator (s: string) = 
        lineSeparatorRx.Replace(s, (fun m -> m.Value + "\n"), 1)

    let private classify i ln =
        let t = match (ln |> isPartOfQuote) with
                | true -> Quoted
                | false -> match (ln |> isEmpty) with
                           | true -> Empty
                           | false -> match (ln |> isSignatureDelimiter) with 
                                      | true -> SignatureDelimiter
                                      | false -> Content
        { Index=i; Visibility=Visible; Type=t; Content=ln }

    let private contentAfter idx lines =
       lines 
       |> List.skip idx 
       |> List.exists (fun ln -> ln.Type = Content)

    let private setLineVisibility lines out ln =
        let v = match ln.Type with
                | Quoted -> match (lines |> contentAfter ln.Index) with 
                            | true -> Visible
                            | false -> Hidden
                | _ -> Visible
        { ln with Visibility=v }::out

    let getLinesPlainText emailBody = 
        emailBody 
        |> String.replace "\r\n" "\n" 
        |> replaceNewLinesInQuoteHeader
        |> addSpaceBeforeLineSeparator
        |> String.split '\n'

    let getLines emailBody =
        let classified = 
            emailBody
            |> getLinesPlainText 
            |> Array.toList
            |> List.mapi classify
            |> List.takeWhile (fun ln -> ln.Type <> SignatureDelimiter)

        let setLineVisibility' = setLineVisibility classified
        
        classified 
        |> List.fold setLineVisibility' []
        |> List.rev
        |> List.toArray

    let getReply emailBody = 
        emailBody
        |> getLines
        |> Array.filter (fun ln -> ln.Visibility = Visible)
        |> Array.map (fun ln -> ln.Content)
        |> String.concat "\n"
        |> String.trim
