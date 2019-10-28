namespace Spect.Net.SpectrumEmu.Abstraction.Devices.Floppy
{
    /// <summary>
    /// This class defines the behavior of a floppy device logger.
    /// </summary>
    public interface IFloppyDeviceLogger
    {
        /// <summary>
        /// Allows to trace a floppy message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        void Trace(string message);

        /// <summary>
        /// A new command byte has been received
        /// </summary>
        /// <param name="cmd">Command to log.</param>
        void CommandReceived(byte cmd);

        /// <summary>
        /// Command parameters received by the controller
        /// </summary>
        /// <param name="pars">Command parameters to log.</param>
        void CommandParamsReceived(byte[] pars);

        /// <summary>
        /// Data received by the controller
        /// </summary>
        /// <param name="data">Data to log.</param>
        void DataReceived(byte[] data);

        /// <summary>
        /// Data sent back by the controller.
        /// </summary>
        /// <param name="data">Data to log.</param>
        void DataSent(byte[] data);

        /// <summary>
        /// Result sent back by the controller
        /// </summary>
        /// <param name="result">Result to log.</param>
        void ResultSent(byte[] result);
    }
}