using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DokuwikiClient.Communication;

namespace DokuwikiClient
{
	class DokuWikiClientConsole
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Connecting to wiki.");
			XmlRpcClient client = new XmlRpcClient(new Uri("http://wiki.froorider.ch/lib/exe/xmlrpc.php"));
			Console.WriteLine("Listing server methods.");
			foreach (String serverMethodName in client.ListServerMethods())
			{
				Console.WriteLine("Method name: {0}", serverMethodName);
			}

            Console.Write("Enter the name of a wikipage you want to see:");
            string pageName = Console.ReadLine();
            Console.WriteLine("Getting wikipage.");
            Console.WriteLine("Wikitext of page: "+client.GetPage(pageName));
			Console.WriteLine();
			Console.WriteLine("Press the any key to exit.");
			Console.Read();
		}
	}
}
