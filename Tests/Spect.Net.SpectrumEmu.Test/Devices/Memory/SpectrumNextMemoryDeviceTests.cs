using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Devices.Memory
{
    [TestClass]
    public class SpectrumNextMemoryDeviceTests
    {
        [TestMethod]
        [DataRow(0, 16)]
        [DataRow(511, 16)]
        [DataRow(512, 16)]
        [DataRow(1023, 16)]
        [DataRow(1024, 80)]
        [DataRow(1535, 80)]
        [DataRow(1536, 144)]
        [DataRow(2047, 144)]
        [DataRow(2048, 208)]
        [DataRow(4096, 208)]
        public void MemoryIsAttachedToMachineProperly(int memorySize, int ramPages)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, memorySize);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            memory.RamPageCount.ShouldBe(ramPages);
            memory.IsInAllRamMode.ShouldBeFalse();
            memory.IsIn8KMode.ShouldBeFalse();
        }
    }

}
