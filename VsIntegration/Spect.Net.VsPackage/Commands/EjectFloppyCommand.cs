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
    [CommandId(0x0820)]
    public class EjectFloppyCommand : VfddCommandBase
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

            mc.Visible = false;
            if (string.Compare(floppyDevice.DriveAFloppy?.Filename, ItemPath,
                StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                mc.Text = "Eject from drive A:";
                mc.Visible = true;
            }
            else if (string.Compare(floppyDevice.DriveBFloppy?.Filename, ItemPath,
                StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                mc.Text = "Eject from drive B:";
                mc.Visible = true;
            }
        }

        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            if (!(Package.MachineViewModel.SpectrumVm.FloppyDevice is FloppyDevice floppyDevice))
            {
                return;
            }

            if (string.Compare(floppyDevice.DriveAFloppy?.Filename, ItemPath,
                    StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                await floppyDevice.EjectDriveA();
            }
            else if (string.Compare(floppyDevice.DriveBFloppy?.Filename, ItemPath,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                await floppyDevice.EjectDriveB();
            }
        }
    }
}