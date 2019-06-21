using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
// ReSharper disable StringLiteralTypo

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class LocalStatementEmitTests
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
        public void AlreadyUsedLocalRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .proc
                myLocal: nop
                    .local myLocal
                .endp
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0449);
        }

    }
}
