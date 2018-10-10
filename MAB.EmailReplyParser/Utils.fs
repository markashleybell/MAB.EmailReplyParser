namespace MAB.EmailReplyParser

module Utils =
    open System.Text.RegularExpressions

    let replace pat (sub: string) s = Regex.Replace(s, pat, sub)

    let split c (s: string) = s.Split [|c|]
