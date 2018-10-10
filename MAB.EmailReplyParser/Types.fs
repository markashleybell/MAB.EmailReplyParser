namespace MAB.EmailReplyParser

type EmailReplyLineVisibility = Visible | Hidden
    
type EmailReplyLineType = Empty | Content | Quoted | SignatureDelimiter

type EmailReplyLine = {
    Index: int
    Type: EmailReplyLineType
    Visibility: EmailReplyLineVisibility;
    Content: string
}
