using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class represents a command that can be associated with a Z80 code file.
    /// </summary>
    public abstract class ExecutableSpectrumProgramCommandBase : SpectrumProgramCommandBase
    {
        /// <summary>
        /// Allows this command only within the active project
        /// </summary>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;

            mc.Visible = IsInActiveProject;
        }
    }
}