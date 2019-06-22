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

        [TestMethod]
        public void ResetWorksAsExpected()
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory);

            // --- Act
            memory.Reset();

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            memory.IsIn8KMode.ShouldBe(false);
            memory.IsInAllRamMode.ShouldBe(false);

            memory.GetSelectedBankIndex(0).ShouldBe(0);
            memory.GetSelectedBankIndex(1).ShouldBe(5);
            memory.GetSelectedBankIndex(2).ShouldBe(2);
            memory.GetSelectedBankIndex(3).ShouldBe(0);

            memory.GetSelectedBankIndex(0, false).ShouldBe(0xFF);
            memory.GetSelectedBankIndex(1, false).ShouldBe(0xFF);
            memory.GetSelectedBankIndex(2, false).ShouldBe(10);
            memory.GetSelectedBankIndex(3, false).ShouldBe(11);
            memory.GetSelectedBankIndex(4, false).ShouldBe(4);
            memory.GetSelectedBankIndex(5, false).ShouldBe(5);
            memory.GetSelectedBankIndex(6, false).ShouldBe(0);
            memory.GetSelectedBankIndex(7, false).ShouldBe(1);
        }

        [TestMethod]
        [DataRow(0, new byte[] { 0xF3, 0xC3, 0xDE }, new byte[] { 0x22, 0xDD, 0x7E })]
        [DataRow(1, new byte[] { 0xC3, 0x00, 0x3F }, new byte[] { 0x1E, 0x60, 0x69 })]
        [DataRow(2, new byte[] { 0x18, 0x72, 0xC3 }, new byte[] { 0xA7, 0xED, 0x52 })]
        [DataRow(3, new byte[] { 0xF3, 0xAF, 0x11 }, new byte[] { 0x0D, 0xCD, 0x79 })]
        public void SelectRomWorksAsExpected(int romIndex, byte[] pattern, byte[] pattern2)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory);

            // --- Act
            memory.SelectRom(romIndex);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            memory.IsIn8KMode.ShouldBe(false);
            memory.IsInAllRamMode.ShouldBe(false);
            memory.GetSelectedRomIndex().ShouldBe(romIndex);
            var contents = memory.GetRomBuffer(romIndex);
            for (var i = 0; i < pattern.Length; i++)
            {
                contents[i].ShouldBe(pattern[i]);
            }
            for (var i = 0; i < pattern2.Length; i++)
            {
                contents[0x2000 + i].ShouldBe(pattern2[i]);
            }
        }

        [TestMethod]
        [DataRow(0, 0, new[] { 0, 5, 2, 0 }, true)]
        [DataRow(1, 2, new[] { 0, 2, 2, 0 }, true)]
        [DataRow(2, 3, new[] { 0, 5, 3, 0 }, true)]
        [DataRow(3, 0, new[] { 0, 5, 2, 0 }, false)]
        [DataRow(3, 1, new[] { 0, 5, 2, 1 }, false)]
        [DataRow(3, 2, new[] { 0, 5, 2, 2 }, false)]
        [DataRow(3, 3, new[] { 0, 5, 2, 3 }, false)]
        [DataRow(3, 4, new[] { 0, 5, 2, 4 }, false)]
        [DataRow(3, 5, new[] { 0, 5, 2, 5 }, false)]
        [DataRow(3, 6, new[] { 0, 5, 2, 6 }, false)]
        [DataRow(3, 7, new[] { 0, 5, 2, 7 }, false)]
        public void PageInWorksWith16KBank(int slot, int bank, int[] pages, bool allRam)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(slot, bank);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            memory.IsIn8KMode.ShouldBe(false);
            memory.IsInAllRamMode.ShouldBe(allRam);
            for (var i = 0; i < pages.Length; i++)
            {
                memory.GetSelectedBankIndex(i).ShouldBe(pages[i]);
                if (!allRam) continue;
                var page = memory.GetRamBank(bank);
                page.Length.ShouldBe(0x4000);
                page[0].ShouldBe((byte)(bank * 2));
                page[0x2000].ShouldBe((byte)(bank * 2 + 1));
            }
        }

        [TestMethod]
        [DataRow(0, 0, 0, new[] { 0, 0xFF, 10, 11, 4, 5, 0, 1 })]
        [DataRow(1, 0, 0, new[] { 0xFF, 0, 10, 11, 4, 5, 0, 1 })]
        [DataRow(2, 13, 13, new[] { 0xFF, 0xFF, 13, 11, 4, 5, 0, 1 })]
        [DataRow(3, 43, 43, new[] { 0xFF, 0xFF, 10, 43, 4, 5, 0, 1 })]
        [DataRow(4, 73, 73, new[] { 0xFF, 0xFF, 10, 11, 73, 5, 0, 1 })]
        [DataRow(5, 0xA3, 0xA3, new[] { 0xFF, 0xFF, 10, 11, 4, 0xA3, 0, 1 })]
        [DataRow(6, 0xA7, 0xA7, new[] { 0xFF, 0xFF, 10, 11, 4, 5, 0xA7, 1 })]
        [DataRow(7, 0xD9, 0xFF, new[] { 0xFF, 0xFF, 10, 11, 4, 5, 0, 0xD9 })]
        public void PageInWorksWith8KBank(int slot, int bank, int resultBytes, int[] pages)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(slot, bank, false);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            memory.IsIn8KMode.ShouldBe(true);
            memory.IsInAllRamMode.ShouldBe(false);
            for (var i = 0; i < pages.Length; i++)
            {
                memory.GetSelectedBankIndex(i, false).ShouldBe(pages[i]);
                if (pages[i] == 0xFF) continue;
                var page = memory.GetRamBank(bank, false);
                page.Length.ShouldBe(0x2000);
                page[0].ShouldBe((byte)resultBytes);
            }
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x2000)]
        [DataRow(0x3FFF, true, 0, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 0, 0x2000)]
        [DataRow(0xFFFF, false, 0, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksAfterReset(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x2000)]
        [DataRow(0x3FFF, true, 0, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 0, 0x2000)]
        [DataRow(0xFFFF, false, 0, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksWithPagingBank0ToSlot3(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 0);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x2000)]
        [DataRow(0x3FFF, true, 0, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 1, 0x0000)]
        [DataRow(0xDFFF, false, 1, 0x1FFF)]
        [DataRow(0xE000, false, 1, 0x2000)]
        [DataRow(0xFFFF, false, 1, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksWithPagingBank1ToSlot3(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 1);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x2000)]
        [DataRow(0x3FFF, true, 0, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 2, 0x0000)]
        [DataRow(0xDFFF, false, 2, 0x1FFF)]
        [DataRow(0xE000, false, 2, 0x2000)]
        [DataRow(0xFFFF, false, 2, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksWithPagingBank2ToSlot3(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 2);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x2000)]
        [DataRow(0x3FFF, true, 0, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 3, 0x0000)]
        [DataRow(0xDFFF, false, 3, 0x1FFF)]
        [DataRow(0xE000, false, 3, 0x2000)]
        [DataRow(0xFFFF, false, 3, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksWithPagingBank3ToSlot3(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 3);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x2000)]
        [DataRow(0x3FFF, true, 0, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 4, 0x0000)]
        [DataRow(0xDFFF, false, 4, 0x1FFF)]
        [DataRow(0xE000, false, 4, 0x2000)]
        [DataRow(0xFFFF, false, 4, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksWithPagingBank4ToSlot3(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 4);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x2000)]
        [DataRow(0x3FFF, true, 0, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 5, 0x0000)]
        [DataRow(0xDFFF, false, 5, 0x1FFF)]
        [DataRow(0xE000, false, 5, 0x2000)]
        [DataRow(0xFFFF, false, 5, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksWithPagingBank5ToSlot3(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 5);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x2000)]
        [DataRow(0x3FFF, true, 0, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 6, 0x0000)]
        [DataRow(0xDFFF, false, 6, 0x1FFF)]
        [DataRow(0xE000, false, 6, 0x2000)]
        [DataRow(0xFFFF, false, 6, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksWithPagingBank6ToSlot3(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 6);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x2000)]
        [DataRow(0x3FFF, true, 0, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 7, 0x0000)]
        [DataRow(0xDFFF, false, 7, 0x1FFF)]
        [DataRow(0xE000, false, 7, 0x2000)]
        [DataRow(0xFFFF, false, 7, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksWithPagingBank7ToSlot3(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 7);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, false, 3, 0x0000)]
        [DataRow(0x1FFF, false, 3, 0x1FFF)]
        [DataRow(0x2000, false, 3, 0x2000)]
        [DataRow(0x3FFF, false, 3, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 0, 0x2000)]
        [DataRow(0xFFFF, false, 0, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksWithPagingBank3ToSlot0(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(0, 3);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, false, 0, 0x0000)]
        [DataRow(0x1FFF, false, 0, 0x1FFF)]
        [DataRow(0x2000, false, 0, 0x2000)]
        [DataRow(0x3FFF, false, 0, 0x3FFF)]
        [DataRow(0x4000, false, 3, 0x0000)]
        [DataRow(0x5FFF, false, 3, 0x1FFF)]
        [DataRow(0x6000, false, 3, 0x2000)]
        [DataRow(0x7FFF, false, 3, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 0, 0x2000)]
        [DataRow(0xFFFF, false, 0, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksWithPagingBank3ToSlot1(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(1, 3);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, false, 0, 0x0000)]
        [DataRow(0x1FFF, false, 0, 0x1FFF)]
        [DataRow(0x2000, false, 0, 0x2000)]
        [DataRow(0x3FFF, false, 0, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 3, 0x0000)]
        [DataRow(0x9FFF, false, 3, 0x1FFF)]
        [DataRow(0xA000, false, 3, 0x2000)]
        [DataRow(0xBFFF, false, 3, 0x3FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 0, 0x2000)]
        [DataRow(0xFFFF, false, 0, 0x3FFF)]
        public void GetAddressLocationWith16KBankWorksWithPagingBank3ToSlot2(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(2, 3);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x2000)]
        [DataRow(0x3FFF, true, 0, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 0, 0x2000)]
        [DataRow(0xFFFF, false, 0, 0x3FFF)]
        public void GetAddressLocationAfterSelectRom0Works(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.SelectRom(0);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 1, 0x0000)]
        [DataRow(0x1FFF, true, 1, 0x1FFF)]
        [DataRow(0x2000, true, 1, 0x2000)]
        [DataRow(0x3FFF, true, 1, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 0, 0x2000)]
        [DataRow(0xFFFF, false, 0, 0x3FFF)]
        public void GetAddressLocationAfterSelectRom1Works(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.SelectRom(1);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 2, 0x0000)]
        [DataRow(0x1FFF, true, 2, 0x1FFF)]
        [DataRow(0x2000, true, 2, 0x2000)]
        [DataRow(0x3FFF, true, 2, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 0, 0x2000)]
        [DataRow(0xFFFF, false, 0, 0x3FFF)]
        public void GetAddressLocationAfterSelectRom2Works(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.SelectRom(2);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 3, 0x0000)]
        [DataRow(0x1FFF, true, 3, 0x1FFF)]
        [DataRow(0x2000, true, 3, 0x2000)]
        [DataRow(0x3FFF, true, 3, 0x3FFF)]
        [DataRow(0x4000, false, 5, 0x0000)]
        [DataRow(0x5FFF, false, 5, 0x1FFF)]
        [DataRow(0x6000, false, 5, 0x2000)]
        [DataRow(0x7FFF, false, 5, 0x3FFF)]
        [DataRow(0x8000, false, 2, 0x0000)]
        [DataRow(0x9FFF, false, 2, 0x1FFF)]
        [DataRow(0xA000, false, 2, 0x2000)]
        [DataRow(0xBFFF, false, 2, 0x3FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 0, 0x2000)]
        [DataRow(0xFFFF, false, 0, 0x3FFF)]
        public void GetAddressLocationAfterSelectRom3Works(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.SelectRom(3);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x0000)]
        [DataRow(0x3FFF, true, 0, 0x1FFF)]
        [DataRow(0x4000, false, 10, 0x0000)]
        [DataRow(0x5FFF, false, 10, 0x1FFF)]
        [DataRow(0x6000, false, 11, 0x0000)]
        [DataRow(0x7FFF, false, 11, 0x1FFF)]
        [DataRow(0x8000, false, 4, 0x0000)]
        [DataRow(0x9FFF, false, 4, 0x1FFF)]
        [DataRow(0xA000, false, 5, 0x0000)]
        [DataRow(0xBFFF, false, 5, 0x1FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 1, 0x0000)]
        [DataRow(0xFFFF, false, 1, 0x1FFF)]
        public void GetAddressLocationWith8KBankWorksAfterReset(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(0, 0xFF, false);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, false, 13, 0x0000)]
        [DataRow(0x1FFF, false, 13, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x0000)]
        [DataRow(0x3FFF, true, 0, 0x1FFF)]
        [DataRow(0x4000, false, 10, 0x0000)]
        [DataRow(0x5FFF, false, 10, 0x1FFF)]
        [DataRow(0x6000, false, 11, 0x0000)]
        [DataRow(0x7FFF, false, 11, 0x1FFF)]
        [DataRow(0x8000, false, 4, 0x0000)]
        [DataRow(0x9FFF, false, 4, 0x1FFF)]
        [DataRow(0xA000, false, 5, 0x0000)]
        [DataRow(0xBFFF, false, 5, 0x1FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 1, 0x0000)]
        [DataRow(0xFFFF, false, 1, 0x1FFF)]
        public void GetAddressLocationWith8KBankWorksAfterPagingIntoSlot0(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(0, 13, false);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, false, 13, 0x0000)]
        [DataRow(0x1FFF, false, 13, 0x1FFF)]
        [DataRow(0x2000, true, 3, 0x0000)]
        [DataRow(0x3FFF, true, 3, 0x1FFF)]
        [DataRow(0x4000, false, 10, 0x0000)]
        [DataRow(0x5FFF, false, 10, 0x1FFF)]
        [DataRow(0x6000, false, 11, 0x0000)]
        [DataRow(0x7FFF, false, 11, 0x1FFF)]
        [DataRow(0x8000, false, 4, 0x0000)]
        [DataRow(0x9FFF, false, 4, 0x1FFF)]
        [DataRow(0xA000, false, 5, 0x0000)]
        [DataRow(0xBFFF, false, 5, 0x1FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 1, 0x0000)]
        [DataRow(0xFFFF, false, 1, 0x1FFF)]
        public void GetAddressLocationWith8KBankWorksAfterSelectRom3AndPagingIntoSlot0(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.SelectRom(3);
            memory.PageIn(0, 13, false);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, false, 13, 0x0000)]
        [DataRow(0x3FFF, false, 13, 0x1FFF)]
        [DataRow(0x4000, false, 10, 0x0000)]
        [DataRow(0x5FFF, false, 10, 0x1FFF)]
        [DataRow(0x6000, false, 11, 0x0000)]
        [DataRow(0x7FFF, false, 11, 0x1FFF)]
        [DataRow(0x8000, false, 4, 0x0000)]
        [DataRow(0x9FFF, false, 4, 0x1FFF)]
        [DataRow(0xA000, false, 5, 0x0000)]
        [DataRow(0xBFFF, false, 5, 0x1FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 1, 0x0000)]
        [DataRow(0xFFFF, false, 1, 0x1FFF)]
        public void GetAddressLocationWith8KBankWorksAfterPagingIntoSlot1(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(1, 13, false);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x0000)]
        [DataRow(0x3FFF, true, 0, 0x1FFF)]
        [DataRow(0x4000, false, 13, 0x0000)]
        [DataRow(0x5FFF, false, 13, 0x1FFF)]
        [DataRow(0x6000, false, 11, 0x0000)]
        [DataRow(0x7FFF, false, 11, 0x1FFF)]
        [DataRow(0x8000, false, 4, 0x0000)]
        [DataRow(0x9FFF, false, 4, 0x1FFF)]
        [DataRow(0xA000, false, 5, 0x0000)]
        [DataRow(0xBFFF, false, 5, 0x1FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 1, 0x0000)]
        [DataRow(0xFFFF, false, 1, 0x1FFF)]
        public void GetAddressLocationWith8KBankWorksAfterPagingIntoSlot2(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(2, 13, false);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x0000)]
        [DataRow(0x3FFF, true, 0, 0x1FFF)]
        [DataRow(0x4000, false, 10, 0x0000)]
        [DataRow(0x5FFF, false, 10, 0x1FFF)]
        [DataRow(0x6000, false, 13, 0x0000)]
        [DataRow(0x7FFF, false, 13, 0x1FFF)]
        [DataRow(0x8000, false, 4, 0x0000)]
        [DataRow(0x9FFF, false, 4, 0x1FFF)]
        [DataRow(0xA000, false, 5, 0x0000)]
        [DataRow(0xBFFF, false, 5, 0x1FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 1, 0x0000)]
        [DataRow(0xFFFF, false, 1, 0x1FFF)]
        public void GetAddressLocationWith8KBankWorksAfterPagingIntoSlot3(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 13, false);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x0000)]
        [DataRow(0x3FFF, true, 0, 0x1FFF)]
        [DataRow(0x4000, false, 10, 0x0000)]
        [DataRow(0x5FFF, false, 10, 0x1FFF)]
        [DataRow(0x6000, false, 11, 0x0000)]
        [DataRow(0x7FFF, false, 11, 0x1FFF)]
        [DataRow(0x8000, false, 23, 0x0000)]
        [DataRow(0x9FFF, false, 23, 0x1FFF)]
        [DataRow(0xA000, false, 5, 0x0000)]
        [DataRow(0xBFFF, false, 5, 0x1FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 1, 0x0000)]
        [DataRow(0xFFFF, false, 1, 0x1FFF)]
        public void GetAddressLocationWith8KBankWorksAfterPagingIntoSlot4(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(4, 23, false);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x0000)]
        [DataRow(0x3FFF, true, 0, 0x1FFF)]
        [DataRow(0x4000, false, 10, 0x0000)]
        [DataRow(0x5FFF, false, 10, 0x1FFF)]
        [DataRow(0x6000, false, 11, 0x0000)]
        [DataRow(0x7FFF, false, 11, 0x1FFF)]
        [DataRow(0x8000, false, 4, 0x0000)]
        [DataRow(0x9FFF, false, 4, 0x1FFF)]
        [DataRow(0xA000, false, 33, 0x0000)]
        [DataRow(0xBFFF, false, 33, 0x1FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 1, 0x0000)]
        [DataRow(0xFFFF, false, 1, 0x1FFF)]
        public void GetAddressLocationWith8KBankWorksAfterPagingIntoSlot5(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(5, 33, false);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x0000)]
        [DataRow(0x3FFF, true, 0, 0x1FFF)]
        [DataRow(0x4000, false, 10, 0x0000)]
        [DataRow(0x5FFF, false, 10, 0x1FFF)]
        [DataRow(0x6000, false, 11, 0x0000)]
        [DataRow(0x7FFF, false, 11, 0x1FFF)]
        [DataRow(0x8000, false, 4, 0x0000)]
        [DataRow(0x9FFF, false, 4, 0x1FFF)]
        [DataRow(0xA000, false, 5, 0x0000)]
        [DataRow(0xBFFF, false, 5, 0x1FFF)]
        [DataRow(0xC000, false, 43, 0x0000)]
        [DataRow(0xDFFF, false, 43, 0x1FFF)]
        [DataRow(0xE000, false, 1, 0x0000)]
        [DataRow(0xFFFF, false, 1, 0x1FFF)]
        public void GetAddressLocationWith8KBankWorksAfterPagingIntoSlot6(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(6, 43, false);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, true, 0, 0x0000)]
        [DataRow(0x1FFF, true, 0, 0x1FFF)]
        [DataRow(0x2000, true, 0, 0x0000)]
        [DataRow(0x3FFF, true, 0, 0x1FFF)]
        [DataRow(0x4000, false, 10, 0x0000)]
        [DataRow(0x5FFF, false, 10, 0x1FFF)]
        [DataRow(0x6000, false, 11, 0x0000)]
        [DataRow(0x7FFF, false, 11, 0x1FFF)]
        [DataRow(0x8000, false, 4, 0x0000)]
        [DataRow(0x9FFF, false, 4, 0x1FFF)]
        [DataRow(0xA000, false, 5, 0x0000)]
        [DataRow(0xBFFF, false, 5, 0x1FFF)]
        [DataRow(0xC000, false, 0, 0x0000)]
        [DataRow(0xDFFF, false, 0, 0x1FFF)]
        [DataRow(0xE000, false, 53, 0x0000)]
        [DataRow(0xFFFF, false, 53, 0x1FFF)]
        public void GetAddressLocationWith8KBankWorksAfterPagingIntoSlot7(int addr, bool isInRom, int index, int resultAddr)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(7, 53, false);
            var addrInfo = memory.GetAddressLocation((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            addrInfo.IsInRom.ShouldBe(isInRom);
            addrInfo.Index.ShouldBe(index);
            addrInfo.Address.ShouldBe((ushort)resultAddr);
        }

        [TestMethod]
        [DataRow(0x0000, 0xF3)]
        [DataRow(0x1FFF, 0x77)]
        [DataRow(0x2000, 0x22)]
        [DataRow(0x3FFF, 0x00)]
        [DataRow(0x4000, 10)]
        [DataRow(0x5FFF, 10)]
        [DataRow(0x6000, 11)]
        [DataRow(0x7FFF, 11)]
        [DataRow(0x8000, 4)]
        [DataRow(0x9FFF, 4)]
        [DataRow(0xA000, 5)]
        [DataRow(0xBFFF, 5)]
        [DataRow(0xC000, 0)]
        [DataRow(0xDFFF, 0)]
        [DataRow(0xE000, 1)]
        [DataRow(0xFFFF, 1)]
        public void ReadAfterResetWorks(int addr, int expectedValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            var value = memory.Read((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)expectedValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0xF3)]
        [DataRow(0x1FFF, 0x77)]
        [DataRow(0x2000, 0x22)]
        [DataRow(0x3FFF, 0x00)]
        [DataRow(0x4000, 10)]
        [DataRow(0x5FFF, 10)]
        [DataRow(0x6000, 11)]
        [DataRow(0x7FFF, 11)]
        [DataRow(0x8000, 4)]
        [DataRow(0x9FFF, 4)]
        [DataRow(0xA000, 5)]
        [DataRow(0xBFFF, 5)]
        [DataRow(0xC000, 12)]
        [DataRow(0xDFFF, 12)]
        [DataRow(0xE000, 13)]
        [DataRow(0xFFFF, 13)]
        public void ReadAfterResetAndPaging16KIntoSlot3Works(int addr, int expectedValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 6);
            var value = memory.Read((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)expectedValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0xF3)]
        [DataRow(0x1FFF, 0x77)]
        [DataRow(0x2000, 0x22)]
        [DataRow(0x3FFF, 0x00)]
        [DataRow(0x4000, 10)]
        [DataRow(0x5FFF, 10)]
        [DataRow(0x6000, 11)]
        [DataRow(0x7FFF, 11)]
        [DataRow(0x8000, 4)]
        [DataRow(0x9FFF, 4)]
        [DataRow(0xA000, 43)]
        [DataRow(0xBFFF, 43)]
        [DataRow(0xC000, 0)]
        [DataRow(0xDFFF, 0)]
        [DataRow(0xE000, 1)]
        [DataRow(0xFFFF, 1)]
        public void ReadAfterResetAndPaging8KIntoSlot5Works(int addr, int expectedValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(5, 43, false);
            var value = memory.Read((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)expectedValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0xF3)]
        [DataRow(0x1FFF, 0x77)]
        [DataRow(0x2000, 0x22)]
        [DataRow(0x3FFF, 0x00)]
        [DataRow(0x4000, 10)]
        [DataRow(0x5FFF, 10)]
        [DataRow(0x6000, 11)]
        [DataRow(0x7FFF, 11)]
        [DataRow(0x8000, 4)]
        [DataRow(0x9FFF, 4)]
        [DataRow(0xA000, 5)]
        [DataRow(0xBFFF, 5)]
        [DataRow(0xC000, 43)]
        [DataRow(0xDFFF, 43)]
        [DataRow(0xE000, 1)]
        [DataRow(0xFFFF, 1)]
        public void ReadAfterResetAndPaging8KIntoSlot6Works(int addr, int expectedValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(6, 43, false);
            var value = memory.Read((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)expectedValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0xF3)]
        [DataRow(0x1FFF, 0x77)]
        [DataRow(0x2000, 0x22)]
        [DataRow(0x3FFF, 0x00)]
        [DataRow(0x4000, 10)]
        [DataRow(0x5FFF, 10)]
        [DataRow(0x6000, 11)]
        [DataRow(0x7FFF, 11)]
        [DataRow(0x8000, 4)]
        [DataRow(0x9FFF, 4)]
        [DataRow(0xA000, 5)]
        [DataRow(0xBFFF, 5)]
        [DataRow(0xC000, 0)]
        [DataRow(0xDFFF, 0)]
        [DataRow(0xE000, 43)]
        [DataRow(0xFFFF, 43)]
        public void ReadAfterResetAndPaging8KIntoSlot7Works(int addr, int expectedValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(7, 43, false);
            var value = memory.Read((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)expectedValue);
        }

        [TestMethod]
        [DataRow(0x0000, 8)]
        [DataRow(0x1FFF, 8)]
        [DataRow(0x2000, 9)]
        [DataRow(0x3FFF, 9)]
        [DataRow(0x4000, 10)]
        [DataRow(0x5FFF, 10)]
        [DataRow(0x6000, 11)]
        [DataRow(0x7FFF, 11)]
        [DataRow(0x8000, 12)]
        [DataRow(0x9FFF, 12)]
        [DataRow(0xA000, 13)]
        [DataRow(0xBFFF, 13)]
        [DataRow(0xC000, 14)]
        [DataRow(0xDFFF, 14)]
        [DataRow(0xE000, 15)]
        [DataRow(0xFFFF, 15)]
        public void ReadWithAllRamModeIgnores8KRead(int addr, int expectedValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();
            memory.PageIn(0, 4);
            memory.PageIn(1, 5);
            memory.PageIn(2, 6);
            memory.PageIn(3, 7);

            // --- Act
            memory.PageIn(3, 43, false);
            var value = memory.Read((ushort)addr);

            // --- Assert
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)expectedValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0x9C, 0xF3)]
        [DataRow(0x1FFF, 0x9C, 0x77)]
        [DataRow(0x2000, 0x9C, 0x22)]
        [DataRow(0x3FFF, 0x9C, 0x00)]
        [DataRow(0x4000, 0x9C, 0x9C)]
        [DataRow(0x5FFF, 0x9D, 0x9D)]
        [DataRow(0x6000, 0x9E, 0x9E)]
        [DataRow(0x7FFF, 0x9F, 0x9F)]
        [DataRow(0x8000, 0x91, 0x91)]
        [DataRow(0x9FFF, 0x92, 0x92)]
        [DataRow(0xA000, 0x93, 0x93)]
        [DataRow(0xBFFF, 0x94, 0x94)]
        [DataRow(0xC000, 0x95, 0x95)]
        [DataRow(0xDFFF, 0x9C, 0x9C)]
        [DataRow(0xE000, 0x9C, 0x9C)]
        [DataRow(0xFFFF, 0x9C, 0x9C)]
        public void WriteAfterResetWorks(int addr, int writeValue, int readValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.Write((ushort)addr, (byte)writeValue);

            // --- Assert
            var value = memory.Read((ushort)addr);
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)readValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0x9C, 0xF3)]
        [DataRow(0x1FFF, 0x9C, 0x77)]
        [DataRow(0x2000, 0x9C, 0x22)]
        [DataRow(0x3FFF, 0x9C, 0x00)]
        [DataRow(0x4000, 0x9C, 0x9C)]
        [DataRow(0x5FFF, 0x9D, 0x9D)]
        [DataRow(0x6000, 0x9E, 0x9E)]
        [DataRow(0x7FFF, 0x9F, 0x9F)]
        [DataRow(0x8000, 0x91, 0x91)]
        [DataRow(0x9FFF, 0x92, 0x92)]
        [DataRow(0xA000, 0x93, 0x93)]
        [DataRow(0xBFFF, 0x94, 0x94)]
        [DataRow(0xC000, 0x95, 0x95)]
        [DataRow(0xDFFF, 0x9C, 0x9C)]
        [DataRow(0xE000, 0x9C, 0x9C)]
        [DataRow(0xFFFF, 0x9C, 0x9C)]
        public void WriteAfterResetAnd16KPagingWorks(int addr, int writeValue, int readValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 6);
            memory.Write((ushort)addr, (byte)writeValue);

            // --- Assert
            var value = memory.Read((ushort)addr);
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)readValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0x9C, 0x9C)]
        [DataRow(0x1FFF, 0x9D, 0x9D)]
        [DataRow(0x2000, 0x9C, 0x22)]
        [DataRow(0x3FFF, 0x9C, 0x00)]
        [DataRow(0x4000, 0x9C, 0x9C)]
        [DataRow(0x5FFF, 0x9D, 0x9D)]
        [DataRow(0x6000, 0x9E, 0x9E)]
        [DataRow(0x7FFF, 0x9F, 0x9F)]
        [DataRow(0x8000, 0x91, 0x91)]
        [DataRow(0x9FFF, 0x92, 0x92)]
        [DataRow(0xA000, 0x93, 0x93)]
        [DataRow(0xBFFF, 0x94, 0x94)]
        [DataRow(0xC000, 0x95, 0x95)]
        [DataRow(0xDFFF, 0x9C, 0x9C)]
        [DataRow(0xE000, 0x9C, 0x9C)]
        [DataRow(0xFFFF, 0x9C, 0x9C)]
        public void WriteAfterResetAnd8KPagingSlot0Works(int addr, int writeValue, int readValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(0, 43, false);
            memory.Write((ushort)addr, (byte)writeValue);

            // --- Assert
            var value = memory.Read((ushort)addr);
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)readValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0x9C, 0xF3)]
        [DataRow(0x1FFF, 0x9C, 0x77)]
        [DataRow(0x2000, 0x9C, 0x9C)]
        [DataRow(0x3FFF, 0x9C, 0x9C)]
        [DataRow(0x4000, 0x9C, 0x9C)]
        [DataRow(0x5FFF, 0x9D, 0x9D)]
        [DataRow(0x6000, 0x9E, 0x9E)]
        [DataRow(0x7FFF, 0x9F, 0x9F)]
        [DataRow(0x8000, 0x91, 0x91)]
        [DataRow(0x9FFF, 0x92, 0x92)]
        [DataRow(0xA000, 0x93, 0x93)]
        [DataRow(0xBFFF, 0x94, 0x94)]
        [DataRow(0xC000, 0x95, 0x95)]
        [DataRow(0xDFFF, 0x9C, 0x9C)]
        [DataRow(0xE000, 0x9C, 0x9C)]
        [DataRow(0xFFFF, 0x9C, 0x9C)]
        public void WriteAfterResetAnd8KPagingSlot1Works(int addr, int writeValue, int readValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(1, 43, false);
            memory.Write((ushort)addr, (byte)writeValue);

            // --- Assert
            var value = memory.Read((ushort)addr);
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)readValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0x9C, 0xF3)]
        [DataRow(0x1FFF, 0x9C, 0x77)]
        [DataRow(0x2000, 0x9C, 0x22)]
        [DataRow(0x3FFF, 0x9C, 0x00)]
        [DataRow(0x4000, 0x9C, 0x9C)]
        [DataRow(0x5FFF, 0x9D, 0x9D)]
        [DataRow(0x6000, 0x9E, 0x9E)]
        [DataRow(0x7FFF, 0x9F, 0x9F)]
        [DataRow(0x8000, 0x91, 0x91)]
        [DataRow(0x9FFF, 0x92, 0x92)]
        [DataRow(0xA000, 0x93, 0x93)]
        [DataRow(0xBFFF, 0x94, 0x94)]
        [DataRow(0xC000, 0x95, 0x95)]
        [DataRow(0xDFFF, 0x9C, 0x9C)]
        [DataRow(0xE000, 0x9C, 0x9C)]
        [DataRow(0xFFFF, 0x9C, 0x9C)]
        public void WriteAfterResetAnd8KPagingSlot2Works(int addr, int writeValue, int readValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(2, 43, false);
            memory.Write((ushort)addr, (byte)writeValue);

            // --- Assert
            var value = memory.Read((ushort)addr);
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)readValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0x9C, 0xF3)]
        [DataRow(0x1FFF, 0x9C, 0x77)]
        [DataRow(0x2000, 0x9C, 0x22)]
        [DataRow(0x3FFF, 0x9C, 0x00)]
        [DataRow(0x4000, 0x9C, 0x9C)]
        [DataRow(0x5FFF, 0x9D, 0x9D)]
        [DataRow(0x6000, 0x9E, 0x9E)]
        [DataRow(0x7FFF, 0x9F, 0x9F)]
        [DataRow(0x8000, 0x91, 0x91)]
        [DataRow(0x9FFF, 0x92, 0x92)]
        [DataRow(0xA000, 0x93, 0x93)]
        [DataRow(0xBFFF, 0x94, 0x94)]
        [DataRow(0xC000, 0x95, 0x95)]
        [DataRow(0xDFFF, 0x9C, 0x9C)]
        [DataRow(0xE000, 0x9C, 0x9C)]
        [DataRow(0xFFFF, 0x9C, 0x9C)]
        public void WriteAfterResetAnd8KPagingSlot3Works(int addr, int writeValue, int readValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(3, 43, false);
            memory.Write((ushort)addr, (byte)writeValue);

            // --- Assert
            var value = memory.Read((ushort)addr);
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)readValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0x9C, 0xF3)]
        [DataRow(0x1FFF, 0x9C, 0x77)]
        [DataRow(0x2000, 0x9C, 0x22)]
        [DataRow(0x3FFF, 0x9C, 0x00)]
        [DataRow(0x4000, 0x9C, 0x9C)]
        [DataRow(0x5FFF, 0x9D, 0x9D)]
        [DataRow(0x6000, 0x9E, 0x9E)]
        [DataRow(0x7FFF, 0x9F, 0x9F)]
        [DataRow(0x8000, 0x91, 0x91)]
        [DataRow(0x9FFF, 0x92, 0x92)]
        [DataRow(0xA000, 0x93, 0x93)]
        [DataRow(0xBFFF, 0x94, 0x94)]
        [DataRow(0xC000, 0x95, 0x95)]
        [DataRow(0xDFFF, 0x9C, 0x9C)]
        [DataRow(0xE000, 0x9C, 0x9C)]
        [DataRow(0xFFFF, 0x9C, 0x9C)]
        public void WriteAfterResetAnd8KPagingSlot4Works(int addr, int writeValue, int readValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(4, 43, false);
            memory.Write((ushort)addr, (byte)writeValue);

            // --- Assert
            var value = memory.Read((ushort)addr);
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)readValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0x9C, 0xF3)]
        [DataRow(0x1FFF, 0x9C, 0x77)]
        [DataRow(0x2000, 0x9C, 0x22)]
        [DataRow(0x3FFF, 0x9C, 0x00)]
        [DataRow(0x4000, 0x9C, 0x9C)]
        [DataRow(0x5FFF, 0x9D, 0x9D)]
        [DataRow(0x6000, 0x9E, 0x9E)]
        [DataRow(0x7FFF, 0x9F, 0x9F)]
        [DataRow(0x8000, 0x91, 0x91)]
        [DataRow(0x9FFF, 0x92, 0x92)]
        [DataRow(0xA000, 0x93, 0x93)]
        [DataRow(0xBFFF, 0x94, 0x94)]
        [DataRow(0xC000, 0x95, 0x95)]
        [DataRow(0xDFFF, 0x9C, 0x9C)]
        [DataRow(0xE000, 0x9C, 0x9C)]
        [DataRow(0xFFFF, 0x9C, 0x9C)]
        public void WriteAfterResetAnd8KPagingSlot5Works(int addr, int writeValue, int readValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(5, 43, false);
            memory.Write((ushort)addr, (byte)writeValue);

            // --- Assert
            var value = memory.Read((ushort)addr);
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)readValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0x9C, 0xF3)]
        [DataRow(0x1FFF, 0x9C, 0x77)]
        [DataRow(0x2000, 0x9C, 0x22)]
        [DataRow(0x3FFF, 0x9C, 0x00)]
        [DataRow(0x4000, 0x9C, 0x9C)]
        [DataRow(0x5FFF, 0x9D, 0x9D)]
        [DataRow(0x6000, 0x9E, 0x9E)]
        [DataRow(0x7FFF, 0x9F, 0x9F)]
        [DataRow(0x8000, 0x91, 0x91)]
        [DataRow(0x9FFF, 0x92, 0x92)]
        [DataRow(0xA000, 0x93, 0x93)]
        [DataRow(0xBFFF, 0x94, 0x94)]
        [DataRow(0xC000, 0x95, 0x95)]
        [DataRow(0xDFFF, 0x9C, 0x9C)]
        [DataRow(0xE000, 0x9C, 0x9C)]
        [DataRow(0xFFFF, 0x9C, 0x9C)]
        public void WriteAfterResetAnd8KPagingSlot6Works(int addr, int writeValue, int readValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(6, 43, false);
            memory.Write((ushort)addr, (byte)writeValue);

            // --- Assert
            var value = memory.Read((ushort)addr);
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)readValue);
        }

        [TestMethod]
        [DataRow(0x0000, 0x9C, 0xF3)]
        [DataRow(0x1FFF, 0x9C, 0x77)]
        [DataRow(0x2000, 0x9C, 0x22)]
        [DataRow(0x3FFF, 0x9C, 0x00)]
        [DataRow(0x4000, 0x9C, 0x9C)]
        [DataRow(0x5FFF, 0x9D, 0x9D)]
        [DataRow(0x6000, 0x9E, 0x9E)]
        [DataRow(0x7FFF, 0x9F, 0x9F)]
        [DataRow(0x8000, 0x91, 0x91)]
        [DataRow(0x9FFF, 0x92, 0x92)]
        [DataRow(0xA000, 0x93, 0x93)]
        [DataRow(0xBFFF, 0x94, 0x94)]
        [DataRow(0xC000, 0x95, 0x95)]
        [DataRow(0xDFFF, 0x9C, 0x9C)]
        [DataRow(0xE000, 0x9C, 0x9C)]
        [DataRow(0xFFFF, 0x9C, 0x9C)]
        public void WriteAfterResetAnd8KPagingSlot7Works(int addr, int writeValue, int readValue)
        {
            // --- Arrange
            var memory = new SpectrumNextMemoryDevice();
            var vm = new SpectrumNextMemoryTestMachine(memory, 2048);
            memory.FillRamWithTestPattern();

            // --- Act
            memory.PageIn(7, 43, false);
            memory.Write((ushort)addr, (byte)writeValue);

            // --- Assert
            var value = memory.Read((ushort)addr);
            vm.MemoryDevice.ShouldBeSameAs(memory);
            value.ShouldBe((byte)readValue);
        }


    }
}
