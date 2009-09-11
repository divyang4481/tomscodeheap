// ========================================================================
// File:     SignalSchedule.cs
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
using System.Threading;

namespace CH.Froorider.Codeheap.Threading
{
    /// <summary>
    /// This is a trigger using an <see cref="AutoResetEvent"/> as base.
    /// A client gets the reference on the <see cref="ISchedule"/> and can wait
    /// until the AutoResetEvent has been set.
    /// </summary>
    internal class SignalSchedule : ISchedule, IDisposable
    {
        #region fields

		/// <summary>
		/// This synchronize the modifications on the <see cref="M:SignalSchedule.Enabled"/> property.
		/// </summary>
		private static volatile ReaderWriterLockSlim enabledLock = new ReaderWriterLockSlim(); 

        private AutoResetEvent trigger;
        private IScheduler owner;
        private TimeSpan period;
        private bool isEnabled;
        private DateTime nextDueTime;
		
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalSchedule"/> class.
        /// </summary>
        /// <remarks>
        /// Per default this creates a trigger which is not enabled! That means the nextTimeDue is set
        /// to DateTime.MaxValue.
        /// </remarks>
        /// <param name="triggerPeriod">The trigger period.</param>
        /// <param name="creator">The creator or owner of this ISchedule.</param>
		/// <exception cref="ArgumentNullException">Is thrown when the <paramref name="creator"/> is null.</exception>
        internal SignalSchedule(TimeSpan triggerPeriod, IScheduler creator)
        {
			if (creator == null)
			{
				throw new ArgumentNullException("creator", "Cannot contain a null value.");
			}

            this.trigger = new AutoResetEvent(false);
            this.period = triggerPeriod;
            this.owner = creator;
            this.nextDueTime = DateTime.MaxValue;
        }

        #endregion

        #region ISchedule Member

        /// <summary>
        /// Gets the <see cref="IScheduler"/> that this <see cref="ISchedule"/> belongs to.
        /// </summary>
        public IScheduler Owner
        {
            get { return this.owner; }
        }

        /// <summary>
        /// Gets the period of this <see cref="ISchedule"/>.
        /// </summary>
        public TimeSpan Period
        {
            get { return this.period; }
        }

        /// <summary>
        /// Gets the signal that a client of this <see cref="ISchedule"/>
        /// waits for to be signaled periodically.
        /// </summary>
        public AutoResetEvent ScheduleSignal
        {
            get { return this.trigger; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ISchedule"/> is currently active.
        /// </summary>
		/// <remarks>
		/// <para>
		/// Setting Enabled to <see langword="true"/> will set the <see cref="M:SignalSchedule.NextDueTime"/> to <see cref="M:DateTime.Now"/>
		/// plus the period.
		/// Setting Enabled to <see langword="false"/> will set the <see cref="M:SignalSchedule.NextDueTime"/> to <see cref="M:DateTime.MaxValue"/>.
		/// </para>
		/// </remarks>
        public bool Enabled
        {
            get
            {
				SignalSchedule.enabledLock.EnterReadLock();
				try
				{
					return this.isEnabled;
				}
				finally
				{
					SignalSchedule.enabledLock.ExitReadLock();
				}
            }

            set
            {
				SignalSchedule.enabledLock.EnterWriteLock();
				try
				{
					if (this.isEnabled != value)
					{
						if (value)
						{
							this.nextDueTime = DateTime.Now + this.period;
						}
						else
						{
							this.nextDueTime = DateTime.MaxValue;
						}

						this.isEnabled = value;
					}
				}
				finally
				{
					SignalSchedule.enabledLock.ExitWriteLock();
				}
            }
        }

        /// <summary>
        /// Gets the next time when <see cref="ISchedule.ScheduleSignal"/> will be <see cref="M:AutoResetEvent.Set"/>.
        /// </summary>
        /// <value>A <see cref="DateTime"/> value.</value>
        public DateTime NextDueTime
        {
            get { return this.nextDueTime; }
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
				if (disposing)
				{
					this.trigger.Close();
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
		/// Finalizes an instance of the <see cref="SignalSchedule"/> class.
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="SignalSchedule"/> is reclaimed by garbage collection.
		/// </summary>
		~SignalSchedule()
		{
			this.Dispose(false);
		}

        #endregion
    }
}
