# MAB.EmailReplyParser

A library for parsing plain-text email replies and removing quotes, signatures etc.

## Usage

In both examples, we'll assume that `body` is a string variable containing the plain text message body extracted from an email. This library does not deal with MIME parsing at all; for that you should definitely use [MimeKit][3].

### From F\#

```fsharp
open MAB.EmailReplyParser

// Returns just the reply portion of the message body 
EmailReplyParser.getReply body

// Returns a normalised* string[] representing the lines in the message body
EmailReplyParser.getLinesPlainText body

// Returns a list of EmailReplyLine records containing information about
// each line (what it is, whether it should be visible etc)
EmailReplyParser.getLines body
```

### From C\#

```csharp
using MAB.EmailReplyParser

EmailReplyParser.getReply(body);
EmailReplyParser.getLinesPlainText(body);
EmailReplyParser.getLines body(body);
```

In almost every case, `EmailReplyParser.getReply` is the function you'll actually want to use. The others can be useful if you're getting incorrect results and need to visualise how the text has been split up and classified.

\* _In this instance, "normalisation" means applying some pre-processing to the message body, like making sure quote headers aren't split across multiple lines and adding a blank line before separators._

## Rationale

There are already a bunch of parsers out there which do roughly the same job, most of which are ports of the [GitHub email parser library][1]. However, that library looks to have been abandoned for some time, and there are several pull requests which fix issues with newer reply/signature patterns but have never been merged.

I created this library to try and consolidate the updates from the various existing libraries, while also taking a different approach to the general parsing logic (which I believe is easier to inspect and debug).

## Inspiration

Thanks to these libraries, without which I wouldn't have had a clue where to start!

- https://github.com/github/email_reply_parser (Ruby)
- https://github.com/zapier/email-reply-parser (Python)
- https://github.com/EricJWHuang/EmailReplyParser (C#)
- https://github.com/mailgun/talon (Python)

[1]: https://github.com/github/email_reply_parser
[2]: https://github.com/EricJWHuang/EmailReplyParser
[3]: https://github.com/jstedfast/MimeKit
