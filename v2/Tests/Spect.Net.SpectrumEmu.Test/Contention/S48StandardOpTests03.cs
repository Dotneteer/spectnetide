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
        [DataRow(0x8000, -100, 7)]
        [DataRow(0x8000, 0, 7)]
        [DataRow(0x8000, 1, 7)]
        [DataRow(0x8000, 2, 7)]
        [DataRow(0x8000, 3, 7)]
        [DataRow(0x8000, 4, 7)]
        [DataRow(0x8000, 5, 7)]
        [DataRow(0x8000, 6, 7)]
        [DataRow(0x8000, 7, 7)]
        [DataRow(0x8000, 8, 7)]
        [DataRow(0x8000, 9, 7)]
        public void LD_B_HLi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x46 // LD B,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 7)]
        [DataRow(0x4100, 0, 12)]
        [DataRow(0x4100, 1, 11)]
        [DataRow(0x4100, 2, 10)]
        [DataRow(0x4100, 3, 9)]
        [DataRow(0x4100, 4, 8)]
        [DataRow(0x4100, 5, 7)]
        [DataRow(0x4100, 6, 7)]
        [DataRow(0x4100, 7, 13)]
        [DataRow(0x4100, 8, 12)]
        [DataRow(0x4100, 9, 11)]
        public void LD_B_HLi_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x46 // LD B,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0xC000; });
        }

        [TestMethod]
        [DataRow(0x4100, -100, 7)]
        [DataRow(0x4100, 0, 16)]
        [DataRow(0x4100, 1, 15)]
        [DataRow(0x4100, 2, 14)]
        [DataRow(0x4100, 3, 13)]
        [DataRow(0x4100, 4, 12)]
        [DataRow(0x4100, 5, 11)]
        [DataRow(0x4100, 6, 10)]
        [DataRow(0x4100, 7, 17)]
        [DataRow(0x4100, 8, 16)]
        [DataRow(0x4100, 9, 15)]
        public void LD_B_HLi_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x46 // LD B,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0x4200; });
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
        public void LD_C_B(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x48 // LD C,B
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
        public void LD_C_B_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x48 // LD C,B
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
        public void LD_C_C(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x49 // LD C_C
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
        public void LD_C_C_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x49 // LD C,C
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
        public void LD_C_D(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4A // LD C,D
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
        public void LD_C_D_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4A // LD C,D
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
        public void LD_C_E(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4B // LD C,E
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
        public void LD_C_E_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4B // LD C,E
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
        public void LD_C_H(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4C // LD C,H
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
        public void LD_C_H_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4C // LD C,H
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
        public void LD_C_L(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4D // LD C,L
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
        public void LD_C_L_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4D // LD C,L
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 7)]
        [DataRow(0x8000, 0, 7)]
        [DataRow(0x8000, 1, 7)]
        [DataRow(0x8000, 2, 7)]
        [DataRow(0x8000, 3, 7)]
        [DataRow(0x8000, 4, 7)]
        [DataRow(0x8000, 5, 7)]
        [DataRow(0x8000, 6, 7)]
        [DataRow(0x8000, 7, 7)]
        [DataRow(0x8000, 8, 7)]
        [DataRow(0x8000, 9, 7)]
        public void LD_C_HLi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4E // LD C,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0xC000; });
        }

        [TestMethod]
        [DataRow(0x4100, -100, 7)]
        [DataRow(0x4100, 0, 12)]
        [DataRow(0x4100, 1, 11)]
        [DataRow(0x4100, 2, 10)]
        [DataRow(0x4100, 3, 9)]
        [DataRow(0x4100, 4, 8)]
        [DataRow(0x4100, 5, 7)]
        [DataRow(0x4100, 6, 7)]
        [DataRow(0x4100, 7, 13)]
        [DataRow(0x4100, 8, 12)]
        [DataRow(0x4100, 9, 11)]
        public void LD_C_HLi_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4E // LD C,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0xC000; });
        }

        [TestMethod]
        [DataRow(0x4100, -100, 7)]
        [DataRow(0x4100, 0, 16)]
        [DataRow(0x4100, 1, 15)]
        [DataRow(0x4100, 2, 14)]
        [DataRow(0x4100, 3, 13)]
        [DataRow(0x4100, 4, 12)]
        [DataRow(0x4100, 5, 11)]
        [DataRow(0x4100, 6, 10)]
        [DataRow(0x4100, 7, 17)]
        [DataRow(0x4100, 8, 16)]
        [DataRow(0x4100, 9, 15)]
        public void LD_C_HLi_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4E // LD C,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0x4200; });
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
        public void LD_C_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4F // LD C,A
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
        public void LD_C_A_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x4F // LD C,A
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
        public void LD_D_B(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x50 // LD D,B
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
        public void LD_D_B_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x50 // LD D,B
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
        public void LD_D_C(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x51 // LD D,C
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
        public void LD_D_C_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x51 // LD D,C
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
        public void LD_D_D(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x52 // LD D,D
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
        public void LD_D_D_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x52 // LD D,D
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
        public void LD_D_E(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x53 // LD D,E
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
        public void LD_D_E_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x53 // LD D,E
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
        public void LD_D_H(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x54 // LD D,H
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
        public void LD_D_H_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x54 // LD D,H
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
        public void LD_D_L(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x55 // LD D,L
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
        public void LD_D_L_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x55 // LD D,L
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 7)]
        [DataRow(0x8000, 0, 7)]
        [DataRow(0x8000, 1, 7)]
        [DataRow(0x8000, 2, 7)]
        [DataRow(0x8000, 3, 7)]
        [DataRow(0x8000, 4, 7)]
        [DataRow(0x8000, 5, 7)]
        [DataRow(0x8000, 6, 7)]
        [DataRow(0x8000, 7, 7)]
        [DataRow(0x8000, 8, 7)]
        [DataRow(0x8000, 9, 7)]
        public void LD_D_HLi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x56 // LD D,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0xC000; });
        }

        [TestMethod]
        [DataRow(0x4100, -100, 7)]
        [DataRow(0x4100, 0, 12)]
        [DataRow(0x4100, 1, 11)]
        [DataRow(0x4100, 2, 10)]
        [DataRow(0x4100, 3, 9)]
        [DataRow(0x4100, 4, 8)]
        [DataRow(0x4100, 5, 7)]
        [DataRow(0x4100, 6, 7)]
        [DataRow(0x4100, 7, 13)]
        [DataRow(0x4100, 8, 12)]
        [DataRow(0x4100, 9, 11)]
        public void LD_D_HLi_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x56 // LD D,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0xC000; });
        }

        [TestMethod]
        [DataRow(0x4100, -100, 7)]
        [DataRow(0x4100, 0, 16)]
        [DataRow(0x4100, 1, 15)]
        [DataRow(0x4100, 2, 14)]
        [DataRow(0x4100, 3, 13)]
        [DataRow(0x4100, 4, 12)]
        [DataRow(0x4100, 5, 11)]
        [DataRow(0x4100, 6, 10)]
        [DataRow(0x4100, 7, 17)]
        [DataRow(0x4100, 8, 16)]
        [DataRow(0x4100, 9, 15)]
        public void LD_D_HLi_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x56 // LD D,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0x4200; });
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
        public void LD_D_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x57 // LD D,A
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
        public void LD_D_A_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x57 // LD D,A
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
        public void LD_E_B(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x58 // LD E,B
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
        public void LD_E_B_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x58 // LD E,B
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
        public void LD_E_C(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x59 // LD E_C
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
        public void LD_E_C_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x59 // LD E,C
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
        public void LD_E_D(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5A // LD E,D
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
        public void LD_E_D_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5A // LD E,D
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
        public void LD_E_E(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5B // LD E,E
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
        public void LD_E_E_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5B // LD E,E
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
        public void LD_E_H(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5C // LD E,H
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
        public void LD_E_H_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5C // LD E,H
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
        public void LD_E_L(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5D // LD E,L
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
        public void LD_E_L_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5D // LD E,L
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 7)]
        [DataRow(0x8000, 0, 7)]
        [DataRow(0x8000, 1, 7)]
        [DataRow(0x8000, 2, 7)]
        [DataRow(0x8000, 3, 7)]
        [DataRow(0x8000, 4, 7)]
        [DataRow(0x8000, 5, 7)]
        [DataRow(0x8000, 6, 7)]
        [DataRow(0x8000, 7, 7)]
        [DataRow(0x8000, 8, 7)]
        [DataRow(0x8000, 9, 7)]
        public void LD_E_HLi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5E // LD E,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0xC000; });
        }

        [TestMethod]
        [DataRow(0x4100, -100, 7)]
        [DataRow(0x4100, 0, 12)]
        [DataRow(0x4100, 1, 11)]
        [DataRow(0x4100, 2, 10)]
        [DataRow(0x4100, 3, 9)]
        [DataRow(0x4100, 4, 8)]
        [DataRow(0x4100, 5, 7)]
        [DataRow(0x4100, 6, 7)]
        [DataRow(0x4100, 7, 13)]
        [DataRow(0x4100, 8, 12)]
        [DataRow(0x4100, 9, 11)]
        public void LD_E_HLi_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5E // LD E,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0xC000; });
        }

        [TestMethod]
        [DataRow(0x4100, -100, 7)]
        [DataRow(0x4100, 0, 16)]
        [DataRow(0x4100, 1, 15)]
        [DataRow(0x4100, 2, 14)]
        [DataRow(0x4100, 3, 13)]
        [DataRow(0x4100, 4, 12)]
        [DataRow(0x4100, 5, 11)]
        [DataRow(0x4100, 6, 10)]
        [DataRow(0x4100, 7, 17)]
        [DataRow(0x4100, 8, 16)]
        [DataRow(0x4100, 9, 15)]
        public void LD_E_HLi_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5E // LD E,(HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0x4200; });
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
        public void LD_E_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5F // LD E,A
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
        public void LD_E_A_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x5F // LD E,A
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }
    }
}
