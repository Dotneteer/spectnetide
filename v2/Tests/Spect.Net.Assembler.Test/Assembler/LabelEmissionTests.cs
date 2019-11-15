using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Assembler.Assembler;

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
        public void LabelOnlyCodeEmissionWorksCs1()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = false },
                @"
                .org #6000
                labelonly:
                    ld a,b
                    ld bc,LabelOnly",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void LabelOnlyCodeEmissionWorksCs2()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = true },
                @"
                .org #6000
                LabelOnly:
                    ld a,b
                    ld bc,LabelOnly",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void LabelOnlyCodeEmissionWorksCs3()
        {
            CodeRaisesError(
                @"
                .org #6000
                labelOnly:
                    ld a,b
                    ld bc,LabelOnly",
                Errors.Z0201, new AssemblerOptions { UseCaseSensitiveSymbols = true });
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
        public void LabelOnlyCodeEmissionWorksWithDottedNameCs1()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = false }, 
                @"
                .org #6000
                label.only:
                    ld a,b
                    ld bc,Label.Only",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void LabelOnlyCodeEmissionWorksWithDottedNameCs2()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = true },
                @"
                .org #6000
                Label.Only:
                    ld a,b
                    ld bc,Label.Only",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void LabelOnlyCodeEmissionWorksWithDottedNameCs3()
        {
            CodeRaisesError(
                @"
                .org #6000
                label.only:
                    ld a,b
                    ld bc,Label.Only",
                Errors.Z0201,
                new AssemblerOptions { UseCaseSensitiveSymbols = true });
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
        public void LabelOnlyWithCommentCodeEmissionWorksCs1()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = false }, 
                @"
                .org #6000
                labelonly: ; Empty label
                    ld a,b
                    ld bc,LabelOnly",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void LabelOnlyWithCommentCodeEmissionWorksCs2()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = true },
                @"
                .org #6000
                LabelOnly: ; Empty label
                    ld a,b
                    ld bc,LabelOnly",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void LabelOnlyWithCommentCodeEmissionWorksCs3()
        {
            CodeRaisesError(
                @"
                .org #6000
                labelOnly: ; Empty label
                    ld a,b
                    ld bc,LabelOnly",
                Errors.Z0201,
                new AssemblerOptions { UseCaseSensitiveSymbols = true });
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
        public void SingleTempLabelWithBackReferenceWorksCs1()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = false }, 
                @"
                Start:
                    .org #6000
                    ld a,b
                `T1:
                    ld bc,`t1",
                0x78, 0x01, 0x01, 0x60);
        }

        [TestMethod]
        public void SingleTempLabelWithBackReferenceWorksCs2()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = true },
                @"
                Start:
                    .org #6000
                    ld a,b
                `t1:
                    ld bc,`t1",
                0x78, 0x01, 0x01, 0x60);
        }

        [TestMethod]
        public void SingleTempLabelWithBackReferenceWorksCs3()
        {
            CodeRaisesError(
                @"
                Start:
                    .org #6000
                    ld a,b
                `T1:
                    ld bc,`t1",
                Errors.Z0201,
                new AssemblerOptions { UseCaseSensitiveSymbols = true });
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
        public void SingleTempLabelWithForwardReferenceWorksCs1()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = false},
                @"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,`t1
                `T1:
                    ld a,b",
                0x78, 0x01, 0x04, 0x60, 0x78);
        }

        [TestMethod]
        public void SingleTempLabelWithForwardReferenceWorksCs2()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = true },
                @"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,`t1
                `t1:
                    ld a,b",
                0x78, 0x01, 0x04, 0x60, 0x78);
        }

        [TestMethod]
        public void SingleTempLabelWithForwardReferenceWorksCs3()
        {
            CodeRaisesError(
                @"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,`t1
                `T1:
                    ld a,b",
                Errors.Z0201,
                new AssemblerOptions { UseCaseSensitiveSymbols = true });
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
        public void StartAndEndLabelWithForwardReferenceWorksCs1()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = false }, 
                @"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,END
                End:
                    ld a,b",
                0x78, 0x01, 0x04, 0x60, 0x78);
        }

        [TestMethod]
        public void StartAndEndLabelWithForwardReferenceWorksCs2()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = true },
                @"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,End
                End:
                    ld a,b",
                0x78, 0x01, 0x04, 0x60, 0x78);
        }

        [TestMethod]
        public void StartAndEndLabelWithForwardReferenceWorksCs3()
        {
            CodeRaisesError(
                @"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,END
                End:
                    ld a,b",
                Errors.Z0201,
                new AssemblerOptions { UseCaseSensitiveSymbols = true });
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
        public void TempLabelInDifferentScopesWorkCs1()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = false},
                @"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,`t1
                `T1:
                    ld a,b
                Next: 
                    ld a,b
                    ld bc,`T1
                `t1:
                    ld a,b",
                0x78, 0x01, 0x04, 0x60, 0x78, 0x78, 0x01, 0x09, 0x60, 0x78);
        }

        [TestMethod]
        public void TempLabelInDifferentScopesWorkCs2()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = true },
                @"
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
        public void TempLabelInDifferentScopesWorkCs3()
        {
            CodeRaisesMultipleErrors(
                @"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,`t1
                `T1:
                    ld a,b
                Next: 
                    ld a,b
                    ld bc,`T1
                `t1:
                    ld a,b",
                new AssemblerOptions { UseCaseSensitiveSymbols = true },
                Errors.Z0201);
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

        [TestMethod]
        public void TempLabelWithLoopWorkCs1()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = false},
                @"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,`t1
                    .loop 3
                    nop
                    .endl
                `T1:
                    ld a,b
                ",
                0x78, 0x01, 0x07, 0x60, 0x00, 0x00, 0x00, 0x78);
        }

        [TestMethod]
        public void TempLabelWithLoopWorkCs2()
        {
            CodeEmitWorksWithOptions(
                new AssemblerOptions { UseCaseSensitiveSymbols = true },
                @"
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

        [TestMethod]
        public void TempLabelWithLoopWorkCs3()
        {
            CodeRaisesError(
                @"
                Start:
                    .org #6000
                    ld a,b
                    ld bc,`t1
                    .loop 3
                    nop
                    .endl
                `T1:
                    ld a,b
                ",
                Errors.Z0201,
                new AssemblerOptions { UseCaseSensitiveSymbols = true });
        }
    }
}
