using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WikiPlex.Formatting;
using WikiPlex;

namespace DokuwikiClient.Parsing
{
	/// <summary>
	/// Class which contains or links all formatters needed to render DokuWiki files correctly to HTML.
	/// </summary>
	public static class DokuWikiFormatters
	{
		/// <summary>
		/// Gets the doku wiki formatters.
		/// </summary>
		/// <returns>A MacroFormatter containing all specific DokuWiki renderers.</returns>
		public static MacroFormatter GetDokuWikiFormatters()
		{
			var siteRenderers = new IRenderer[] { new DokuWikiHeadingsRenderer() };
			IEnumerable<IRenderer> allRenderers = Renderers.All.Union(siteRenderers);
			return new MacroFormatter(allRenderers);
		}
	}
}
