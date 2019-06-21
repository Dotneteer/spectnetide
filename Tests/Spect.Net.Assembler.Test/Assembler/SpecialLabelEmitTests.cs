using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
// ReSharper disable StringLiteralTypo

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class SpecialLabelEmitTests: AssemblerTestBed
    {
        [TestMethod]
        [DataRow("continue")]
        [DataRow("CONTINUE")]
        [DataRow("break")]
        [DataRow("BREAK")]
        [DataRow("loop")]
        [DataRow("LOOP")]
        [DataRow("endm")]
        [DataRow("ENDM")]
        [DataRow("mend")]
        [DataRow("MEND")]
        [DataRow("endl")]
        [DataRow("ENDL")]
        [DataRow("lend")]
        [DataRow("LEND")]
        [DataRow("proc")]
        [DataRow("PROC")]
        [DataRow("endp")]
        [DataRow("ENDP")]
        [DataRow("pend")]
        [DataRow("PEND")]
        [DataRow("repeat")]
        [DataRow("REPEAT")]
        [DataRow("while")]
        [DataRow("WHILE")]
        [DataRow("endw")]
        [DataRow("ENDW")]
        [DataRow("wend")]
        [DataRow("WEND")]
        [DataRow("ends")]
        [DataRow("ENDS")]
        [DataRow("until")]
        [DataRow("UNTIL")]
        [DataRow("elif")]
        [DataRow("ELIF")]
        public void SpecialLabelWorks(string source)
        {
            CodeEmitWorks($@"
                ld bc,{source}
                {source}: nop
            ", 
                0x01, 0x03, 0x80, 0x00);
        }

        [TestMethod]
        public void ContinueLabelWorksWithoutColon()
        {
            CodeEmitWorks(@"
                ld bc,continue
                continue nop
            ",
                0x01, 0x03, 0x80, 0x00);
        }

        [TestMethod]
        public void HangingContinueLabelWorks()
        {
            CodeEmitWorks(@"
                ld bc,continue
                continue:
                    nop
            ",
                0x01, 0x03, 0x80, 0x00);
        }

        [TestMethod]
        public void HangingContinueLabelFailsWithoutColon()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld bc,continue
                continue
                    nop
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
        }

    }
}
