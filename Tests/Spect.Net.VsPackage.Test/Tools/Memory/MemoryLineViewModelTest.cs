using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.VsPackage.ToolWindows.Memory;

namespace Spect.Net.VsPackage.Test.Tools.Memory
{
    [TestClass]
    public class MemoryLineViewModelTest
    {
        [TestMethod]
        public void ConstructionSavesStartAndTopAddresses()
        {
            // --- Act
            var ml = new MemoryLineViewModel(0x1234, 0x2345);

            // --- Assert
            ml.BaseAddress.ShouldBe(0x1234);
            ml.TopAddress.ShouldBe(0x2345);
        }

        [TestMethod]
        public void BindToUsesAddressesProperly()
        {
            // --- Arrange
            var memory = new byte[]
            {
                0x00, 0x01,
                0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11,
                0x12, 0x13
            };
            var ml = new MemoryLineViewModel(0x0002, 0x0100);

            // --- Act
            ml.BindTo(memory);

            // --- Assert
            ml.Addr1.ShouldBe("0002");
            ml.Addr2.ShouldBe("000A");
        }

        [TestMethod]
        public void BindToUsesDataBytesProperly()
        {
            // --- Arrange
            var memory = new byte[]
            {
                0x00, 0x01,
                0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11,
                0x12, 0x13
            };
            var ml = new MemoryLineViewModel(0x0002, 0x0100);

            // --- Act
            ml.BindTo(memory);

            // --- Assert
            ml.Value0.ShouldBe("02");
            ml.Value1.ShouldBe("03");
            ml.Value2.ShouldBe("04");
            ml.Value3.ShouldBe("05");
            ml.Value4.ShouldBe("06");
            ml.Value5.ShouldBe("07");
            ml.Value6.ShouldBe("08");
            ml.Value7.ShouldBe("09");
            ml.Value8.ShouldBe("0A");
            ml.Value9.ShouldBe("0B");
            ml.ValueA.ShouldBe("0C");
            ml.ValueB.ShouldBe("0D");
            ml.ValueC.ShouldBe("0E");
            ml.ValueD.ShouldBe("0F");
            ml.ValueE.ShouldBe("10");
            ml.ValueF.ShouldBe("11");
        }

        [TestMethod]
        public void BindToWithShortLineUsesAddressesProperly()
        {
            // --- Arrange
            var memory = new byte[]
            {
                0x00, 0x01,
                0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11,
                0x12, 0x13
            };
            var ml = new MemoryLineViewModel(0x0002, 0x0008);

            // --- Act
            ml.BindTo(memory);

            // --- Assert
            ml.Addr1.ShouldBe("0002");
            ml.Addr2.ShouldBeNull();
        }


        [TestMethod]
        public void BindToWithShortLineUsesDataBytesProperly1()
        {
            // --- Arrange
            var memory = new byte[]
            {
                0x00, 0x01,
                0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11,
                0x12, 0x13
            };
            var ml = new MemoryLineViewModel(0x0002, 0x0008);

            // --- Act
            ml.BindTo(memory);

            // --- Assert
            ml.Value0.ShouldBe("02");
            ml.Value1.ShouldBe("03");
            ml.Value2.ShouldBe("04");
            ml.Value3.ShouldBe("05");
            ml.Value4.ShouldBe("06");
            ml.Value5.ShouldBe("07");
            ml.Value6.ShouldBe("08");
            ml.Value7.ShouldBeNull();
            ml.Value8.ShouldBeNull();
            ml.Value9.ShouldBeNull();
            ml.ValueA.ShouldBeNull();
            ml.ValueB.ShouldBeNull();
            ml.ValueC.ShouldBeNull();
            ml.ValueD.ShouldBeNull();
            ml.ValueE.ShouldBeNull();
            ml.ValueF.ShouldBeNull();
        }

        [TestMethod]
        public void BindToWithShortLineUsesDataBytesProperly2()
        {
            // --- Arrange
            var memory = new byte[]
            {
                0x00, 0x01,
                0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11,
                0x12, 0x13
            };
            var ml = new MemoryLineViewModel(0x0002, 0x000C);

            // --- Act
            ml.BindTo(memory);

            // --- Assert
            ml.Value0.ShouldBe("02");
            ml.Value1.ShouldBe("03");
            ml.Value2.ShouldBe("04");
            ml.Value3.ShouldBe("05");
            ml.Value4.ShouldBe("06");
            ml.Value5.ShouldBe("07");
            ml.Value6.ShouldBe("08");
            ml.Value7.ShouldBe("09");
            ml.Value8.ShouldBe("0A");
            ml.Value9.ShouldBe("0B");
            ml.ValueA.ShouldBe("0C");
            ml.ValueB.ShouldBeNull();
            ml.ValueC.ShouldBeNull();
            ml.ValueD.ShouldBeNull();
            ml.ValueE.ShouldBeNull();
            ml.ValueF.ShouldBeNull();
        }

    }
}
