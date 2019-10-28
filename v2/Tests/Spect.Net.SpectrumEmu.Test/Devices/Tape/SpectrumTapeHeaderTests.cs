using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.Tape;
// ReSharper disable UseObjectOrCollectionInitializer

namespace Spect.Net.SpectrumEmu.Test.Devices.Tape
{
    [TestClass]
    public class SpectrumTapeHeaderTests
    {
        [TestMethod]
        public void ConstructionWorksAsExpected()
        {
            // --- Act
            var head = new SpectrumTapeHeader();

            // --- Assert
            head.Type.ShouldBe((byte)0);
            head.Name.Length.ShouldBe(10);
            head.DataLength.ShouldBe((ushort)0);
            head.Parameter1.ShouldBe((ushort)0);
            head.Parameter2.ShouldBe((ushort)0);
            head.Checksum.ShouldBe((byte)0);
        }

        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(1, 1, 1)]
        [DataRow(2, 2, 2)]
        [DataRow(3, 3, 3)]
        [DataRow(4, 0, 0)]
        public void TypePropertyAsExpected(int value, int expected, int chk)
        {
            // --- Arrange
            var head = new SpectrumTapeHeader();

            // --- Act
            head.Type = (byte)value;

            // --- Assert
            head.Type.ShouldBe((byte)expected);
            head.Checksum.ShouldBe((byte)chk);
        }

        [TestMethod]
        [DataRow("A", "A", 97)]
        [DataRow("LONGlongName", "LONGlongNa", 47)]
        [DataRow("ProperName", "ProperName", 13)]
        public void NamePropertyAsExpected(string value, string expected, int chk)
        {
            // --- Arrange
            var head = new SpectrumTapeHeader();

            // --- Act
            head.Name = value;

            // --- Assert
            head.Name.ShouldBe(expected);
            head.Checksum.ShouldBe((byte)chk);
        }

        [TestMethod]
        [DataRow(0x1000, 16)]
        [DataRow(0x2345, 102)]
        [DataRow(0xCB98, 83)]
        public void Parameter1PropertyAsExpected(int value, int chk)
        {
            // --- Arrange
            var head = new SpectrumTapeHeader();

            // --- Act
            head.Parameter1 = (ushort)value;

            // --- Assert
            head.Parameter1.ShouldBe((ushort)value);
            head.Checksum.ShouldBe((byte)chk);
        }

        [TestMethod]
        [DataRow(0x1000, 16)]
        [DataRow(0x2345, 102)]
        [DataRow(0xCB98, 83)]
        public void Parameter2PropertyAsExpected(int value, int chk)
        {
            // --- Arrange
            var head = new SpectrumTapeHeader();

            // --- Act
            head.Parameter2 = (ushort)value;

            // --- Assert
            head.Parameter2.ShouldBe((ushort)value);
            head.Checksum.ShouldBe((byte)chk);
        }

    }
}
