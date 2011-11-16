using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestClient
{
    using System.ServiceModel;

    using CH.Froorider.JamesSharpContracts.Protocols;

    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding { Security = { Mode = SecurityMode.None } };
            EndpointAddress address = new EndpointAddress("net.tcp://localhost:25");
            TestClient client = new TestClient(binding, address);
            client.Open();
            client.ProcessMessage();
            client.Close();
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }
}
