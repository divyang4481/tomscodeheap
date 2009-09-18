using DokuwikiClient.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CH.Froorider.Codeheap.Domain;
using CH.Froorider.Codeheap.Persistence;
using DokuwikiClient.Domain.Entities;
using System;
using System.Collections.Generic;

namespace DokuWikiClientTests
{
    /// <summary>
    ///This is a test class for FileManagerTest and is intended
    ///to contain all FileManagerTest Unit Tests
    ///</summary>
	[TestClass()]
	public class FileManagerTest
	{
		#region fields

		private static WikiAccount commonAccount = new WikiAccount();

		#endregion

		#region tests

		[TestMethod()]
		public void Register_SaveANewCommonBO_Successful()
		{
			FileManager manager = new FileManager();
			manager.Save<WikiAccount>(commonAccount);
			List<WikiAccount> account = manager.LoadObjects<WikiAccount>(typeof(WikiAccount).Name);
			Assert.IsTrue(account.Count != 0);
		}

		#endregion

		#region helpers // Init // etc.

		[ClassInitialize]
		public static void InitializeTestEnvironment(TestContext context)
		{
			commonAccount.WikiUrl = new Uri("http://wiki.froorider.ch/lib/exe/xmlrpc.php");
			commonAccount.AccountName = "Froorider's wiki";
			commonAccount.LoginName = "foobar";
			commonAccount.Password = "barfoo";
		}

		#endregion
	}
}
