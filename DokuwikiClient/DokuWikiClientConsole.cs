using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DokuwikiClient.Communication;
using log4net;
using DokuwikiClient.Communication.XmlRpcMessages;
using log4net.Config;

namespace DokuwikiClient
{
	class DokuWikiClientConsole
	{
		private static ILog logger = LogManager.GetLogger(typeof(DokuWikiClientConsole));
		private static bool exitLoop = false;

		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += DokuWikiClientConsole.OnUnhandledException;
            XmlConfigurator.Configure();

			Console.WriteLine("Connecting to wiki.");
			XmlRpcClient client = new XmlRpcClient(new Uri("http://wiki.froorider.ch/lib/exe/xmlrpc.php"));
			
			Console.WriteLine("Listing server methods.");
			try
			{
				foreach (String serverMethodName in client.ListServerMethods())
				{
					Console.WriteLine("Method name: {0}", serverMethodName);
				}
			}
			catch (ArgumentException ae)
			{
				Console.WriteLine(ae.Message);
				Console.WriteLine("Press enter to exit.");
				Console.ReadLine();
				Environment.Exit(0);
			}

			Console.WriteLine("----------------------------------------------------");

			do
			{
				Console.Write(" 0 := Exit application\n 1 := Get Wikipage \n 2 := GetAllPages \n 3 := Get WikiPage as HTML \n");
				string input = Console.ReadLine();
				if (input.Equals("0"))
				{
					exitLoop = true;
				}
				else if (input.Equals("1"))
				{
					Console.WriteLine("Enter pagename of wikipage.");
					string pageName = Console.ReadLine();
					Console.WriteLine("Wikitext of page: \n" + client.GetPage(pageName));
				}
				else if (input.Equals("2"))
				{
					Console.WriteLine("Getting all page items.");
					PageItem[] pages = client.GetAllPages();
					foreach (PageItem pageItem in pages)
					{
						Console.WriteLine("ID: " + pageItem.Identificator);
						Console.WriteLine("LastModified: " + pageItem.LastModified);
						Console.WriteLine("Permissions: " + pageItem.Permissions);
						Console.WriteLine("Size: " + pageItem.Size);
						Console.WriteLine();
					}
				}
                else if (input.Equals("3"))
                {
                    Console.WriteLine("Enter pagename of wikipage.");
                    string pageName = Console.ReadLine();
                    Console.WriteLine("Page as HTML: \n" + client.GetPageHtml(pageName));
                }
                else
                {
                    Console.WriteLine("Unknown command.");

                }
				Console.WriteLine("----------------------------------------------------");
			}
			while (!exitLoop);
		}

		/// <summary>
		/// Called when an [unhandled exception] occures.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="args">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
		public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			Exception exc = (Exception)args.ExceptionObject;
			logger.Fatal("Unhandled exception caught.", exc);
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
			Environment.Exit(-1);
		}
	}
}
