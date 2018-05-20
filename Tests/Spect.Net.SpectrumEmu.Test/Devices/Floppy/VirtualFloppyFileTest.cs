using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.Floppy;

namespace Spect.Net.SpectrumEmu.Test.Devices.Floppy
{
    [TestClass]
    public class VirtualFloppyFileTest
    {
        public static string TestFile { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Vfdd.vfdd");
        }

        [TestMethod]
        public void CreatingAFloppyFileWorksAsExpected()
        {
            // --- Act
            VirtualFloppyFile.CreateSpectrumFloppyFile(TestFile);

            // --- Assert
            VirtualFloppyFile.OpenFloppyFile(TestFile);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void OpeningANonExistingFloppyFileFails()
        {
            // --- Arrange
            File.Delete(TestFile);

            // --- Act
            VirtualFloppyFile.OpenFloppyFile(TestFile);
        }

        [TestMethod]
        public void OpenOrCreateCreatesANewFile()
        {
            // --- Arrange
            File.Delete(TestFile);

            // --- Act
            VirtualFloppyFile.OpenOrCreateFloppyFile(TestFile);

            // --- Assert
            VirtualFloppyFile.OpenFloppyFile(TestFile);
        }

        [TestMethod]
        public void OpenOrCreateOpensAnExistingNewFile()
        {
            // --- Arrange
            var floppy = VirtualFloppyFile.CreateSpectrumFloppyFile(TestFile);
            var data = new byte[] {0x01, 0x02, 0x03, 0x04};
            floppy.WriteData(1, 3, 5, data);

            // --- Act
            floppy = VirtualFloppyFile.OpenOrCreateFloppyFile(TestFile);

            // --- Assert
            var dataBack = floppy.ReadData(1, 3, 5, data.Length);
            dataBack.SequenceEqual(data).ShouldBeTrue();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DataRow(2, 1, 1, 1)]
        [DataRow(0, -1, 1, 1)]
        [DataRow(0, 40, 1, 1)]
        [DataRow(0, 30, 0, 1)]
        [DataRow(0, 30, 10, 1)]
        [DataRow(0, 30, 3, 0)]
        [DataRow(0, 30, 3, 513)]
        public void InvalidReadParameterRaisesError(int head, int track, int sector, int length)
        {
            // --- Arrange
            var floppy = VirtualFloppyFile.CreateSpectrumFloppyFile(TestFile);

            // --- Act
            floppy.ReadData(head, track, sector, length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DataRow(2, 1, 1, 1)]
        [DataRow(0, -1, 1, 1)]
        [DataRow(0, 40, 1, 1)]
        [DataRow(0, 30, 0, 1)]
        [DataRow(0, 30, 10, 1)]
        [DataRow(0, 30, 3, 0)]
        [DataRow(0, 30, 3, 513)]
        public void InvalidWriteParameterRaisesError(int head, int track, int sector, int length)
        {
            // --- Arrange
            var floppy = VirtualFloppyFile.CreateSpectrumFloppyFile(TestFile);
            var data = new byte[length];

            // --- Act
            floppy.WriteData(head, track, sector, data);
        }

    }
}
