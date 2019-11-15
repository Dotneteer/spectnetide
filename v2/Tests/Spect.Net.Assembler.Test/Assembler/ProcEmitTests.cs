using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class ProcEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void EntPragmaCannotBeUsedInProc()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .proc
                .ent #8000;
                .endp
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0407);
        }

        [TestMethod]
        public void XentPragmaCannotBeUsedInProc()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .proc
                .xent #8000;
                .endp
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0407);
        }

        [TestMethod]
        [DataRow(".endp")]
        [DataRow("endp")]
        [DataRow(".ENDP")]
        [DataRow("ENDP")]
        [DataRow(".pend")]
        [DataRow("pend")]
        [DataRow(".PEND")]
        [DataRow("PEND")]
        public void EndProcWithoutOpenTagFails(string source)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(source);

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0405);
        }

        [TestMethod]
        public void MissingProcEndIsDetected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .proc
                    ld a,b
                    ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0401);
        }

        [TestMethod]
        public void ProcWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .proc
                    .endp
                    ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void LabeledProcWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    MyProc: .proc
                        .endp
                    ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYPROC").ShouldBeTrue();
            output.Symbols["MYPROC"].Value.Value.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void HangingLabeledLoopWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    MyProc: 
                        .proc
                        .endp
                    ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYPROC").ShouldBeTrue();
            output.Symbols["MYPROC"].Value.Value.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void EndLabeledProcWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .proc
                    MyEnd: .endp
                    ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYEND").ShouldBeFalse();
        }

        [TestMethod]
        public void HangingEndLabeledProcWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                        .proc
                    MyEnd: 
                        .endp
                    ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYEND").ShouldBeFalse();
        }

        [TestMethod]
        public void LoopEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                    .proc
                        ld bc,#1234
                    .endp
                    ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12);
        }

        [TestMethod]
        public void LoopEmitWithMultipleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                    .proc
                        inc b
                        inc c
                        inc d
                    .endp
                    ";

            CodeEmitWorks(SOURCE, 0x04, 0x0C, 0x14);
        }

        [TestMethod]
        public void ProcWithInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                    .proc
                        ThisLabel: ld bc,ThisLabel
                    .endp
                    ";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80);
        }

        [TestMethod]
        public void ProcWithInternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                    .proc
                        ld bc,ThisLabel
                        ThisLabel: nop
                    .endp
                    ";

            CodeEmitWorks(SOURCE, 0x01, 0x03, 0x80, 0x00);
        }

        [TestMethod]
        public void ProcWithStartLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                    StartLabel: .proc
                        ld bc,StartLabel
                        nop
                    .endp
                    ";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void ProcWithEndLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                    .proc
                        ld bc,EndLabel
                        nop
                    EndLabel: .endp
                    ";

            CodeEmitWorks(SOURCE, 0x01, 0x04, 0x80, 0x00);
        }

        [TestMethod]
        public void ProcWithExternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                    .proc
                        ld bc,OuterLabel
                        nop
                    .endp
                OuterLabel: nop
                    ";

            CodeEmitWorks(SOURCE, 0x01, 0x04, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void NestedProcEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                    .proc
                        ld bc,#1234
                        .proc
                            inc a
                        .endp
                    .endp
                    ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C);
        }

        [TestMethod]
        public void NestedProcWithLabelsWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                    .proc
                        inc a
                        .proc
                            ld hl,EndLabel
                            ld bc,NopLabel
                        EndLabel: .endp
                        NopLabel: nop
                    .endp
                    ";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x07, 0x80, 0x00);
        }

        [TestMethod]
        public void NestedLoopWithLabelsWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                    .proc
                        inc a
                        .proc
                            ld hl,EndLabel
                            ld bc,NopLabel
                        EndLabel: 
                            nop
                            .endp
                        NopLabel: nop
                    .endp
                    ";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x08, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void LoopWithVarWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                    index = 1;
                    .proc
                        ld a,index
                        index = index + 1
                        nop
                        ld a,index
                    EndLabel: .endp
                    ";

            CodeEmitWorks(SOURCE, 0x3E, 0x01, 0x00, 0x3E, 0x02);
        }

        [TestMethod]
        public void LoopWithNestedVarWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                    index = 1;
                    .proc
                        index = 5
                        ld a,index
                        index := index + 1
                        nop
                        ld a,index
                    EndLabel: .endp
                    ";

            CodeEmitWorks(SOURCE, 0x3E, 0x05, 0x00, 0x3E, 0x06);
        }

        [TestMethod]
        public void NestedProcWithVarWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                    index = 1;
                    .proc
                        ld a,index
                        .proc
                            index = index + 1
                            nop
                            ld a,index
                        .endp
                    EndLabel: .endp
                    ";

            CodeEmitWorks(SOURCE, 0x3E, 0x01, 0x00, 0x3E, 0x02);
        }

        [TestMethod]
        public void NestedProcWithVarWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                    index .equ 1;
                    .proc
                        ld a,index
                        .proc
                            nop
                            index .equ 5
                            ld a,index
                        .endp
                        ld a,index
                    EndLabel: .endp
                    ";

            CodeEmitWorks(SOURCE, 0x3E, 0x01, 0x00, 0x3E, 0x05, 0x3E, 0x01);
        }


        [TestMethod]
        public void LocalLabelsWorkInProcWithNoExplicitLocal()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .proc
                myLabel: nop;
                .endp
                .proc
                myLabel: nop;
                .endp
                ",
                new AssemblerOptions { ProcExplicitLocalsOnly = false });

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void LocalLabelsWorkInProcWithExplicitLocal()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .proc
                local myLabel
                myLabel: nop
                .endp
                .proc
                local myLabel
                myLabel: nop
                .endp
                ",
                new AssemblerOptions
                {
                    ProcExplicitLocalsOnly = true,
                    UseCaseSensitiveSymbols = true
                });

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }
    }
}