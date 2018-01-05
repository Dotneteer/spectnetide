using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Contention
{
    [TestClass]
    public class S48StandardOpTests02 : ContentionTestBed
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
        public void Rlca(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x07 // RLCA
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
        public void Rlca_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x07 // RLCA
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
        public void EX_AF_AF(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x08 // EX AF,AF'
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
        public void EX_AF_AF_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x08 // EX AF,AF' 
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
        public void Rrca(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0F // RRCA
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
        public void Rrca_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0F // RRCA
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
        public void Rla(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x17 // RLA
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
        public void Rla_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x17 // RLA
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
        public void Rra(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1F // RRA
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
        public void Rra_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1F // RRA
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
        public void Daa(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x27 // DAA
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
        public void Daa_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x27 // DAA
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
        public void Cpl(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2F // CPL
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
        public void Cpl_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2F // CPL
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
        public void Scf(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x37 // SCF
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
        public void Scf_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x37 // SCF
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
        public void Ccf(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3F // CCF
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
        public void Ccf_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3F // CCF
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 11)]
        [DataRow(0x8000, 0, 11)]
        [DataRow(0x8000, 1, 11)]
        [DataRow(0x8000, 2, 11)]
        [DataRow(0x8000, 3, 11)]
        [DataRow(0x8000, 4, 11)]
        [DataRow(0x8000, 5, 11)]
        [DataRow(0x8000, 6, 11)]
        [DataRow(0x8000, 7, 11)]
        [DataRow(0x8000, 8, 11)]
        [DataRow(0x8000, 9, 11)]
        public void ADD_HL_BC(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x09 // ADD HL,BC
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 11)]
        [DataRow(0x4100, 0, 16)]
        [DataRow(0x4100, 1, 15)]
        [DataRow(0x4100, 2, 14)]
        [DataRow(0x4100, 3, 13)]
        [DataRow(0x4100, 4, 12)]
        [DataRow(0x4100, 5, 11)]
        [DataRow(0x4100, 6, 11)]
        [DataRow(0x4100, 7, 17)]
        [DataRow(0x4100, 8, 16)]
        [DataRow(0x4100, 9, 15)]
        public void ADD_HL_BC_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x09 // ADD HL,BC
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 11)]
        [DataRow(0x8000, 0, 11)]
        [DataRow(0x8000, 1, 11)]
        [DataRow(0x8000, 2, 11)]
        [DataRow(0x8000, 3, 11)]
        [DataRow(0x8000, 4, 11)]
        [DataRow(0x8000, 5, 11)]
        [DataRow(0x8000, 6, 11)]
        [DataRow(0x8000, 7, 11)]
        [DataRow(0x8000, 8, 11)]
        [DataRow(0x8000, 9, 11)]
        public void ADD_HL_DE(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x19 // ADD HL,DE
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 11)]
        [DataRow(0x4100, 0, 16)]
        [DataRow(0x4100, 1, 15)]
        [DataRow(0x4100, 2, 14)]
        [DataRow(0x4100, 3, 13)]
        [DataRow(0x4100, 4, 12)]
        [DataRow(0x4100, 5, 11)]
        [DataRow(0x4100, 6, 11)]
        [DataRow(0x4100, 7, 17)]
        [DataRow(0x4100, 8, 16)]
        [DataRow(0x4100, 9, 15)]
        public void ADD_HL_DE_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x19 // ADD HL,DE
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 11)]
        [DataRow(0x8000, 0, 11)]
        [DataRow(0x8000, 1, 11)]
        [DataRow(0x8000, 2, 11)]
        [DataRow(0x8000, 3, 11)]
        [DataRow(0x8000, 4, 11)]
        [DataRow(0x8000, 5, 11)]
        [DataRow(0x8000, 6, 11)]
        [DataRow(0x8000, 7, 11)]
        [DataRow(0x8000, 8, 11)]
        [DataRow(0x8000, 9, 11)]
        public void ADD_HL_HL(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x29 // ADD HL,HL
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 11)]
        [DataRow(0x4100, 0, 16)]
        [DataRow(0x4100, 1, 15)]
        [DataRow(0x4100, 2, 14)]
        [DataRow(0x4100, 3, 13)]
        [DataRow(0x4100, 4, 12)]
        [DataRow(0x4100, 5, 11)]
        [DataRow(0x4100, 6, 11)]
        [DataRow(0x4100, 7, 17)]
        [DataRow(0x4100, 8, 16)]
        [DataRow(0x4100, 9, 15)]
        public void ADD_HL_HL_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x29 // ADD HL,HL
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 11)]
        [DataRow(0x8000, 0, 11)]
        [DataRow(0x8000, 1, 11)]
        [DataRow(0x8000, 2, 11)]
        [DataRow(0x8000, 3, 11)]
        [DataRow(0x8000, 4, 11)]
        [DataRow(0x8000, 5, 11)]
        [DataRow(0x8000, 6, 11)]
        [DataRow(0x8000, 7, 11)]
        [DataRow(0x8000, 8, 11)]
        [DataRow(0x8000, 9, 11)]
        public void ADD_HL_SP(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x39 // ADD HL,SP
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 11)]
        [DataRow(0x4100, 0, 16)]
        [DataRow(0x4100, 1, 15)]
        [DataRow(0x4100, 2, 14)]
        [DataRow(0x4100, 3, 13)]
        [DataRow(0x4100, 4, 12)]
        [DataRow(0x4100, 5, 11)]
        [DataRow(0x4100, 6, 11)]
        [DataRow(0x4100, 7, 17)]
        [DataRow(0x4100, 8, 16)]
        [DataRow(0x4100, 9, 15)]
        public void ADD_HL_SP_Contented(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x39 // ADD HL,SP
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
        public void LD_A_BCi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0A // LD A,(BC)
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
        public void LD_A_BCi_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0A // LD A,(BC)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.BC = 0x0000; });
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
        public void LD_A_BCi_Contended2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0A // LD A,(BC)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.BC = 0x4200; });
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
        public void LD_A_DEi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1A // LD A,(DE)
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
        public void LD_A_DEi_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1A // LD A,(DE)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.DE = 0x0000; });
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
        public void LD_A_DEi_Contended2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1A // LD A,(DE)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.DE = 0x4200; });
        }

        [TestMethod]
        [DataRow(0x8000, -100, 16)]
        [DataRow(0x8000, 0, 16)]
        [DataRow(0x8000, 1, 16)]
        [DataRow(0x8000, 2, 16)]
        [DataRow(0x8000, 3, 16)]
        [DataRow(0x8000, 4, 16)]
        [DataRow(0x8000, 5, 16)]
        [DataRow(0x8000, 6, 16)]
        [DataRow(0x8000, 7, 16)]
        [DataRow(0x8000, 8, 16)]
        [DataRow(0x8000, 9, 16)]
        public void LD_NNi_HL(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x22, 0x00, 0x00 // LD (0x0000),HL
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 16)]
        [DataRow(0x8000, 0, 24)]
        [DataRow(0x8000, 1, 23)]
        [DataRow(0x8000, 2, 22)]
        [DataRow(0x8000, 3, 21)]
        [DataRow(0x8000, 4, 20)]
        [DataRow(0x8000, 5, 27)]
        [DataRow(0x8000, 6, 26)]
        [DataRow(0x8000, 7, 25)]
        [DataRow(0x8000, 8, 24)]
        [DataRow(0x8000, 9, 23)]
        public void LD_NNi_HL_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x22, 0x00, 0x42 // LD (0x4200),HL
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 16)]
        [DataRow(0x4100, 0, 40)]
        [DataRow(0x4100, 1, 39)]
        [DataRow(0x4100, 2, 38)]
        [DataRow(0x4100, 3, 37)]
        [DataRow(0x4100, 4, 36)]
        [DataRow(0x4100, 5, 35)]
        [DataRow(0x4100, 6, 34)]
        [DataRow(0x4100, 7, 41)]
        [DataRow(0x4100, 8, 40)]
        [DataRow(0x4100, 9, 39)]
        public void LD_NNi_HL_Contended2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x22, 0x00, 0x42 // LD (0x4200),HL
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 16)]
        [DataRow(0x8000, 0, 16)]
        [DataRow(0x8000, 1, 16)]
        [DataRow(0x8000, 2, 16)]
        [DataRow(0x8000, 3, 16)]
        [DataRow(0x8000, 4, 16)]
        [DataRow(0x8000, 5, 16)]
        [DataRow(0x8000, 6, 16)]
        [DataRow(0x8000, 7, 16)]
        [DataRow(0x8000, 8, 16)]
        [DataRow(0x8000, 9, 16)]
        public void LD_HL_NNi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2A, 0x00, 0x00 // LD HL,(0x0000)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 16)]
        [DataRow(0x8000, 0, 24)]
        [DataRow(0x8000, 1, 23)]
        [DataRow(0x8000, 2, 22)]
        [DataRow(0x8000, 3, 21)]
        [DataRow(0x8000, 4, 20)]
        [DataRow(0x8000, 5, 27)]
        [DataRow(0x8000, 6, 26)]
        [DataRow(0x8000, 7, 25)]
        [DataRow(0x8000, 8, 24)]
        [DataRow(0x8000, 9, 23)]
        public void LD_HL_NNi_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2A, 0x00, 0x42 // LD HL,(0x4200)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 16)]
        [DataRow(0x4100, 0, 40)]
        [DataRow(0x4100, 1, 39)]
        [DataRow(0x4100, 2, 38)]
        [DataRow(0x4100, 3, 37)]
        [DataRow(0x4100, 4, 36)]
        [DataRow(0x4100, 5, 35)]
        [DataRow(0x4100, 6, 34)]
        [DataRow(0x4100, 7, 41)]
        [DataRow(0x4100, 8, 40)]
        [DataRow(0x4100, 9, 39)]
        public void LD_HL_NNi_Contended2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2A, 0x00, 0x42 // LD HL,(0x4200)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 13)]
        [DataRow(0x8000, 0, 13)]
        [DataRow(0x8000, 1, 13)]
        [DataRow(0x8000, 2, 13)]
        [DataRow(0x8000, 3, 13)]
        [DataRow(0x8000, 4, 13)]
        [DataRow(0x8000, 5, 13)]
        [DataRow(0x8000, 6, 13)]
        [DataRow(0x8000, 7, 13)]
        [DataRow(0x8000, 8, 13)]
        [DataRow(0x8000, 9, 13)]
        public void LD_NNi_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x32, 0x00, 0x00 // LD (0x0000),A
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 13)]
        [DataRow(0x8000, 0, 16)]
        [DataRow(0x8000, 1, 15)]
        [DataRow(0x8000, 2, 14)]
        [DataRow(0x8000, 3, 13)]
        [DataRow(0x8000, 4, 13)]
        [DataRow(0x8000, 5, 19)]
        [DataRow(0x8000, 6, 18)]
        [DataRow(0x8000, 7, 17)]
        [DataRow(0x8000, 8, 16)]
        [DataRow(0x8000, 9, 15)]
        public void LD_NNi_A_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x32, 0x00, 0x42 // LD (0x4200),A
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 13)]
        [DataRow(0x4100, 0, 32)]
        [DataRow(0x4100, 1, 31)]
        [DataRow(0x4100, 2, 30)]
        [DataRow(0x4100, 3, 29)]
        [DataRow(0x4100, 4, 28)]
        [DataRow(0x4100, 5, 27)]
        [DataRow(0x4100, 6, 26)]
        [DataRow(0x4100, 7, 33)]
        [DataRow(0x4100, 8, 32)]
        [DataRow(0x4100, 9, 31)]
        public void LD_NNi_A_Contended2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x32, 0x00, 0x42 // LD (0x4200),A
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 13)]
        [DataRow(0x8000, 0, 13)]
        [DataRow(0x8000, 1, 13)]
        [DataRow(0x8000, 2, 13)]
        [DataRow(0x8000, 3, 13)]
        [DataRow(0x8000, 4, 13)]
        [DataRow(0x8000, 5, 13)]
        [DataRow(0x8000, 6, 13)]
        [DataRow(0x8000, 7, 13)]
        [DataRow(0x8000, 8, 13)]
        [DataRow(0x8000, 9, 13)]
        public void LD_A_NNi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3A, 0x00, 0x00 // LD A,(0x0000)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 13)]
        [DataRow(0x8000, 0, 16)]
        [DataRow(0x8000, 1, 15)]
        [DataRow(0x8000, 2, 14)]
        [DataRow(0x8000, 3, 13)]
        [DataRow(0x8000, 4, 13)]
        [DataRow(0x8000, 5, 19)]
        [DataRow(0x8000, 6, 18)]
        [DataRow(0x8000, 7, 17)]
        [DataRow(0x8000, 8, 16)]
        [DataRow(0x8000, 9, 15)]
        public void LD_A_NNi_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3A, 0x00, 0x42 // LD A,(0x4200)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 13)]
        [DataRow(0x4100, 0, 32)]
        [DataRow(0x4100, 1, 31)]
        [DataRow(0x4100, 2, 30)]
        [DataRow(0x4100, 3, 29)]
        [DataRow(0x4100, 4, 28)]
        [DataRow(0x4100, 5, 27)]
        [DataRow(0x4100, 6, 26)]
        [DataRow(0x4100, 7, 33)]
        [DataRow(0x4100, 8, 32)]
        [DataRow(0x4100, 9, 31)]
        public void LD_A_NNi_Contended2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3A, 0x00, 0x42 // LD A,(0x4200)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }
    }
}
