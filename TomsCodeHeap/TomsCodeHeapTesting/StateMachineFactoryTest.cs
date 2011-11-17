using CH.Froorider.Codeheap.StateMachine;
using CH.Froorider.Codeheap.StateMachine.States;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TomsCodeHeapTesting
{
    /// <summary>
    ///This is a test class for StateMachineFactoryTest and is intended
    ///to contain all StateMachineFactoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StateMachineFactoryTest
    {
        /// <summary>
        ///A test for CreateStateMachine
        ///</summary>
        [TestMethod()]
        public void CreateStateMachine_StandardOperation_MachineIsCreatedandInitialized()
        {
            var startState = new Mock<IState>();
            string name = "Testmachine";
            IStateMachine actual = StateMachineFactory.CreateStateMachine(this, startState.Object, name);
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.ActualState, startState.Object);
        }
    }
}
