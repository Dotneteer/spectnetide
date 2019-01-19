using System;
using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Floppy;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Represents the command that ejects a floppy
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

            if (!(Package.MachineViewModel.SpectrumVm.FloppyDevice is FloppyDevice floppyDevice))
            {
                mc.Visible = false;
                return;
            }

            mc.Visible = string.Compare(floppyDevice.DriveAFloppy?.Filename, ItemPath,
                             StringComparison.InvariantCultureIgnoreCase) != 0
                         && string.Compare(floppyDevice.DriveBFloppy?.Filename, ItemPath,
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