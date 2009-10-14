using System;
using System.Collections.Generic;
using System.Windows;
using DokuwikiClient.Domain.Entities;
using DokuWikiEditor.Dialog;
using System.Windows.Controls;
using System.Windows.Media;

namespace DokuWikiEditor
{
	/// <summary>
	/// Interaction logic for WikiAccountManagementWindow.xaml
	/// </summary>
	public partial class WikiAccountManagementWindow : Window
	{
		#region fields

		private List<WikiAccount> wikiAccounts;
		public DokuwikiClient.DokuWikiClient clientToUse;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="WikiAccountManagementWindow"/> class.
		/// </summary>
		/// <param name="accounts">The accounts.</param>
		public WikiAccountManagementWindow(List<WikiAccount> accounts)
		{
			InitializeComponent();
			this.wikiAccounts = accounts;
		}

		#endregion

		#region events

		/// <summary>
		/// Is performed when after the window has been loaded
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			if (this.wikiAccounts.Count == 0)
			{
				MessageBox.Show("No accounts yet.");
			}
			else
			{
				this.showWikiAccounts();
			}
		}

		#endregion

		#region private methods

		private void managementMenuNew_Click(object sender, RoutedEventArgs e)
		{
			WikiAccount newAccount = new WikiAccount();
			newAccount.AccountName = "My dokuwiki account somewhere.";
			newAccount.LoginName = "foo@bar.com";
			newAccount.Password = "AVerySecretPassword";
			newAccount.WikiUrlRaw = "http://www.mywiki.url/lib/exe/xmlrpc.php";

			WikiAccountDataDialog dataInputDialog = new WikiAccountDataDialog(newAccount);
			dataInputDialog.ShowDialog();
			if (String.IsNullOrEmpty(newAccount.Password))
			{
				MessageBox.Show("Account not saved.");
			}
			else
			{
				clientToUse.SaveWikiAccount(newAccount);
				this.wikiAccounts = this.clientToUse.LoadWikiAccounts();
				this.showWikiAccounts();
			}
		}

		private void showWikiAccounts()
		{
			this.contentPanel.Children.Clear();
			foreach (WikiAccount account in this.wikiAccounts)
			{
				Label accountLabel = new Label();
				TextBlock block = new TextBlock();
				block.Text = String.Format("Wiki account settings: \n Account name: {0} \n Login Name: {1} \n Password: {2} \n Wiki Url: {3}",
					account.AccountName, account.LoginName, account.Password, account.WikiUrlRaw);

				accountLabel.Margin = new Thickness(5);
				accountLabel.Padding = new Thickness(5);
				accountLabel.BorderThickness = new Thickness(5);
				accountLabel.BorderBrush = Brushes.BlueViolet;

				accountLabel.Content = block;
				this.contentPanel.Children.Add(accountLabel);
			}
		}

		#endregion
	}
}
