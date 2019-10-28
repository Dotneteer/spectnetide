using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.VsPackage.VsxLibrary.Command;
using System;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command ejects the specified virtual floppy disk within the active project.
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
            if (FloppyDevice == null)
            {
                mc.Visible = false;
                return;
            }

            mc.Visible = false;
            if (string.Compare(FloppyDevice.DriveAFloppy?.Filename, ItemPath,
                StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                mc.Text = "Eject from drive A:";
                mc.Visible = true;
            }
            else if (string.Compare(FloppyDevice.DriveBFloppy?.Filename, ItemPath,
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
            if (FloppyDevice == null)
            {
                return;
            }

            if (string.Compare(FloppyDevice.DriveAFloppy?.Filename, ItemPath,
                    StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                await FloppyDevice.EjectDriveA();
            }
            else if (string.Compare(FloppyDevice.DriveBFloppy?.Filename, ItemPath,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                await FloppyDevice.EjectDriveB();
            }
        }
    }
}