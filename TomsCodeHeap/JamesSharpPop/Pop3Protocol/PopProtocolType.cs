using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CH.Froorider.JamesSharpContracts.Protocols;

namespace JamesSharpPop.Pop3Protocol
{
    public class PopProtocolType : ProtocolType
    {
        public static readonly ProtocolType Pop3Protocol = new PopProtocolType("POP3");

        private PopProtocolType(string name) : base(name) { }
    }
}
