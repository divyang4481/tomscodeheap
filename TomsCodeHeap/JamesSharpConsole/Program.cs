using System;
using CH.Froorider.JamesSharp;
using log4net.Config;

namespace JamesSharpConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            Console.WriteLine("Starting Email Server");
            JamesSharp server = new JamesSharp();
            server.StartUp();
        }
    }
}
