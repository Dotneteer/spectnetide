using AntlrZ80Asm.SyntaxTree.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
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
            var line = visitor.Compilation.Lines[0] as IoOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("OUT");
            line.Port.ShouldBeNull();
            line.Register.ShouldBeNull();
        }

        [TestMethod]
        public void InCWorksAsExpected()
        {
            // --- Act
            var visitor = Parse("in (c)");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as IoOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("IN");
            line.Port.ShouldBeNull();
            line.Register.ShouldBeNull();
        }

        [TestMethod]
        public void IoOperationsWorkAsExpected()
        {
            IoOperationWorks("in a,(#fe)", "IN", null, true);
            IoOperationWorks("in a,(c)", "IN", "A", false);
            IoOperationWorks("in b,(c)", "IN", "B", false);
            IoOperationWorks("in c,(c)", "IN", "C", false);
            IoOperationWorks("in d,(c)", "IN", "D", false);
            IoOperationWorks("in e,(c)", "IN", "E", false);
            IoOperationWorks("in h,(c)", "IN", "H", false);
            IoOperationWorks("in l,(c)", "IN", "L", false);

            IoOperationWorks("out (c),a", "OUT", "A", false);
            IoOperationWorks("out (c),b", "OUT", "B", false);
            IoOperationWorks("out (c),c", "OUT", "C", false);
            IoOperationWorks("out (c),d", "OUT", "D", false);
            IoOperationWorks("out (c),e", "OUT", "E", false);
            IoOperationWorks("out (c),h", "OUT", "H", false);
            IoOperationWorks("out (c),l", "OUT", "L", false);
        }

        public void IoOperationWorks(string instruction, string type, string register, bool port)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as IoOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Register.ShouldBe(register);
            if (port)
            {
                line.Port.ShouldNotBeNull();
            }
            else
            {
                line.Port.ShouldBeNull();
            }
        }
    }
}