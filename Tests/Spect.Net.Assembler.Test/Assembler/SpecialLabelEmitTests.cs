using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class SpecialLabelEmitTests: AssemblerTestBed
    {
        [TestMethod]
        public void ContinueLabelWorks()
        {
            CodeEmitWorks(@"
                ld bc,continue
                continue nop
            ", 
                0x01, 0x03, 0x80, 0x00);
        }

        [TestMethod]
        public void LoopLabelWorks()
        {
            CodeEmitWorks(@"
                ld bc,loop
                loop nop
            ",
                0x01, 0x03, 0x80, 0x00);
        }
    }
}
