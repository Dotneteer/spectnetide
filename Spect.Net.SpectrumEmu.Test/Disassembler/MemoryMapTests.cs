using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Disassembler;
// ReSharper disable UseObjectOrCollectionInitializer

namespace Spect.Net.SpectrumEmu.Test.Disassembler
{
    [TestClass]
    public class MemoryMapTests
    {
        [TestMethod]
        public void ConstructorCreatesAnEmptyMap()
        {
            // --- Act
            var mm = new MemoryMap();
            
            // --- Assert
            mm.Count.ShouldBe(0);
        }

        [TestMethod]
        public void AddWorksWithEmptyMap()
        {
            // --- Arrange
            var mm = new MemoryMap();

            // --- Act
            mm.Add(new MemorySection(0x0000, 0x100));

            // --- Assert
            mm.Count.ShouldBe(1);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].Length.ShouldBe((ushort)0x100);
        }

        [TestMethod]
        public void AddWorksWithNoOverlap1()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x1000));
            mm.Add(new MemorySection(0x2000, 0x1000));

            // --- Act
            mm.Add(new MemorySection(0x1000, 0x0100));

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].Length.ShouldBe((ushort)0x1000);
            mm[1].StartAddress.ShouldBe((ushort)0x1000);
            mm[1].Length.ShouldBe((ushort)0x0100);
            mm[2].StartAddress.ShouldBe((ushort)0x2000);
            mm[2].Length.ShouldBe((ushort)0x01000);
        }

        [TestMethod]
        public void AddWorksWithNoOverlap2()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x1000));
            mm.Add(new MemorySection(0x2000, 0x1000));

            // --- Act
            mm.Add(new MemorySection(0x1000, 0x1000));

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].Length.ShouldBe((ushort)0x1000);
            mm[1].StartAddress.ShouldBe((ushort)0x1000);
            mm[1].Length.ShouldBe((ushort)0x1000);
            mm[2].StartAddress.ShouldBe((ushort)0x2000);
            mm[2].Length.ShouldBe((ushort)0x01000);
        }

        [TestMethod]
        public void AddWorksWithNoOverlap3()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x1000));
            mm.Add(new MemorySection(0x2000, 0x1000));

            // --- Act
            mm.Add(new MemorySection(0x3000, 0x1000));

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].Length.ShouldBe((ushort)0x1000);
            mm[1].StartAddress.ShouldBe((ushort)0x2000);
            mm[1].Length.ShouldBe((ushort)0x1000);
            mm[2].StartAddress.ShouldBe((ushort)0x3000);
            mm[2].Length.ShouldBe((ushort)0x01000);
        }

    }
}
