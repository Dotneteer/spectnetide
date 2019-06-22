namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// This interface provides logger functionality for Next Register
    /// operations
    /// </summary>
    public interface INextRegisterAccessLogger
    {
        /// <summary>
        /// Logs that a register index has been set
        /// </summary>
        /// <param name="index">Register index</param>
        void RegisterIndexSet(byte index);

        /// <summary>
        /// Logs that a register value has been set
        /// </summary>
        /// <param name="value">Register value</param>
        void RegisterValueSet(byte value);

        /// <summary>
        /// Logs that a register value has been obtained
        /// </summary>
        /// <param name="value">Register value</param>
        void RegisterValueObtained(byte value);
    }
}