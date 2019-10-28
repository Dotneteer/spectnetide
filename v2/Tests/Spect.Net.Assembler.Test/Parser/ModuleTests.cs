using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Statements;
// ReSharper disable StringLiteralTypo

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class ModuleTests: ParserTestBed
    {
        [TestMethod]
        [DataRow(".endmodule")]
        [DataRow("endmodule")]
        [DataRow(".ENDMODULE")]
        [DataRow("ENDMODULE")]
        [DataRow(".moduleend")]
        [DataRow("moduleend")]
        [DataRow(".MODULEEND")]
        [DataRow("MODULEEND")]
        [DataRow(".endscope")]
        [DataRow("endscope")]
        [DataRow(".ENDSCOPE")]
        [DataRow("ENDSCOPE")]
        [DataRow(".scopeend")]
        [DataRow("scopeend")]
        [DataRow(".SCOPEEND")]
        [DataRow("SCOPEEND")]
        public void EndModuleParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<ModuleEndStatement>();
        }

        [TestMethod]
        [DataRow(".module")]
        [DataRow("module")]
        [DataRow(".MODULE")]
        [DataRow("MODULE")]
        [DataRow(".scope")]
        [DataRow("SCOPE")]
        [DataRow(".SCOPE")]
        [DataRow("SCOPE")]
        public void ModuleParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ModuleStatement;
            line.ShouldNotBeNull();
        }

        [TestMethod]
        [DataRow(".module myMod")]
        [DataRow("module myMod")]
        [DataRow(".MODULE mymod ")]
        [DataRow("MODULE MYMOD")]
        [DataRow(".scope MYmod")]
        [DataRow("SCOPE mymod")]
        [DataRow(".SCOPE mymod")]
        [DataRow("SCOPE mymod")]
        public void ModuleParsingWithNameWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ModuleStatement;
            line.ShouldNotBeNull();
            line.Name.ToUpper().ShouldBe("MYMOD");
        }
    }
}
