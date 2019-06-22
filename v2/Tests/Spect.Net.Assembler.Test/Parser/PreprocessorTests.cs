using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class PreprocessorTests : ParserTestBed
    {
        [TestMethod]
        public void PreprocessorDirectivesWorkAsExpected()
        {
            PreprocessingWorks("#ifdef myId", "#IFDEF", "MYID");
            PreprocessingWorks("#ifndef myId", "#IFNDEF", "MYID");
            PreprocessingWorks("#ifmod Spectrum48", "#IFMOD", "SPECTRUM48");
            PreprocessingWorks("#ifnmod Spectrum128", "#IFNMOD", "SPECTRUM128");
            PreprocessingWorks("#define myId", "#DEFINE", "MYID");
            PreprocessingWorks("#undef myId", "#UNDEF", "MYID");
            PreprocessingWorks("#else", "#ELSE", null);
            PreprocessingWorks("#endif", "#ENDIF", null);
        }

        [TestMethod]
        public void IncludeDirectiveWorksWithStringAsExpected()
        {
            // --- Act
            var visitor = Parse("#include \"myfile.z80asm\"");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as IncludeDirective;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("#INCLUDE");
            line.Filename.ShouldBe("myfile.z80asm");
        }

        [TestMethod]
        public void IncludeDirectiveWorksWithFStringAsExpected()
        {
            // --- Act
            var visitor = Parse("#include <myfile.z80asm>");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as IncludeDirective;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("#INCLUDE");
            line.Filename.ShouldBe("<myfile.z80asm>");
        }

        [TestMethod]
        public void IncludeFailsWithLabel()
        {
            // --- Act/Assert
            Parse("mylabel #include \"myfile.z80asm\"", 1);
            Parse("mylabel: #include \"myfile.z80asm>\"", 1);
            Parse("mylabel #include <myfile.z80asm>", 1);
            Parse("mylabel: #include <myfile.z80asm>", 1);
        }

        protected void PreprocessingWorks(string instruction, string mnemonic, string identifier)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as Directive;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(mnemonic);
            line.Identifier.ShouldBe(identifier);
        }
    }
}
