using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Contention
{
    [TestClass]
    public class S48StandardOpTests04: ContentionTestBed
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
        public void LD_H_B(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x60 // LD H,B
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
        public void LD_H_B_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x60 // LD H,B
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
        public void LD_H_C(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x61 // LD H,C
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
        public void LD_H_C_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x61 // LD H,C
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
        public void LD_H_D(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x62 // LD H,D
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
        public void LD_H_D_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x62 // LD H,D
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
        public void LD_H_E(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x63 // LD H,E
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
        public void LD_H_E_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x63 // LD H,E
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
        public void LD_H_H(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x64 // LD H,H
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
        public void LD_H_H_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x64 // LD H,H
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
        public void LD_H_L(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x65 // LD H,L
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
        public void LD_H_L_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x65 // LD H,L
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
        public void LD_H_HLi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x66 // LD H,(HL)
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
        public void LD_H_HLi_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x66 // LD H,(HL)
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
        public void LD_H_HLi_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x66 // LD H,(HL)
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
        public void LD_H_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x67 // LD H,A
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
        public void LD_H_A_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x67 // LD H,A
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
        public void LD_L_B(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x68 // LD L,B
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
        public void LD_L_B_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x68 // LD L,B
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
        public void LD_L_C(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x69 // LD L_C
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
        public void LD_L_C_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x69 // LD L,C
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
        public void LD_L_D(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6A // LD L,D
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
        public void LD_L_D_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6A // LD L,D
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
        public void LD_L_E(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6B // LD L,E
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
        public void LD_L_E_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6B // LD L,E
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
        public void LD_L_H(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6C // LD L,H
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
        public void LD_L_H_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6C // LD L,H
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
        public void LD_L_L(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6D // LD L,L
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
        public void LD_L_L_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6D // LD L,L
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
        public void LD_L_HLi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6E // LD L,(HL)
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
        public void LD_L_HLi_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6E // LD L,(HL)
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
        public void LD_L_HLi_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6E // LD L,(HL)
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
        public void LD_L_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6F // LD L,A
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
        public void LD_L_A_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x6F // LD L,A
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
        public void LD_A_B(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x78 // LD A,B
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
        public void LD_A_B_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x78 // LD A,B
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
        public void LD_A_C(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x79 // LD A,C
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
        public void LD_A_C_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x79 // LD A,C
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
        public void LD_A_D(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7A // LD A,D
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
        public void LD_A_D_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7A // LD A,D
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
        public void LD_A_E(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7B // LD A,E
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
        public void LD_A_E_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7B // LD A,E
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
        public void LD_A_H(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7C // LD A,H
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
        public void LD_A_H_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7C // LD A,H
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
        public void LD_A_L(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7D // LD A,L
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
        public void LD_A_L_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7D // LD A,L
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
        public void LD_A_HLi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7E // LD A,(HL)
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
        public void LD_A_HLi_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7E // LD A,(HL)
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
        public void LD_A_HLi_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7E // LD A,(HL)
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
        public void LD_A_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7F // LD A,A
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
        public void LD_A_A_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x7F // LD A,A
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
        public void LD_HLi_B(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x70 // LD (HL),B
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
        public void LD_HLi_B_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x70 // LD (HL),B
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
        public void LD_HLi_B_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x70 // LD (HL),B
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0x4200; });
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
        public void LD_HLi_C(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x71 // LD (HL),C
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
        public void LD_HLi_C_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x71 // LD (HL),C
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
        public void LD_HLi_C_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x71 // LD (HL),C
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0x4200; });
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
        public void LD_HLi_D(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x72 // LD (HL),D
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
        public void LD_HLi_D_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x72 // LD (HL),D
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
        public void LD_HLi_D_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x72 // LD (HL),D
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0x4200; });
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
        public void LD_HLi_E(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x73 // LD (HL),E
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
        public void LD_HLi_E_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x73 // LD (HL),E
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
        public void LD_HLi_E_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x73 // LD (HL),E
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0x4200; });
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
        public void LD_HLi_H(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x74 // LD (HL),H
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
        public void LD_HLi_H_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x74 // LD (HL),H
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
        public void LD_HLi_H_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x74 // LD (HL),H
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0x4200; });
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
        public void LD_HLi_L(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x75 // LD (HL),L
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
        public void LD_HLi_L_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x75 // LD (HL),L
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
        public void LD_HLi_L_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x75 // LD (HL),L
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0x4200; });
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
        public void LD_HLi_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x77 // LD (HL),A
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
        public void LD_HLi_A_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x77 // LD (HL),A
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
        public void LD_HLi_A_Contented2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x77 // LD (HL),A
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                sp => { sp.Cpu.Registers.HL = 0x4200; });
        }
    }
}