// ========================================================================
// File:     IDokuWikiClient.cs
//
// Author:   $Author$
// Date:     $LastChangedDate$
// Revision: $Revision$
// ========================================================================
// Copyright [2009] [$Author$]
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================

using CookComputing.XmlRpc;

namespace DokuwikiClient.Communication
{
	/// <summary>
	/// Definition of the remote procedure calls of the DokuWikiServer. 
	/// </summary>
	public interface IDokuWikiClient : IXmlRpcProxy
	{
		#region DokuWiki specific remote methods

		/// <summary>
		/// Lists all pages within a given namespace.
		/// </summary>
		/// <param name="nameSpace">The namespace which should be searched.</param>
		/// <param name="options">The options for the php method searchAllPages().</param>
		/// <returns>
		/// A string of the page item names in this namespace.
		/// </returns>
		/// <remarks>The options are passed directly to the PHP method searchAllPages().</remarks>
		[XmlRpcMethod("dokuwiki.getPagelist")]
		string[] GetPageList(string nameSpace, string[] options);

		/// <summary>
		/// Gets the doku wiki version.
		/// </summary>
		/// <returns>A string containing the version number of the remote dokuwiki.</returns>
		[XmlRpcMethod("dokuwiki.getVersion")]
		string GetDokuWikiVersion();

		/// <summary>
		/// Gets the current time at the remote wiki server as Unix timestamp. 
		/// </summary>
		/// <returns>An integer value indicating the server time.</returns>
		[XmlRpcMethod("dokuwiki.getTime")]
		int GetTime();

		#endregion

		#region common remote methods

		// [XmlRpcMethod("")]
		// string MethodName(string parameter);
		#endregion
	}
}
