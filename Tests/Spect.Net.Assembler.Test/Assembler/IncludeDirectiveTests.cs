using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class IncludeDirectiveTests: ParserTestBed
    {
        private const string TEST_FOLDER = "TestFiles";

        [TestMethod]
        public void NoIncludeIncludeWorksAsExpected()
        {
            // --- Act
            var output = Compile("NoInclude.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            Path.GetFileName(output.SourceItem.Filename).ShouldBe("NoInclude.z80Asm");
            output.SourceItem.Includes.Count.ShouldBe(0);
        }

        [TestMethod]
        public void SingleIncludeWorksAsExpected()
        {
            // --- Act
            var output = Compile("SingleInclude.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            Path.GetFileName(output.SourceItem.Filename).ShouldBe("SingleInclude.z80Asm");
            output.SourceItem.Includes.Count.ShouldBe(1);
            Path.GetFileName(output.SourceItem.Includes[0].Filename).ShouldBe("inc1.z80Asm");
        }

        [TestMethod]
        public void MultipleIncludeWorksAsExpected()
        {
            // --- Act
            var output = Compile("MultipleInclude.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            Path.GetFileName(output.SourceItem.Filename).ShouldBe("MultipleInclude.z80Asm");
            output.SourceItem.Includes.Count.ShouldBe(2);
            Path.GetFileName(output.SourceItem.Includes[0].Filename).ShouldBe("inc1.z80Asm");
            Path.GetFileName(output.SourceItem.Includes[1].Filename).ShouldBe("inc2.z80Asm");
        }

        [TestMethod]
        public void IncludeFailsWithRepetition()
        {
            // --- Act
            var output = Compile("RepetitionInclude.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
        }

        [TestMethod]
        public void IncludeFailsWithSingleCircularity()
        {
            // --- Act
            var output = Compile("SingleCircular.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
        }

        [TestMethod]
        public void NestedIncludeWorksAsExpected()
        {
            // --- Act
            var output = Compile("NestedInclude.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            Path.GetFileName(output.SourceItem.Filename).ShouldBe("NestedInclude.z80Asm");
            output.SourceItem.Includes.Count.ShouldBe(2);
            Path.GetFileName(output.SourceItem.Includes[0].Filename).ShouldBe("incA.z80Asm");
            var itemA = output.SourceItem.Includes[0];
            itemA.Includes.Count.ShouldBe(2);
            Path.GetFileName(itemA.Includes[0].Filename).ShouldBe("inc1.z80Asm");
            Path.GetFileName(itemA.Includes[1].Filename).ShouldBe("inc2.z80Asm");
            Path.GetFileName(output.SourceItem.Includes[1].Filename).ShouldBe("incB.z80Asm");
            var itemB = output.SourceItem.Includes[0];
            itemB.Includes.Count.ShouldBe(2);
            Path.GetFileName(itemB.Includes[0].Filename).ShouldBe("inc1.z80Asm");
            Path.GetFileName(itemB.Includes[1].Filename).ShouldBe("inc2.z80Asm");
        }

        [TestMethod]
        public void MissingEndifIsDetected1()
        {
            // --- Act
            var output = Compile("MissingEndif1.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0062);
        }

        [TestMethod]
        public void MissingEndifIsDetected2()
        {
            // --- Act
            var output = Compile("MissingEndif2.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0062);
        }

        [TestMethod]
        public void MissingEndifIsDetected3()
        {
            // --- Act
            var output = Compile("MissingEndif3.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0062);
        }

        [TestMethod]
        public void MissingEndifIsDetected4()
        {
            // --- Act
            var output = Compile("MissingEndif4.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0062);
        }

        [TestMethod]
        public void Scenario1Works()
        {
            CompileFileWorks("Scenario1.z80Asm",
                0x78, 0x01, 0xcd, 0xab, 0x41);
        }

        [TestMethod]
        public void Scenario1GeneratesProperOutput()
        {
            // --- Act
            var output = Compile("Scenario1.z80Asm");
            
            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.SourceFileList.Count.ShouldBe(2);
            Path.GetFileName(output.SourceFileList[0].Filename).ShouldBe("Scenario1.z80Asm");
            Path.GetFileName(output.SourceFileList[1].Filename).ShouldBe("Scenario1.inc1.z80Asm");

            var map = output.SourceMap;
            map.Count.ShouldBe(3);
            var mi = map[0x8000];     // Line #1, Scenario1.z80Asm 
            mi.FileIndex.ShouldBe(0);
            mi.Line.ShouldBe(1);
            mi = map[0x8001];         // Line #1, Scenario1.inc1.z80Asm
            mi.FileIndex.ShouldBe(1);
            mi.Line.ShouldBe(1);
            mi = map[0x8004];         // Line #3, Scenario1.z80Asm
            mi.FileIndex.ShouldBe(0);
            mi.Line.ShouldBe(3);
        }

        private AssemblerOutput Compile(string filename)
        {
            var folder = Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty;
            var fullname = Path.Combine(Path.Combine(folder, TEST_FOLDER), filename);

            var asm = new Z80Assembler();
            return asm.CompileFile(fullname);
        }

        protected void CompileFileWorks(string filename, params byte[] opCodes)
        {
            var output = Compile(filename);

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
    }
}
