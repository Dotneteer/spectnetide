using System;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.VsPackage.Vsx;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Z80Programs.Floppy
{
    /// <summary>
    /// This class is intended to the the base of floppy commands
    /// </summary>
    public abstract class InsertFloppyCommandBase : VfddCommandBase
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
            mc.Visible = FloppyFile(floppyDevice) == null 
                && string.Compare(floppyDevice.DriveAFloppy?.Filename, ItemPath, StringComparison.InvariantCultureIgnoreCase) != 0
                && string.Compare(floppyDevice.DriveBFloppy?.Filename, ItemPath, StringComparison.InvariantCultureIgnoreCase) != 0;
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
            try
            {
                await InsertFloppy(floppyDevice, ItemPath);
            }
            catch (Exception)
            {
                VsxDialogs.Show("Cannot mount this virtual floppy disk file. It might be corrupt, or is inaccessible.",
                    "Error inserting floppy", MessageBoxButton.OK, VsxMessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// The floppy file that belongs to the menu function
        /// </summary>
        /// <param name="device">Floppy device</param>
        protected abstract VirtualFloppyFile FloppyFile(FloppyDevice device);

        /// <summary>
        /// Inserts the floppy to the device
        /// </summary>
        /// <param name="device">Floppy device</param>
        /// <param name="vfddPath">Path of the floppy file</param>
        /// <returns></returns>
        protected abstract Task InsertFloppy(FloppyDevice device, string vfddPath);
    }
}