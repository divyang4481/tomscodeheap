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
	/// This class represents a scheduler. Cleints can register triggers at the scheduler which signals them
	/// in a defined time interval to wake up.
	/// The class is a singleton, so only one instance is running. The scheduler itself is using his own thread
	/// and does not need a hosting thread.
	/// </summary>
	/// <remarks>Multithreading: This class is thread-safe.</remarks>
	[Serializable()]
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
		/// The reference on the one and only instance of this class.
		/// </summary>
		[NonSerialized]
		private static volatile Scheduler instance;

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
		private static volatile List<ISchedule> triggerList = new List<ISchedule>();

		/// <summary>
		/// The global timer which is used to trigger all the registered events periodically.
		/// </summary>
		private static volatile System.Timers.Timer timer;

		/// <summary>
		/// Common object to synchronize the access on the trigger list over multiple threads.
		/// </summary>
		private static ReaderWriterLockSlim listLock = new ReaderWriterLockSlim();

		/// <summary>
		/// Common object to synchronize the access on the timer over multiple threads.
		/// </summary>
		private static ReaderWriterLockSlim timerLock = new ReaderWriterLockSlim();

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
			timer = new System.Timers.Timer();
			timer.AutoReset = true;
			timer.Elapsed += Timer_Elapsed;
		}

		#endregion

		#region public methods

		/// <summary>
		/// Gives you access on the <see cref="IScheduler"/> representation of the Scheduler.
		/// </summary>
		/// <remarks>
		/// This singleton creator uses double-locking with explicit memory barries to ensure that
		/// this is thread-safe in a multi-threaded and multi-core environment. 
		/// </remarks>
		/// <returns>An <see cref="IScheduler"/>. </returns>
		public static IScheduler Instance()
		{
			if (Scheduler.instance == null)
			{
				lock (instanceLocker)
				{
					if (Scheduler.instance == null)
					{
						Scheduler.instance = new Scheduler();
					}
				}
			}

			return Scheduler.instance;
		}

		#endregion

		#region private methods

		/// <summary>
		/// Handles the Elapsed event of the timer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
		private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			DateTime timerElapsedTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, JitterTimeInMilliseconds));
			Console.WriteLine("Timer elapsed event raised at: " + timerElapsedTime.Ticks);
			try
			{
				if (timerLock.TryEnterWriteLock(WaitOnLockInMilliseconds))
				{
					if (listLock.TryEnterUpgradeableReadLock(WaitOnLockInMilliseconds))
					{
						foreach (ISchedule currentSchedule in triggerList)
						{
							if (currentSchedule.Enabled)
							{
								int comparison = currentSchedule.NextDueTime.CompareTo(timerElapsedTime);

								if (comparison <= 0)
								{
									if (listLock.TryEnterWriteLock(WaitOnLockInMilliseconds))
									{
										Console.WriteLine("Signaling schedule with period: " + currentSchedule.Period);
										currentSchedule.ScheduleSignal.Set();
										currentSchedule.Enabled = true;
										listLock.ExitWriteLock();
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
				timerLock.ExitWriteLock();
				listLock.ExitUpgradeableReadLock();
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
				if (timerLock.TryEnterWriteLock(WaitOnLockInMilliseconds))
				{
					timer.Interval = period.TotalMilliseconds;
					timer.Enabled = enableTimer;

					Console.WriteLine("Timer interval set to: " + timer.Interval + " ms.");
					Console.WriteLine("Timer enabled: " + timer.Enabled + ".");
					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				timerLock.ExitWriteLock();
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
				if (timerLock.TryEnterWriteLock(WaitOnLockInMilliseconds))
				{
					IEnumerable<ISchedule> orderedList = triggerList.OrderBy<ISchedule, long>(p => p.Period.Ticks);
					timer.Interval = orderedList.FirstOrDefault().Period.TotalMilliseconds;
					timer.Enabled = true;

					Console.WriteLine("Timer interval set to: " + timer.Interval + " ms.");
					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				timerLock.ExitWriteLock();
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
		/// <exception cref="ApplicationException">Is thrown when the new trigger couldn't be created.</exception>
		public ISchedule Add(TimeSpan period, bool enable)
		{
			ISchedule trigger = new SignalSchedule(period, this);
			trigger.Enabled = enable;

			try
			{
				if (listLock.TryEnterWriteLock(WaitOnLockInMilliseconds))
				{
					Scheduler.triggerList.Add(trigger);
					if (Scheduler.triggerList.Count > 1)
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
					throw new ApplicationException("Couldn't add the new ISchedule. List is blocked.");
				}
			}
			finally
			{
				listLock.ExitWriteLock();
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
				if (listLock.TryEnterWriteLock(Scheduler.WaitOnLockInMilliseconds))
				{
					removeResult = Scheduler.triggerList.Remove(schedule);
					if (Scheduler.triggerList.Count > 0)
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
				listLock.ExitWriteLock();
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

			if (Scheduler.triggerList.Count < index + 1)
			{
				throw new ArgumentOutOfRangeException("index", "Index is too big.");

			}

			Scheduler.triggerList.Remove(this[index]);
		}

		/// <summary>
		/// Gets the number of <see cref="ISchedule"/>s owned by this <see cref="IScheduler"/>.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The number of <see cref="ISchedule"/>s owned by this <see cref="IScheduler"/>
		/// </returns>
		/// <exception cref="ApplicationException">Is thrown when the trigger list cannot be read because it is modified at the moment.</exception>
		public int Count
		{
			get
			{
				try
				{
					if (Scheduler.listLock.TryEnterReadLock(Scheduler.WaitOnLockInMilliseconds))
					{
						return Scheduler.triggerList.Count;
					}
					else
					{
						throw new ApplicationException("Trigger list is blocked at the moment.");
					}
				}
				finally
				{
					Scheduler.listLock.ExitReadLock();
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="ISchedule"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get.</param>
		/// <value>The <see cref="ISchedule"/> at the specified index.</value>
		/// <exception cref="ApplicationException">Is thrown when the trigger list cannot be read because it is modified at the moment.</exception>
		public ISchedule this[int index]
		{
			get
			{
				try
				{
					if (Scheduler.listLock.TryEnterReadLock(Scheduler.WaitOnLockInMilliseconds))
					{
						return Scheduler.triggerList[index];
					}
					else
					{
						throw new ApplicationException("Trigger list is blocked at the moment.");
					}
				}
				finally
				{
					Scheduler.listLock.ExitReadLock();
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
		/// <exception cref="ApplicationException">Is thrown when the trigger list cannot be read because it is modified at the moment.</exception>
		public bool Contains(ISchedule schedule)
		{
			try
			{
				if (Scheduler.listLock.TryEnterReadLock(Scheduler.WaitOnLockInMilliseconds))
				{
					return Scheduler.triggerList.Exists(p => p.Equals(schedule));
				}
				else
				{
					throw new ApplicationException("Trigger list is blocked at the moment.");
				}
			}
			finally
			{
				Scheduler.listLock.ExitReadLock();
			}
		}

		/// <summary>
		/// Determines the index of a specific <see cref="ISchedule"/> in this <see cref="IScheduler"/>.
		/// </summary>
		/// <param name="schedule">The <see cref="ISchedule"/> to locate in this <see cref="IScheduler"/>.</param>
		/// <returns>
		/// The index of <paramref name="schedule"/> if found in this <see cref="IScheduler"/>; otherwise, -1.
		/// </returns>
		/// <exception cref="ApplicationException">Is thrown when the trigger list cannot be read because it is modified at the moment.</exception>
		public int IndexOf(ISchedule schedule)
		{
			try
			{
				if (Scheduler.listLock.TryEnterReadLock(Scheduler.WaitOnLockInMilliseconds))
				{
					return Scheduler.triggerList.FindIndex(p => p.Equals(schedule));
				}
				else
				{
					throw new ApplicationException("Trigger list is blocked at the moment.");
				}
			}
			finally
			{
				Scheduler.listLock.ExitReadLock();
			}
		}

		#endregion

		#region IEnumerable<ISchedule> Members

		/// <summary>
		/// Gibt einen Enumerator zurück, der die Auflistung durchläuft.
		/// </summary>
		/// <returns>
		/// Ein <see cref="T:System.Collections.Generic.IEnumerator`1"/>, der zum Durchlaufen der Auflistung verwendet werden kann.
		/// </returns>
		public IEnumerator<ISchedule> GetEnumerator()
		{
			return Scheduler.triggerList.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Gibt einen Enumerator zurück, der eine Auflistung durchläuft.
		/// </summary>
		/// <returns>
		/// Ein <see cref="T:System.Collections.IEnumerator"/>-Objekt, das zum Durchlaufen der Auflistung verwendet werden kann.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region IDisposable - implementation

		/// <summary>
		/// Disposes the unmanaged ressources.
		/// </summary>
		/// <param name="disposing">Always true.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				timer.Dispose();
			}
		}

		/// <summary>
		/// Dispose - pattern.
		/// Call base.Dispose() first if possible, then the Dispose of the class and finally
		/// Supress the GC-Finalizer.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
