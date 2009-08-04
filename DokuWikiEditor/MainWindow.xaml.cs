using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DokuwikiClient;
using DokuwikiClient.Communication;
using System.Windows.Threading;
using System.Threading;

namespace DokuWikiEditor
{
	/// <summary>
	/// Interaktionslogik für Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static XmlRpcClient client;
		private static bool isConnected = false;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void connectToWikiButton_Click(object sender, RoutedEventArgs e)
		{
			this.statusLabel.Content = "Connecting to wiki!";
			this.statusProgress.Visibility = Visibility.Visible;
			Thread connectToWiki = new Thread(MainWindow.ConnectToWiki);
			connectToWiki.Start(this.connectoToWikiText.Text);
			while (!isConnected)
			{
				Thread.Sleep(100);
			}
			this.statusProgress.Visibility = Visibility.Hidden;
			this.statusLabel.Content = "Connection to wiki established.";
			this.outputBox.AppendText("Listing server methods: \n" + client.ListServerMethods() + "\n");
			this.outputBox.AppendText("Listing server capabilites: \n" + client.GetServerCapabilites().Dump() + "\n");
		}

		private void getWikipageButton_Click(object sender, RoutedEventArgs e)
		{
			this.statusLabel.Content = "Getting wikipage: " + this.getWikipageText.Text;
			string rawWikipage = MainWindow.client.GetPage(this.getWikipageText.Text);
			FlowDocument document = this.outputBox.Document;
			TextRange wholeDocument = new TextRange(document.ContentStart, document.ContentEnd);
			wholeDocument.Text = rawWikipage;
		}

		protected static void ConnectToWiki(object urlToWiki)
		{
			MainWindow.client = new XmlRpcClient(new Uri(urlToWiki as string));
			isConnected = true;
		}

	}
}
