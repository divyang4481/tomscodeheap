﻿// ========================================================================
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

        #region IDokuWikiClient Member

        /// <summary>
        /// Gets a wikipage identified by it's name as raw wiki text.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <returns>The raw Wiki text for a page.</returns>
        /// <exception cref="ArgumentNullException">Is thrown when the passed argument is null.</exception>
        public string GetPage(string pageName)
        {
            string wikiText = String.Empty;

            if (String.IsNullOrEmpty(pageName))
            {
                throw new ArgumentNullException("pageName");
            }

            try
            {
                wikiText = this.clientProxy.GetPage(pageName);
            }
            catch (WebException we)
            {
                logger.Error("Underlying HTTP - Connection had errors. Cause: " + we.Message);
            }
            catch (XmlRpcException xrpce)
            {
                logger.Error("XmlRpc mechanism had errors. Cause: " + xrpce.Message);
            }

            return wikiText;
        }

        public string[] GetPageList(string nameSpace, string[] options)
        {
            throw new NotImplementedException();
        }

        public string GetDokuWikiVersion()
        {
            throw new NotImplementedException();
        }

        public int GetTime()
        {
            throw new NotImplementedException();
        }

        public int GetXmlRpcApiVersion()
        {
            throw new NotImplementedException();
        }

        public int Login(string user, string password)
        {
            throw new NotImplementedException();
        }

        public string[] SetLocks(string[] pagesToLockOrUnlock)
        {
            throw new NotImplementedException();
        }

        public string GetRpcVersionSupported()
        {
            throw new NotImplementedException();
        }

        public string GetPageVersion(string pageName, int timestamp)
        {
            throw new NotImplementedException();
        }

        public object[] GetPageVersions(string pageName, int offset)
        {
            throw new NotImplementedException();
        }

        public object[] GetPageInfo(string pageName)
        {
            throw new NotImplementedException();
        }

        public object[] GetPageInfoVersion(string pageName, int timestamp)
        {
            throw new NotImplementedException();
        }

        public string GetPageHtml(string pageName)
        {
            throw new NotImplementedException();
        }

        public string GetPageHtmlVersion(string pageName, int timestamp)
        {
            throw new NotImplementedException();
        }

        public bool PutPage(string pageName, string rawWikiText, object putParameters)
        {
            throw new NotImplementedException();
        }

        public object[] ListLinks(string pageName)
        {
            throw new NotImplementedException();
        }

        public object[] GetAllPages()
        {
            throw new NotImplementedException();
        }

        public object[] GetBackLinks(string pageName)
        {
            throw new NotImplementedException();
        }

        public object[] GetRecentChanges(int timestamp)
        {
            throw new NotImplementedException();
        }

        public object[] GetAttachments(string nameSpace, object[] attachmentOptions)
        {
            throw new NotImplementedException();
        }

        public object GetAttachment(string mediaFileId)
        {
            throw new NotImplementedException();
        }

        public object[] GetAttachmentInfo(string mediaFileId)
        {
            throw new NotImplementedException();
        }

        public void PutAttachment(string mediaFileId, object mediaFileData, object attachmentOptions)
        {
            throw new NotImplementedException();
        }

        public void DeleteAttachment(string mediaFileId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
