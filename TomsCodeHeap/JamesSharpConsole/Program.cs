using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }
    }
}
