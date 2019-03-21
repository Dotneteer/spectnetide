using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Floppy;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Insert a virtual floppy disk into drive B:
    /// </summary>
    [CommandId(0x0819)]
    public class InsertDriveBCommand : InsertFloppyCommandBase
    {
        /// <summary>
        /// Override this method to define the status query action
        /// </summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;

            mc.Visible = Package.MachineViewModel.SpectrumVm.FloppyConfiguration?.DriveBPresent == true;
        }

        /// <summary>
        /// The floppy file that belongs to the menu function
        /// </summary>
        /// <param name="device">Floppy device</param>
        protected override VirtualFloppyFile FloppyFile(FloppyDevice device)
            => device.DriveBFloppy;

        /// <summary>
        /// Inserts the floppy to the device
        /// </summary>
        /// <param name="device">Floppy device</param>
        /// <param name="vfddPath">Path of the floppy file</param>
        /// <returns></returns>
        protected override Task InsertFloppyAsync(FloppyDevice device, string vfddPath)
            => device.InsertDriveB(vfddPath);
    }
}