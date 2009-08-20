using CH.Froorider.Codeheap.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CH.Froorider.Codeheap.Domain;
using System;

namespace TomsCodeHeapTesting
{
    /// <summary>
    ///This is a test class for PersistenceManagerTest and is intended
    ///to contain all PersistenceManagerTest Unit Tests
    ///</summary>
	[TestClass()]
	public class PersistenceManagerTest
	{
		#region fields / properties

		private static volatile object testSynchronizer = new object();
 
		private static TestBusinessObject testBusinessObject = new TestBusinessObject() { SimpleProperty = "Some test data" };
		private static TestBusinessObject testBusinessObject2 = new TestBusinessObject() { SimpleProperty = "Some other test data." };

		#endregion

		/// <summary>
		/// Tests that a business object is serialized correctly.
		///</summary>
		[TestMethod()]
		public void SerializeTest()
		{
			lock (testSynchronizer)
			{
				string expected = testBusinessObject.Serialize();
				string actual = testBusinessObject.ObjectIdentifier;
				Assert.AreEqual(expected, actual);
				actual = testBusinessObject2.Serialize();
				Assert.AreNotEqual(expected, actual);
			}
		}

		/// <summary>
		/// Tests that a serialized business object can be deserialzed again.
		///</summary>
		[TestMethod()]
		public void DeserializeTest()
		{
			lock (testSynchronizer)
			{
				string fileName = testBusinessObject.Serialize();
				string fileName2 = testBusinessObject2.Serialize();
				TestBusinessObject deserializedBusinessObject = PersistenceManager.DeserializeObject<TestBusinessObject>(fileName);
				TestBusinessObject deserializedBusinessObject2 = PersistenceManager.DeserializeObject<TestBusinessObject>(fileName2);
				Assert.AreEqual(deserializedBusinessObject, testBusinessObject);
				Assert.AreEqual(deserializedBusinessObject2, testBusinessObject2);
				Assert.IsFalse(deserializedBusinessObject.Equals(deserializedBusinessObject2));
			}
		}

	}
}
