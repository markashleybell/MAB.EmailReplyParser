namespace MAB.EmailReplyParser

module String =
    open System.Text.RegularExpressions

    let replace pat (sub: string) s = Regex.Replace(s, pat, sub)

    let split c (s: string) = s.Split [|c|]

    let trim (s: string) = s.Trim()

    let trimEnd (s: string) = s.TrimEnd()

    let trimStart (s: string) = s.TrimStart()
