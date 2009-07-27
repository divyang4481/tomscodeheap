// ========================================================================
// File:     XmlRpcClient.cs
//
// Author:   $Author$
// Date:     $LastChangeDate$
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

using System;
using System.IO;
using System.Net;
using CookComputing.XmlRpc;
using log4net;

namespace DokuwikiClient.Communication
{
	/// <summary>
	/// Proxy class for the communication between the program and the XmlRpcServer.
	/// Wraps all the remote method calls in a common way.
	/// </summary>
	internal class XmlRpcClient
	{
		#region fields

		private static ILog logger = LogManager.GetLogger(typeof(XmlRpcClient));

		private IDokuWikiClient clientProxy;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlRpcClient"/> class.
		/// </summary>
		/// <param name="serverUrl">The server URL.</param>
		/// <exception cref="ArgumentException">Is thrown when the passed server URL was not valid.</exception>
		public XmlRpcClient(Uri serverUrl)
		{
			try
			{
				this.clientProxy = XmlRpcProxyGen.Create<IDokuWikiClient>();
				this.clientProxy.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
				this.clientProxy.Url = serverUrl.AbsoluteUri;

#if(TRACE)
				RequestResponseLogger dumper = new RequestResponseLogger();
				dumper.Directory = @"C:\logs\xmlrpctests";
				if (!Directory.Exists(dumper.Directory))
				{
					Directory.CreateDirectory(dumper.Directory);
				}

				dumper.Attach(this.clientProxy);
#endif

				Console.WriteLine("XmlRpc proxy to URL: " + serverUrl.AbsoluteUri + " generated.");
			}
			catch (UriFormatException ufe)
			{
				Console.WriteLine(ufe);
				throw new ArgumentException("serverUrl", "Server URL is not valid. Cause: " + ufe.Message);
			}
		}

		#endregion

		#region Introspection API

		/// <summary>
		/// Returns a list of methods implemented by the server.
		/// </summary>
		/// <returns>An array of strings listing all remote method names.</returns>
		public string[] ListServerMethods()
		{
			try
			{
				return this.clientProxy.SystemListMethods();
			}
			catch (WebException we)
			{
				logger.Warn(we);
				string[] errorMessage = { "URL to remote server is not valid." };
				return errorMessage;
			}
		}

		/// <summary>
		/// Gives you a list of possible methods implemented at the server.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <returns>An array containing all method signatures this remote method call offers.</returns>
		public object[] GetMethodSignatures(string methodName)
		{
			try
			{
				return this.clientProxy.SystemMethodSignature(methodName);
			}
			catch (XmlRpcFaultException xrfe)
			{
				logger.Warn(xrfe);
				string[] errorMessage = { "Unknown method name" };
				return errorMessage;
			}
		}

		/// <summary>
		/// Gives you a string describing the use of a certain method.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <returns>A description for the usage of this remote method.</returns>
		public string GetMethodHelp(string methodName)
		{
			return this.clientProxy.SystemMethodHelp(methodName);
		}

		#endregion
	}
}
