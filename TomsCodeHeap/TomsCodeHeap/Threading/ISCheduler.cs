// ========================================================================
// File:     IScheduler.cs
// 
// Author:   $Author$
// Date:     $LastChangedDate$
// Revision: $Revision: 13 $
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
using System.Collections.Generic;

namespace CH.Froorider.Codeheap.Threading
{
	/// <summary>
	/// A scheduler holds and maintains a list of <see cref="ISchedule"/>s. Client's registrating or
	/// adding <see cref="ISchedule"/>s are signaled, whenever the trigger fires.
	/// The scheduler offers an enumerable interface. So you can loop with a foreach
	/// over all registered <see cref="ISchedule"/>s.
	/// </summary>
	/// <remarks>Multithreading: An implementation of this interface must be thread-safe.</remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection but a wrapper around a collection")]
	public interface IScheduler : IEnumerable<ISchedule>
	{
		#region Properties

		/// <summary>
		/// Gets the number of <see cref="ISchedule"/>s owned by this <see cref="IScheduler"/>.
		/// </summary>
		/// <returns>
		/// The number of <see cref="ISchedule"/>s owned by this <see cref="IScheduler"/>
		/// </returns>
		int Count
		{
			get;
		}

		#endregion Properties

		#region Indexer

		/// <summary>
		/// Gets the <see cref="ISchedule"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get.</param>
		ISchedule this[int index]
		{
			get;
		}

		#endregion Indexer

		#region Methods

		/// <summary>
		/// Adds a new <see cref="IScheduler"/> with a specified period.
		/// </summary>
		/// <param name="period">The period of the new <see cref="ISchedule"/>.</param>
		/// <param name="enable">If set to <see langword="true"/>, enables the new <see cref="ISchedule"/> immediately.</param>
		/// <returns>Returns the new <see cref="ISchedule"/>.</returns>
		ISchedule Add(TimeSpan period, bool enable);

		/// <summary>
		/// Determines whether this <see cref="IScheduler"/> contains a specific <see cref="ISchedule"/>.
		/// </summary>
		/// <param name="schedule">The <see cref="ISchedule"/> to locate in this <see cref="IScheduler"/>.</param>
		/// <returns>
		/// Returns <see langword="true"/> if <paramref name="schedule"/> is found in this <see cref="IScheduler"/>; otherwise, <see langword="false"/>.
		/// </returns>
		bool Contains(ISchedule schedule);

		/// <summary>
		/// Removes and <see cref="IDisposable.Dispose"/>s the first occurrence of a specific <see cref="ISchedule"/> 
		/// from this <see cref="IScheduler"/>.
		/// </summary>
		/// <param name="schedule">The <see cref="ISchedule"/> to remove from this <see cref="IScheduler"/>.</param>
		/// <returns>
		/// Returns <see langword="true"/> if <paramref name="schedule"/> was successfully removed from this <see cref="IScheduler"/>; otherwise, <see langword="false"/>. 
		/// This method also returns false if <paramref name="schedule"/> is not found in this <see cref="IScheduler"/>.
		/// </returns>
		bool Remove(ISchedule schedule);

		/// <summary>
		/// Determines the index of a specific <see cref="ISchedule"/> in this <see cref="IScheduler"/>.
		/// </summary>
		/// <param name="schedule">The <see cref="ISchedule"/> to locate in this <see cref="IScheduler"/>.</param>
		/// <returns>
		/// The index of <paramref name="schedule"/> if found in this <see cref="IScheduler"/>; otherwise, -1.
		/// </returns>
		int IndexOf(ISchedule schedule);

		/// <summary>
		/// Removes the <see cref="ISchedule"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the <see cref="ISchedule"/> to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
		/// </exception>
		void RemoveAt(int index);

		#endregion Methods
	}
}