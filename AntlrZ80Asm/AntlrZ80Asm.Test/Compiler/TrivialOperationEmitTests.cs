using AntlrZ80Asm.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Compiler
{
    [TestClass]
    public class TrivialOperationEmitTests
    {
        [TestMethod]
        public void StandardTrivialOpsWorkAsExpected()
        {
            TrivialOpWorks("nop", 0x00);
            TrivialOpWorks("rlca", 0x07);
            TrivialOpWorks("rrca", 0x0F);
            TrivialOpWorks("rla", 0x17);
            TrivialOpWorks("rra", 0x1F);
            TrivialOpWorks("daa", 0x27);
            TrivialOpWorks("scf", 0x37);
            TrivialOpWorks("ccf", 0x3F);
            TrivialOpWorks("ret", 0xC9);
            TrivialOpWorks("exx", 0xD9);
            TrivialOpWorks("di", 0xF3);
            TrivialOpWorks("ei", 0xFB);
            TrivialOpWorks("neg", 0xED, 0x44);
        }

        [TestMethod]
        public void ExtendedTrivialOpsWorkAsExpected()
        {
            TrivialOpWorks("neg", 0xED, 0x44);
            TrivialOpWorks("retn", 0xED, 0x45);
            TrivialOpWorks("reti", 0xED, 0x4D);
            TrivialOpWorks("rrd", 0xED, 0x67);
            TrivialOpWorks("rld", 0xED, 0x6F);
            TrivialOpWorks("ldi", 0xED, 0xA0);
            TrivialOpWorks("cpi", 0xED, 0xA1);
            TrivialOpWorks("ini", 0xED, 0xA2);
            TrivialOpWorks("outi", 0xED, 0xA3);
            TrivialOpWorks("ldd", 0xED, 0xA8);
            TrivialOpWorks("cpd", 0xED, 0xA9);
            TrivialOpWorks("ind", 0xED, 0xAA);
            TrivialOpWorks("outd", 0xED, 0xAB);
            TrivialOpWorks("ldir", 0xED, 0xB0);
            TrivialOpWorks("cpir", 0xED, 0xB1);
            TrivialOpWorks("inir", 0xED, 0xB2);
            TrivialOpWorks("otir", 0xED, 0xB3);
            TrivialOpWorks("lddr", 0xED, 0xB8);
            TrivialOpWorks("cpdr", 0xED, 0xB9);
            TrivialOpWorks("indr", 0xED, 0xBA);
            TrivialOpWorks("otdr", 0xED, 0xBB);
        }

        private void TrivialOpWorks(string source, params byte[] opCodes)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(source);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var bytes = output.Segments[0].EmittedCode;
            bytes.Count.ShouldBe(opCodes.Length);
            for (var i = 0; i < opCodes.Length; i++)
            {
                bytes[i].ShouldBe(opCodes[i]);
            }
        }
    }
}
