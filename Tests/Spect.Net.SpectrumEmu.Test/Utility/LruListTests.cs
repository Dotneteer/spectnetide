using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Utility;
// ReSharper disable UseObjectOrCollectionInitializer

namespace Spect.Net.SpectrumEmu.Test.Utility
{
    [TestClass]
    public class LruListTests
    {
        [TestMethod]
        public void ConstructionWorksAsExpected()
        {
            // --- Act
            var lru = new LruList<int>(23);

            // --- Assert
            lru.Capacity.ShouldBe(23);
            lru.Count.ShouldBe(0);
        }

        [TestMethod]
        public void ConstructionWithDefaultCapacityWorksAsExpected()
        {
            // --- Act
            var lru = new LruList<int>();

            // --- Assert
            lru.Capacity.ShouldBe(10);
            lru.Count.ShouldBe(0);
        }

        [TestMethod]
        public void AddWorksWhenCapacityIsNotExceeded()
        {
            // --- Arrange
            var lru = new LruList<int>(3);

            // --- Act
            lru.Add(100);
            lru.Add(200);

            // --- Assert
            lru.Capacity.ShouldBe(3);
            lru.Count.ShouldBe(2);
            lru[0].ShouldBe(100);
            lru[1].ShouldBe(200);
        }

        [TestMethod]
        public void AddWorksWhenCapacityIsReached()
        {
            // --- Arrange
            var lru = new LruList<int>(3);

            // --- Act
            lru.Add(100);
            lru.Add(200);
            lru.Add(300);

            // --- Assert
            lru.Capacity.ShouldBe(3);
            lru.Count.ShouldBe(3);
            lru[0].ShouldBe(100);
            lru[1].ShouldBe(200);
            lru[2].ShouldBe(300);
        }

        [TestMethod]
        public void AddWorksWhenCapacityIsExceeded()
        {
            // --- Arrange
            var lru = new LruList<int>(3);

            // --- Act
            lru.Add(100);
            lru.Add(200);
            lru.Add(300);
            lru.Add(400);

            // --- Assert
            lru.Capacity.ShouldBe(3);
            lru.Count.ShouldBe(3);
            lru[0].ShouldBe(200);
            lru[1].ShouldBe(300);
            lru[2].ShouldBe(400);
        }

        [TestMethod]
        public void AddWorksWhenCapacityIsExceededMultipleTimes()
        {
            // --- Arrange
            var lru = new LruList<int>(3);

            // --- Act
            lru.Add(100);
            lru.Add(200);
            lru.Add(300);
            lru.Add(400);
            lru.Add(500);
            lru.Add(600);
            lru.Add(700);
            lru.Add(800);

            // --- Assert
            lru.Capacity.ShouldBe(3);
            lru.Count.ShouldBe(3);
            lru[0].ShouldBe(600);
            lru[1].ShouldBe(700);
            lru[2].ShouldBe(800);
        }

        [TestMethod]
        public void ContainsWorksWhenCapacityIsNotExceeded()
        {
            // --- Arrange
            var lru = new LruList<int>(3);

            // --- Act
            lru.Add(100);
            lru.Add(200);

            // --- Assert
            lru.Contains(100).ShouldBeTrue();
            lru.Contains(200).ShouldBeTrue();
            lru.Contains(300).ShouldBeFalse();
        }

        [TestMethod]
        public void ContainsWorksWhenCapacityIsReached()
        {
            // --- Arrange
            var lru = new LruList<int>(3);

            // --- Act
            lru.Add(100);
            lru.Add(200);
            lru.Add(300);

            // --- Assert
            lru.Contains(100).ShouldBeTrue();
            lru.Contains(200).ShouldBeTrue();
            lru.Contains(300).ShouldBeTrue();
            lru.Contains(400).ShouldBeFalse();
        }

        [TestMethod]
        public void ContainsWorksWhenCapacityIsExceeded()
        {
            // --- Arrange
            var lru = new LruList<int>(3);

            // --- Act
            lru.Add(100);
            lru.Add(200);
            lru.Add(300);
            lru.Add(400);

            // --- Assert
            lru.Contains(100).ShouldBeFalse();
            lru.Contains(200).ShouldBeTrue();
            lru.Contains(300).ShouldBeTrue();
            lru.Contains(400).ShouldBeTrue();
            lru.Contains(500).ShouldBeFalse();
        }

        [TestMethod]
        public void ContainsWorksWhenCapacityIsExceededMultipleTimes()
        {
            // --- Arrange
            var lru = new LruList<int>(3);

            // --- Act
            lru.Add(100);
            lru.Add(200);
            lru.Add(300);
            lru.Add(400);
            lru.Add(500);
            lru.Add(600);
            lru.Add(700);
            lru.Add(800);

            // --- Assert
            lru.Contains(100).ShouldBeFalse();
            lru.Contains(200).ShouldBeFalse();
            lru.Contains(300).ShouldBeFalse();
            lru.Contains(400).ShouldBeFalse();
            lru.Contains(500).ShouldBeFalse();
            lru.Contains(600).ShouldBeTrue();
            lru.Contains(700).ShouldBeTrue();
            lru.Contains(800).ShouldBeTrue();
            lru.Contains(900).ShouldBeFalse();
        }


    }
}
