using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.VsPackage.ToolWindows.Disassembly;

namespace Spect.Net.VsPackage.Test.Tools.Disassembly
{
    [TestClass]
    public class DisassemblyToolWindowViewModelTests
    {
        [TestMethod]
        public void VmConstructionWorks()
        {
            // --- Act
            var vm = new DisassemblyToolWindowViewModel();
        }
    }
}
