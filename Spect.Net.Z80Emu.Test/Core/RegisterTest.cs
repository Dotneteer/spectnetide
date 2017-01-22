using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Core;
using Spect.Net.Z80Emu.Core.Exceptions;

namespace Spect.Net.Z80Emu.Test.Core
{
    [TestClass]
    public class RegisterTest
    {
        [TestMethod]
        public void ExchangeAfSetWorksAsExpected()
        {
            // --- Arrange
            var regs = new Registers();
            regs.AF = 0xABCD;
            regs._AF_ = 0x2345;

            // --- Act
            regs.ExchangeAfSet();

            // --- Assert
            regs.AF.ShouldBe((ushort)0x2345);
            regs._AF_.ShouldBe((ushort)0xABCD);
        }

        [TestMethod]
        public void ExchangeRegisterSetWorksAsExpected()
        {
            // --- Arrange
            var regs = new Registers();
            regs.BC = 0xABCD;
            regs._BC_ = 0x2345;
            regs.DE = 0xBCDE;
            regs._DE_ = 0x3456;
            regs.HL = 0xCDEF;
            regs._HL_ = 0x4567;

            // --- Act
            regs.ExchangeRegisterSet();

            // --- Assert
            regs.BC.ShouldBe((ushort)0x2345);
            regs._BC_.ShouldBe((ushort)0xABCD);
            regs.DE.ShouldBe((ushort)0x3456);
            regs._DE_.ShouldBe((ushort)0xBCDE);
            regs.HL.ShouldBe((ushort)0x4567);
            regs._HL_.ShouldBe((ushort)0xCDEF);
        }

        [TestMethod]
        public void Reg8IndexingWorksWithCorrectIndex()
        {
            // --- Arrange
            var regs = new Registers();
            regs.AF = 0x0001;
            regs.BC = 0x2345;
            regs.DE = 0x6789;
            regs.HL = 0xABCD;

            // --- Act/Assert
            regs[Reg8Index.A].ShouldBe(regs.A);
            regs[Reg8Index.F].ShouldBe(regs.F);
            regs[Reg8Index.B].ShouldBe(regs.B);
            regs[Reg8Index.C].ShouldBe(regs.C);
            regs[Reg8Index.D].ShouldBe(regs.D);
            regs[Reg8Index.E].ShouldBe(regs.E);
            regs[Reg8Index.H].ShouldBe(regs.H);
            regs[Reg8Index.L].ShouldBe(regs.L);
        }

        [TestMethod]
        public void Reg8SetWorksWithCorrectIndex()
        {
            // --- Arrange
            var regs = new Registers();

            // --- Act
            regs[Reg8Index.A] = 0x00;
            regs[Reg8Index.F] = 0x01;
            regs[Reg8Index.B] = 0x23;
            regs[Reg8Index.C] = 0x34;
            regs[Reg8Index.D] = 0x45;
            regs[Reg8Index.E] = 0x56;
            regs[Reg8Index.H] = 0x67;
            regs[Reg8Index.L] = 0x78;

            // --- Assert
            regs.A.ShouldBe((byte)0x00);
            regs.F.ShouldBe((byte)0x01);
            regs.B.ShouldBe((byte)0x23);
            regs.C.ShouldBe((byte)0x34);
            regs.D.ShouldBe((byte)0x45);
            regs.E.ShouldBe((byte)0x56);
            regs.H.ShouldBe((byte)0x67);
            regs.L.ShouldBe((byte)0x78);
        }

        [TestMethod]
        [ExpectedException(typeof(RegisterAddressException))]
        public void Reg8IndexingFailWithInvalidIndex()
        {
            // --- Arrange
            var regs = new Registers();

            // --- Act/Assert
            // ReSharper disable once UnusedVariable
            var err = regs[(Reg8Index) 12];
        }

        [TestMethod]
        [ExpectedException(typeof(RegisterAddressException))]
        public void Reg8SetFailWithInvalidIndex()
        {
            // --- Arrange
            var regs = new Registers();

            // --- Act/Assert
            // ReSharper disable once UnusedVariable
            regs[(Reg8Index)12] = 0x00;
        }

        [TestMethod]
        public void Reg16IndexingWorksWithCorrectIndex()
        {
            // --- Arrange
            var regs = new Registers();
            regs.SP = 0x0001;
            regs.BC = 0x2345;
            regs.DE = 0x6789;
            regs.HL = 0xABCD;

            // --- Act/Assert
            regs[Reg16Index.BC].ShouldBe(regs.BC);
            regs[Reg16Index.DE].ShouldBe(regs.DE);
            regs[Reg16Index.HL].ShouldBe(regs.HL);
            regs[Reg16Index.SP].ShouldBe(regs.SP);
        }

        [TestMethod]
        public void Reg16SetWorksWithCorrectIndex()
        {
            // --- Arrange
            var regs = new Registers();

            // --- Act
            regs[Reg16Index.BC] = 0x2345;
            regs[Reg16Index.DE] = 0x3456;
            regs[Reg16Index.HL] = 0x4567;
            regs[Reg16Index.SP] = 0x5678;

            // --- Assert
            regs.BC.ShouldBe((ushort)0x2345);
            regs.DE.ShouldBe((ushort)0x3456);
            regs.HL.ShouldBe((ushort)0x4567);
            regs.SP.ShouldBe((ushort)0x5678);
        }

        [TestMethod]
        [ExpectedException(typeof(RegisterAddressException))]
        public void Reg16IndexingFailWithInvalidIndex()
        {
            // --- Arrange
            var regs = new Registers();

            // --- Act/Assert
            // ReSharper disable once UnusedVariable
            var err = regs[(Reg16Index)12];
        }

        [TestMethod]
        [ExpectedException(typeof(RegisterAddressException))]
        public void Reg16SetFailWithInvalidIndex()
        {
            // --- Arrange
            var regs = new Registers();

            // --- Act/Assert
            // ReSharper disable once UnusedVariable
            regs[(Reg16Index)12] = 0x00;
        }
    }
}
