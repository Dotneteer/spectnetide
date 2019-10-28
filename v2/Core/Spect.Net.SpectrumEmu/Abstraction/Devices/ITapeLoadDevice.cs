using System;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the device that loads data from the tape.
    /// </summary>
    public interface ITapeLoadDevice : ICpuOperationBoundDevice, ISpectrumBoundDevice
    {
        /// <summary>
        /// This flag indicates if the tape is in load mode (EAR bit is set by the tape).
        /// </summary>
        bool IsInLoadMode { get; }

        /// <summary>
        /// Gets the EAR bit read from the tape.
        /// </summary>
        /// <param name="cpuTicks">CPU tacts count when the EAR bit is read.</param>
        /// <returns></returns>
        bool GetEarBit(long cpuTicks);

        /// <summary>
        /// Processes the the change of the MIC bit.
        /// </summary>
        /// <param name="micBit">New MIC bit value.</param>
        void ProcessMicBit(bool micBit);

        /// <summary>
        /// External entities can respond to the event when a fast load completed.
        /// </summary>
        event EventHandler LoadCompleted;

        /// <summary>
        /// Signs that the device entered LOAD mode.
        /// </summary>
        event EventHandler EnteredLoadMode;

        /// <summary>
        /// Signs that the device has just left LOAD mode.
        /// </summary>
        event EventHandler LeftLoadMode;
    }
}