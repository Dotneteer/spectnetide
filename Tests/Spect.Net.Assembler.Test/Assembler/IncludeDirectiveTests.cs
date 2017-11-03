using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class IncludeDirectiveTests: ParserTestBed
    {
        private const string TEST_FOLDER = "TestFiles";

        [TestMethod]
        public void Include_WhenInvoked_ShouldReturnTokensForIncludeAndTheFileName()
        {
            var expectedFileName = "myFileName";

            var visitor = Parse($"#include \"{expectedFileName}\"");

            visitor.Compilation.Lines.Count.ShouldBe(1);
            AssertFilename(visitor.Compilation.Lines[0], expectedFileName);
        }

        [TestMethod]
        public void Include_WhenInvokedMultipleTimes_ShouldReturnAllFilenamesToInclude()
        {
            var visitor = Parse($"#include \"hello.z80asm\" {Environment.NewLine}#include \"world.z80asm\"");

            visitor.Compilation.Lines.Count.ShouldBe(2);
            AssertFilename(visitor.Compilation.Lines[0], "hello.z80asm");
            AssertFilename(visitor.Compilation.Lines[1], "world.z80asm");
        }

        private void AssertFilename(SourceLineBase line, string expectedFileName)
        {
            var includeLine = line as IncludeDirective;
            includeLine.ShouldNotBeNull();
            includeLine.Filename.ShouldBe(expectedFileName);
        }

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
            var addrMap = output.AddressMap;
            map.Count.ShouldBe(3);
            addrMap.Count.ShouldBe(3);
            var mi = map[0x8000];     // Line #1, Scenario1.z80Asm 
            mi.FileIndex.ShouldBe(0);
            mi.Line.ShouldBe(1);
            addrMap[(0,1)].ShouldBe((ushort)0x8000);

            mi = map[0x8001];         // Line #1, Scenario1.inc1.z80Asm
            mi.FileIndex.ShouldBe(1);
            mi.Line.ShouldBe(1);
            addrMap[(1, 1)].ShouldBe((ushort)0x8001);

            mi = map[0x8004];         // Line #3, Scenario1.z80Asm
            mi.FileIndex.ShouldBe(0);
            mi.Line.ShouldBe(3);
            addrMap[(0, 3)].ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void Scenario2Works()
        {
            CompileFileWorks("Scenario2.z80Asm",
                0x78, 0x01, 0x34, 0x12, 0x41, 0x01, 0x45, 0x23, 0x37, 0x21, 0x56, 0x34, 0xc9);
        }

        [TestMethod]
        public void Scenario2GeneratesProperOutput()
        {
            // --- Act
            var output = Compile("Scenario2.z80Asm");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.SourceFileList.Count.ShouldBe(4);
            Path.GetFileName(output.SourceFileList[0].Filename).ShouldBe("Scenario2.z80Asm");
            Path.GetFileName(output.SourceFileList[1].Filename).ShouldBe("Scenario2.inc1.z80Asm");
            Path.GetFileName(output.SourceFileList[2].Filename).ShouldBe("Scenario2.inc2.z80Asm");
            Path.GetFileName(output.SourceFileList[3].Filename).ShouldBe("Scenario2.inc21.z80Asm");

            var map = output.SourceMap;
            map.Count.ShouldBe(7);
            var mi = map[0x8000];     // Line #1, Scenario2.z80Asm 
            mi.FileIndex.ShouldBe(0);
            mi.Line.ShouldBe(1);
            mi = map[0x8001];         // Line #1, Scenario2.inc1.z80Asm
            mi.FileIndex.ShouldBe(1);
            mi.Line.ShouldBe(1);
            mi = map[0x8004];         // Line #3, Scenario2.z80Asm
            mi.FileIndex.ShouldBe(0);
            mi.Line.ShouldBe(3);
            mi = map[0x8005];         // Line #1, Scenario2.inc2.z80Asm
            mi.FileIndex.ShouldBe(2);
            mi.Line.ShouldBe(1);
            mi = map[0x8008];         // Line #1, Scenario2.inc21.z80Asm
            mi.FileIndex.ShouldBe(3);
            mi.Line.ShouldBe(1);
            mi = map[0x8009];         // Line #3, Scenario2.inc2.z80Asm
            mi.FileIndex.ShouldBe(2);
            mi.Line.ShouldBe(3);
            mi = map[0x800C];         // Line #5, Scenario2.z80Asm
            mi.FileIndex.ShouldBe(0);
            mi.Line.ShouldBe(5);
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
