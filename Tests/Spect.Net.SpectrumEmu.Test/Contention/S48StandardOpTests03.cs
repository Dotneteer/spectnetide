using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Contention
{
    [TestClass]
    public class S48StandardOpTests03: ContentionTestBed
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
        public void LD_B_B(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x40 // LD B,B
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 4)]
        [DataRow(0x4100, 0, 9)]
        [DataRow(0x4100, 1, 8)]
        [DataRow(0x4100, 2, 7)]
        [DataRow(0x4100, 3, 6)]
        [DataRow(0x4100, 4, 5)]
        [DataRow(0x4100, 5, 4)]
        [DataRow(0x4100, 6, 4)]
        [DataRow(0x4100, 7, 10)]
        [DataRow(0x4100, 8, 9)]
        [DataRow(0x4100, 9, 8)]
        public void LD_B_B_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x40 // LD B,B
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

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
        public void LD_B_C(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x41 // LD B,C
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 4)]
        [DataRow(0x4100, 0, 9)]
        [DataRow(0x4100, 1, 8)]
        [DataRow(0x4100, 2, 7)]
        [DataRow(0x4100, 3, 6)]
        [DataRow(0x4100, 4, 5)]
        [DataRow(0x4100, 5, 4)]
        [DataRow(0x4100, 6, 4)]
        [DataRow(0x4100, 7, 10)]
        [DataRow(0x4100, 8, 9)]
        [DataRow(0x4100, 9, 8)]
        public void LD_B_C_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x41 // LD B,C
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

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
        public void LD_B_D(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x42 // LD B,D
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 4)]
        [DataRow(0x4100, 0, 9)]
        [DataRow(0x4100, 1, 8)]
        [DataRow(0x4100, 2, 7)]
        [DataRow(0x4100, 3, 6)]
        [DataRow(0x4100, 4, 5)]
        [DataRow(0x4100, 5, 4)]
        [DataRow(0x4100, 6, 4)]
        [DataRow(0x4100, 7, 10)]
        [DataRow(0x4100, 8, 9)]
        [DataRow(0x4100, 9, 8)]
        public void LD_B_D_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x42 // LD B,D
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

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
        public void LD_B_E(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x43 // LD B,E
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 4)]
        [DataRow(0x4100, 0, 9)]
        [DataRow(0x4100, 1, 8)]
        [DataRow(0x4100, 2, 7)]
        [DataRow(0x4100, 3, 6)]
        [DataRow(0x4100, 4, 5)]
        [DataRow(0x4100, 5, 4)]
        [DataRow(0x4100, 6, 4)]
        [DataRow(0x4100, 7, 10)]
        [DataRow(0x4100, 8, 9)]
        [DataRow(0x4100, 9, 8)]
        public void LD_B_E_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x43 // LD B,E
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

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
        public void LD_B_H(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x44 // LD B,H
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 4)]
        [DataRow(0x4100, 0, 9)]
        [DataRow(0x4100, 1, 8)]
        [DataRow(0x4100, 2, 7)]
        [DataRow(0x4100, 3, 6)]
        [DataRow(0x4100, 4, 5)]
        [DataRow(0x4100, 5, 4)]
        [DataRow(0x4100, 6, 4)]
        [DataRow(0x4100, 7, 10)]
        [DataRow(0x4100, 8, 9)]
        [DataRow(0x4100, 9, 8)]
        public void LD_B_H_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x44 // LD B,H
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

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
        public void LD_B_L(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x45 // LD B,L
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 4)]
        [DataRow(0x4100, 0, 9)]
        [DataRow(0x4100, 1, 8)]
        [DataRow(0x4100, 2, 7)]
        [DataRow(0x4100, 3, 6)]
        [DataRow(0x4100, 4, 5)]
        [DataRow(0x4100, 5, 4)]
        [DataRow(0x4100, 6, 4)]
        [DataRow(0x4100, 7, 10)]
        [DataRow(0x4100, 8, 9)]
        [DataRow(0x4100, 9, 8)]
        public void LD_B_L_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x45 // LD B,L
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

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
        public void LD_B_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x47 // LD B,A
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 4)]
        [DataRow(0x4100, 0, 9)]
        [DataRow(0x4100, 1, 8)]
        [DataRow(0x4100, 2, 7)]
        [DataRow(0x4100, 3, 6)]
        [DataRow(0x4100, 4, 5)]
        [DataRow(0x4100, 5, 4)]
        [DataRow(0x4100, 6, 4)]
        [DataRow(0x4100, 7, 10)]
        [DataRow(0x4100, 8, 9)]
        [DataRow(0x4100, 9, 8)]
        public void LD_B_A_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x47 // LD B,A
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

    }
}
