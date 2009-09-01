using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WikiPlex.Formatting;
using WikiPlex;

namespace DokuwikiClient.Parsing
{
	/// <summary>
	/// Renders all the Heading tags (H1 - H5).
	/// </summary>
	public class DokuWikiHeadingsRenderer : IRenderer
	{
		#region fields

		private readonly string rendererName = typeof(DokuWikiHeadingsRenderer).ToString();

		private readonly string headingOneFormat = "<head><link rel='stylesheet' media='all' type='text/css' href='/design.css'/></head><h1>{0}</h1>";
		#endregion

		#region Properties

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The unique name of this renderer.</value>
		public string Id
		{
			get { return this.rendererName; }
		}

		#endregion

		#region IRenderer Members

		/// <summary>
		/// Determines whether this instance can expand the specified scope name.
		/// </summary>
		/// <param name="scopeName">Name of the scope.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can expand the specified scope name; otherwise, <c>false</c>.
		/// </returns>
		public bool CanExpand(string scopeName)
		{
			if (scopeName == DokuWikiScope.HeadingOne)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Expands the specified scope name.
		/// </summary>
		/// <param name="scopeName">Name of the scope.</param>
		/// <param name="input">The input.</param>
		/// <param name="htmlEncode">The HTML encode.</param>
		/// <param name="attributeEncode">The attribute encode.</param>
		/// <returns></returns>
		public string Expand(string scopeName, string input, Func<string, string> htmlEncode, Func<string, string> attributeEncode)
		{
			return String.Format(this.headingOneFormat, htmlEncode(input));
		}

		#endregion
	}
}
