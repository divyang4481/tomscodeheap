// ========================================================================
// File:     SignalSchedule.cs
// 
// Author:   $Author$
// Date:     
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

namespace ch.froorider.codeheap.Threading
{
    /// <summary>
    /// This is a trigger using an <see cref="AutoResetEvent"/> as base.
    /// A client gets the reference on the <see cref="ISchedule"/> and can wait
    /// until the AutoResetEvent has been set.
    /// </summary>
    internal class SignalSchedule : ISchedule
    {
        #region fields

        private AutoResetEvent trigger;
        private IScheduler owner;
        private TimeSpan period;
        private bool isEnabled;
        private DateTime nextTimeDue;

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
        internal SignalSchedule(TimeSpan triggerPeriod, IScheduler creator)
        {
            this.trigger = new AutoResetEvent(false);
            this.period = triggerPeriod;
            this.owner = creator;
            this.isEnabled = false;
            this.nextTimeDue = DateTime.MaxValue;
        }

        #endregion

        #region ISchedule Member

        /// <summary>
        /// Gets the <see cref="IScheduler"/> that this <see cref="ISchedule"/> belongs to.
        /// </summary>
        /// <value></value>
        IScheduler ISchedule.Owner
        {
            get { return this.owner; }
        }

        /// <summary>
        /// Gets the period of this <see cref="ISchedule"/>.
        /// </summary>
        /// <value></value>
        TimeSpan ISchedule.Period
        {
            get { return this.period; }
        }

        /// <summary>
        /// Gets the signal that a client of this <see cref="ISchedule"/>
        /// waits for to be signaled periodically.
        /// </summary>
        /// <value></value>
        AutoResetEvent ISchedule.ScheduleSignal
        {
            get { return this.trigger; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ISchedule"/> is currently active.
        /// </summary>
        /// <value></value>
        bool ISchedule.Enabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                this.isEnabled = value;
                if (this.isEnabled)
                {
                    this.nextTimeDue = DateTime.Now + this.period;
                }
                else
                {
                    this.nextTimeDue = DateTime.MaxValue;
                }
            }
        }

        /// <summary>
        /// Gets the next time when <see cref="ISchedule.ScheduleSignal"/> will be <see cref="M:AutoResetEvent.Set"/>;
        /// or <see cref="DateTime.MaxValue"/> if <see cref="ISchedule.Enabled"/> is <see langword="false"/>.
        /// </summary>
        /// <value>A <see cref="DateTime"/> value.</value>
        DateTime ISchedule.NextDueTime
        {
            get { return this.nextTimeDue; }
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
                this.trigger.Close();
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
