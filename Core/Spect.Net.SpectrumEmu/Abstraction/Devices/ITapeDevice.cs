using System;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the device that is responsible for
    /// displaying the border color
    /// </summary>
    public interface ITapeDevice : ISpectrumBoundDevice
    {
        /// <summary>
        /// Gets the EAR bit read from the tape
        /// </summary>
        /// <param name="cpuTicks"></param>
        /// <returns></returns>
        bool GetEarBit(long cpuTicks);

        /// <summary>
        /// Sets the current tape mode according to the current PC register
        /// and the MIC bit state
        /// </summary>
        void SetTapeMode();

        /// <summary>
        /// Processes the the change of the MIC bit
        /// </summary>
        /// <param name="micBit"></param>
        void ProcessMicBit(bool micBit);

        /// <summary>
        /// External entities can respond to the event when a fast load completed.
        /// </summary>
        event EventHandler LoadCompleted;

        /// <summary>
        /// Signs that the device entered LOAD mode
        /// </summary>
        event EventHandler EnteredLoadMode;

        /// <summary>
        /// Signs that the device has just left LOAD mode
        /// </summary>
        event EventHandler LeftLoadMode;

        /// <summary>
        /// Signs that the device entered SAVE mode
        /// </summary>
        event EventHandler EnteredSaveMode;

        /// <summary>
        /// Signs that the device has just left SAVE mode
        /// </summary>
        event EventHandler LeftSaveMode;
    }
}