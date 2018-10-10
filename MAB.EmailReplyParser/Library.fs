namespace MAB.EmailReplyParser

module EmailReplyParser =
    open System.Text.RegularExpressions

    let multiLineQuoteHeaderRx = new Regex(@"(?!On.*On\s.+?wrote:)(On\s(.+?)wrote:)", RegexOptions.Singleline)

    let gmailQuoteHeaderRx = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2} GMT.?\d{2}:\d{2} .*:)$", RegexOptions.Singleline)

    let lineSeparatorRx = new Regex(@"([^\n])((?=\n_{7}_+)|(?=\n-{7}-+))$")
    
    let signatureDelimiterRx = new Regex(@"(?m)(--\s*$|__\s*$|—\s*$|\w-$)|(^Sent from my (\w+\s*){1,3}$)|(From:.*)")
    
    let quoteHeaderRx = new Regex(@"^On.*wrote:$")
    
    let quoteRx = new Regex(@"^>+")
    
    let replaceNewLinesInQuoteHeader (s: string) =
        multiLineQuoteHeaderRx.Replace(s, (fun m -> m.Value.Replace("\n", "")))
    
    let addSpaceBeforeLineSeparator (s: string) = 
        lineSeparatorRx.Replace(s, (fun m -> m.Value + "\n"), 1)

    let isQuoteHeader s = quoteHeaderRx.IsMatch s

    let isSignatureDelimiter s = signatureDelimiterRx.IsMatch s
    
    let isQuoted s = quoteRx.IsMatch s

    let isPartOfQuote s = s |> isQuoted || s |> isQuoteHeader

    let isEmpty ln = ln = ""

    let classifyLine i ln =
        let t = match (ln |> isPartOfQuote) with
                | true -> Quoted
                | false -> match (ln |> isEmpty) with
                           | true -> Empty
                           | false -> match (ln |> isSignatureDelimiter) with 
                                      | true -> SignatureDelimiter
                                      | false -> Content
        { Index=i; Visibility=Visible; Type=t; Content=ln }

    let contentAfter idx lines =
       lines 
       |> List.skip idx 
       |> List.exists (fun ln -> ln.Type = Content)

    let setLineVisibility lines out ln =
        let v = match ln.Type with
                | Quoted -> match (lines |> contentAfter ln.Index) with 
                            | true -> Visible
                            | false -> Hidden
                | _ -> Visible
        { ln with Visibility=v }::out

    let getLines emailBody = 
        emailBody 
        |> String.replace "\r\n" "\n" 
        |> replaceNewLinesInQuoteHeader
        |> addSpaceBeforeLineSeparator
        |> String.split '\n'
        |> Array.toList

    let getVisibleLines emailBody =
        let classified = 
            emailBody
            |> getLines 
            |> List.mapi classifyLine 
            |> List.takeWhile (fun ln -> ln.Type <> SignatureDelimiter)

        let setLineVisibility' = setLineVisibility classified
        
        classified 
        |> List.fold setLineVisibility' [] 
        |> List.filter (fun ln -> ln.Visibility = Visible)
        |> List.rev

    let parse emailBody = 
        emailBody
        |> getVisibleLines
        |> List.map (fun ln -> ln.Content)
        |> String.concat "\n"
        |> String.trim
