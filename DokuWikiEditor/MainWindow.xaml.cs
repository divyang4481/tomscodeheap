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

namespace DokuWikiEditor
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XmlRpcClient client;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void connectToWikiButton_Click(object sender, RoutedEventArgs e)
        {
            this.client = new XmlRpcClient(new Uri(this.connectoToWikiText.Text));
            this.outputBox.AppendText("Listing server methods: \n" + client.ListServerMethods() + "\n");
            this.outputBox.AppendText("Listing server capabilites: \n" + client.GetServerCapabilites().Dump() + "\n");
        }

        private void getWikipageButton_Click(object sender, RoutedEventArgs e)
        {
            this.outputBox.AppendText("Raw wiki page: \n" + this.client.GetPage(this.getWikipageText.Text));
        }
    }
}
