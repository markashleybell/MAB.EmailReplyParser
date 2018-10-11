<Query Kind="Program">
  <Reference Relative="..\MAB.EmailReplyParser\bin\Debug\netstandard2.0\MAB.EmailReplyParser.dll">C:\Src\MAB.EmailReplyParser\MAB.EmailReplyParser\bin\Debug\netstandard2.0\MAB.EmailReplyParser.dll</Reference>
  <NuGetReference>FSharp.Core</NuGetReference>
  <Namespace>MAB.EmailReplyParser</Namespace>
</Query>

void Main()
{
    //var body = GetBody("email_gmail");
    //var body = GetBody("email_1_1");
    var body = GetBody("basic_test");
    
    EmailReplyParser.getLinesPlainText(body).Dump();
    EmailReplyParser.getLines(body).Select(l => Display(l)).Dump();
    EmailReplyParser.getReply(body).Dump();
}

public string GetBody(string id)
{
    var workingDir = Path.GetDirectoryName(Util.CurrentQueryPath);
    var path = $@"{workingDir}\..\MAB.EmailReplyParser.Test\TestData\{id}.txt";
    
    return File.ReadAllText(path);
}

public (int, string, string, string) Display(EmailReplyLine ln) =>
    (ln.Index, ln.Visibility.ToString(), ln.Type.ToString(), ln.Content);
