namespace CH.Froorider.JamesSharpContracts.Protocols
{
    using System;

    /// <summary>
    /// Class for implementing the type-safe enum pattern
    /// </summary>
    public class ProtocolType
    {
        private readonly String _name;

        public static readonly ProtocolType TcpProtocol = new ProtocolType("TcpProtocol");

        protected ProtocolType(String name)
        {
            _name = name;
        }

        public override String ToString()
        {
            return _name;
        }

    }
}
