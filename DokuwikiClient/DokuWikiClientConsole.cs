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
			Console.WriteLine();
			Console.WriteLine("Press the any key to exit.");
			Console.Read();
		}
	}
}
