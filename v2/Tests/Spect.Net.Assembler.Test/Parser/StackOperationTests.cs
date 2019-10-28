﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Operations;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class StackOperationTests : ParserTestBed
    {
        [TestMethod]
        public void StackOperationWorkAsExpected()
        {
            StackOperationWorks("push bc", "PUSH", "BC");
            StackOperationWorks("push de", "PUSH", "DE");
            StackOperationWorks("push hl", "PUSH", "HL");
            StackOperationWorks("push af", "PUSH", "AF");
            StackOperationWorks("push ix", "PUSH", "IX");
            StackOperationWorks("push iy", "PUSH", "IY");

            StackOperationWorks("pop bc", "POP", "BC");
            StackOperationWorks("pop de", "POP", "DE");
            StackOperationWorks("pop hl", "POP", "HL");
            StackOperationWorks("pop af", "POP", "AF");
            StackOperationWorks("pop ix", "POP", "IX");
            StackOperationWorks("pop iy", "POP", "IY");
        }

        protected void StackOperationWorks(string instruction, string type, string reg)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand.Register.ShouldBe(reg);
        }
    }
}
