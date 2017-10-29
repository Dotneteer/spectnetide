using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

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
            output.Symbols["MYSYMBOL"].ShouldBe((ushort)0x6789);
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
    }
}
