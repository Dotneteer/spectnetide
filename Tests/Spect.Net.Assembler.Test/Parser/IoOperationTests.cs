using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Operations;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class IoOperationTests : ParserTestBed
    {
        [TestMethod]
        public void OutC0WorksAsExpected()
        {
            // --- Act
            var visitor = Parse("out (c), 0");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("OUT");
            line.Operand.Type.ShouldBe(OperandType.CPort);
            line.Operand2.Type.ShouldBe(OperandType.Expr);
        }

        [TestMethod]
        public void InCWorksAsExpected()
        {
            // --- Act
            var visitor = Parse("in (c)");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("IN");
            line.Operand.Type.ShouldBe(OperandType.CPort);
        }

        [TestMethod]
        public void IoOperationsWorkAsExpected()
        {
            InOperationWorks("in a,(#fe)", "A", true);
            InOperationWorks("in a,(c)", "A");
            InOperationWorks("in b,(c)", "B");
            InOperationWorks("in c,(c)", "C");
            InOperationWorks("in d,(c)", "D");
            InOperationWorks("in e,(c)", "E");
            InOperationWorks("in h,(c)", "H");
            InOperationWorks("in l,(c)", "L");

            OutOperationWorks("out (#fe),a", "A", true);
            OutOperationWorks("out (c),a", "A");
            OutOperationWorks("out (c),b", "B");
            OutOperationWorks("out (c),c", "C");
            OutOperationWorks("out (c),d", "D");
            OutOperationWorks("out (c),e", "E");
            OutOperationWorks("out (c),h", "H");
            OutOperationWorks("out (c),l", "L");
        }

        public void InOperationWorks(string instruction, string register, bool port = false)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("IN");
            line.Operand.Register.ShouldBe(register);
            if (port)
            {
                line.Operand2.Expression.ShouldNotBeNull();
            }
            else
            {
                line.Operand2.Expression.ShouldBeNull();
            }
        }

        public void OutOperationWorks(string instruction, string register, bool port = false)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("OUT");
            line.Operand2.Register.ShouldBe(register);
            if (port)
            {
                line.Operand.Expression.ShouldNotBeNull();
            }
            else
            {
                line.Operand.Expression.ShouldBeNull();
            }
        }
    }
}