using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;
// ReSharper disable InconsistentNaming

namespace Spect.Net.SpectrumEmu.Test.Devices.DivIde
{
    [TestClass]
    public class DivIdeTests
    {
        [TestMethod]
        public void DiIdeInitializesProperly()
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();

            // --- Act
            var divIde = vm.DivIdeDevice;

            // --- Assert
            divIde.ShouldNotBeNull();
            divIde.ConMem.ShouldBeFalse();
            divIde.MapRam.ShouldBeFalse();
            divIde.Bank.ShouldBe(0);
            divIde.IsDivIdePagedIn.ShouldBeFalse();
        }

        [TestMethod]
        [DataRow(0x0000)]
        [DataRow(0x0008)]
        [DataRow(0x0038)]
        [DataRow(0x0066)]
        [DataRow(0x046C)]
        [DataRow(0x0562)]
        [DataRow(0x3D00)]
        [DataRow(0x3DFF)]
        public void CpuEntryPointPagesDivIdeRomIn(int entryPoint)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();

            // --- Act
            vm.Cpu.Registers.PC = (ushort)entryPoint;
            vm.Cpu.ExecuteCpuCycle();

            // --- Assert
            var divIde = vm.DivIdeDevice;
            divIde.ShouldNotBeNull();
            divIde.ConMem.ShouldBeFalse();
            divIde.MapRam.ShouldBeFalse();
            divIde.Bank.ShouldBe(0);
            divIde.IsDivIdePagedIn.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow(0x0001)]
        [DataRow(0x0007)]
        [DataRow(0x0009)]
        [DataRow(0x0037)]
        [DataRow(0x0039)]
        [DataRow(0x0065)]
        [DataRow(0x0067)]
        [DataRow(0x046B)]
        [DataRow(0x046D)]
        [DataRow(0x0561)]
        [DataRow(0x0563)]
        [DataRow(0x3CFF)]
        [DataRow(0x3E00)]
        public void CpuEntryPointDoesNotPagesDivIdeRomIn(int entryPoint)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();

            // --- Act
            vm.Cpu.Registers.PC = (ushort)entryPoint;
            vm.Cpu.ExecuteCpuCycle();

            // --- Assert
            var divIde = vm.DivIdeDevice;
            divIde.ShouldNotBeNull();
            divIde.ConMem.ShouldBeFalse();
            divIde.MapRam.ShouldBeFalse();
            divIde.Bank.ShouldBe(0);
            divIde.IsDivIdePagedIn.ShouldBeFalse();
        }
    }
}
