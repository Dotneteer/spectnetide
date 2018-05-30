using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Devices.Floppy;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This class controls the floppy device
    /// </summary>
    public interface IFloppyDevice: ISpectrumBoundDevice
    {
        /// <summary>
        /// The value of the main status register
        /// </summary>
        byte MainStatusRegister { get; }

        /// <summary>
        /// Sets the flag that indicates executin mode
        /// </summary>
        /// <param name="exm">Execution mode flag</param>
        void SetExecutionMode(bool exm);

        /// <summary>
        /// Sets the Data Input/Output (DIO) flag
        /// </summary>
        /// <param name="dio">DIO flag</param>
        void SetDioFlag(bool dio);

        /// <summary>
        /// Sends a command byte to the controller
        /// </summary>
        /// <param name="cmd">Command byte</param>
        void WriteCommandByte(byte cmd);

        /// <summary>
        /// Reads a result byte
        /// </summary>
        /// <param name="executionMode">Execution mode after read</param>
        /// <returns>Result byte received</returns>
        byte ReadResultByte(out bool executionMode);

        /// <summary>
        /// Gets the virtual floppy in Drive A:
        /// </summary>
        VirtualFloppyFile DriveAFloppy { get; }

        /// <summary>
        /// Gets the virtual floppy in Drive B:
        /// </summary>
        VirtualFloppyFile DriveBFloppy { get; }

        /// <summary>
        /// Inserts a virtual FDD file into Drive A:
        /// </summary>
        /// <param name="vfddPath"></param>
        Task InsertDriveA(string vfddPath);

        /// <summary>
        /// Inserts a virtual FDD file into Drive B:
        /// </summary>
        /// <param name="vfddPath"></param>
        Task InsertDriveB(string vfddPath);

        /// <summary>
        /// Ejects the disk from Drive A:
        /// </summary>
        Task EjectDriveA();

        /// <summary>
        /// Ejects the disk from Drive A:
        /// </summary>
        Task EjectDriveB();
    }
}