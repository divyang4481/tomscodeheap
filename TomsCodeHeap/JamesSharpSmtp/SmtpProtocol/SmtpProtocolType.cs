using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CH.Froorider.JamesSharpContracts.Protocols;

namespace JamesSharpSmtp.SmtpProtocol
{
    public class SmtpProtocolType : ProtocolType
    {
        public static readonly ProtocolType SmtpProtocol = new SmtpProtocolType("SMTP");

        private SmtpProtocolType(string name) : base(name){ }
    }
}
