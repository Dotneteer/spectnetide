using System.IO;
using Shouldly;
using Spect.Net.TestParser.Compiler;
using Spect.Net.TestParser.Plan;

namespace Spect.Net.TestParser.Test
{
    public class CompilerTestBed
    {
        private const string TEST_FOLDER = "TestFiles";


        protected TestFilePlan Compile(string sourceText)
        {
            var folder = Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty;
            var testFolder = Path.Combine(folder, TEST_FOLDER);
            var compiler = new Z80TestCompiler { DefaultSourceFolder = testFolder };
            var result = compiler.Compile(sourceText);
            return result;
        }

        protected TestFilePlan CompileWorks(string sourceText)
        {
            var plan = Compile(sourceText);
            plan.Errors.Count.ShouldBe(0);
            return plan;
        }

        protected TestFilePlan CompileFile(string filename)
        {
            var folder = Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty;
            var testFolder = Path.Combine(folder, TEST_FOLDER);
            var fullname = Path.Combine(testFolder, filename);

            var compiler = new Z80TestCompiler {DefaultSourceFolder = testFolder};
            return compiler.CompileFile(fullname);
        }

        protected TestFilePlan CompileFileWorks(string filename)
        {
            var plan = CompileFile(filename);
            plan.Errors.Count.ShouldBe(0);
            return plan;
        }
    }
}