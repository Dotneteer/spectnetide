namespace Spect.Net.SpectrumEmu.Devices.Floppy
{
    /// <summary>
    /// This class defines the behavior of a floppy device logger
    /// </summary>
    public interface IFloppyDeviceLogger
    {
        /// <summary>
        /// Allows to trace a floppy message
        /// </summary>
        /// <param name="message"></param>
        void Trace(string message);

        /// <summary>
        /// A new command byte has been received
        /// </summary>
        /// <param name="cmd"></param>
        void CommandReceived(byte cmd);

        /// <summary>
        /// Command parameters received by the controller
        /// </summary>
        /// <param name="pars"></param>
        void CommandParamsReceived(byte[] pars);

        /// <summary>
        /// Data received by the controller
        /// </summary>
        /// <param name="data"></param>
        void DataReceived(byte[] data);

        /// <summary>
        /// Data sent back by the controller
        /// </summary>
        /// <param name="data"></param>
        void DataSent(byte[] data);

        /// <summary>
        /// Result sent back by the controller
        /// </summary>
        /// <param name="result"></param>
        void ResultSent(byte[] result);
    }
}