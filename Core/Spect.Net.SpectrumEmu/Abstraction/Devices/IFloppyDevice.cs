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
        /// Sets the flag that indicates if an FDD is busy
        /// </summary>
        /// <param name="fdd">FDD index (0..3)</param>
        /// <param name="busy">True: in seek mode; false: accepts commands</param>
        void SetFddBusy(int fdd, bool busy);

        /// <summary>
        /// Sets the flag that indicates if the controller is busy
        /// </summary>
        /// <param name="busy">True: busy; false: accepts commands</param>
        void SetFdcBusy(bool busy);

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
        /// Sets the Request for Master (RQM) flag
        /// </summary>
        /// <param name="rqm">RQM flag</param>
        void SetRqmFlag(bool rqm);

        /// <summary>
        /// Sends a command byte to the controller
        /// </summary>
        /// <param name="cmd">Command byte</param>
        void WriteCommandByte(byte cmd);

        /// <summary>
        /// Reads a result byte
        /// </summary>
        /// <returns>Result byte received</returns>
        byte ReadResultByte();
    }
}