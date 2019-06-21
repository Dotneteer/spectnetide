using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
// ReSharper disable StringLiteralTypo

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class LocalStatementEmitTests: AssemblerTestBed
    {
        [TestMethod]
        public void LocalStatementWithTemporarySymbolRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .proc
                    .local `myLocal 
                .endp
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0447);
        }

        [TestMethod]
        public void LocalStatementOutOfLocalScopeRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .local myLocal 
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0448);
        }

        [TestMethod]
        public void AlreadyDeclaredLocalRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .proc
                    .local myLocal
                    .local myLocal
                .endp
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0449);
        }

        [TestMethod]
        public void LocalWorksWithPublishedLabel()
        {
            CodeEmitWorks(@"
                .proc
                    local nonpublished
                    published: nop
                    nonpublished: nop
                .endp
                ld bc,published
            ",
                0x00, 0x00, 0x01, 0x00, 0x80);
        }

        [TestMethod]
        public void LocalWorksWithPublishedLabelAndFixUp()
        {
            CodeEmitWorks(@"
                ld bc,published
                .proc
                    local nonpublished
                    published: nop
                    nonpublished: nop
                .endp
            ",
                0x01, 0x03, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void LocalFailsWithNonPublishedLabel()
        {
            CodeRaisesError(@"
                .proc
                    local nonpublished
                    published: nop
                    nonpublished: nop
                .endp
                ld bc,nonpublished
            ", Errors.Z0201);
        }

        [TestMethod]
        public void LocalFailsWithNonPublishedLabelAndOption()
        {
            CodeRaisesError(@"
                .proc
                    local nonpublished
                    published: nop
                    nonpublished: nop
                .endp
                ld bc,nonpublished
            ", Errors.Z0201, new AssemblerOptions { ProcExplicitLocalsOnly = true });
        }

        [TestMethod]
        public void LocalFailsWithNonPublishedLabelAndOption2()
        {
            CodeRaisesError(@"
                .proc
                    local nonpublished
                    published: nop
                    nonpublished: nop
                .endp
                ld bc,nonpublished
            ", Errors.Z0201, new AssemblerOptions { ProcExplicitLocalsOnly = true });
        }

        [TestMethod]
        public void LocalWorksWithExplicitlyPublishedLabel()
        {
            CodeEmitWorksWithOptions(new AssemblerOptions { ProcExplicitLocalsOnly = true },  @"
                .proc
                    published: nop
                    nonpublished: nop
                .endp
                ld bc,published
            ",
                0x00, 0x00, 0x01, 0x00, 0x80);
        }


    }
}
