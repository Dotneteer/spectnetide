using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
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
            var divIde = vm.DivIdeDevice;
            divIde.EnableAutoMapping = true;

            // --- Act
            vm.Cpu.Registers.PC = (ushort)entryPoint;
            vm.Cpu.ExecuteCpuCycle();

            // --- Assert
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

        [TestMethod]
        [DataRow(0x1FF8)]
        [DataRow(0x1FF9)]
        [DataRow(0x1FFA)]
        [DataRow(0x1FFB)]
        [DataRow(0x1FFC)]
        [DataRow(0x1FFD)]
        [DataRow(0x1FFE)]
        [DataRow(0x1FFF)]
        public void CpuEntryPointPagesOutDivIdeRomIn(int entryPoint)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            divIde.EnableAutoMapping = true;
            vm.Cpu.Registers.PC = 0x0000;
            vm.Cpu.ExecuteCpuCycle();
            var before = divIde.IsDivIdePagedIn;

            // --- Act
            vm.Cpu.Registers.PC = (ushort)entryPoint;
            vm.Cpu.ExecuteCpuCycle();

            // --- Assert
            before.ShouldBeTrue();
            divIde.ConMem.ShouldBeFalse();
            divIde.MapRam.ShouldBeFalse();
            divIde.Bank.ShouldBe(0);
            divIde.IsDivIdePagedIn.ShouldBeFalse();
        }

        [TestMethod]
        [DataRow(0x0020)]
        [DataRow(0x1FF0)]
        [DataRow(0x2000)]
        public void CpuEntryPointDoesNotPagesOutDivIdeRomIn(int entryPoint)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            divIde.EnableAutoMapping = true;
            vm.Cpu.Registers.PC = 0x0000;
            vm.Cpu.ExecuteCpuCycle();
            var before = divIde.IsDivIdePagedIn;

            // --- Act
            vm.Cpu.Registers.PC = (ushort)entryPoint;
            vm.Cpu.ExecuteCpuCycle();

            // --- Assert
            before.ShouldBeTrue();
            divIde.ConMem.ShouldBeFalse();
            divIde.MapRam.ShouldBeFalse();
            divIde.Bank.ShouldBe(0);
            divIde.IsDivIdePagedIn.ShouldBeTrue();
        }

        [TestMethod]
        public void CpuEntryPointPageInAfterResetUsesRamBank0()
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            divIde.EnableAutoMapping = true;
            var testMemDevice = (IMemoryDivIdeTestSupport) vm.MemoryDevice;
            vm.Cpu.Registers.PC = 0x0000;
            vm.Cpu.ExecuteCpuCycle();

            // --- Act
            testMemDevice[0, 0x0000] = 0xCA; // 0x2000

            // --- Assert
            divIde.Bank.ShouldBe(0);
            var value = vm.MemoryDevice.Read(0x2000, true);
            value.ShouldBe((byte)0xCA);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void PagedInDivIdeRamBankIsNotActivated(int bankNo)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            var testMemDevice = (IMemoryDivIdeTestSupport)vm.MemoryDevice;
            vm.Cpu.Registers.PC = 0x1000;
            vm.Cpu.ExecuteCpuCycle();

            // --- Act
            divIde.SetControlRegister((byte)bankNo);
            testMemDevice[bankNo, 0x0000] = 0xCA; // 0x2000

            // --- Assert
            divIde.Bank.ShouldBe(bankNo);
            var value = vm.MemoryDevice.Read(0x2000, true);
            value.ShouldNotBe((byte)0xCA);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void DivIdeRamBankIsActivatedAfterPageIn(int bankNo)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            divIde.EnableAutoMapping = true;
            var testMemDevice = (IMemoryDivIdeTestSupport)vm.MemoryDevice;
            vm.Cpu.Registers.PC = 0x1000;
            vm.Cpu.ExecuteCpuCycle();

            // --- Act
            divIde.SetControlRegister((byte)bankNo);
            vm.Cpu.Registers.PC = 0x3D00;
            vm.Cpu.ExecuteCpuCycle();

            testMemDevice[bankNo, 0x0000] = 0xCA; // 0x2000

            // --- Assert
            divIde.Bank.ShouldBe(bankNo);
            var value = vm.MemoryDevice.Read(0x2000, true);
            value.ShouldBe((byte)0xCA);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void DivIdeRamBankIsInactiveAfterPageOut(int bankNo)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            var testMemDevice = (IMemoryDivIdeTestSupport)vm.MemoryDevice;
            vm.Cpu.Registers.PC = 0x1000;
            vm.Cpu.ExecuteCpuCycle();

            // --- Act
            divIde.SetControlRegister((byte)bankNo);
            vm.Cpu.Registers.PC = 0x3D00;
            vm.Cpu.ExecuteCpuCycle();             // Page in
            testMemDevice[bankNo, 0x0000] = 0xCA; // 0x2000
            vm.Cpu.Registers.PC = 0x1FF8;
            vm.Cpu.ExecuteCpuCycle();             // Page out

            // --- Assert
            divIde.Bank.ShouldBe(bankNo);
            var value = vm.MemoryDevice.Read(0x2000, true);
            value.ShouldNotBe((byte)0xCA);
        }

        [TestMethod]
        [DataRow(0x40)]
        [DataRow(0x41)]
        [DataRow(0x42)]
        [DataRow(0x43)]
        public void DivIdeRamBankWithMapRamIsNotActivated(int bankNo)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            var testMemDevice = (IMemoryDivIdeTestSupport)vm.MemoryDevice;
            vm.Cpu.Registers.PC = 0x1000;
            vm.Cpu.ExecuteCpuCycle();

            // --- Act
            divIde.SetControlRegister((byte)bankNo);
            testMemDevice[bankNo, 0x0000] = 0xCA; // 0x2000

            // --- Assert
            divIde.Bank.ShouldBe(bankNo & 0x03);
            var value = vm.MemoryDevice.Read(0x2000, true);
            value.ShouldNotBe((byte)0xCA);
        }

        [TestMethod]
        [DataRow(0x40)]
        [DataRow(0x41)]
        [DataRow(0x42)]
        [DataRow(0x43)]
        public void DivIdeRamBankWithMapRamIsActivatedAfterPageIn(int bankNo)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            divIde.EnableAutoMapping = true;
            var testMemDevice = (IMemoryDivIdeTestSupport)vm.MemoryDevice;
            vm.Cpu.Registers.PC = 0x1000;
            vm.Cpu.ExecuteCpuCycle();

            // --- Act
            divIde.SetControlRegister((byte)bankNo);
            vm.Cpu.Registers.PC = 0x3D00;
            vm.Cpu.ExecuteCpuCycle();

            testMemDevice[bankNo, 0x0000] = 0xCA; // 0x2000

            // --- Assert
            divIde.Bank.ShouldBe(bankNo &0x03);
            var value = vm.MemoryDevice.Read(0x2000, true);
            value.ShouldBe((byte)0xCA);
            var bank3Value = vm.MemoryDevice.Read(0x1000, true);
            bank3Value.ShouldBe((byte)0x00);
        }

        [TestMethod]
        [DataRow(0x40)]
        [DataRow(0x41)]
        [DataRow(0x42)]
        [DataRow(0x43)]
        public void DivIdeRamBankWithMapRamIsInactiveAfterPageOut(int bankNo)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            var testMemDevice = (IMemoryDivIdeTestSupport)vm.MemoryDevice;
            vm.Cpu.Registers.PC = 0x1000;
            vm.Cpu.ExecuteCpuCycle();

            // --- Act
            divIde.SetControlRegister((byte)bankNo);
            vm.Cpu.Registers.PC = 0x3D00;
            vm.Cpu.ExecuteCpuCycle();             // Page in
            testMemDevice[bankNo, 0x0000] = 0xCA; // 0x2000
            vm.Cpu.Registers.PC = 0x1FF8;
            vm.Cpu.ExecuteCpuCycle();             // Page out

            // --- Assert
            divIde.Bank.ShouldBe(bankNo & 0x03);
            var value = vm.MemoryDevice.Read(0x2000, true);
            value.ShouldNotBe((byte)0xCA);
            var bank3Value = vm.MemoryDevice.Read(0x1000, true);
            bank3Value.ShouldNotBe((byte)0x00);
        }

        [TestMethod]
        [DataRow(0x40)]
        [DataRow(0x41)]
        [DataRow(0x42)]
        [DataRow(0x43)]
        public void DivIdeRamBank3IsWriteProtected(int bankNo)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            divIde.EnableAutoMapping = true;
            var testMemDevice = (IMemoryDivIdeTestSupport)vm.MemoryDevice;
            testMemDevice[3, 0x1000] = 0x98; // 0x1000
            vm.Cpu.Registers.PC = 0x1000;
            vm.Cpu.ExecuteCpuCycle();
            divIde.SetControlRegister((byte)bankNo);
            vm.Cpu.Registers.PC = 0x3D00;
            vm.Cpu.ExecuteCpuCycle();

            // --- Act
            vm.MemoryDevice.Write(0x1000, 0x43);

            // --- Assert
            divIde.Bank.ShouldBe(bankNo &0x03);
            var value = vm.MemoryDevice.Read(0x1000, true);
            value.ShouldBe((byte)0x98);
        }

        [TestMethod]
        [DataRow(0x80)]
        [DataRow(0x81)]
        [DataRow(0x82)]
        [DataRow(0x83)]
        public void ConMemActivatesDivIdeRamBank(int bankNo)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            var testMemDevice = (IMemoryDivIdeTestSupport)vm.MemoryDevice;
            testMemDevice[bankNo, 0x0000] = 0xCA; // 0x2000
            vm.Cpu.Registers.PC = 0x1000;
            vm.Cpu.ExecuteCpuCycle();

            // --- Act
            divIde.SetControlRegister((byte)bankNo);

            // --- Assert
            divIde.Bank.ShouldBe(bankNo & 0x03);
            divIde.ConMem.ShouldBeTrue();
            var value = vm.MemoryDevice.Read(0x2000, true);
            value.ShouldBe((byte)0xCA);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void ConMemInactivatesDivIdeRamBank(int bankNo)
        {
            // --- Arrange
            var vm = new SpectrumNextDivIdeTestMachine();
            var divIde = vm.DivIdeDevice;
            divIde.EnableAutoMapping = true;
            var testMemDevice = (IMemoryDivIdeTestSupport)vm.MemoryDevice;
            testMemDevice[bankNo, 0x0000] = 0xCA; // 0x2000
            divIde.SetControlRegister((byte)bankNo);
            vm.Cpu.Registers.PC = 0x0008;
            vm.Cpu.ExecuteCpuCycle();
            var valueBefore = vm.MemoryDevice.Read(0x2000, true);

            // --- Act
            divIde.SetControlRegister((byte)bankNo);
            vm.Cpu.ExecuteCpuCycle();

            // --- Assert
            divIde.Bank.ShouldBe(bankNo & 0x03);
            valueBefore.ShouldBe((byte)0xCA);
            var value = vm.MemoryDevice.Read(0x2000, true);
            value.ShouldNotBe((byte)0xCA);
        }
    }
}
