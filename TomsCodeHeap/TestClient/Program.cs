using System;

namespace TestClient
{


    class Program
    {
        static void Main(string[] args)
        {
            TestClient client = new TestClient();
            client.ProcessMessage();
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }
}
