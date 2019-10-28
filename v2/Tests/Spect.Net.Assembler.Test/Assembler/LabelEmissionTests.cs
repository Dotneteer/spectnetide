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
        public void LabelOnlyCodeEmissionWorksWithDottedName()
        {
            CodeEmitWorks(@"
                .org #6000
                Label.Only:
                    ld a,b
                    ld bc,Label.Only",
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

        [TestMethod]
        public void OrgWithHangingLabelWorks()
        {
            CodeEmitWorks(@"
                LabelOnly:
                    .org #6000
                    ld a,b
                    ld bc,LabelOnly",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void EquWithHangingLabelWorks()
        {
            CodeEmitWorks(@"
                LabelOnly:
                    .org #6000
                LabelOnly2:
                    .equ #4567
                    ld a,b
                    ld bc,LabelOnly2",
                0x78, 0x01, 0x67, 0x45);
        }

        [TestMethod]
        public void VarWithHangingLabelWorks()
        {
            CodeEmitWorks(@"
                LabelOnly:
                    .org #6000
                LabelOnly2:
                    .var #4567
                    ld a,b
                    ld bc,LabelOnly2",
                0x78, 0x01, 0x67, 0x45);
        }

        [TestMethod]
        public void OrphanHangingLabelWorks()
        {
            CodeEmitWorks(@"
                    .org #6000
                    ld a,b
                    ld bc,LabelOnly
                LabelOnly:",
                0x78, 0x01, 0x04, 0x60);
        }

        [TestMethod]
        public void SingleTempLabelWithBackReferenceWorks()
        {
            CodeEmitWorks(@"
                Start:
                    .org #6000
                    ld a,b
                `t1:
                    ld bc,`t1",
                0x78, 0x01, 0x01, 0x60);
        }

        [TestMethod]
        public void SingleTempLabelWithForwardReferenceWorks()
        {
            CodeEmitWorks(@"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,`t1
                `t1:
                    ld a,b",
                0x78, 0x01, 0x04, 0x60, 0x78);
        }

        [TestMethod]
        public void StartAndEndLabelWithForwardReferenceWorks()
        {
            CodeEmitWorks(@"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,End
                End:
                    ld a,b",
                0x78, 0x01, 0x04, 0x60, 0x78);
        }

        [TestMethod]
        public void TempLabelInDifferentScopesWork()
        {
            CodeEmitWorks(@"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,`t1
                `t1:
                    ld a,b
                Next: 
                    ld a,b
                    ld bc,`t1
                `t1:
                    ld a,b",
                0x78, 0x01, 0x04, 0x60, 0x78, 0x78, 0x01, 0x09, 0x60, 0x78);
        }

        [TestMethod]
        public void TempLabelWithLoopWork()
        {
            CodeEmitWorks(@"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,`t1
                    .loop 3
                    nop
                    .endl
                `t1:
                    ld a,b
                ",
                0x78, 0x01, 0x07, 0x60, 0x00, 0x00, 0x00, 0x78);
        }
    }
}
