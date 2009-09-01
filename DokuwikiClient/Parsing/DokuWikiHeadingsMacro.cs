using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WikiPlex.Compilation.Macros;
using WikiPlex.Compilation;
using WikiPlex;

namespace DokuwikiClient.Parsing
{
	/// <summary>
	/// Lexer to find the Headings in wikitext.
	/// </summary>
	public class DokuWikiHeadingsMacro : IMacro
	{
		#region IMacro Members

		public string Id
		{
			get { return "DokuWikiHeadingsMacro"; }
		}

		public IList<WikiPlex.Compilation.MacroRule> Rules
		{
			get 
			{
				return new List<MacroRule>
                           {
                               new MacroRule(EscapeRegexPatterns.CurlyBraceEscape),
                               new MacroRule(@"((?<=)={6})(.*)((?=)={6})",
                                             new Dictionary<int, string>
                                                 {
                                                     {1, ScopeName.Remove},
                                                     {2, DokuWikiScope.HeadingOne},
                                                     {3, ScopeName.Remove}
                                                 }
                                   )
                           };
			}
		}

		#endregion
	}
}
