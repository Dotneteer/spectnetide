using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class LabelEmissionTests : AssemblerTestBed
    {
        [TestMethod]
        public void LabelOnlyCodeEmissionWorks()
        {
            CodeEmitWorks(@"
                .org #6000
                LabelOnly:
                    ld a,b
                    ld bc,LabelOnly", 
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void LabelOnlyWithCommentCodeEmissionWorks()
        {
            CodeEmitWorks(@"
                .org #6000
                LabelOnly: ; Empty label
                    ld a,b
                    ld bc,LabelOnly",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void MultiLabelOnlyCodeEmissionWorks()
        {
            CodeEmitWorks(@"
                .org #6000
                LabelOnly1:
                LabelOnly2:
                LabelOnly3:
                LabelOnly4:
                LabelOnly5:
                    ld a,b
                    ld bc,LabelOnly3",
                0x78, 0x01, 0x00, 0x60);
        }

    }
}
