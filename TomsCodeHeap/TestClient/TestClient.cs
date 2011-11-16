using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CH.Froorider.JamesSharpContracts.Protocols;
using System.ServiceModel;

namespace TestClient
{
    using System.ServiceModel.Channels;

    public class TestClient : ClientBase<IProtocol>, IProtocol
    {
        public TestClient()
        {
        }

        public TestClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public TestClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public TestClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public TestClient(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public void ProcessMessage()
        {
            base.Channel.ProcessMessage();
        }
    }
}
