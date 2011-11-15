using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH.Froorider.JamesSharpContracts.Protocols
{
    /// <summary>
    /// Base implemenation of a protocol. Is used by the host to load and start the protocol.
    /// </summary>
    public class BaseProtocol : IProtocol
    {
        public void ProcessMessage()
        {
            Console.WriteLine("Processing Message");
        }
    }
}
