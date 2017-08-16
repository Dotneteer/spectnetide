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
            mm[0].EndAddress.ShouldBe((ushort)0x100);
        }

        [TestMethod]
        public void AddWorksWithNoOverlap1()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x0FFF));
            mm.Add(new MemorySection(0x2000, 0x2FFF));

            // --- Act
            mm.Add(new MemorySection(0x1000, 0x10FF));

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].EndAddress.ShouldBe((ushort)0x0FFF);
            mm[1].StartAddress.ShouldBe((ushort)0x1000);
            mm[1].EndAddress.ShouldBe((ushort)0x10FF);
            mm[2].StartAddress.ShouldBe((ushort)0x2000);
            mm[2].EndAddress.ShouldBe((ushort)0x02FFF);
        }

        [TestMethod]
        public void AddWorksWithNoOverlap2()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x0FFF));
            mm.Add(new MemorySection(0x2000, 0x2FFF));

            // --- Act
            mm.Add(new MemorySection(0x1000, 0x1FFF));

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].EndAddress.ShouldBe((ushort)0x0FFF);
            mm[1].StartAddress.ShouldBe((ushort)0x1000);
            mm[1].EndAddress.ShouldBe((ushort)0x1FFF);
            mm[2].StartAddress.ShouldBe((ushort)0x2000);
            mm[2].EndAddress.ShouldBe((ushort)0x02FFF);
        }

        [TestMethod]
        public void AddWorksWithNoOverlap3()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x0FFF));
            mm.Add(new MemorySection(0x2000, 0x2FFF));

            // --- Act
            mm.Add(new MemorySection(0x3000, 0x3FFF));

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].EndAddress.ShouldBe((ushort)0x0FFF);
            mm[1].StartAddress.ShouldBe((ushort)0x2000);
            mm[1].EndAddress.ShouldBe((ushort)0x2FFF);
            mm[2].StartAddress.ShouldBe((ushort)0x3000);
            mm[2].EndAddress.ShouldBe((ushort)0x3FFF);
        }

        [TestMethod]
        public void AddWorksWithOverlap1()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x0FFF));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x10FF));

            // --- Assert
            mm.Count.ShouldBe(2);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].EndAddress.ShouldBe((ushort)0x00FF);
            mm[1].StartAddress.ShouldBe((ushort)0x0100);
            mm[1].EndAddress.ShouldBe((ushort)0x10FF);
        }

        [TestMethod]
        public void AddWorksWithOverlap2()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x0FFF));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x02FF));

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].EndAddress.ShouldBe((ushort)0x00FF);
            mm[1].StartAddress.ShouldBe((ushort)0x0100);
            mm[1].EndAddress.ShouldBe((ushort)0x02FF);
            mm[2].StartAddress.ShouldBe((ushort)0x0300);
            mm[2].EndAddress.ShouldBe((ushort)0x0FFF);
        }

        [TestMethod]
        public void AddWorksWithOverlap3()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x0FFF));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x10FF));

            // --- Assert
            mm.Count.ShouldBe(2);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].EndAddress.ShouldBe((ushort)0x00FF);
            mm[1].StartAddress.ShouldBe((ushort)0x0100);
            mm[1].EndAddress.ShouldBe((ushort)0x10FF);
        }

        [TestMethod]
        public void AddWorksWithOverlap4()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x08FF, MemorySectionType.ByteArray));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x10FF));

            // --- Assert
            mm.Count.ShouldBe(1);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x10FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void AddWorksWithOverlap5()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0200, 0x08FF, MemorySectionType.ByteArray));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x10FF));

            // --- Assert
            mm.Count.ShouldBe(1);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x10FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void AddWorksWithOverlap6()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0200, 0x0F00, MemorySectionType.ByteArray));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x0F00));

            // --- Assert
            mm.Count.ShouldBe(1);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x0F00);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void AddWorksWithOverlap7()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x10FF, MemorySectionType.ByteArray));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x10FF));

            // --- Assert
            mm.Count.ShouldBe(1);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x10FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void AddWorksWithOverlap8()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0000, 0x01FF, MemorySectionType.ByteArray));
            mm.Add(new MemorySection(0x0300, 0x03FF, MemorySectionType.ByteArray));
            mm.Add(new MemorySection(0x0500, 0x05FF, MemorySectionType.ByteArray));
            mm.Add(new MemorySection(0x0700, 0x16FF, MemorySectionType.ByteArray));

            // --- Act
            mm.Add(new MemorySection(0x0100, 0x10FF));

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0000);
            mm[0].EndAddress.ShouldBe((ushort)0x00FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.ByteArray);
            mm[1].StartAddress.ShouldBe((ushort)0x0100);
            mm[1].EndAddress.ShouldBe((ushort)0x10FF);
            mm[1].SectionType.ShouldBe(MemorySectionType.Disassemble);
            mm[2].StartAddress.ShouldBe((ushort)0x1100);
            mm[2].EndAddress.ShouldBe((ushort)0x16FF);
            mm[2].SectionType.ShouldBe(MemorySectionType.ByteArray);
        }

        [TestMethod]
        public void NormalizeWorksWithNoAdjacentSections()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x01FF));
            mm.Add(new MemorySection(0x0300, 0x03FF));
            mm.Add(new MemorySection(0x0500, 0x05FF));

            // --- Act
            mm.Normalize();

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x01FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
            mm[1].StartAddress.ShouldBe((ushort)0x0300);
            mm[1].EndAddress.ShouldBe((ushort)0x03FF);
            mm[1].SectionType.ShouldBe(MemorySectionType.Disassemble);
            mm[2].StartAddress.ShouldBe((ushort)0x0500);
            mm[2].EndAddress.ShouldBe((ushort)0x05FF);
            mm[2].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void NormalizeWorksWithTwoAdjacentSections1()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x01FF));
            mm.Add(new MemorySection(0x0200, 0x02FF));
            mm.Add(new MemorySection(0x0500, 0x05FF));

            // --- Act
            mm.Normalize();

            // --- Assert
            mm.Count.ShouldBe(2);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x02FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
            mm[1].StartAddress.ShouldBe((ushort)0x0500);
            mm[1].EndAddress.ShouldBe((ushort)0x05FF);
            mm[1].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void NormalizeWorksWithTwoAdjacentSections2()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x01FF));
            mm.Add(new MemorySection(0x0300, 0x03FF));
            mm.Add(new MemorySection(0x0400, 0x04FF));

            // --- Act
            mm.Normalize();

            // --- Assert
            mm.Count.ShouldBe(2);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x01FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
            mm[1].StartAddress.ShouldBe((ushort)0x0300);
            mm[1].EndAddress.ShouldBe((ushort)0x04FF);
            mm[1].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void NormalizeWorksWithThreeAdjacentSections()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x01FF));
            mm.Add(new MemorySection(0x0200, 0x02FF));
            mm.Add(new MemorySection(0x0300, 0x03FF));

            // --- Act
            mm.Normalize();

            // --- Assert
            mm.Count.ShouldBe(1);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x03FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void NormalizeDoesNotMergeDifferentTypes1()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x01FF));
            mm.Add(new MemorySection(0x0200, 0x02FF, MemorySectionType.ByteArray));
            mm.Add(new MemorySection(0x0300, 0x03FF));

            // --- Act
            mm.Normalize();

            // --- Assert
            mm.Count.ShouldBe(3);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x01FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
            mm[1].StartAddress.ShouldBe((ushort)0x0200);
            mm[1].EndAddress.ShouldBe((ushort)0x02FF);
            mm[1].SectionType.ShouldBe(MemorySectionType.ByteArray);
            mm[2].StartAddress.ShouldBe((ushort)0x0300);
            mm[2].EndAddress.ShouldBe((ushort)0x03FF);
            mm[2].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void NormalizeDoesNotMergeDifferentTypes2()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x01FF));
            mm.Add(new MemorySection(0x0200, 0x02FF));
            mm.Add(new MemorySection(0x0300, 0x03FF, MemorySectionType.WordArray));

            // --- Act
            mm.Normalize();

            // --- Assert
            mm.Count.ShouldBe(2);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x02FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
            mm[1].StartAddress.ShouldBe((ushort)0x0300);
            mm[1].EndAddress.ShouldBe((ushort)0x03FF);
            mm[1].SectionType.ShouldBe(MemorySectionType.WordArray);
        }

        [TestMethod]
        public void NormalizeDoesNotMergeDifferentTypes3()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x01FF, MemorySectionType.Skip));
            mm.Add(new MemorySection(0x0200, 0x02FF));
            mm.Add(new MemorySection(0x0300, 0x03FF));

            // --- Act
            mm.Normalize();

            // --- Assert
            mm.Count.ShouldBe(2);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x01FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.Skip);
            mm[1].StartAddress.ShouldBe((ushort)0x0200);
            mm[1].EndAddress.ShouldBe((ushort)0x03FF);
            mm[1].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }

        [TestMethod]
        public void NormalizeWorksWithMultipleSections()
        {
            // --- Arrange
            var mm = new MemoryMap();
            mm.Add(new MemorySection(0x0100, 0x01FF));
            mm.Add(new MemorySection(0x0300, 0x03FF));
            mm.Add(new MemorySection(0x0400, 0x04FF));
            mm.Add(new MemorySection(0x0600, 0x06FF));
            mm.Add(new MemorySection(0x0700, 0x07FF));
            mm.Add(new MemorySection(0x0800, 0x08FF));
            mm.Add(new MemorySection(0x0900, 0x09FF));
            mm.Add(new MemorySection(0x1000, 0x1FFF));

            // --- Act
            mm.Normalize();

            // --- Assert
            mm.Count.ShouldBe(4);
            mm[0].StartAddress.ShouldBe((ushort)0x0100);
            mm[0].EndAddress.ShouldBe((ushort)0x01FF);
            mm[0].SectionType.ShouldBe(MemorySectionType.Disassemble);
            mm[1].StartAddress.ShouldBe((ushort)0x0300);
            mm[1].EndAddress.ShouldBe((ushort)0x04FF);
            mm[1].SectionType.ShouldBe(MemorySectionType.Disassemble);
            mm[2].StartAddress.ShouldBe((ushort)0x0600);
            mm[2].EndAddress.ShouldBe((ushort)0x09FF);
            mm[2].SectionType.ShouldBe(MemorySectionType.Disassemble);
            mm[3].StartAddress.ShouldBe((ushort)0x1000);
            mm[3].EndAddress.ShouldBe((ushort)0x1FFF);
            mm[3].SectionType.ShouldBe(MemorySectionType.Disassemble);
        }


    }
}
