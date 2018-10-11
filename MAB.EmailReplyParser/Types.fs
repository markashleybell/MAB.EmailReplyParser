namespace MAB.EmailReplyParser

type EmailReplyLineVisibility = 
    | Visible
    | Hidden
    
type EmailReplyLineType = 
    | Quoted
    | Signature
    | Content

type EmailReplyLine = {
    Index: int
    Type: EmailReplyLineType
    Visibility: EmailReplyLineVisibility;
    Content: string
}
