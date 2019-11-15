using System;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test
{
    public class AssemblerTestBed : ParserTestBed
    {
        protected void CodeEmitWorks(string source, params byte[] opCodes)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(source);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var bytes = output.Segments[0].EmittedCode;
            bytes.Count.ShouldBe(opCodes.Length);
            for (var i = 0; i < opCodes.Length; i++)
            {
                bytes[i].ShouldBe(opCodes[i]);
            }
        }

        protected void CodeEmitWorksWithOptions(AssemblerOptions options, string source, params byte[] opCodes)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(source, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var bytes = output.Segments[0].EmittedCode;
            bytes.Count.ShouldBe(opCodes.Length);
            for (var i = 0; i < opCodes.Length; i++)
            {
                bytes[i].ShouldBe(opCodes[i]);
            }
        }

        public void CodeRaisesError(string instruction, string errorCode, AssemblerOptions options = null)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(instruction, options);

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(errorCode);
        }

        public void CodeRaisesMultipleErrors(string instruction, AssemblerOptions options, params string[] errorCodes)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(instruction, options);

            // --- Assert
            output.ErrorCount.ShouldBe(errorCodes.Length);
            for (var i = 0; i < errorCodes.Length; i++)
            {
                output.Errors[i].ErrorCode.ShouldBe(errorCodes[i]);
            }
        }
    }
}