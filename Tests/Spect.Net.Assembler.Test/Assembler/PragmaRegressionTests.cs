using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class PragmaRegressionTests : AssemblerTestBed
    {
        [TestMethod]
        public void OrgPragmaSetsLabelAddress()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol .org #6789
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Symbols["MYSYMBOL"].Value.Value.ShouldBe((ushort)0x6789);
        }

        [TestMethod]
        public void OrgPragmaRaisesErrorWithDuplicatedLabel()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol .equ #100
                MySymbol .org #6789
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0040);
        }

        [TestMethod]
        public void VarPragmaRefusesSymbolCreatedWithEqu()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol: .equ #4000
                MySymbol: .var #6000");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0087);
        }

        [TestMethod]
        public void VarPragmaRefusesExistingSymbol()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol: ld a,b
                MySymbol: .var #6000");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0087);
        }

        [TestMethod]
        public void DefwPragmaWorksWithExpression()
        {
            CodeEmitWorks(@"
                MySymbol .org #8000
                    .defw MySymbol",
                0x00, 0x80);
        }

        [TestMethod]
        public void EntPragmaWorksWithLateEvaluation()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6789
                .ent MyStart
                
            MyStart: ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.EntryAddress.ShouldBe((ushort)0x6789);
        }

        [TestMethod]
        public void XentPragmaWorksWithLateEvaluation()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6789
                .xent MyStart
                
            MyStart: ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.ExportEntryAddress.ShouldBe((ushort)0x6789);
        }

        [TestMethod]
        public void DefsPragmaRaisesErrorWithUndefinedSymbol()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol .defs unknown");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void FilbPragmaRaisesErrorWithUndefinedSymbol1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol .fillb unknown, #25");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void FilbPragmaRaisesErrorWithUndefinedSymbol2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol .fillb #25, unknown");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void FilwPragmaRaisesErrorWithUndefinedSymbol1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol .fillw unknown, #25");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void FilwPragmaRaisesErrorWithUndefinedSymbol2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol .fillw #25, unknown");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        [DataRow(".dm textof(ld)", new byte[] { 0x4C, 0x44 })]
        [DataRow(".dm TEXTOF(ld)", new byte[] { 0x4C, 0x44 })]
        public void TextOfWorksAsExpected(string source, byte[] expected)
        {
            CodeEmitWorks(source, expected);
        }

        [TestMethod]
        [DataRow(".dm ltextof(ld)", new byte[] { 0x6C, 0x64 })]
        [DataRow(".dm LTEXTOF(ld)", new byte[] { 0x6C, 0x64 })]
        public void LTextOfWorksAsExpected(string source, byte[] expected)
        {
            CodeEmitWorks(source, expected);
        }

    }
}
