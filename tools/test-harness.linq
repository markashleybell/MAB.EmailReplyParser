<Query Kind="FSharpProgram">
  <Reference Relative="..\MAB.EmailReplyParser\bin\Debug\netstandard2.0\MAB.EmailReplyParser.dll">C:\Src\MAB.EmailReplyParser\MAB.EmailReplyParser\bin\Debug\netstandard2.0\MAB.EmailReplyParser.dll</Reference>
  <Namespace>MAB.EmailReplyParser</Namespace>
</Query>


let workingDir = Path.GetDirectoryName(Util.CurrentQueryPath)

let display (i, v, t, l) = (i, v.ToString(), t.ToString(), l)

let getBody n = 
    let path = sprintf "%s\..\MAB.EmailReplyParser.Test\TestData\%s.txt" workingDir n
    File.ReadAllText(path)

//let body = getBody "email_gmail"
//let body = getBody "email_1_4"
let body = getBody "basic_test"

body 
|> EmailReplyParser.getVisibleLines  
|> List.map display
|> Dump 
|> ignore
