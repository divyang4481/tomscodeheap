// ========================================================================
// File:     ISchedulerProvider.cs
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

namespace CH.Froorider.Codeheap.Threading
{
	/// <summary>
	/// An ISchedulerProvider is implemented by the class which is responsible for the lifecycle maintenance
	/// of the <see cref="IScheduler"/>. Clients consuming the services of the scheduler do not need to know
	/// anything about the creation and destruction of the concrete scheduler instance. They can just "consume"
	/// an interfaced instance of it.
	/// </summary>
	public interface ISchedulerProvider
	{
		/// <summary>
		/// Gets an instance of an Scheduler as an <see cref="IScheduler"/>.
		/// </summary>
		/// <value>The reference on the <see cref="Scheduler"/>.</value>
		IScheduler Scheduler { get; }
	}
}
