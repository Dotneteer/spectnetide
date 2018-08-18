using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Floppy;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Insert a virtual floppy disk into drive A:
    /// </summary>
    [CommandId(0x0818)]
    public class InsertDriveACommand : InsertFloppyCommandBase
    {
        /// <summary>
        /// The floppy file that belongs to the menu function
        /// </summary>
        /// <param name="device">Floppy device</param>
        protected override VirtualFloppyFile FloppyFile(FloppyDevice device)
            => device.DriveAFloppy;

        /// <summary>
        /// Inserts the floppy to the device
        /// </summary>
        /// <param name="device">Floppy device</param>
        /// <param name="vfddPath">Path of the floppy file</param>
        /// <returns></returns>
        protected override Task InsertFloppyAsync(FloppyDevice device, string vfddPath)
            => device.InsertDriveA(vfddPath);
    }
}