using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class IncludeBinPragmaTests : ParserTestBed
    {
        private const string TEST_FOLDER = "TestFiles";

        [TestMethod]
        public void CompilationWithNonExistingIncludeBinFileFails()
        {
            // --- Act
            var output = Compile("IncludeBinNotExists.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0423);
        }

        [TestMethod]
        public void CompilationWithExistingIncludeBinFileWorks()
        {
            // --- Act
            var output = Compile("IncludeBinExists.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            var code = output.Segments[0].EmittedCode;
            code.Count.ShouldBe(0x4000);
            code[0].ShouldBe((byte)0xF3);
        }

        [TestMethod]
        public void CompilationWithZeroOffsetWorks()
        {
            // --- Act
            var output = Compile("IncludeBinWithZeroOffset.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            var code = output.Segments[0].EmittedCode;
            code.Count.ShouldBe(0x4000);
            code[0].ShouldBe((byte)0xF3);
        }

        [TestMethod]
        public void CompilationWithNonZeroOffsetWorks()
        {
            // --- Act
            var output = Compile("IncludeBinWithNonZeroOffset.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            var code = output.Segments[0].EmittedCode;
            code.Count.ShouldBe(0x3000);
            code[0].ShouldBe((byte)0x6D);
        }

        [TestMethod]
        public void CompilationWithZeroOffsetAndLengthWorks()
        {
            // --- Act
            var output = Compile("IncludeBinWithZeroOffsetAndLength.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            var code = output.Segments[0].EmittedCode;
            code.Count.ShouldBe(0x1800);
            code[0].ShouldBe((byte)0xF3);
        }

        [TestMethod]
        public void CompilationWithNonZeroOffsetAndLengthWorks()
        {
            // --- Act
            var output = Compile("IncludeBinWithNonZeroOffsetAndLength.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            var code = output.Segments[0].EmittedCode;
            code.Count.ShouldBe(0x1800);
            code[0].ShouldBe((byte)0x6D);
        }

        [TestMethod]
        public void CompilationWithNegativeOffsetFails()
        {
            // --- Act
            var output = Compile("IncludeBinWithNegativeOffset.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0424);
        }

        [TestMethod]
        public void CompilationWithTooLongOffsetFails()
        {
            // --- Act
            var output = Compile("IncludeBinWithTooLongOffset.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0424);
        }

        [TestMethod]
        public void CompilationWithTightOffsetWorks()
        {
            // --- Act
            var output = Compile("IncludeBinWithTightOffset.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            var code = output.Segments[0].EmittedCode;
            code.Count.ShouldBe(0x01);
            code[0].ShouldBe((byte)0x3C);
        }

        [TestMethod]
        public void CompilationWithNegativeLengthFails()
        {
            // --- Act
            var output = Compile("IncludeBinWithNegativeLength.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0425);
        }

        [TestMethod]
        public void CompilationWithTooLongSegmentFails()
        {
            // --- Act
            var output = Compile("IncludeBinWithTooLongSegment.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0425);
        }

        [TestMethod]
        public void CompilationWithTightSegmentWorks()
        {
            // --- Act
            var output = Compile("IncludeBinWithTightSegment.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            var code = output.Segments[0].EmittedCode;
            code.Count.ShouldBe(0x2000);
            code[0].ShouldBe((byte)0x0D);
        }

        private AssemblerOutput Compile(string filename)
        {
            var folder = Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty;
            var fullname = Path.Combine(Path.Combine(folder, TEST_FOLDER), filename);

            var asm = new Z80Assembler();
            return asm.CompileFile(fullname);
        }
    }
}
