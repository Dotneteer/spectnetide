using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class CompareBinPragmaTests : ParserTestBed
    {
        private const string TEST_FOLDER = "TestFiles";

        [TestMethod]
        public void CompilationWithNonExistingCompareBinFileFails()
        {
            // --- Act
            var output = Compile("CompareBinNotExists.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0446);
        }

        [TestMethod]
        public void CompilationWithExistingCompareBinFileWorks()
        {
            // --- Act
            var output = Compile("CompareBinExists.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void CompilationWithZeroOffsetWorks()
        {
            // --- Act
            var output = Compile("CompareBinWithZeroOffset.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void CompilationWithNonZeroOffsetWorks()
        {
            // --- Act
            var output = Compile("CompareBinWithNonZeroOffset.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void CompilationWithZeroOffsetAndLengthWorks()
        {
            // --- Act
            var output = Compile("CompareBinWithZeroOffsetAndLength.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void CompilationWithNonZeroOffsetAndLengthWorks()
        {
            // --- Act
            var output = Compile("CompareBinWithNonZeroOffsetAndLength.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void CompilationWithNegativeOffsetFails()
        {
            // --- Act
            var output = Compile("CompareBinWithNegativeOffset.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0443);
        }

        [TestMethod]
        public void CompilationWithTooLongOffsetFails()
        {
            // --- Act
            var output = Compile("CompareBinWithTooLongOffset.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0443);
        }

        [TestMethod]
        public void CompilationWithTightOffsetWorks()
        {
            // --- Act
            var output = Compile("CompareBinWithTightOffset.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void CompilationWithNegativeLengthFails()
        {
            // --- Act
            var output = Compile("CompareBinWithNegativeLength.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0444);
        }

        [TestMethod]
        public void CompilationWithTooShortSegmentFails()
        {
            // --- Act
            var output = Compile("CompareBinWithTooLongSegment.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0445);
        }

        [TestMethod]
        public void CompilationWithDifferentDataFails()
        {
            // --- Act
            var output = Compile("CompareBinWithNoMatch.z80asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0445);
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