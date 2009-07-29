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
using DokuwikiClient.Communication.Messages;
using DokuwikiClient.Communication.XmlRpcMessages;

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
		/// <exception cref="ArgumentException">Is thrown when the XmlRpc server is not enabled.</exception>
		/// <exception cref="CommunicationException">Is thrown when the Xml-Rpc mechanism had errors.</exception>
		/// <exception cref="WebException">Is thrown when the HTTP connection had errors.</exception>
		public string[] ListServerMethods()
		{
			try
			{
				return this.clientProxy.SystemListMethods();
			}
			catch (WebException we)
			{
				logger.Warn(we);
				throw;
			}
			catch (XmlRpcIllFormedXmlException)
			{
				throw new ArgumentException("XmlRpc server not enabled.");
			}
			catch (XmlRpcException xrpce)
			{
				logger.Warn(xrpce);
				throw new CommunicationException(xrpce.Message);
			}
		}

		/// <summary>
		/// Gives you a list of possible methods implemented at the server.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <returns>An array containing all method signatures this remote method call offers.</returns>
		/// <exception cref="ArgumentException">Is thrown when the <paramref name="methodName"/> is unkown at remote host.</exception>
		/// <exception cref="CommunicationException">Is thrown when the Xml-Rpc mechanism had errors.</exception>
		/// <exception cref="WebException">Is thrown when the HTTP connection had errors.</exception>
		public object[] GetMethodSignatures(string methodName)
		{
			try
			{
				return this.clientProxy.SystemMethodSignature(methodName);
			}
			catch (WebException we)
			{
				logger.Warn(we);
				throw;
			}
			catch (XmlRpcFaultException xrfe)
			{
				logger.Warn(xrfe);
				throw new ArgumentException("Unknown remote method.","methodName");
			}
			catch (XmlRpcException xrpce)
			{
				logger.Warn(xrpce);
				throw new CommunicationException(xrpce.Message);
			}
		}

		/// <summary>
		/// Gives you a string describing the use of a certain method.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <returns>A description for the usage of this remote method.</returns>
		/// <exception cref="ArgumentException">Is thrown when the <paramref name="methodName"/> is unkown at remote host.</exception>
		/// <exception cref="CommunicationException">Is thrown when the Xml-Rpc mechanism had errors.</exception>
		/// <exception cref="WebException">Is thrown when the HTTP connection had errors.</exception>
		public string GetMethodHelp(string methodName)
		{
			try
			{
				return this.clientProxy.SystemMethodHelp(methodName);
			}
			catch (WebException we)
			{
				logger.Warn(we);
				throw;
			}
			catch (XmlRpcFaultException xrfe)
			{
				logger.Warn(xrfe);
				throw new ArgumentException("Unknown remote method.", "methodName");
			}
			catch (XmlRpcException xrpce)
			{
				logger.Warn(xrpce);
				throw new CommunicationException(xrpce.Message);
			}
		}

		#endregion

        #region IDokuWikiClient Member

        /// <summary>
        /// Gets a wikipage identified by it's name as raw wiki text.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <returns>The raw Wiki text for a page.</returns>
        /// <exception cref="ArgumentNullException">Is thrown when the passed argument is null.</exception>
		/// <exception cref="ArgumentException">Is thrown when the <paramref name="pageName"/> is unkown at remote host.</exception>
		/// <exception cref="CommunicationException">Is thrown when the Xml-Rpc mechanism had errors.</exception>
		/// <exception cref="WebException">Is thrown when the HTTP connection had errors.</exception>
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
				return wikiText;
            }
			catch (WebException we)
			{
				logger.Warn(we);
				throw;
			}
			catch (XmlRpcFaultException xrfe)
			{
				logger.Warn(xrfe);
				throw new ArgumentException("Unknown wiki page.", "pageName");
			}
			catch (XmlRpcException xrpce)
			{
				logger.Warn(xrpce);
				throw new CommunicationException(xrpce.Message);
			}
        }

		/// <summary>
		/// Gets all pages of the remote wiki.
		/// </summary>
		/// <returns>An array of <see cref="PageItem"/>s.</returns>
		/// <exception cref="CommunicationException">Is thrown when the Xml-Rpc mechanism had errors.</exception>
		/// <exception cref="WebException">Is thrown when the HTTP connection had errors.</exception>
		public PageItem[] GetAllPages()
		{
			try
			{
				return this.clientProxy.GetAllPages();
			}
			catch (WebException we)
			{
				logger.Warn(we);
				throw;
			}
			catch (XmlRpcException xrpce)
			{
				logger.Warn(xrpce);
				throw new CommunicationException(xrpce.Message);
			}
		}

        /// <summary>
        /// Gets the a wiki page pre-formatted in HTML.
        /// </summary>
        /// <param name="pageName">Name of the wikpage.</param>
        /// <returns>XHTML represenation of the wiki page.</returns>
        /// <exception cref="ArgumentNullException">Is thrown when the passed argument is null.</exception>
        /// <exception cref="ArgumentException">Is thrown when the <paramref name="pageName"/> is unkown at remote host.</exception>
        /// <exception cref="CommunicationException">Is thrown when the Xml-Rpc mechanism had errors.</exception>
        /// <exception cref="WebException">Is thrown when the HTTP connection had errors.</exception>
        public string GetPageHtml(string pageName)
        {
            string wikiTextAsHtml = String.Empty;

            if (String.IsNullOrEmpty(pageName))
            {
                throw new ArgumentNullException("pageName");
            }

            try
            {
                wikiTextAsHtml = this.clientProxy.GetPageHtml(pageName);
                return wikiTextAsHtml;
            }
            catch (WebException we)
            {
                logger.Warn(we);
                throw;
            }
            catch (XmlRpcFaultException xrfe)
            {
                logger.Warn(xrfe);
                throw new ArgumentException("Unknown wiki page.", "pageName");
            }
            catch (XmlRpcException xrpce)
            {
                logger.Warn(xrpce);
                throw new CommunicationException(xrpce.Message);
            }
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
