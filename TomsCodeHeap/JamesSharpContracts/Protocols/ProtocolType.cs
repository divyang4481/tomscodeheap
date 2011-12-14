namespace CH.Froorider.JamesSharpContracts.Protocols
{
    using System;

    /// <summary>
    /// Class implementing the type safe enum pattern
    /// </summary>
    public class ProtocolType
    {
        private readonly String _name;

        public static readonly ProtocolType TcpProtocol = new ProtocolType("TcpProtocol");
        public static readonly ProtocolType SmtpProtocol = new ProtocolType("SMTP");

        private ProtocolType(String name)
        {
            _name = name;
        }

        public override String ToString()
        {
            return _name;
        }

    }
}
