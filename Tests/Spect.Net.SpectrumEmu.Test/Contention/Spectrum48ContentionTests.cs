using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Contention
{
    [TestClass]
    public class Spectrum48ContentionTests: ContentionTestBed
    {
        [TestMethod]
        [DataRow(0x8000, -100, 4)]
        [DataRow(0x8000, 0, 4)]
        [DataRow(0x8000, 1, 4)]
        [DataRow(0x8000, 2, 4)]
        [DataRow(0x8000, 3, 4)]
        [DataRow(0x8000, 4, 4)]
        [DataRow(0x8000, 5, 4)]
        [DataRow(0x8000, 6, 4)]
        [DataRow(0x8000, 7, 4)]
        [DataRow(0x8000, 8, 4)]
        [DataRow(0x8000, 9, 4)]
        public void Nop(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x00 // NOP
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 4)]
        [DataRow(0x4100, 0, 4 + 5)]
        [DataRow(0x4100, 1, 4 + 4)]
        [DataRow(0x4100, 2, 4 + 3)]
        [DataRow(0x4100, 3, 4 + 2)]
        [DataRow(0x4100, 4, 4 + 1)]
        [DataRow(0x4100, 5, 4 + 0)]
        [DataRow(0x4100, 6, 4 + 0)]
        [DataRow(0x4100, 7, 4 + 6)]
        [DataRow(0x4100, 8, 4 + 5)]
        [DataRow(0x4100, 9, 4 + 4)]
        public void Nop_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x00 // NOP
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 10)]
        [DataRow(0x8000, 0, 10)]
        [DataRow(0x8000, 1, 10)]
        [DataRow(0x8000, 2, 10)]
        [DataRow(0x8000, 3, 10)]
        [DataRow(0x8000, 4, 10)]
        [DataRow(0x8000, 5, 10)]
        [DataRow(0x8000, 6, 10)]
        [DataRow(0x8000, 7, 10)]
        [DataRow(0x8000, 8, 10)]
        [DataRow(0x8000, 9, 10)]
        public void LD_BC_NN(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x01, 0x34, 0x12 // LD BC,0x1234
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 10)]
        [DataRow(0x4100, 0, 10 + 5 + 4 + 5)]
        [DataRow(0x4100, 1, 10 + 4 + 4 + 5)]
        [DataRow(0x4100, 2, 10 + 3 + 4 + 5)]
        [DataRow(0x4100, 3, 10 + 2 + 4 + 5)]
        [DataRow(0x4100, 4, 10 + 1 + 4 + 5)]
        [DataRow(0x4100, 5, 10 + 0 + 4 + 5)]
        [DataRow(0x4100, 6, 10 + 0 + 3 + 5)]
        [DataRow(0x4100, 7, 10 + 6 + 4 + 5)]
        [DataRow(0x4100, 8, 10 + 5 + 4 + 5)]
        [DataRow(0x4100, 9, 10 + 4 + 4 + 5)]
        public void LD_BC_NN_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x01, 0x34, 0x12 // LD BC,0x1234
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 10)]
        [DataRow(0x8000, 0, 10)]
        [DataRow(0x8000, 1, 10)]
        [DataRow(0x8000, 2, 10)]
        [DataRow(0x8000, 3, 10)]
        [DataRow(0x8000, 4, 10)]
        [DataRow(0x8000, 5, 10)]
        [DataRow(0x8000, 6, 10)]
        [DataRow(0x8000, 7, 10)]
        [DataRow(0x8000, 8, 10)]
        [DataRow(0x8000, 9, 10)]
        public void LD_DE_NN(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x11, 0x34, 0x12 // LD DE,0x1234
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 10)]
        [DataRow(0x4100, 0, 10 + 5 + 4 + 5)]
        [DataRow(0x4100, 1, 10 + 4 + 4 + 5)]
        [DataRow(0x4100, 2, 10 + 3 + 4 + 5)]
        [DataRow(0x4100, 3, 10 + 2 + 4 + 5)]
        [DataRow(0x4100, 4, 10 + 1 + 4 + 5)]
        [DataRow(0x4100, 5, 10 + 0 + 4 + 5)]
        [DataRow(0x4100, 6, 10 + 0 + 3 + 5)]
        [DataRow(0x4100, 7, 10 + 6 + 4 + 5)]
        [DataRow(0x4100, 8, 10 + 5 + 4 + 5)]
        [DataRow(0x4100, 9, 10 + 4 + 4 + 5)]
        public void LD_DE_NN_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x11, 0x34, 0x12 // LD DE,0x1234
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 10)]
        [DataRow(0x8000, 0, 10)]
        [DataRow(0x8000, 1, 10)]
        [DataRow(0x8000, 2, 10)]
        [DataRow(0x8000, 3, 10)]
        [DataRow(0x8000, 4, 10)]
        [DataRow(0x8000, 5, 10)]
        [DataRow(0x8000, 6, 10)]
        [DataRow(0x8000, 7, 10)]
        [DataRow(0x8000, 8, 10)]
        [DataRow(0x8000, 9, 10)]
        public void LD_HL_NN(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x21, 0x34, 0x12 // LD HL,0x1234
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 10)]
        [DataRow(0x4100, 0, 10 + 5 + 4 + 5)]
        [DataRow(0x4100, 1, 10 + 4 + 4 + 5)]
        [DataRow(0x4100, 2, 10 + 3 + 4 + 5)]
        [DataRow(0x4100, 3, 10 + 2 + 4 + 5)]
        [DataRow(0x4100, 4, 10 + 1 + 4 + 5)]
        [DataRow(0x4100, 5, 10 + 0 + 4 + 5)]
        [DataRow(0x4100, 6, 10 + 0 + 3 + 5)]
        [DataRow(0x4100, 7, 10 + 6 + 4 + 5)]
        [DataRow(0x4100, 8, 10 + 5 + 4 + 5)]
        [DataRow(0x4100, 9, 10 + 4 + 4 + 5)]
        public void LD_HL_NN_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x21, 0x34, 0x12 // LD HL,0x1234
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 10)]
        [DataRow(0x8000, 0, 10)]
        [DataRow(0x8000, 1, 10)]
        [DataRow(0x8000, 2, 10)]
        [DataRow(0x8000, 3, 10)]
        [DataRow(0x8000, 4, 10)]
        [DataRow(0x8000, 5, 10)]
        [DataRow(0x8000, 6, 10)]
        [DataRow(0x8000, 7, 10)]
        [DataRow(0x8000, 8, 10)]
        [DataRow(0x8000, 9, 10)]
        public void LD_SP_NN(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x31, 0x34, 0x12 // LD SP,0x1234
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 10)]
        [DataRow(0x4100, 0, 10 + 5 + 4 + 5)]
        [DataRow(0x4100, 1, 10 + 4 + 4 + 5)]
        [DataRow(0x4100, 2, 10 + 3 + 4 + 5)]
        [DataRow(0x4100, 3, 10 + 2 + 4 + 5)]
        [DataRow(0x4100, 4, 10 + 1 + 4 + 5)]
        [DataRow(0x4100, 5, 10 + 0 + 4 + 5)]
        [DataRow(0x4100, 6, 10 + 0 + 3 + 5)]
        [DataRow(0x4100, 7, 10 + 6 + 4 + 5)]
        [DataRow(0x4100, 8, 10 + 5 + 4 + 5)]
        [DataRow(0x4100, 9, 10 + 4 + 4 + 5)]
        public void LD_SP_NN_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x31, 0x34, 0x12 // LD SP,0x1234
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

    }
}
