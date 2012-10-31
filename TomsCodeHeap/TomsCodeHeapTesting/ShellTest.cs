using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TomsCodeHeapTesting
{
    using CH.Froorider.Codeheap.Shell;

    /// <summary>
    /// Summary description for ShellTest
    /// </summary>
    [TestClass]
    public class ShellTest
    {

        [TestMethod]
        public void ShellTest_SimpleTesting()
        {
            var shell = new Shell();
            shell.Init();
            shell.ProcessCommands();
        }
    }
}
