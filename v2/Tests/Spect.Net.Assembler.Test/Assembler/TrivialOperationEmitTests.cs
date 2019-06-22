using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class TrivialOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void StandardTrivialOpsWorkAsExpected()
        {
            CodeEmitWorks("nop", 0x00);
            CodeEmitWorks("rlca", 0x07);
            CodeEmitWorks("rrca", 0x0F);
            CodeEmitWorks("rla", 0x17);
            CodeEmitWorks("rra", 0x1F);
            CodeEmitWorks("daa", 0x27);
            CodeEmitWorks("cpl", 0x2F);
            CodeEmitWorks("scf", 0x37);
            CodeEmitWorks("ccf", 0x3F);
            CodeEmitWorks("ret", 0xC9);
            CodeEmitWorks("halt", 0x76);
            CodeEmitWorks("exx", 0xD9);
            CodeEmitWorks("di", 0xF3);
            CodeEmitWorks("ei", 0xFB);
        }

        [TestMethod]
        public void ExtendedTrivialOpsWorkAsExpected()
        {
            CodeEmitWorks("neg", 0xED, 0x44);
            CodeEmitWorks("retn", 0xED, 0x45);
            CodeEmitWorks("reti", 0xED, 0x4D);
            CodeEmitWorks("rrd", 0xED, 0x67);
            CodeEmitWorks("rld", 0xED, 0x6F);
            CodeEmitWorks("ldi", 0xED, 0xA0);
            CodeEmitWorks("cpi", 0xED, 0xA1);
            CodeEmitWorks("ini", 0xED, 0xA2);
            CodeEmitWorks("outi", 0xED, 0xA3);
            CodeEmitWorks("ldd", 0xED, 0xA8);
            CodeEmitWorks("cpd", 0xED, 0xA9);
            CodeEmitWorks("ind", 0xED, 0xAA);
            CodeEmitWorks("outd", 0xED, 0xAB);
            CodeEmitWorks("ldir", 0xED, 0xB0);
            CodeEmitWorks("cpir", 0xED, 0xB1);
            CodeEmitWorks("inir", 0xED, 0xB2);
            CodeEmitWorks("otir", 0xED, 0xB3);
            CodeEmitWorks("lddr", 0xED, 0xB8);
            CodeEmitWorks("cpdr", 0xED, 0xB9);
            CodeEmitWorks("indr", 0xED, 0xBA);
            CodeEmitWorks("otdr", 0xED, 0xBB);
        }

        [TestMethod]
        public void TrivialNextOpsWorkAsExpected()
        {
            CodeEmitWorks(".model next \r\n swapnib", 0xED, 0x23);
            CodeEmitWorks(".model next \r\n mul", 0xED, 0x30);
            CodeEmitWorks(".model next \r\n outinb", 0xED, 0x90);
            CodeEmitWorks(".model next \r\n ldix", 0xED, 0xA4);
            CodeEmitWorks(".model next \r\n ldirx", 0xED, 0xB4);
            CodeEmitWorks(".model next \r\n lddx", 0xED, 0xAC);
            CodeEmitWorks(".model next \r\n lddrx", 0xED, 0xBC);
            CodeEmitWorks(".model next \r\n pixeldn", 0xED, 0x93);
            CodeEmitWorks(".model next \r\n pixelad", 0xED, 0x94);
            CodeEmitWorks(".model next \r\n setae", 0xED, 0x95);
            CodeEmitWorks(".model next \r\n ldpirx", 0xED, 0xB7);
            CodeEmitWorks(".model next \r\n ldirscale", 0xED, 0xB6);
        }

        [TestMethod]
        public void TrivialNextOpsRaiseError()
        {
            CodeEmitWithoutNextRaisesIssue("swapnib");
            CodeEmitWithoutNextRaisesIssue("mul");
            CodeEmitWithoutNextRaisesIssue("outinb");
            CodeEmitWithoutNextRaisesIssue("ldix");
            CodeEmitWithoutNextRaisesIssue("ldirx");
            CodeEmitWithoutNextRaisesIssue("lddx");
            CodeEmitWithoutNextRaisesIssue("lddrx");
            CodeEmitWithoutNextRaisesIssue("pixeldn");
            CodeEmitWithoutNextRaisesIssue("pixelad");
            CodeEmitWithoutNextRaisesIssue("setae");
            CodeEmitWithoutNextRaisesIssue("ldpirx");
            CodeEmitWithoutNextRaisesIssue("ldirscale");
        }

        protected void CodeEmitWithoutNextRaisesIssue(string source)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(source);

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0102);
        }
    }
}
