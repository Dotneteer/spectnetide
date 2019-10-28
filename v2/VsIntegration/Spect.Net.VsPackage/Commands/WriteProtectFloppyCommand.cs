using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.VsPackage.VsxLibrary.Command;
using System;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command makes the specified virtual floppy disk write protected.
    /// </summary>
    [CommandId(0x0821)]
    public class WriteProtectFloppyCommand : VfddCommandBase
    {
        /// <summary>
        /// Override this method to define the status query action
        /// </summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;

            if (FloppyDevice == null)
            {
                mc.Visible = false;
                return;
            }

            mc.Visible = string.Compare(FloppyDevice.DriveAFloppy?.Filename, ItemPath,
                             StringComparison.InvariantCultureIgnoreCase) != 0
                         && string.Compare(FloppyDevice.DriveBFloppy?.Filename, ItemPath,
                             StringComparison.InvariantCultureIgnoreCase) != 0;
            if (!mc.Visible) return;

            mc.Text = VirtualFloppyFile.CheckWriteProtection(ItemPath)
                ? "Remove write protection"
                : "Make write protected";
        }

        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override Task ExecuteAsync()
        {
            var currentProtection = VirtualFloppyFile.CheckWriteProtection(ItemPath);
            VirtualFloppyFile.SetWriteProtection(ItemPath, !currentProtection);
            return Task.FromResult(0);
        }
    }
}