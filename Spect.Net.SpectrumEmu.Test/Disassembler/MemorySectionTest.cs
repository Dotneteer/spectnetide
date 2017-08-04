using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.SpectrumEmu.Test.Disassembler
{
    [TestClass]
    public class MemorySectionTest
    {
        [TestMethod]
        public void AdjustWorksWithConstructor()
        {
            // --- Act
            var ms1 = new MemorySection(0xFF00, 0x100);
            var ms2 = new MemorySection(0x0000, 0xFFF0);
            var ms3 = new MemorySection(0x8000, 0x8000);
            var ms4 = new MemorySection(0x8000, 0x8001);

            // --- Assert
            ms1.Length.ShouldBe((ushort)0x100);
            ms2.Length.ShouldBe((ushort)0xFFF0);
            ms3.Length.ShouldBe((ushort)0x8000);
            ms4.Length.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void AdjustWorksAsExpected()
        {
            // --- Arrange
            var ms1 = new MemorySection {StartAddress = 0xFF00, Length = 0x100};
            var ms2 = new MemorySection {StartAddress = 0x0000, Length = 0xFFF0};
            var ms3 = new MemorySection {StartAddress = 0x8000, Length = 0x8000};
            var ms4 = new MemorySection {StartAddress = 0x8000, Length = 0x8001};

            // --- Act
            ms1.Adjust();
            ms2.Adjust();
            ms3.Adjust();
            ms4.Adjust();

            // --- Assert
            ms1.Length.ShouldBe((ushort)0x100);
            ms2.Length.ShouldBe((ushort)0xFFF0);
            ms3.Length.ShouldBe((ushort)0x8000);
            ms4.Length.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void OverlapWorksWithDiscreteSections1()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x0000, 0x1000);
            var ms2 = new MemorySection(0x2000, 0x1000);

            // --- Assert
            ms1.Overlaps(ms2).ShouldBeFalse();
            ms2.Overlaps(ms1).ShouldBeFalse();
        }

        [TestMethod]
        public void OverlapWorksWithDiscreteSections2()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x0000, 0x1000);
            var ms2 = new MemorySection(0x1000, 0x1000);

            // --- Assert
            ms1.Overlaps(ms2).ShouldBeFalse();
            ms2.Overlaps(ms1).ShouldBeFalse();
        }

        [TestMethod]
        public void OverlapIsCaught1()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1000);
            var ms2 = new MemorySection(0x0000, 0x1001);

            // --- Assert
            ms1.Overlaps(ms2).ShouldBeTrue();
            ms2.Overlaps(ms1).ShouldBeTrue();
        }

        [TestMethod]
        public void OverlapIsCaught2()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1000);
            var ms2 = new MemorySection(0x1FFF, 0x1000);

            // --- Assert
            ms1.Overlaps(ms2).ShouldBeTrue();
            ms2.Overlaps(ms1).ShouldBeTrue();
        }

        [TestMethod]
        public void OverlapIsCaught3()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1000);
            var ms2 = new MemorySection(0x1010, 0x800);

            // --- Assert
            ms1.Overlaps(ms2).ShouldBeTrue();
            ms2.Overlaps(ms1).ShouldBeTrue();
        }

        [TestMethod]
        public void SameSectionWorksAsExpected1()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x100, MemorySectionType.WordArray);
            var ms2 = new MemorySection(0x1000, 0x100);

            // --- Assert
            ms1.SameSection(ms2).ShouldBeTrue();
            ms2.SameSection(ms1).ShouldBeTrue();
        }

        [TestMethod]
        public void SameSectionWorksAsExpected2()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x100, MemorySectionType.WordArray);
            var ms2 = new MemorySection(0x1001, 0x100);

            // --- Assert
            ms1.SameSection(ms2).ShouldBeFalse();
            ms2.SameSection(ms1).ShouldBeFalse();
        }

        [TestMethod]
        public void SameSectionWorksAsExpected3()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x100, MemorySectionType.WordArray);
            var ms2 = new MemorySection(0x1000, 0x101);

            // --- Assert
            ms1.SameSection(ms2).ShouldBeFalse();
            ms2.SameSection(ms1).ShouldBeFalse();
        }

        [TestMethod]
        public void SameSectionWorksAsExpected4()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x101, MemorySectionType.WordArray);
            var ms2 = new MemorySection(0x1001, 0x100);

            // --- Assert
            ms1.SameSection(ms2).ShouldBeFalse();
            ms2.SameSection(ms1).ShouldBeFalse();
        }

        [TestMethod]
        public void IntersectionWorksAsExpected1()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x100);
            var ms2 = new MemorySection(0x0000, 0x100);

            // --- Assert
            ms1.Intersection(ms2).ShouldBeNull();
            ms2.Intersection(ms1).ShouldBeNull();
        }

        [TestMethod]
        public void IntersectionWorksAsExpected2()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1000);
            var ms2 = new MemorySection(0x0000, 0x1000);

            // --- Assert
            ms1.Intersection(ms2).ShouldBeNull();
            ms2.Intersection(ms1).ShouldBeNull();
        }

        [TestMethod]
        public void IntersectionWorksAsExpected3()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1000);
            var ms2 = new MemorySection(0x1FFF, 0x100);

            // --- Act
            var intersection1 = ms1.Intersection(ms2);
            var intersection2 = ms2.Intersection(ms1);

            // --- Assert
            intersection1.ShouldNotBeNull();
            intersection1.StartAddress.ShouldBe((ushort)0x1FFF);
            intersection1.Length.ShouldBe((ushort)0x0001);
            intersection2.ShouldNotBeNull();
            intersection2.StartAddress.ShouldBe((ushort)0x1FFF);
            intersection2.Length.ShouldBe((ushort)0x0001);
        }

        [TestMethod]
        public void IntersectionWorksAsExpected4()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1000);
            var ms2 = new MemorySection(0x1000, 0x100);

            // --- Act
            var intersection1 = ms1.Intersection(ms2);
            var intersection2 = ms2.Intersection(ms1);

            // --- Assert
            intersection1.ShouldNotBeNull();
            intersection1.StartAddress.ShouldBe((ushort)0x1000);
            intersection1.Length.ShouldBe((ushort)0x100);
            intersection2.ShouldNotBeNull();
            intersection2.StartAddress.ShouldBe((ushort)0x1000);
            intersection2.Length.ShouldBe((ushort)0x100);
        }

        [TestMethod]
        public void IntersectionWorksAsExpected5()
        {
            // --- Arrange
            var ms1 = new MemorySection(0x1000, 0x1000);
            var ms2 = new MemorySection(0x1100, 0x100);

            // --- Act
            var intersection1 = ms1.Intersection(ms2);
            var intersection2 = ms2.Intersection(ms1);

            // --- Assert
            intersection1.ShouldNotBeNull();
            intersection1.StartAddress.ShouldBe((ushort)0x1100);
            intersection1.Length.ShouldBe((ushort)0x100);
            intersection2.ShouldNotBeNull();
            intersection2.StartAddress.ShouldBe((ushort)0x1100);
            intersection2.Length.ShouldBe((ushort)0x100);
        }
    }
}
