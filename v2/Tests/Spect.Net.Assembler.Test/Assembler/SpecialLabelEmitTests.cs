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
        [DataRow("else")]
        [DataRow("ELSE")]
        [DataRow("endif")]
        [DataRow("ENDIF")]
        public void SpecialLabelsWork(string source)
        {
            CodeEmitWorks($@"
                ld bc,{source}
                {source}: nop
            ", 
                0x01, 0x03, 0x80, 0x00);
        }

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
        [DataRow("else")]
        [DataRow("ELSE")]
        [DataRow("endif")]
        [DataRow("ENDIF")]
        public void SpecialLabelsWorkWithoutColon(string source)
        {
            CodeEmitWorks($@"
                ld bc,{source}
                {source} nop
            ",
                0x01, 0x03, 0x80, 0x00);
        }

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
        [DataRow("else")]
        [DataRow("ELSE")]
        [DataRow("endif")]
        [DataRow("ENDIF")]
        public void SpecialLabelsWorkAsHangingLabels(string source)
        {
            CodeEmitWorks($@"
                ld bc,{source}
                {source}:
                    nop
            ",
                0x01, 0x03, 0x80, 0x00);
        }
        [TestMethod]
        [DataRow("continue")]
        [DataRow("CONTINUE")]
        [DataRow("break")]
        [DataRow("BREAK")]
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
        [DataRow("endw")]
        [DataRow("ENDW")]
        [DataRow("wend")]
        [DataRow("WEND")]
        [DataRow("ends")]
        [DataRow("ENDS")]
        [DataRow("else")]
        [DataRow("ELSE")]
        [DataRow("endif")]
        [DataRow("ENDIF")]
        public void SpecialLabelsWorkAsHangingLabelsWithoutColon(string source)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile($@"
                ld bc,{source}
                {source}
                    nop
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
        }

    }
}
