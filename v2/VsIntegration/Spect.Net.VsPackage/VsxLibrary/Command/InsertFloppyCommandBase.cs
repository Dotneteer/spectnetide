using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using System;
using System.Windows;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class represents an abstract insert floppy command
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

            if (FloppyDevice == null)
            {
                mc.Visible = false;
                return;
            }
            mc.Visible = FloppyFile(FloppyDevice) == null
                && string.Compare(FloppyDevice.DriveAFloppy?.Filename, ItemPath, StringComparison.InvariantCultureIgnoreCase) != 0
                && string.Compare(FloppyDevice.DriveBFloppy?.Filename, ItemPath, StringComparison.InvariantCultureIgnoreCase) != 0;
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
            try
            {
                await InsertFloppyAsync(FloppyDevice, ItemPath);
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
        protected abstract Task InsertFloppyAsync(FloppyDevice device, string vfddPath);
    }
}