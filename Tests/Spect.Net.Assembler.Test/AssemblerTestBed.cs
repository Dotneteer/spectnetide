﻿using System;
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

        protected void CodeEmitWorksWithFixup(string source, FixupType type, int offset, params byte[] opCodes)
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
            output.Fixups.Count.ShouldBe(1);
            var fixup = output.Fixups[0];
            fixup.Type.ShouldBe(type);
            fixup.SegmentIndex.ShouldBe(0);
            fixup.Offset.ShouldBe(offset);
            fixup.Expression.ShouldNotBeNull();
        }

        public void CodeRaisesError(string instruction, string errorCode)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(instruction);

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            Console.WriteLine(output.Errors[0].Message);
            output.Errors[0].ErrorCode.ShouldBe(errorCode);
        }
    }
}