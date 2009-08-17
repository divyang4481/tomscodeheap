//========================================================================
//File:     SchedulerTest.cs
//
//Author:   $Author$
//Date:     $LastChangedDate$
//Revision: $Revision$
//========================================================================
//Copyright [2009] [$Author$]
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//========================================================================

using System;
using System.Collections.Generic;
using ch.froorider.codeheap.Testing;
using Ch.Froorider.Codeheap.Persistence;
using CH.Froorider.Codeheap.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TomsCodeHeapTesting
{
    /// <summary>
    ///This is a test class for SchedulerTest and is intended
    ///to contain all SchedulerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SchedulerTest
    {
        #region fields

        private TestContext testContextInstance;
        
        /// <summary>
        /// Instance that is used by all test's in paralell. Because on multicore machines
        /// multiple test's are running in paralell, we lock it sometimes, when we need
        /// distinct results.
        /// </summary>
        private static IScheduler theScheduler = Scheduler.Instance();

        #endregion

        #region Test attributes etc.

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// Cleanup's the test infrastructure.
        /// </summary>
        [ClassCleanup()]
        public static void MyTestCleanup()
        {
            theScheduler.Dispose();
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        /// <summary>
        /// Initialize the test infrastructure.
        /// </summary>
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        #endregion

        #endregion

        #region Constructor / Singleton - Test's

        /// <summary>
        ///A test for the Instance - method. Tet's the singelton pattern.
        ///</summary>
        [TestMethod()]
        public void InstanceTest()
        {
            IScheduler firstInstance = Scheduler.Instance();
            IScheduler secondInstance = Scheduler.Instance();

            string expected = MD5HashGenerator.GenerateKey(firstInstance);
            string actual = MD5HashGenerator.GenerateKey(secondInstance);

            Assert.IsFalse(String.IsNullOrEmpty(expected), "First instance has no key.");
            Assert.IsFalse(String.IsNullOrEmpty(actual), "Second instance has no key.");
            Assert.AreEqual(expected, actual, "Singleton instances are not the same.");
        }

        #endregion

        #region IScheduler - Test's

        /// <summary>
        ///Tet's the add - method and that the timer interval is recalculated.
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            ISchedule createdSchedule = theScheduler.Add(new TimeSpan(0, 0, 5), true);
            Assert.IsTrue(theScheduler.Contains(createdSchedule), "Created schedule couldn't be found");
            
            Console.WriteLine("Next time schedule is signaled: " + createdSchedule.NextDueTime);
            int waitTimeInMilliseconds = Convert.ToInt32(createdSchedule.Period.TotalMilliseconds);
            Console.WriteLine("Waiting for: " + waitTimeInMilliseconds + " ms to be signaled.");

            if (createdSchedule.ScheduleSignal.WaitOne(waitTimeInMilliseconds*2, false))
            {
                Console.WriteLine("Signal was set in expected wait interval.");
            }
            else
            {
                Assert.Fail("Signal was not set in expected wait interval.");
            }

            //We add a new Schedule and look if the timing mechanism is recalculated
            ISchedule fastSchedule = theScheduler.Add(new TimeSpan(0, 0, 1), true);
            Assert.IsTrue(theScheduler.Contains(createdSchedule), "Created schedule couldn't be found");

            if(fastSchedule.ScheduleSignal.WaitOne(1000*2, false))
            {
                Console.WriteLine("Fast signal was set in expected wait interval.");
            }
            else
            {
                Assert.Fail("Fast signal was not set in expected wait interval.");
            }

            theScheduler.Remove(createdSchedule);
            theScheduler.Remove(fastSchedule);
        }

        /// <summary>
        ///Test's the Remove - method.
        ///</summary>
        [TestMethod()]
        public void RemoveTest()
        {
            ISchedule scheduleToRemove = theScheduler.Add(new TimeSpan(0, 0, 11), true);
            Assert.IsTrue(theScheduler.Contains(scheduleToRemove));
            theScheduler.Remove(scheduleToRemove);
            Assert.IsFalse(theScheduler.Contains(scheduleToRemove));
        }

        /// <summary>
        ///Test's the index based removal.
        ///</summary>
        [TestMethod()]
        public void RemoveAtTest()
        {
            ISchedule scheduleToRemove = theScheduler.Add(new TimeSpan(0, 0, 12), true);
            Assert.IsTrue(theScheduler.Contains(scheduleToRemove));
            int scheduleIndex = theScheduler.IndexOf(scheduleToRemove);
            theScheduler.RemoveAt(scheduleIndex);
            Assert.IsFalse(theScheduler.Contains(scheduleToRemove));

            scheduleIndex = -1;
            AssertException.Throws<ArgumentOutOfRangeException,int>(theScheduler.RemoveAt,scheduleIndex);
            scheduleIndex = theScheduler.Count + 1;
            AssertException.Throws<ArgumentOutOfRangeException, int>(theScheduler.RemoveAt, scheduleIndex);
        }

        /// <summary>
        ///A test for Count.
        ///</summary>
        [TestMethod()]
        public void CountTest()
        {
            lock (theScheduler)
            {
                int currentCount = theScheduler.Count;
                Assert.IsTrue(currentCount >= 0);
                
                ISchedule addedSchedule = theScheduler.Add(new TimeSpan(0, 1, 0), false);
                Assert.IsTrue(theScheduler.Count == currentCount + 1);

                theScheduler.Remove(addedSchedule);
                Assert.IsTrue(theScheduler.Count == currentCount);
            }
        }

        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod()]
        public void ContainsTest()
        {
            ISchedule aSchedule = theScheduler.Add(new TimeSpan(0, 0, 15), true);
            Assert.IsTrue(theScheduler.Contains(aSchedule));
            theScheduler.Remove(aSchedule);
            Assert.IsFalse(theScheduler.Contains(aSchedule));
        }

        #endregion

        #region Indexer - Test's

        /// <summary>
        /// Test's the index - based access on the Schedule list.
        ///</summary>
        [TestMethod()]
        public void ItemTest()
        {
            ISchedule aSchedule = theScheduler.Add(new TimeSpan(0, 1, 0), false);
            Assert.IsTrue(theScheduler.Contains(aSchedule), "Created schedule couldn't be found");
            int indexOfSchedule = theScheduler.IndexOf(aSchedule);

            ISchedule givenSchedule = theScheduler[indexOfSchedule];
            Assert.IsNotNull(givenSchedule, "Given schedule is null");
            Assert.AreEqual(givenSchedule, aSchedule,"Schedules are not equal but should be.");
        }

        /// <summary>
        ///A test for IndexOf
        ///</summary>
        [TestMethod()]
        public void IndexOfTest()
        {
            ISchedule aSchedule = theScheduler.Add(new TimeSpan(0, 16, 0), false);
            Assert.IsTrue(theScheduler.Contains(aSchedule), "Created schedule couldn't be found");
            int foundIndexOfSchedule = 0;
            foreach (ISchedule currentSchedule in theScheduler)
            {
                if (currentSchedule.Equals(aSchedule))
                {
                    break;
                }
                else
                {
                    foundIndexOfSchedule++;
                }
            }
            
            int indexOfSchedule = theScheduler.IndexOf(aSchedule);

            Assert.AreEqual(indexOfSchedule, foundIndexOfSchedule, "Indices are not equal.");
        }

        #endregion

        #region Enumerator - Test's

        /// <summary>
        ///A test for GetEnumerator
        ///</summary>
        [TestMethod()]
        public void GetEnumeratorTest()
        {
            IEnumerator<ISchedule> expected = null; 
            IEnumerator<ISchedule> actual;

            List<ISchedule> schedules = new List<ISchedule>();
            lock (theScheduler)
            {
                foreach (ISchedule currentSchedule in theScheduler)
                {
                    schedules.Add(currentSchedule);
                }
                expected = schedules.GetEnumerator();

                actual = theScheduler.GetEnumerator();
                actual.MoveNext();
                do
                {
                    ISchedule currentSchedule = actual.Current;
                    Assert.IsTrue(schedules.Contains(currentSchedule));
                }
                while (actual.MoveNext());
            }
        }

        #endregion

    }
}
