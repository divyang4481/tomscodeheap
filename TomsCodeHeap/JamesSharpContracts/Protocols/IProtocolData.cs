using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH.Froorider.JamesSharpContracts.Protocols
{
    public interface IProtocolData
    {
        int PortNumber { get; }
        string ProtocolName { get; }
    }
}
