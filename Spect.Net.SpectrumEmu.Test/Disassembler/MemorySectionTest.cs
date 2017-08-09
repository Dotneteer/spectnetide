using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.SpectrumEmu.Test.Disassembler
{
    [TestClass]
    public class MemorySectionTest
    {
        [TestMethod]
        public void OverlapWorksWithDiscreteSections1()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x0000, 0x0FFF);
            var ms2 = new MemorySection(0x2000, 0x2FFF);

            // --- Assert
            ms1.Overlaps(ms2).ShouldBeFalse();
            ms2.Overlaps(ms1).ShouldBeFalse();
        }

        [TestMethod]
        public void OverlapWorksWithDiscreteSections2()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x0000, 0x0FFF);
            var ms2 = new MemorySection(0x1000, 0x1FFF);

            // --- Assert
            ms1.Overlaps(ms2).ShouldBeFalse();
            ms2.Overlaps(ms1).ShouldBeFalse();
        }

        [TestMethod]
        public void OverlapIsCaught1()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1FFF);
            var ms2 = new MemorySection(0x0000, 0x1000);

            // --- Assert
            ms1.Overlaps(ms2).ShouldBeTrue();
            ms2.Overlaps(ms1).ShouldBeTrue();
        }

        [TestMethod]
        public void OverlapIsCaught2()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1FFF);
            var ms2 = new MemorySection(0x1FFF, 0x2FFF);

            // --- Assert
            ms1.Overlaps(ms2).ShouldBeTrue();
            ms2.Overlaps(ms1).ShouldBeTrue();
        }

        [TestMethod]
        public void OverlapIsCaught3()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1FFF);
            var ms2 = new MemorySection(0x1010, 0x180F);

            // --- Assert
            ms1.Overlaps(ms2).ShouldBeTrue();
            ms2.Overlaps(ms1).ShouldBeTrue();
        }

        [TestMethod]
        public void SameSectionWorksAsExpected1()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x10FF, MemorySectionType.WordArray);
            var ms2 = new MemorySection(0x1000, 0x10FF);

            // --- Assert
            ms1.SameSection(ms2).ShouldBeTrue();
            ms2.SameSection(ms1).ShouldBeTrue();
        }

        [TestMethod]
        public void SameSectionWorksAsExpected2()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x10FF, MemorySectionType.WordArray);
            var ms2 = new MemorySection(0x1001, 0x10);

            // --- Assert
            ms1.SameSection(ms2).ShouldBeFalse();
            ms2.SameSection(ms1).ShouldBeFalse();
        }

        [TestMethod]
        public void SameSectionWorksAsExpected3()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x10FF, MemorySectionType.WordArray);
            var ms2 = new MemorySection(0x1000, 0x1100);

            // --- Assert
            ms1.SameSection(ms2).ShouldBeFalse();
            ms2.SameSection(ms1).ShouldBeFalse();
        }

        [TestMethod]
        public void SameSectionWorksAsExpected4()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1100, MemorySectionType.WordArray);
            var ms2 = new MemorySection(0x1001, 0x10FF);

            // --- Assert
            ms1.SameSection(ms2).ShouldBeFalse();
            ms2.SameSection(ms1).ShouldBeFalse();
        }

        [TestMethod]
        public void IntersectionWorksAsExpected1()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x10FF);
            var ms2 = new MemorySection(0x0000, 0x00FF);

            // --- Assert
            ms1.Intersect(ms2).ShouldBeNull();
            ms2.Intersect(ms1).ShouldBeNull();
        }

        [TestMethod]
        public void IntersectionWorksAsExpected2()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1FFF);
            var ms2 = new MemorySection(0x0000, 0x0FFF);

            // --- Assert
            ms1.Intersect(ms2).ShouldBeNull();
            ms2.Intersect(ms1).ShouldBeNull();
        }

        [TestMethod]
        public void IntersectionWorksAsExpected3()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1FFF);
            var ms2 = new MemorySection(0x1FFF, 0x20FF);

            // --- Act
            var intersection1 = ms1.Intersect(ms2);
            var intersection2 = ms2.Intersect(ms1);

            // --- Assert
            intersection1.ShouldNotBeNull();
            intersection1.StartAddress.ShouldBe((ushort)0x1FFF);
            intersection1.EndAddress.ShouldBe((ushort)0x1FFF);
            intersection2.ShouldNotBeNull();
            intersection2.StartAddress.ShouldBe((ushort)0x1FFF);
            intersection2.EndAddress.ShouldBe((ushort)0x1FFF);
        }

        [TestMethod]
        public void IntersectionWorksAsExpected4()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1FFF);
            var ms2 = new MemorySection(0x1000, 0x10FF);

            // --- Act
            var intersection1 = ms1.Intersect(ms2);
            var intersection2 = ms2.Intersect(ms1);

            // --- Assert
            intersection1.ShouldNotBeNull();
            intersection1.StartAddress.ShouldBe((ushort)0x1000);
            intersection1.EndAddress.ShouldBe((ushort)0x10FF);
            intersection2.ShouldNotBeNull();
            intersection2.StartAddress.ShouldBe((ushort)0x1000);
            intersection2.EndAddress.ShouldBe((ushort)0x10FF);
        }

        [TestMethod]
        public void IntersectionWorksAsExpected5()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1FFF);
            var ms2 = new MemorySection(0x1100, 0x11FF);

            // --- Act
            var intersection1 = ms1.Intersect(ms2);
            var intersection2 = ms2.Intersect(ms1);

            // --- Assert
            intersection1.ShouldNotBeNull();
            intersection1.StartAddress.ShouldBe((ushort)0x1100);
            intersection1.EndAddress.ShouldBe((ushort)0x11FF);
            intersection2.ShouldNotBeNull();
            intersection2.StartAddress.ShouldBe((ushort)0x1100);
            intersection2.EndAddress.ShouldBe((ushort)0x11FF);
        }
    }
}
