
namespace JamesSharpSmtp.SmtpProtocol
{
    internal enum SmtpVerbs
    {
        EHLO,
        HELO,
        MAIL,
        RCPT,
        DATA,
        RSET,
        NOOP,
        QUIT,
        VRFY
    }
}
