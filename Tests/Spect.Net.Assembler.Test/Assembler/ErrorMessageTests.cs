using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class ErrorMessageTests : ParserTestBed
    {
        private const string TEST_FOLDER = "TestFiles";

        [TestMethod]
        public void Assemble_WhenAnErrorExistsInTheInitialFile_ShouldPutTHeInitialFilePathInTheError()
        {
            // --- Act
            var output = Compile("ErrorsNoInclude.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            Path.GetFileName(output.Errors.First().Filename).ShouldBe("ErrorsNoInclude.z80Asm");
        }

        [TestMethod]
        public void Assemble_WhenAnErrorExistsInANestedIncludeFile_ShouldPutTheActualErrorFilePathInTheError()
        {
            // --- Act
            var output = Compile("ErrorsInIncludedFilesLevel0.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            Path.GetFileName(output.Errors.First().Filename).ShouldBe("ErrorsInIncludedFilesLevel2.z80Asm");
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