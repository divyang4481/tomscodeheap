using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CH.Froorider.JamesSharpContracts.Protocols;
using System.ServiceModel;

namespace TestClient
{
    public class TestClient : ClientBase<IProtocol>
    {
        ChannelFactory<IProtocol> factory = new ChannelFactory<IProtocol>();
        
    }
}
