using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class enables tool window commands only when the ZX Spectrum emulator has
    /// already been initialized.
    /// </summary>
    public abstract class SpectrumToolWindowCommandBase : ShowToolWindowCommandBase
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = SpectNetPackage.Default.EmulatorViewModel.Machine != null;
    }
}