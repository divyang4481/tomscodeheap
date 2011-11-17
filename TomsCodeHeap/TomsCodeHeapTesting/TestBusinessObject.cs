using System;
using CH.Froorider.Codeheap.Domain;

namespace TomsCodeHeapTesting
{
	[Serializable]
	public class TestBusinessObject: BusinessObject
	{
		#region Properties

		/// <summary>
		/// Gets or sets the simple property. This property just contains a string which can be filled with test data.
		/// </summary>
		/// <value>Just a string.</value>
		public string SimpleProperty { get; set; }

		#endregion
	}
}
