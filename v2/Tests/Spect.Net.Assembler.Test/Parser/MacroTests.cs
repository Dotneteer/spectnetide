using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Operations;
using Spect.Net.Assembler.SyntaxTree.Statements;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class MacroTests : ParserTestBed
    {
        [TestMethod]
        [DataRow(".endm")]
        [DataRow("endm")]
        [DataRow(".ENDM")]
        [DataRow("ENDM")]
        [DataRow(".mend")]
        [DataRow("mend")]
        [DataRow(".MEND")]
        [DataRow("MEND")]
        public void EndMacroParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<MacroEndStatement>();
        }

        [TestMethod]
        [DataRow(".macro()")]
        [DataRow("macro()")]
        [DataRow(".MACRO()")]
        [DataRow("MACRO()")]
        public void MacroWithNoParameterWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as MacroStatement;
            line.ShouldNotBeNull();
        }

        [TestMethod]
        [DataRow(".macro(arg1)", "arg1")]
        [DataRow("macro(arg1)", "arg1")]
        [DataRow(".MACRO(arg1)", "arg1")]
        [DataRow(".MACRO(arg1)", "arg1")]
        public void MacroWithArgumentsWorks(string source, string arg1)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as MacroStatement;
            line.ShouldNotBeNull();
            line.Arguments.Count.ShouldBe(1);
            line.Arguments[0].ShouldBe(arg1);
        }

        [TestMethod]
        [DataRow(".macro(arg1, secondArg)", "arg1", "secondArg")]
        [DataRow("macro(arg1, secondArg)", "arg1", "secondArg")]
        [DataRow(".MACRO(arg1, secondArg)", "arg1", "secondArg")]
        [DataRow("MACRO(arg1, secondArg)", "arg1", "secondArg")]
        public void MacroWithTwoArgumentsWorks(string source, string arg1, string arg2)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as MacroStatement;
            line.ShouldNotBeNull();
            line.Arguments.Count.ShouldBe(2);
            line.Arguments[0].ShouldBe(arg1);
            line.Arguments[1].ShouldBe(arg2);
        }

        [TestMethod]
        [DataRow(".macro(arg1, secondArg, third)", "arg1", "secondArg", "third")]
        [DataRow("macro(arg1, secondArg, third)", "arg1", "secondArg", "third")]
        [DataRow(".MACRO(arg1, secondArg, third)", "arg1", "secondArg", "third")]
        [DataRow("MACRO(arg1, secondArg, third)", "arg1", "secondArg", "third")]
        public void MacroWithThreeArgumentsWorks(string source, string arg1, string arg2, string arg3)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as MacroStatement;
            line.ShouldNotBeNull();
            line.Arguments.Count.ShouldBe(3);
            line.Arguments[0].ShouldBe(arg1);
            line.Arguments[1].ShouldBe(arg2);
            line.Arguments[2].ShouldBe(arg3);
        }

        [TestMethod]
        public void MacroParamOperationWorks()
        {
            // --- Act
            var visitor = Parse("{{ MyMacroParam }}");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as MacroParamLine;
            line.ShouldNotBeNull();
            line.Identifier.ShouldBe("MyMacroParam");
            line.MacroParamNames.Count.ShouldBe(1);
            line.MacroParamNames[0].ShouldBe("MyMacroParam");
        }

        [TestMethod]
        public void MacroInvocationWorks()
        {
            // --- Act
            var visitor = Parse("MyMacro()");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as MacroOrStructInvocation;
            line.Name.ShouldBe("MyMacro");
        }

        [TestMethod]
        public void MacroParamOperationWithLabelWorks()
        {
            // --- Act
            var visitor = Parse("MyLabel: {{ MyMacroParam }}");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var origLine = visitor.Compilation.Lines[0];
            var line = visitor.Compilation.Lines[0] as MacroParamLine;
            line.ShouldNotBeNull();
            line.Identifier.ShouldBe("MyMacroParam");
            origLine.Label.ShouldBe("MyLabel");
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
            line.Identifier.ShouldBe("MyMacroParam");
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
            line.Identifier.ShouldBe("MyMacroParam");
            origLine.Label.ShouldBe("MyLabel");
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
        [DataRow("bit 2,{{MyMacroParam}}")]
        [DataRow("set 2,{{MyMacroParam}}")]
        [DataRow("res 2,{{MyMacroParam}}")]
        public void MacroParamInFirstOperandWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Operand.Type.ShouldBe(OperandType.Expr);
            line.MacroParamNames.Count.ShouldBe(1);
            line.MacroParamNames[0].ShouldBe("MyMacroParam");
        }

        [TestMethod]
        [DataRow("jr nz,{{MyMacroParam}}")]
        [DataRow("jp nz,{{MyMacroParam}}")]
        [DataRow("call nz,{{MyMacroParam}}")]
        public void MacroParamInJumpOperandWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Operand.Type.ShouldBe(OperandType.Condition);
            line.MacroParamNames.Count.ShouldBe(1);
            line.MacroParamNames[0].ShouldBe("MyMacroParam");
        }

        [TestMethod]
        [DataRow("ld {{MyMacroParam}},{{par}}")]
        [DataRow("ex {{MyMacroParam}},{{par}}")]
        [DataRow("add {{MyMacroParam}},{{par}}")]
        [DataRow("adc {{MyMacroParam}},{{par}}")]
        [DataRow("sub {{MyMacroParam}},{{par}}")]
        [DataRow("sbc {{MyMacroParam}},{{par}}")]
        [DataRow("and {{MyMacroParam}},{{par}}")]
        [DataRow("xor {{MyMacroParam}},{{par}}")]
        [DataRow("or {{MyMacroParam}},{{par}}")]
        [DataRow("cp {{MyMacroParam}},{{par}}")]
        [DataRow("in {{MyMacroParam}},{{par}}")]
        [DataRow("out {{MyMacroParam}},{{par}}")]
        [DataRow("rlc {{MyMacroParam}},{{par}}")]
        [DataRow("rl {{MyMacroParam}},{{par}}")]
        [DataRow("rrc {{MyMacroParam}},{{par}}")]
        [DataRow("rr {{MyMacroParam}},{{par}}")]
        [DataRow("sla {{MyMacroParam}},{{par}}")]
        [DataRow("sra {{MyMacroParam}},{{par}}")]
        [DataRow("sll {{MyMacroParam}},{{par}}")]
        [DataRow("srl {{MyMacroParam}},{{par}}")]
        [DataRow("nextreg {{MyMacroParam}},{{par}}")]
        public void MacroParamInBothOperandsWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Operand.Type.ShouldBe(OperandType.Expr);
            line.MacroParamNames.Count.ShouldBe(2);
            line.MacroParamNames[0].ShouldBe("MyMacroParam");
            line.MacroParamNames[1].ShouldBe("par");
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
            line.MacroParamNames.Count.ShouldBe(1);
            line.MacroParamNames[0].ShouldBe("MyMacroParam");
        }

        [TestMethod]
        [DataRow("bit {{MyMacroParam}},b")]
        [DataRow("set {{MyMacroParam}},b")]
        [DataRow("res {{MyMacroParam}},b")]
        public void MacroParamInBitOperationsWork(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Operand.Expression.HasMacroParameter.ShouldBeTrue();
            line.MacroParamNames.Count.ShouldBe(1);
            line.MacroParamNames[0].ShouldBe("MyMacroParam");
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
            line.MacroParamNames.Count.ShouldBe(1);
            line.MacroParamNames[0].ShouldBe("par");
        }
    }
}
