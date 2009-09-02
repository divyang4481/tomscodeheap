using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using DokuwikiClient;
using DokuwikiClient.Communication;
using DokuwikiClient.Communication.Messages;
using DokuwikiClient.Communication.XmlRpcMessages;
using System.Windows.Media.Imaging;

namespace DokuWikiEditor
{
	/// <summary>
	/// Interaktionslogik für Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region fields

		private XmlRpcClient client;
		private BackgroundWorker worker;
		private DokuWikiEngine engine = new DokuWikiEngine();

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			this.statusProgress.Visibility = Visibility.Hidden;
			this.InitializeBackgroundWorker();
		}

		/// <summary>
		/// Initializes the background worker.
		/// </summary>
		private void InitializeBackgroundWorker()
		{
			this.worker = new BackgroundWorker();
			worker.DoWork += this.BeginConnectToWiki;
			worker.RunWorkerCompleted += this.EndConnectToWiki;
		}

		#endregion

		#region event handlers

		private void connectToWikiButton_Click(object sender, RoutedEventArgs e)
		{
			this.statusLabel.Content = "Connecting to wiki!";
			this.statusProgress.Visibility = Visibility.Visible;
			if (!this.worker.IsBusy)
			{
				this.worker.RunWorkerAsync(this.connectoToWikiText.Text);
			}
			else
			{
				this.statusLabel.Content = "Dokuwiki client is already working.";
			}
		}

		private void getWikipageButton_Click(object sender, RoutedEventArgs e)
		{
			this.statusLabel.Content = "Getting wikipage: " + this.getWikipageText.Text;

			try
			{
				string rawWikipage = this.client.GetPage(this.getWikipageText.Text);
				FlowDocument document = this.outputBox.Document;
				TextRange wholeDocument = new TextRange(document.ContentStart, document.ContentEnd);
				wholeDocument.Text = rawWikipage;
			}
			catch (CommunicationException ce)
			{
				this.statusLabel.Content = "Couldn't load wiki page. Cause: " + ce.Message;
			}
		}

		private void saveWikipage_Click(object sender, RoutedEventArgs e)
		{
			this.statusLabel.Content = "Saving wikipage.";

			try
			{
				string rawWikipage = String.Empty;
				FlowDocument document = this.outputBox.Document;
				TextRange wholeDocument = new TextRange(document.ContentStart, document.ContentEnd);
				rawWikipage = wholeDocument.Text;
				if (!String.IsNullOrEmpty(rawWikipage))
				{
					List<PutParameters> putParametersList = new List<PutParameters>();
					PutParameters putParameters = new PutParameters();
					putParameters.ChangeSummary = "Generated summary";
					putParameters.IsMinor = true;
					putParametersList.Add(putParameters);

					this.client.PutPage(this.getWikipageText.Text, rawWikipage, putParametersList.ToArray());
				}
				else
				{
					this.statusLabel.Content = "Nothing to save.";
				}
			}
			catch (CommunicationException ce)
			{
				this.statusLabel.Content = "Couldn't save wiki page. Cause: " + ce.Message;
			}

			this.statusLabel.Content = "Wikipage saved!";
		}

		private void previewButton_Click(object sender, RoutedEventArgs e)
		{
			string rawWikipage = String.Empty;
			string htmlWikipage = String.Empty;

			FlowDocument document = this.outputBox.Document;
			TextRange wholeDocument = new TextRange(document.ContentStart, document.ContentEnd);
			rawWikipage = wholeDocument.Text;
			htmlWikipage = engine.Render(rawWikipage);

			Window browserWindow = new Window();
			Uri iconUri = new Uri("pack://application:,,,/Document Blank.ico", UriKind.RelativeOrAbsolute);
			browserWindow.Icon = BitmapFrame.Create(iconUri);
			StackPanel panel = new StackPanel();
			WebBrowser browser = new WebBrowser();
			browser.Height = 800;
			browser.NavigateToString(htmlWikipage);
			panel.Children.Add(browser);
			browserWindow.Content = panel;
			browserWindow.Show();
		}

		#endregion

		#region private methods

		/// <summary>
		/// Initializes the connection to the remote Xml - Rpc wiki server. 
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
		private void BeginConnectToWiki(object sender, DoWorkEventArgs e)
		{
			string urlOfWiki = e.Argument as string;
			this.client = new XmlRpcClient(new Uri(urlOfWiki));
			client.ListServerMethods();
		}

		/// <summary>
		/// Performs the work after the Xml - Rpc Client to the wiki has been created.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
		private void EndConnectToWiki(object sender, RunWorkerCompletedEventArgs e)
		{
			this.statusProgress.Visibility = Visibility.Hidden;
			this.statusLabel.Content = "Connection to wiki established.";
			this.outputBox.AppendText("Listing server methods: \n" + client.ListServerMethods() + "\n");
			this.outputBox.AppendText("Listing server capabilites: \n" + client.GetServerCapabilites().Dump() + "\n");
		}

		#endregion
	}
}
