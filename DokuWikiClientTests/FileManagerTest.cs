using DokuwikiClient.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CH.Froorider.Codeheap.Domain;
using CH.Froorider.Codeheap.Persistence;
using DokuwikiClient.Domain.Entities;
using System;

namespace DokuWikiClientTests
{
    /// <summary>
    ///This is a test class for FileManagerTest and is intended
    ///to contain all FileManagerTest Unit Tests
    ///</summary>
	[TestClass()]
	public class FileManagerTest
	{
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		[TestMethod()]
		public void RegisterTest()
		{
			FileManager manager = new FileManager();
			WikiAccount account = new WikiAccount();
			account.WikiUrl = new Uri("http://wiki.froorider.ch/lib/exe/xmlrpc.php");
			account.AccountName = "Froorider's wiki";
			account.LoginName = "foobar";
			account.Password = "barfoo";
			account.Serialize();
			manager.Register<WikiAccount>(account);
		}
	}
}
