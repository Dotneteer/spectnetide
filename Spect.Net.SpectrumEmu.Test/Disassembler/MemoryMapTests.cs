using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Disassembler;
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable CollectionNeverUpdated.Local

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

        [TestMethod]
        public void AddWorksWithOverlap1()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x1000));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x1000));

            // --- Assert
            mm.Count.ShouldBe(2);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].Length.ShouldBe((ushort)0x0100);
            mm[1].StartAddress.ShouldBe((ushort)0x0100);
            mm[1].Length.ShouldBe((ushort)0x1000);
        }

        [TestMethod]
        public void AddWorksWithOverlap2()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x1000));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x0200));

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].Length.ShouldBe((ushort)0x0100);
            mm[1].StartAddress.ShouldBe((ushort)0x0100);
            mm[1].Length.ShouldBe((ushort)0x0200);
            mm[2].StartAddress.ShouldBe((ushort)0x0300);
            mm[2].Length.ShouldBe((ushort)0x0D00);
        }

        [TestMethod]
        public void AddWorksWithOverlap3()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x1000));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x1000));

            // --- Assert
            mm.Count.ShouldBe(2);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].Length.ShouldBe((ushort)0x0100);
            mm[1].StartAddress.ShouldBe((ushort)0x0100);
            mm[1].Length.ShouldBe((ushort)0x1000);
        }

        [TestMethod]
        public void AddWorksWithOverlap4()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x0800, MemorySectionType.ByteArray));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x1000));

            // --- Assert
            mm.Count.ShouldBe(1);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].Length.ShouldBe((ushort)0x1000);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void AddWorksWithOverlap5()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0200, 0x0800, MemorySectionType.ByteArray));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x1000));

            // --- Assert
            mm.Count.ShouldBe(1);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].Length.ShouldBe((ushort)0x1000);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void AddWorksWithOverlap6()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0200, 0x0F00, MemorySectionType.ByteArray));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x1000));

            // --- Assert
            mm.Count.ShouldBe(1);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].Length.ShouldBe((ushort)0x1000);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void AddWorksWithOverlap7()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x1000, MemorySectionType.ByteArray));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x1000));

            // --- Assert
            mm.Count.ShouldBe(1);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].Length.ShouldBe((ushort)0x1000);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void AddWorksWithOverlap8()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x200, MemorySectionType.ByteArray));
            mm.Add(new MemorySection(0x0300, 0x100, MemorySectionType.ByteArray));
            mm.Add(new MemorySection(0x0500, 0x100, MemorySectionType.ByteArray));
            mm.Add(new MemorySection(0x0700, 0x1000, MemorySectionType.ByteArray));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x1000));

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].Length.ShouldBe((ushort)0x0100);
            mm[0].SectionType.ShouldBe(MemorySectionType.ByteArray);
            mm[1].StartAddress.ShouldBe((ushort)0x0100);
            mm[1].Length.ShouldBe((ushort)0x1000);
            mm[1].SectionType.ShouldBe(MemorySectionType.Disassemble);
            mm[2].StartAddress.ShouldBe((ushort)0x1100);
            mm[2].Length.ShouldBe((ushort)0x0600);
            mm[2].SectionType.ShouldBe(MemorySectionType.ByteArray);
        }
    }
}
