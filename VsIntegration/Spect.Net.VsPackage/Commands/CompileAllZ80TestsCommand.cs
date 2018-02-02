using Spect.Net.VsPackage.Vsx;
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
            var testFiles = Package.TestManager.CompileAllFiles();
            Output.Clear();
            foreach (var testFile in testFiles.TestFilePlans)
            {
                Output.Add(testFile);
            }
            return Output.ErrorCount == 0;
        }
    }
}