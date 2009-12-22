// ========================================================================
// File:     BaseHttpServiceEndpoint.cs
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

using System.Diagnostics;
using System.Globalization;
using System.Net;
using log4net;

namespace CH.Froorider.Codeheap.Http
{
	/// <summary>
	/// Basic abstract implementation of an ServiceEndpoint for the HTTP Server.
	/// </summary>
	public abstract class BaseHttpServiceEndpoint
	{
		#region fields

		private static ILog logger = LogManager.GetLogger(typeof(BaseHttpServiceEndpoint));

		#endregion

		#region properties

		/// <summary>
		/// Gets or sets the Http listener context which contains the request and response streams, settings etc.
		/// </summary>
		/// <value>An instance of <see cref="HttpListenerContext"/>.</value>
		public HttpListenerContext Context { get; set; }

		/// <summary>
		/// Gets the logger, which can be used to write log messages.
		/// </summary>
		/// <value>An instance of <see cref="ILog"/> from the Apache log4net framework.</value>
		protected static ILog Logger 
		{
			get { return BaseHttpServiceEndpoint.logger; }
		}

		#endregion

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseHttpServiceEndpoint"/> class.
		/// </summary>
		protected BaseHttpServiceEndpoint()
		{
		}

		#endregion

		#region public methods

		/// <summary>
		/// Wrapper method to use the class in a multithreaded environment. 
		/// </summary>
		/// <remarks><b>If compiled with LOG_NETWORK_TRAFFIC - variable then all HTTP Traffic is logged.</b></remarks>
		/// <param name="message">A message given by the HTTP Server to transfer some information. E.g. how to deal with the connection.</param>
		public void ProcessConnection(object message)
		{
			if (this.Context != null)
			{
				this.ProcessRequest(this.Context);
			}

			this.LogHttpTraffic();
		}

		/// <summary>
		/// Processes the incoming HTTP request and sends back the HTTP response to the sender.
		/// </summary>
		/// <param name="requestContext">The <see cref="HttpListenerContext"/> in which this request arrived.</param>
		public abstract void ProcessRequest(HttpListenerContext requestContext);

		#endregion

		#region private methods

		/// <summary>
		/// Logs the HTTP traffic with all headers. Is appended to the log file with level "DEBUG".
		/// </summary>
		/// <remarks>Activate the "LOG_NETWORK_TRAFFIC" variable to enable this.</remarks>
		[Conditional("LOG_NETWORK_TRAFFIC")]
		private void LogHttpTraffic()
		{
			logger.Debug("Request processed. ");

			WebHeaderCollection headers = this.Context.Response.Headers;

			// Get each header and display each value.
			logger.Debug("Response header to client:");
			foreach (string key in headers.AllKeys)
			{
				string[] values = headers.GetValues(key);
				if (values != null)
				{
					if (values.Length > 0)
					{
						logger.DebugFormat("The values of the {0} header are: ", key);
						foreach (string value in values)
						{
							logger.DebugFormat(CultureInfo.CurrentCulture, "   {0}", value);
						}
					}
					else
					{
						logger.Debug("There is no value associated with the header.");
					}
				}
			}

			logger.DebugFormat(CultureInfo.CurrentCulture, "Status code of response: " + this.Context.Response.StatusCode);
		}

		#endregion
	}
}
