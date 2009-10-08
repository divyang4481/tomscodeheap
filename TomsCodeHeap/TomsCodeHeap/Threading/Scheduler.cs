// ========================================================================
// File:     Scheduler.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CH.Froorider.Codeheap.Threading
{
	/// <summary>
	/// This class represents a scheduler. Clients can register triggers at the scheduler which signals them
	/// in a defined time interval to wake up.
	/// The class is a singleton, so only one instance is running. The scheduler itself is using his own thread
	/// and does not need a hosting thread.
	/// </summary>
	/// <remarks>Multithreading: This class is thread-safe.</remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
		Justification = "A Scheduler is not a collection but a wrapper around a collection")]
	public class Scheduler : IScheduler, IDisposable
	{
		#region fields

		/// <summary>
		/// Default timespan we wait for a lock until throwing an exception.
		/// </summary>
		private const int WaitOnLockInMilliseconds = 1000;

		/// <summary>
		/// The System.Timer.ElapsedEvent is too fast. So we have to add a jitter. 
		/// </summary>
		private const int JitterTimeInMilliseconds = 12;

		/// <summary>
		/// Used to synchronize the creation of the scheduler.
		/// </summary>
		private static volatile object instanceLocker = new object();

		/// <summary>
		/// This is the internal bookkeeping of the triggers. Contains a reference on all registered triggers.
		/// </summary>
		/// <remarks>
		/// The list contains a reference on all created schedules and is periodically scanned to
		/// look if a signal has to be set.
		/// </remarks>
		private List<ISchedule> triggerList = new List<ISchedule>();

		/// <summary>
		/// The global timer which is used to trigger all the registered events periodically.
		/// </summary>
		private System.Timers.Timer timer;

		/// <summary>
		/// Common object to synchronize the access on the trigger list over multiple threads.
		/// </summary>
		private ReaderWriterLockSlim listLock = new ReaderWriterLockSlim();

		/// <summary>
		/// Common object to synchronize the access on the timer over multiple threads.
		/// </summary>
		private ReaderWriterLockSlim timerLock = new ReaderWriterLockSlim();

		/// <summary>
		/// TimeSpan used for the un-enabled timer. Is one hour.
		/// </summary>
		private TimeSpan disabledTimerSpan = new TimeSpan(1, 0, 0);

		#endregion

		#region Constructors

		/// <summary>
		/// Prevents a default instance of the <see cref="Scheduler"/> class from being created.
		/// Initializes a new instance of the <see cref="Scheduler"/> class.
		/// </summary>
		private Scheduler()
		{
			this.timer = new System.Timers.Timer();
			this.timer.AutoReset = true;
			this.timer.Elapsed += this.Timer_Elapsed;
		}

		#endregion

		#region public methods

		/// <summary>
		/// Factory method which creates an <see cref="IScheduler"/> representation of the Scheduler.
		/// </summary>
		/// <remarks>
		/// This is thread - safe.
		/// </remarks>
		/// <returns>An <see cref="IScheduler"/>. </returns>
		public static IScheduler CreateInstance()
		{
			lock (instanceLocker)
			{
				return new Scheduler();
			}
		}

		#endregion

		#region private methods

		/// <summary>
		/// Handles the Elapsed event of the timer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
		private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			DateTime timerElapsedTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, JitterTimeInMilliseconds));
			Console.WriteLine("Timer elapsed event raised at: " + timerElapsedTime.Ticks);
			try
			{
				if (this.timerLock.TryEnterWriteLock(WaitOnLockInMilliseconds))
				{
					if (this.listLock.TryEnterUpgradeableReadLock(WaitOnLockInMilliseconds))
					{
						foreach (ISchedule currentSchedule in this.triggerList)
						{
							if (currentSchedule.Enabled)
							{
								int comparison = currentSchedule.NextDueTime.CompareTo(timerElapsedTime);

								if (comparison <= 0)
								{
									if (this.listLock.TryEnterWriteLock(WaitOnLockInMilliseconds))
									{
										Console.WriteLine("Signaling schedule with period: " + currentSchedule.Period);
										currentSchedule.ScheduleSignal.Set();
										currentSchedule.Enabled = true;
										this.listLock.ExitWriteLock();
									}
								}
							}
						}
					}
					else
					{
						Console.WriteLine("Couldn't lock the trigger list for reading. Trigger's were not signaled.");
					}
				}
				else
				{
					Console.WriteLine("Couldn't lock the timer. Trigger's were not signaled.");
				}
			}
			finally
			{
				this.timerLock.ExitWriteLock();
				this.listLock.ExitUpgradeableReadLock();
			}
		}

		/// <summary>
		/// Sets the timer properties (period and enabled) manually.
		/// </summary>
		/// <remarks>
		/// This method can be used either to start the timer, stop the timer or manually override the settings determined by
		/// the recalculation method. It is useless to have a running timer, when e.g. the Remove() - method has deleted the last
		/// remaining Schedule in the trigger list. 
		/// It could also be that we want have another period than the calculation would determine. E.g. fix period over all Schedules.
		/// So this can be used to adapt the behaviour of the timer for special cases.
		/// </remarks>
		/// <param name="period">The period the timer should wake-up.</param>
		/// <param name="enableTimer">If <see langword="true"/> the timer is started. If set to <see langword="false"/> the timer is stopped.</param>
		/// <returns>Is <see langword="true"/> if the timer properites could be modified. Is <see langword="false"/> if the timer was blocked.</returns>
		private bool SetTimerProperties(TimeSpan period, bool enableTimer)
		{
			try
			{
				if (this.timerLock.TryEnterWriteLock(WaitOnLockInMilliseconds))
				{
					this.timer.Interval = period.TotalMilliseconds;
					this.timer.Enabled = enableTimer;

					Console.WriteLine("Timer interval set to: " + this.timer.Interval + " ms.");
					Console.WriteLine("Timer enabled: " + this.timer.Enabled + ".");
					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				this.timerLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Recalculates and set's the period when the timer has to wake up.
		/// </summary>
		/// <returns>
		/// 	Is <see langword="true"/> if the recalculation could be done, Is <see langword="false"/> if not.
		/// </returns>
		/// <remarks>
		/// After the recalculation of the timer period, the timer is always enabled!
		/// </remarks>
		private bool RecalculateTimerPeriod()
		{
			try
			{
				if (this.timerLock.TryEnterWriteLock(WaitOnLockInMilliseconds))
				{
					IEnumerable<ISchedule> orderedList = this.triggerList.OrderBy<ISchedule, long>(p => p.Period.Ticks);
					this.timer.Interval = orderedList.FirstOrDefault().Period.TotalMilliseconds;
					this.timer.Enabled = true;

					Console.WriteLine("Timer interval set to: " + this.timer.Interval + " ms.");
					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				this.timerLock.ExitWriteLock();
			}
		}

		#endregion

		#region IScheduler Members

		/// <summary>
		/// Adds a new <see cref="IScheduler"/> with a specified period.
		/// </summary>
		/// <param name="period">The period of the new <see cref="ISchedule"/>.</param>
		/// <param name="enable">If set to <see langword="true"/>, enables the new <see cref="ISchedule"/> immediately.</param>
		/// <returns>
		/// Returns the new <see cref="ISchedule"/>.
		/// </returns>
		/// <exception cref="ArgumentException">Is thrown when the new trigger couldn't be created.</exception>
		public ISchedule Add(TimeSpan period, bool enable)
		{
			ISchedule trigger = new SignalSchedule(period, this);
			trigger.Enabled = enable;

			try
			{
				if (this.listLock.TryEnterWriteLock(WaitOnLockInMilliseconds))
				{
					this.triggerList.Add(trigger);
					if (this.triggerList.Count > 1)
					{
						this.RecalculateTimerPeriod();
					}
					else
					{
						this.SetTimerProperties(trigger.Period, true);
					}
				}
				else
				{
					throw new ArgumentException("Couldn't add the new ISchedule. List is blocked.");
				}
			}
			finally
			{
				this.listLock.ExitWriteLock();
			}

			return trigger;
		}

		/// <summary>
		/// Removes and <see cref="IDisposable.Dispose"/>s the first occurrence of a specific <see cref="ISchedule"/>
		/// from this <see cref="IScheduler"/>.
		/// </summary>
		/// <param name="schedule">The <see cref="ISchedule"/> to remove from this <see cref="IScheduler"/>.</param>
		/// <returns>
		/// 	Is <see langword="true"/> if <paramref name="schedule"/> was successfully removed from this <see cref="IScheduler"/>; otherwise, <see langword="false"/>.
		/// This method also returns false if <paramref name="schedule"/> is not found in this <see cref="IScheduler"/>.
		/// </returns>
		public bool Remove(ISchedule schedule)
		{
			bool removeResult;

			try
			{
				if (this.listLock.TryEnterWriteLock(Scheduler.WaitOnLockInMilliseconds))
				{
					removeResult = this.triggerList.Remove(schedule);
					if (this.triggerList.Count > 0)
					{
						this.RecalculateTimerPeriod();
					}
					else
					{
						this.SetTimerProperties(this.disabledTimerSpan, false);
					}
				}
				else
				{
					removeResult = false;
				}
			}
			finally
			{
				this.listLock.ExitWriteLock();
			}

			return removeResult;
		}

		/// <summary>
		/// Removes the <see cref="ISchedule"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the <see cref="ISchedule"/> to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
		/// </exception>
		public void RemoveAt(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Index must be a positiv number.");
			}

			if (this.triggerList.Count < index + 1)
			{
				throw new ArgumentOutOfRangeException("index", "Index is too big.");
			}

			this.triggerList.Remove(this[index]);
		}

		/// <summary>
		/// Gets the number of <see cref="ISchedule"/>s owned by this <see cref="IScheduler"/>.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The number of <see cref="ISchedule"/>s owned by this <see cref="IScheduler"/>
		/// </returns>
		/// <exception cref="ArgumentException">Is thrown when the trigger list cannot be read because it is modified at the moment.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
			Justification = "Don't know an alternative at the moment.")]
		public int Count
		{
			get
			{
				try
				{
					if (this.listLock.TryEnterReadLock(Scheduler.WaitOnLockInMilliseconds))
					{
						return this.triggerList.Count;
					}
					else
					{
						throw new ArgumentException("Trigger list is blocked at the moment.");
					}
				}
				finally
				{
					this.listLock.ExitReadLock();
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="ISchedule"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get.</param>
		/// <value>The <see cref="ISchedule"/> at the specified index.</value>
		/// <exception cref="ArgumentException">Is thrown when the trigger list cannot be read because it is modified at the moment.</exception>
		public ISchedule this[int index]
		{
			get
			{
				try
				{
					if (this.listLock.TryEnterReadLock(Scheduler.WaitOnLockInMilliseconds))
					{
						return this.triggerList[index];
					}
					else
					{
						throw new ArgumentException("Trigger list is blocked at the moment.");
					}
				}
				finally
				{
					this.listLock.ExitReadLock();
				}
			}
		}

		/// <summary>
		/// Determines whether this <see cref="IScheduler"/> contains a specific <see cref="ISchedule"/>.
		/// </summary>
		/// <param name="schedule">The <see cref="ISchedule"/> to locate in this <see cref="IScheduler"/>.</param>
		/// <returns>
		/// 	Is <see langword="true"/> if <paramref name="schedule"/> is found in this <see cref="IScheduler"/>; otherwise, <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentException">Is thrown when the trigger list cannot be read because it is modified at the moment.</exception>
		public bool Contains(ISchedule schedule)
		{
			try
			{
				if (this.listLock.TryEnterReadLock(Scheduler.WaitOnLockInMilliseconds))
				{
					return this.triggerList.Exists(p => p.Equals(schedule));
				}
				else
				{
					throw new ArgumentException("Trigger list is blocked at the moment.");
				}
			}
			finally
			{
				this.listLock.ExitReadLock();
			}
		}

		/// <summary>
		/// Determines the index of a specific <see cref="ISchedule"/> in this <see cref="IScheduler"/>.
		/// </summary>
		/// <param name="schedule">The <see cref="ISchedule"/> to locate in this <see cref="IScheduler"/>.</param>
		/// <returns>
		/// The index of <paramref name="schedule"/> if found in this <see cref="IScheduler"/>; otherwise, -1.
		/// </returns>
		/// <exception cref="ArgumentException">Is thrown when the trigger list cannot be read because it is modified at the moment.</exception>
		public int IndexOf(ISchedule schedule)
		{
			try
			{
				if (this.listLock.TryEnterReadLock(Scheduler.WaitOnLockInMilliseconds))
				{
					return this.triggerList.FindIndex(p => p.Equals(schedule));
				}
				else
				{
					throw new ArgumentException("Trigger list is blocked at the moment.");
				}
			}
			finally
			{
				this.listLock.ExitReadLock();
			}
		}

		#endregion

		#region IEnumerable<ISchedule> Members

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<ISchedule> GetEnumerator()
		{
			// Use a local copy of the data to be thread-safe.
			ISchedule[] scheduleArray;
			try
			{
				if (this.listLock.TryEnterReadLock(Scheduler.WaitOnLockInMilliseconds))
				{
					scheduleArray = new ISchedule[this.triggerList.Count];
					scheduleArray = this.triggerList.ToArray();
				}
				else
				{
					throw new ApplicationException("Trigger list is blocked at the moment.");
				}
			}
			finally
			{
				this.listLock.ExitReadLock();
			}

			for (int i = 0; i < scheduleArray.Length; i++)
			{
				yield return scheduleArray[i];
			}
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region IDisposable - implementation

		private bool disposed;

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>True</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.timer != null)
				{
					this.timer.Enabled = false;
					this.timer.Dispose();
					this.listLock.Dispose();
					this.timerLock.Dispose();
				}
			}

			this.disposed = true;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="Scheduler"/> class.
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Scheduler"/> is reclaimed by garbage collection.
		/// </summary>
		~Scheduler()
		{
			this.Dispose(false);
		}

		#endregion
	}
}
