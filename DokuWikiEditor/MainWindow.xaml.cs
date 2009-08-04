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
using System.ComponentModel;
using DokuwikiClient.Communication.Messages;

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
            this.worker.RunWorkerAsync(this.connectoToWikiText.Text);
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
