using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class ContinueTests: AssemblerTestBed
    {
        [TestMethod]
        public void ContinueRaisesErrorInGlobalScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                .continue
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0416);
        }

        [TestMethod]
        public void ContinueRaisesErrorInNonLoopScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                .if true
                    .continue
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0416);
        }

        [TestMethod]
        public void ContinueWorksInLoopScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                .loop 3
                    .continue
                .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void ContinueWorksInRepeatScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                .repeat
                    .continue
                .until true
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void ContinueWorksInWhileScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                exit = false
                .while !exit
                    exit = true;
                    .continue
                .endw
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void ContinueWorksInForNextScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                .for _i = 0 .to 3
                    .continue
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void ContinueWithNestedLoopEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,#1234
                    .loop 3
                        inc a
                        .if $cnt == 2
                            .continue;
                        .endif
                        nop
                    .endl
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x3C, 0x00, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x3C, 0x00);
        }

        [TestMethod]
        public void ContinueWithLoopEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 5
                    .if $cnt == 4
                        .continue
                    .endif
                        .db $cnt
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03, 0x05);
        }

        [TestMethod]
        public void ContinueWithRepeatEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .repeat
                    .if $cnt == 4
                        .continue
                    .endif
                    .db $cnt
                .until $cnt == 5
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03, 0x05);
        }

        [TestMethod]
        public void ContinueWithWhileEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .while $cnt <= 5 
                    .if $cnt == 4
                        .continue
                    .endif
                    .db $cnt
                .endw";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03, 0x05);
        }

        [TestMethod]
        public void ContinueWithForEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .for value = 1 to 5
                    .if value == 4
                        .continue
                    .endif
                    .db value
                .next";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03, 0x05);
        }

        [TestMethod]
        public void ContinueWithNestedRepeatEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,#1234
                    counter = 0
                    .repeat
                        counter = counter + 1
                        inc a
                        .if $cnt == 2
                            .continue;
                        .endif
                        nop
                    .until counter == 3
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x3C, 0x00, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x3C, 0x00);
        }

        [TestMethod]
        public void ContinueWithNestedWhileEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,#1234
                    counter = 0
                    .while counter < 3
                        counter = counter + 1
                        inc a
                        .if $cnt == 2
                            .continue;
                        .endif
                        nop
                    .endw
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x3C, 0x00, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x3C, 0x00);
        }

        [TestMethod]
        public void ContinueWithNestedForEmitsProperly()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,#1234
                    .for _i = 1 to 3
                        inc a
                        .if $cnt == 2
                            .continue;
                        .endif
                        nop
                    .next
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x3C, 0x00, 0x01, 0x34, 0x12, 0x3C, 0x00, 0x3C, 0x3C, 0x00);
        }

    }
}