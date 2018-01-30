using System;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Vsx.Output;
using Spect.Net.VsPackage.Z80Programs;
using Spect.Net.VsPackage.Z80Programs.Commands;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Run Z80 tests command
    /// </summary>
    [CommandId(0x0816)]
    public class CompileAllZ80TestsCommand : Z80TestCommandBase
    {
        /// <summary>
        /// Override this property to allow project item selection
        /// </summary>
        public override bool AllowProjectItem => true;

        /// <summary>
        /// Compiles the code.
        /// </summary>
        /// <returns>True, if compilation successful; otherwise, false</returns>
        protected override bool CompileCode()
        {
            var testFiles = Package.CodeDiscoverySolution.CurrentProject.Z80TestProjectItems;
            if (testFiles.Count == 0) return false;

            var testManager = Package.TestManager;
            var start = DateTime.Now;
            var pane = OutputWindow.GetPane<Z80BuildOutputPane>();
            pane.WriteLine("Z80 Test Compiler");
            foreach (var file in testFiles)
            {
                var filename = file.Filename;
                pane.WriteLine($"Compiling {filename}");
                var testPlan = testManager.CompileFile(filename);
                Output.Add(testPlan);
            }
            var duration = (DateTime.Now - start).TotalMilliseconds;
            pane.WriteLine($"Compile time: {duration}ms");
            return Output.ErrorCount == 0;
        }
    }
}