using Spect.Net.SpectrumEmu.Devices.Tape;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface defines the operations that support 
    /// the testing of a tape device
    /// </summary>
    public interface ITapeDeviceTestSupport
    {
        /// <summary>
        /// The current operation mode of the tape
        /// </summary>
        TapeOperationMode CurrentMode { get; }

        /// <summary>
        /// The object that can playback tape content
        /// </summary>
        CommonTapeFilePlayer TapeFilePlayer { get; }

        /// <summary>
        /// The CPU tact of the last MIC bit activity
        /// </summary>
        long LastMicBitActivityTact { get; }

        /// <summary>
        /// Gets the current state of the MIC bit
        /// </summary>
        bool MicBitState { get; }

        /// <summary>
        /// The current phase of the SAVE operation
        /// </summary>
        SavePhase SavePhase { get; }

        /// <summary>
        /// The number of PILOT pulses emitted
        /// </summary>
        int PilotPulseCount { get; }

        /// <summary>
        /// The bit offset within a byte when data is emitted
        /// </summary>
        int BitOffset { get; }

        /// <summary>
        /// The current data byte emitted
        /// </summary>
        byte DataByte { get; }

        /// <summary>
        /// The number of bytes emitted in the current data block
        /// </summary>
        int DataLength { get; }

        /// <summary>
        /// The buffer that holds the emitted data block
        /// </summary>
        byte[] DataBuffer { get; }

        /// <summary>
        /// The previous data pulse emitted
        /// </summary>
        MicPulseType PrevDataPulse { get; }

        /// <summary>
        /// The number of data blocks saved
        /// </summary>
        int DataBlockCount { get; }
    }
}