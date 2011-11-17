// ========================================================================
// File:     SimpleHttpServerTest.cs
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

using System;
using CH.Froorider.Codeheap.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TomsCodeHeapTesting
{
	[TestClass]
	public class SimpleHttpServerTest
	{
		[TestMethod]
		public void SimpleHttpServer_CreateAnInstance_InstanceIsCreatedWithoutAnyErrors()
		{
			var mockedServiceEndpoint = new Mock<BaseHttpServiceEndpoint>();
			SimpleHttpServer server = new SimpleHttpServer(mockedServiceEndpoint.Object,12345);
			Assert.IsNotNull(server);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SimpleHttpServer_SanityChecks_NullReferenceThrowsAnArgumentNullException()
		{
			SimpleHttpServer server = new SimpleHttpServer(null, 12345);
			Assert.Fail("Constructor should throw an exception.");
		}

		[TestMethod]
		public void SimpleHttpServer_SanityChecks_WellKnownPortNumbersAreMappedToDefault()
		{
			var mockedServiceEndpoint = new Mock<BaseHttpServiceEndpoint>();
			Random portRandomizer = new Random();

			SimpleHttpServer server = new SimpleHttpServer(mockedServiceEndpoint.Object, portRandomizer.Next(1023));
			Assert.AreEqual(server.ListeningPort, 80);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void SimpleHttpServer_SanityChecks_TooBigPortNumberThrowsException()
		{
			var mockedServiceEndpoint = new Mock<BaseHttpServiceEndpoint>();
			SimpleHttpServer server = new SimpleHttpServer(mockedServiceEndpoint.Object, 65536);
			Assert.Fail("Constructor should throw an exception.");
		}
	}
}
