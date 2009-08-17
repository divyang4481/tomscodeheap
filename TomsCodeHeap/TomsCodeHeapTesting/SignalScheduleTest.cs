using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using CH.Froorider.Codeheap.Threading;

namespace TomsCodeHeapTesting
{
    
    /// <summary>
    ///This is a test class for SignalScheduleTest and is intended
    ///to contain all SignalScheduleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SignalScheduleTest
    {

        #region fields

        private TestContext testContextInstance;
        private static IScheduler theScheduler;

        #endregion

        #region TestContext etc.

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

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            theScheduler = Scheduler.Instance();
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            theScheduler.Dispose();
        }
        
        #region Additional test attributes
        
        
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #endregion

        #region Constructor - Tests

        /// <summary>
        ///A test for SignalSchedule Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("TomsCodeHeap.dll")]
        public void SignalScheduleConstructorTest()
        {
            TimeSpan triggerPeriod = new TimeSpan(0,0,5); 
            SignalSchedule_Accessor target = new SignalSchedule_Accessor(triggerPeriod, theScheduler);
            Assert.AreEqual(triggerPeriod, target.period);
            Assert.AreEqual(theScheduler, target.owner);
            //Assert.AreEqual(DateTime.MaxValue, target.nextTimeDue);
            Assert.IsFalse(target.isEnabled);
        }

        #endregion

        #region ISchedule - Tests

        /// <summary>
        ///Test's the mechanism when a signal schedule is enabled and disabled. Especially if the nextduetime is modified.
        ///</summary>
        [TestMethod()]
        public void EnabledTest()
        {
            TimeSpan triggerPeriod = new TimeSpan(5, 0, 0);
            ISchedule target = theScheduler.Add(triggerPeriod,false);
            Assert.IsFalse(target.Enabled);
            
            target.Enabled = true;
            DateTime nextDueTime = DateTime.Now + target.Period;
            Assert.AreNotEqual(target.NextDueTime, DateTime.MaxValue);
            Assert.AreEqual(target.NextDueTime, nextDueTime);

            target.Enabled = false;
            Assert.AreEqual(target.NextDueTime, DateTime.MaxValue);
        }

        #endregion

    }
}
