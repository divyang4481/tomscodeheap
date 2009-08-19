using CH.Froorider.Codeheap.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CH.Froorider.Codeheap.Domain;

namespace TomsCodeHeapTesting
{
    /// <summary>
    ///This is a test class for PersistenceManagerTest and is intended
    ///to contain all PersistenceManagerTest Unit Tests
    ///</summary>
	[TestClass()]
	public class PersistenceManagerTest
	{
		#region properties

		private static TestBusinessObject testBusinessObject = new TestBusinessObject() { SimpleProperty = "Some test data" };

		#endregion

		/// <summary>
		/// Tests that a business object is serialized correctly.
		///</summary>
		[TestMethod()]
		public void SerializeTest()
		{
			string expected = testBusinessObject.Serialize();
			string actual;
			actual = PersistenceManager.Serialize(testBusinessObject);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for DeserializeObject
		///</summary>
		public void DeserializeObjectTestHelper<T>()
		{
			string filename = string.Empty; // TODO: Initialize to an appropriate value
			T expected = default(T); // TODO: Initialize to an appropriate value
			T actual;
			actual = PersistenceManager.DeserializeObject<T>(filename);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[TestMethod()]
		public void DeserializeObjectTest()
		{
			DeserializeObjectTestHelper<GenericParameterHelper>();
		}
	}
}
