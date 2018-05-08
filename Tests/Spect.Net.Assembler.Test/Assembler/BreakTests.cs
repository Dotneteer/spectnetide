using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class BreakTests: AssemblerTestBed
    {
        [TestMethod]
        public void BreakRaisesErrorInGlobalScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                .break
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0415);
        }

        [TestMethod]
        public void BreakRaisesErrorInNonLoopScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                .if true
                    .break
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0415);
        }

        [TestMethod]
        public void BreakWorksInLoopScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                .loop 3
                    .break
                .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void BreakWorksInRepeatScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                .repeat
                    .break
                .until true
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void BreakWorksInWhileScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                exit = false
                .while !exit
                    .break
                    exit = true;
                .endw
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void BreakWorksInForNextScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                .for _i = 0 .to 3
                    .break
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void BreakWithLoopEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 5
                    .if $cnt == 4
                        .break
                    .endif
                    .db $cnt
                .endl";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03);
        }

        [TestMethod]
        public void BreakWithRepeatEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .repeat
                    .if $cnt == 4
                        .break
                    .endif
                    .db $cnt
                .until $cnt == 5";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03);
        }

        [TestMethod]
        public void BreakWithWhileEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .while $cnt < 5
                    .if $cnt == 4
                        .break
                    .endif
                    .db $cnt
                .endw";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03);
        }

        [TestMethod]
        public void BreakWithForEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .for value = 1 to 5
                    .if value == 4
                        .break
                    .endif
                    .db value
                .next";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03);
        }

        [TestMethod]
        public void BreakWithNestedLoopEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,#1234
                    .loop 3
                        inc a
                        .if $cnt == 2
                            .break;
                        .endif
                        nop
                    .endl
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C);
        }

        [TestMethod]
        public void BreakWithNestedRepeatEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,#1234
                    counter = 0
                    .repeat
                        inc a
                        .if $cnt == 2
                            .break;
                        .endif
                        nop
                        counter = counter + 1
                    .until counter == 3
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C);
        }

        [TestMethod]
        public void BreakWithNestedWhileEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,#1234
                    counter = 0
                    .while counter < 3
                        inc a
                        .if $cnt == 2
                            .break;
                        .endif
                        nop
                        counter = counter + 1
                    .endw
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C);
        }

        [TestMethod]
        public void BreakWithNestedForEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,#1234
                    .for _i = 1 to 3
                        inc a
                        .if $cnt == 2
                            .break;
                        .endif
                        nop
                    .next
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C);
        }

    }
}
