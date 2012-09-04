using CH.Froorider.JamesSharpContracts.Protocols;

namespace JamesSharpSmtp.SmtpProtocol
{
    public class SmtpProtocolType : ProtocolType
    {
        public static readonly ProtocolType SmtpProtocol = new SmtpProtocolType("SMTP");

        private SmtpProtocolType(string name) : base(name){ }
    }
}
