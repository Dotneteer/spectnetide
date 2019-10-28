using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Contention
{
    [TestClass]
    public class S48StandardOpTests01: ContentionTestBed
    {
        [TestMethod]
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
        [DataRow(0x8000, -100, 4)]
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
        public void LD_BCi_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x02 // LD (BC),A
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
        public void LD_BCi_A_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x02 // LD (BC),A
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
        public void LD_BCi_A_Contended2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x02 // LD (BC),A
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
        public void LD_DEi_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x12 // LD (DE),A
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
        public void LD_DEi_A_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x12 // LD (DE),A
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
        public void LD_DEi_A_Contended2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x12 // LD (DE),A
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.DE = 0x4200; });
        }

        [TestMethod]
        [DataRow(0x8000, -100, 6)]
        [DataRow(0x8000, 0, 6)]
        [DataRow(0x8000, 1, 6)]
        [DataRow(0x8000, 2, 6)]
        [DataRow(0x8000, 3, 6)]
        [DataRow(0x8000, 4, 6)]
        [DataRow(0x8000, 5, 6)]
        [DataRow(0x8000, 6, 6)]
        [DataRow(0x8000, 7, 6)]
        [DataRow(0x8000, 8, 6)]
        [DataRow(0x8000, 9, 6)]
        public void INC_BC(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x03 // INC BC
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 6)]
        [DataRow(0x4100, 0, 11)]
        [DataRow(0x4100, 1, 10)]
        [DataRow(0x4100, 2, 9)]
        [DataRow(0x4100, 3, 8)]
        [DataRow(0x4100, 4, 7)]
        [DataRow(0x4100, 5, 6)]
        [DataRow(0x4100, 6, 6)]
        [DataRow(0x4100, 7, 12)]
        [DataRow(0x4100, 8, 11)]
        [DataRow(0x4100, 9, 10)]
        public void INC_BC_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x03 // INC BC
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 6)]
        [DataRow(0x8000, 0, 6)]
        [DataRow(0x8000, 1, 6)]
        [DataRow(0x8000, 2, 6)]
        [DataRow(0x8000, 3, 6)]
        [DataRow(0x8000, 4, 6)]
        [DataRow(0x8000, 5, 6)]
        [DataRow(0x8000, 6, 6)]
        [DataRow(0x8000, 7, 6)]
        [DataRow(0x8000, 8, 6)]
        [DataRow(0x8000, 9, 6)]
        public void INC_DE(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x13 // INC DE
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 6)]
        [DataRow(0x4100, 0, 11)]
        [DataRow(0x4100, 1, 10)]
        [DataRow(0x4100, 2, 9)]
        [DataRow(0x4100, 3, 8)]
        [DataRow(0x4100, 4, 7)]
        [DataRow(0x4100, 5, 6)]
        [DataRow(0x4100, 6, 6)]
        [DataRow(0x4100, 7, 12)]
        [DataRow(0x4100, 8, 11)]
        [DataRow(0x4100, 9, 10)]
        public void INC_DE_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x13 // INC DE
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 6)]
        [DataRow(0x8000, 0, 6)]
        [DataRow(0x8000, 1, 6)]
        [DataRow(0x8000, 2, 6)]
        [DataRow(0x8000, 3, 6)]
        [DataRow(0x8000, 4, 6)]
        [DataRow(0x8000, 5, 6)]
        [DataRow(0x8000, 6, 6)]
        [DataRow(0x8000, 7, 6)]
        [DataRow(0x8000, 8, 6)]
        [DataRow(0x8000, 9, 6)]
        public void INC_HL(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x23 // INC HL
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 6)]
        [DataRow(0x4100, 0, 11)]
        [DataRow(0x4100, 1, 10)]
        [DataRow(0x4100, 2, 9)]
        [DataRow(0x4100, 3, 8)]
        [DataRow(0x4100, 4, 7)]
        [DataRow(0x4100, 5, 6)]
        [DataRow(0x4100, 6, 6)]
        [DataRow(0x4100, 7, 12)]
        [DataRow(0x4100, 8, 11)]
        [DataRow(0x4100, 9, 10)]
        public void INC_HL_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x23 // INC HL
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 6)]
        [DataRow(0x8000, 0, 6)]
        [DataRow(0x8000, 1, 6)]
        [DataRow(0x8000, 2, 6)]
        [DataRow(0x8000, 3, 6)]
        [DataRow(0x8000, 4, 6)]
        [DataRow(0x8000, 5, 6)]
        [DataRow(0x8000, 6, 6)]
        [DataRow(0x8000, 7, 6)]
        [DataRow(0x8000, 8, 6)]
        [DataRow(0x8000, 9, 6)]
        public void INC_SP(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x33 // INC SP
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 6)]
        [DataRow(0x4100, 0, 11)]
        [DataRow(0x4100, 1, 10)]
        [DataRow(0x4100, 2, 9)]
        [DataRow(0x4100, 3, 8)]
        [DataRow(0x4100, 4, 7)]
        [DataRow(0x4100, 5, 6)]
        [DataRow(0x4100, 6, 6)]
        [DataRow(0x4100, 7, 12)]
        [DataRow(0x4100, 8, 11)]
        [DataRow(0x4100, 9, 10)]
        public void INC_SP_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x33 // INC SP
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
        public void INC_B(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x04 // INC B
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
        public void INC_B_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x04 // INC B
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
        public void INC_C(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0C // INC C
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
        public void INC_C_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0C // INC C
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
        public void INC_D(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x14 // INC D
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
        public void INC_D_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x14 // INC D
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
        public void INC_E(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1C // INC E
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
        public void INC_E_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1C // INC E
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
        public void INC_H(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x24 // INC H
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
        public void INC_H_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x24 // INC H
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
        public void INC_L(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2C // INC L
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
        public void INC_L_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2C // INC L
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
        public void INC_HLi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x34 // INC (HL))
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.HL = 0x0000; });
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
        public void INC_HLi_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x34 // INC (HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.HL = 0x0000; });
        }

        [TestMethod]
        [DataRow(0x4100, -100, 11)]
        [DataRow(0x4100, 0, 25)]
        [DataRow(0x4100, 1, 24)]
        [DataRow(0x4100, 2, 23)]
        [DataRow(0x4100, 3, 22)]
        [DataRow(0x4100, 4, 21)]
        [DataRow(0x4100, 5, 20)]
        [DataRow(0x4100, 6, 19)]
        [DataRow(0x4100, 7, 26)]
        [DataRow(0x4100, 8, 25)]
        [DataRow(0x4100, 9, 24)]
        public void INC_HLi_Contended2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x34 // INC (HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.HL = 0x4200; });
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
        public void INC_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3C // INC A
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
        public void INC_A_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3C // INC A
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
        public void DEC_B(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x05 // DEC B
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
        public void DEC_B_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x05 // DEC B
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
        public void DEC_C(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0D // DEC C
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
        public void DEC_C_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0D // DEC C
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
        public void DEC_D(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x15 // DEC D
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
        public void DEC_D_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x15 // DEC D
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
        public void DEC_E(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1D // DEC E
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
        public void DEC_E_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1D // DEC E
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
        public void DEC_H(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x25 // DEC H
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
        public void DEC_H_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x25 // DEC H
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
        public void DEC_L(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2D // DEC L
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
        public void DEC_L_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2D // DEC L
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
        public void DEC_HLi(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x35 // DEC (HL)
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
        public void DEC_HLi_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x35 // DEC (HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.HL = 0x0000; });
        }

        [TestMethod]
        [DataRow(0x4100, -100, 11)]
        [DataRow(0x4100, 0, 25)]
        [DataRow(0x4100, 1, 24)]
        [DataRow(0x4100, 2, 23)]
        [DataRow(0x4100, 3, 22)]
        [DataRow(0x4100, 4, 21)]
        [DataRow(0x4100, 5, 20)]
        [DataRow(0x4100, 6, 19)]
        [DataRow(0x4100, 7, 26)]
        [DataRow(0x4100, 8, 25)]
        [DataRow(0x4100, 9, 24)]
        public void DEC_HLi_Contended2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x35 // DEC (HL)
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.HL = 0x4200; });
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
        public void DEC_A(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3D // DEC A
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
        public void DEC_A_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3D // DEC A
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 6)]
        [DataRow(0x8000, 0, 6)]
        [DataRow(0x8000, 1, 6)]
        [DataRow(0x8000, 2, 6)]
        [DataRow(0x8000, 3, 6)]
        [DataRow(0x8000, 4, 6)]
        [DataRow(0x8000, 5, 6)]
        [DataRow(0x8000, 6, 6)]
        [DataRow(0x8000, 7, 6)]
        [DataRow(0x8000, 8, 6)]
        [DataRow(0x8000, 9, 6)]
        public void DEC_BC(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0B // DEC BC
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 6)]
        [DataRow(0x4100, 0, 11)]
        [DataRow(0x4100, 1, 10)]
        [DataRow(0x4100, 2, 9)]
        [DataRow(0x4100, 3, 8)]
        [DataRow(0x4100, 4, 7)]
        [DataRow(0x4100, 5, 6)]
        [DataRow(0x4100, 6, 6)]
        [DataRow(0x4100, 7, 12)]
        [DataRow(0x4100, 8, 11)]
        [DataRow(0x4100, 9, 10)]
        public void DEC_BC_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0B // DEC BC
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 6)]
        [DataRow(0x8000, 0, 6)]
        [DataRow(0x8000, 1, 6)]
        [DataRow(0x8000, 2, 6)]
        [DataRow(0x8000, 3, 6)]
        [DataRow(0x8000, 4, 6)]
        [DataRow(0x8000, 5, 6)]
        [DataRow(0x8000, 6, 6)]
        [DataRow(0x8000, 7, 6)]
        [DataRow(0x8000, 8, 6)]
        [DataRow(0x8000, 9, 6)]
        public void DEC_DE(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1B // DEC DE
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 6)]
        [DataRow(0x4100, 0, 11)]
        [DataRow(0x4100, 1, 10)]
        [DataRow(0x4100, 2, 9)]
        [DataRow(0x4100, 3, 8)]
        [DataRow(0x4100, 4, 7)]
        [DataRow(0x4100, 5, 6)]
        [DataRow(0x4100, 6, 6)]
        [DataRow(0x4100, 7, 12)]
        [DataRow(0x4100, 8, 11)]
        [DataRow(0x4100, 9, 10)]
        public void DEC_DE_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1B // DEC DE
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 6)]
        [DataRow(0x8000, 0, 6)]
        [DataRow(0x8000, 1, 6)]
        [DataRow(0x8000, 2, 6)]
        [DataRow(0x8000, 3, 6)]
        [DataRow(0x8000, 4, 6)]
        [DataRow(0x8000, 5, 6)]
        [DataRow(0x8000, 6, 6)]
        [DataRow(0x8000, 7, 6)]
        [DataRow(0x8000, 8, 6)]
        [DataRow(0x8000, 9, 6)]
        public void DEC_HL(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2B // DEC HL
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 6)]
        [DataRow(0x4100, 0, 11)]
        [DataRow(0x4100, 1, 10)]
        [DataRow(0x4100, 2, 9)]
        [DataRow(0x4100, 3, 8)]
        [DataRow(0x4100, 4, 7)]
        [DataRow(0x4100, 5, 6)]
        [DataRow(0x4100, 6, 6)]
        [DataRow(0x4100, 7, 12)]
        [DataRow(0x4100, 8, 11)]
        [DataRow(0x4100, 9, 10)]
        public void DEC_HL_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2B // DEC HL
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x8000, -100, 6)]
        [DataRow(0x8000, 0, 6)]
        [DataRow(0x8000, 1, 6)]
        [DataRow(0x8000, 2, 6)]
        [DataRow(0x8000, 3, 6)]
        [DataRow(0x8000, 4, 6)]
        [DataRow(0x8000, 5, 6)]
        [DataRow(0x8000, 6, 6)]
        [DataRow(0x8000, 7, 6)]
        [DataRow(0x8000, 8, 6)]
        [DataRow(0x8000, 9, 6)]
        public void DEC_SP(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3B // DEC SP
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 6)]
        [DataRow(0x4100, 0, 11)]
        [DataRow(0x4100, 1, 10)]
        [DataRow(0x4100, 2, 9)]
        [DataRow(0x4100, 3, 8)]
        [DataRow(0x4100, 4, 7)]
        [DataRow(0x4100, 5, 6)]
        [DataRow(0x4100, 6, 6)]
        [DataRow(0x4100, 7, 12)]
        [DataRow(0x4100, 8, 11)]
        [DataRow(0x4100, 9, 10)]
        public void DEC_SP_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3B // DEC SP
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
        public void LD_B_N(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x06, 0x19 // LD B,0x19
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
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
        public void LD_B_N_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x06, 0x19 // LD B,0x19
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
        public void LD_C_N(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0E, 0x19 // LD C,0x19
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
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
        public void LD_C_N_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x0E, 0x19 // LD C,0x19
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
        public void LD_D_N(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x16, 0x19 // LD D,0x19
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
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
        public void LD_D_N_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x16, 0x19 // LD D,0x19
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
        public void LD_E_N(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1E, 0x19 // LD D,0x19
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
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
        public void LD_E_N_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x1E, 0x19 // LD E,0x19
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
        public void LD_H_N(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x26, 0x19 // LD H,0x19
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
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
        public void LD_H_N_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x26, 0x19 // LD H,0x19
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
        public void LD_L_N(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2E, 0x19 // LD L,0x19
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
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
        public void LD_L_N_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x2E, 0x19 // LD L,0x19
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
        public void LD_HLi_N(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x36, 0x19 // LD (HL),0x19
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

        [TestMethod]
        [DataRow(0x4100, -100, 10)]
        [DataRow(0x4100, 0, 19)]
        [DataRow(0x4100, 1, 18)]
        [DataRow(0x4100, 2, 17)]
        [DataRow(0x4100, 3, 16)]
        [DataRow(0x4100, 4, 15)]
        [DataRow(0x4100, 5, 14)]
        [DataRow(0x4100, 6, 13)]
        [DataRow(0x4100, 7, 20)]
        [DataRow(0x4100, 8, 19)]
        [DataRow(0x4100, 9, 18)]
        public void LD_HLi_N_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x36, 0x19 // LD (HL),0x19
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.HL = 0x0000; });
        }

        [TestMethod]
        [DataRow(0x4100, -100, 10)]
        [DataRow(0x4100, 0, 24)]
        [DataRow(0x4100, 1, 23)]
        [DataRow(0x4100, 2, 22)]
        [DataRow(0x4100, 3, 21)]
        [DataRow(0x4100, 4, 20)]
        [DataRow(0x4100, 5, 19)]
        [DataRow(0x4100, 6, 18)]
        [DataRow(0x4100, 7, 25)]
        [DataRow(0x4100, 8, 24)]
        [DataRow(0x4100, 9, 23)]
        public void LD_HLi_N_Contended2(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x36, 0x19 // LD (HL),0x19
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength,
                s => { s.Cpu.Registers.HL = 0x4200; });
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
        public void LD_A_N(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3E, 0x19 // LD A,0x19
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
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
        public void LD_A_N_Contended(int codeAddress, int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var ops = new List<byte>
            {
                0x3E, 0x19 // LD A,0x19
            };

            // --- Act/Assert
            ExecuteContentionTest(ops, codeAddress, tactsFromFirstPixel, expectedLength);
        }

    }
}
