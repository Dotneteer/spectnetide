using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Operations;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class MacroParamsTests : ParserTestBed
    {
        [TestMethod]
        public void MacroParamOperationWorks()
        {
            // --- Act
            var visitor = Parse("{{ MyMacroParam }}");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as MacroParamLine;
            line.ShouldNotBeNull();
            line.Identifier.ShouldBe("MYMACROPARAM");
        }

        [TestMethod]
        public void MacroParamOperationWithLabelWorks()
        {
            // --- Act
            var visitor = Parse("MyLabel {{ MyMacroParam }}");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var origLine = visitor.Compilation.Lines[0];
            var line = visitor.Compilation.Lines[0] as MacroParamLine;
            line.ShouldNotBeNull();
            line.Identifier.ShouldBe("MYMACROPARAM");
            origLine.Label.ShouldBe("MYLABEL");
        }

        [TestMethod]
        public void MacroParamOperationWithCommentWorks()
        {
            // --- Act
            var visitor = Parse("{{ MyMacroParam }} ; MyComment");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var origLine = visitor.Compilation.Lines[0];
            var line = visitor.Compilation.Lines[0] as MacroParamLine;
            line.ShouldNotBeNull();
            line.Identifier.ShouldBe("MYMACROPARAM");
            origLine.Comment.ShouldBe("; MyComment");
        }

        [TestMethod]
        public void MacroParamOperationWithLabelAndCommentWorks()
        {
            // --- Act
            var visitor = Parse("MyLabel: {{ MyMacroParam }} ; MyComment");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var origLine = visitor.Compilation.Lines[0];
            var line = visitor.Compilation.Lines[0] as MacroParamLine;
            line.ShouldNotBeNull();
            line.Identifier.ShouldBe("MYMACROPARAM");
            origLine.Label.ShouldBe("MYLABEL");
            origLine.Comment.ShouldBe("; MyComment");
        }

        [TestMethod]
        [DataRow("ld {{MyMacroParam}},3")]
        [DataRow("inc {{MyMacroParam}}")]
        [DataRow("dec {{MyMacroParam}}")]
        [DataRow("ex {{MyMacroParam}},af'")]
        [DataRow("add {{MyMacroParam}},3")]
        [DataRow("adc {{MyMacroParam}},3")]
        [DataRow("sub {{MyMacroParam}},3")]
        [DataRow("sub {{MyMacroParam}}")]
        [DataRow("sbc {{MyMacroParam}},3")]
        [DataRow("and {{MyMacroParam}},3")]
        [DataRow("and {{MyMacroParam}}")]
        [DataRow("xor {{MyMacroParam}},3")]
        [DataRow("xor {{MyMacroParam}}")]
        [DataRow("or {{MyMacroParam}},3")]
        [DataRow("or {{MyMacroParam}}")]
        [DataRow("cp {{MyMacroParam}},3")]
        [DataRow("cp {{MyMacroParam}}")]
        [DataRow("rst {{MyMacroParam}}")]
        [DataRow("push {{MyMacroParam}}")]
        [DataRow("pop {{MyMacroParam}}")]
        [DataRow("in {{MyMacroParam}},3")]
        [DataRow("in {{MyMacroParam}}")]
        [DataRow("out {{MyMacroParam}},3")]
        [DataRow("out {{MyMacroParam}}")]
        [DataRow("im {{MyMacroParam}}")]
        [DataRow("rlc {{MyMacroParam}},3")]
        [DataRow("rlc {{MyMacroParam}}")]
        [DataRow("rl {{MyMacroParam}},3")]
        [DataRow("rl {{MyMacroParam}}")]
        [DataRow("rrc {{MyMacroParam}},3")]
        [DataRow("rrc {{MyMacroParam}}")]
        [DataRow("rr {{MyMacroParam}},3")]
        [DataRow("rr {{MyMacroParam}}")]
        [DataRow("sla {{MyMacroParam}},3")]
        [DataRow("sla {{MyMacroParam}}")]
        [DataRow("sra {{MyMacroParam}},3")]
        [DataRow("sra {{MyMacroParam}}")]
        [DataRow("sll {{MyMacroParam}},3")]
        [DataRow("sll {{MyMacroParam}}")]
        [DataRow("srl {{MyMacroParam}},3")]
        [DataRow("srl {{MyMacroParam}}")]
        [DataRow("mirror {{MyMacroParam}}")]
        [DataRow("test {{MyMacroParam}}")]
        [DataRow("nextreg {{MyMacroParam}},3")]
        [DataRow("jr {{MyMacroParam}}")]
        [DataRow("jp {{MyMacroParam}}")]
        [DataRow("call {{MyMacroParam}}")]
        [DataRow("jr nz,{{MyMacroParam}}")]
        [DataRow("jp nz,{{MyMacroParam}}")]
        [DataRow("call nz,{{MyMacroParam}}")]
        public void MacroParamInFirstOperandWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Operand.Type.ShouldBe(OperandType.Expr);
        }

        [TestMethod]
        [DataRow("ld 3,{{MyMacroParam}}")]
        [DataRow("ex af,{{MyMacroParam}}")]
        [DataRow("add a,{{MyMacroParam}}")]
        [DataRow("adc a,{{MyMacroParam}}")]
        [DataRow("sub a,{{MyMacroParam}}")]
        [DataRow("sbc a,{{MyMacroParam}}")]
        [DataRow("and a,{{MyMacroParam}}")]
        [DataRow("xor a,{{MyMacroParam}}")]
        [DataRow("or a,{{MyMacroParam}}")]
        [DataRow("cp a,{{MyMacroParam}}")]
        [DataRow("in a,{{MyMacroParam}}")]
        [DataRow("out (#fe),{{MyMacroParam}}")]
        [DataRow("rlc b,{{MyMacroParam}}")]
        [DataRow("rl b,{{MyMacroParam}}")]
        [DataRow("rrc b,{{MyMacroParam}}")]
        [DataRow("rr b,{{MyMacroParam}}")]
        [DataRow("sla b,{{MyMacroParam}}")]
        [DataRow("sra b,{{MyMacroParam}}")]
        [DataRow("sll b,{{MyMacroParam}}")]
        [DataRow("srl b,{{MyMacroParam}}")]
        [DataRow("nextreg 3,{{MyMacroParam}}")]
        public void MacroParamInSecondOperandWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Operand2.Type.ShouldBe(OperandType.Expr);
        }

        [TestMethod]
        [DataRow("{{par}}")]
        [DataRow("{{par}}+3")]
        [DataRow("3+{{par}}")]
        [DataRow(".true ? {{par}} : 4")]
        [DataRow("3 + [4 * {{par}}]")]
        public void ExpressionHasMacroParameter(string source)
        {
            // --- Act
            var visitor = Parse("ld a," + source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Operand2.Type.ShouldBe(OperandType.Expr);
            line.Operand2.Expression.HasMacroParameter.ShouldBeTrue();
        }
    }
}
