using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class MacroArgumentEmitTests: AssemblerTestBed
    {
        [TestMethod]
        public void MacroArgumentInGlobalScopeFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                {{MyParam}}
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0420);
        }

        [TestMethod]
        public void MacroArgumentInLocalScopeFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .loop 3
                {{MyParam}}
                .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0420);
        }

        [TestMethod]
        public void MacroArgumentInArgumentlessMacroDeclarationFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro()
                {{MyParam}}
                .endm
                MyMacro()
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(2);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0403);
        }

        [TestMethod]
        [DataRow("ld {{ First}},{{ MyMacro }}", "MyMacro")]
        [DataRow("ld a,{{ MyMacro }}", "MyMacro")]
        public void MacroArgumentRegexWorks(string source, string expected)
        {
            // --- Arrange
            var matches = Z80Assembler.MacroParamRegex.Matches(source);
        }


    }
}
