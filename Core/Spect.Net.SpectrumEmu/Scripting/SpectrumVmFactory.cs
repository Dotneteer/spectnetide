// ReSharper disable InconsistentNaming
namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// This class provides methods that create Spectrum virtual machine instances.
    /// </summary>
    public static class SpectrumVmFactory
    {
        /// <summary>
        /// Creates a Spectrum instance with the specified model and edition name
        /// </summary>
        /// <param name="modelName">Spectrum model name</param>
        /// <param name="editionName">Edition name</param>
        /// <returns>The newly create Spectrum machine</returns>
        public static Spectrum Create(string modelName, string editionName)
        {
            return null;
        }

        /// <summary>
        /// Creates a Spectrum 48K instance PAL edition
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static Spectrum CreateSpectrum48Pal()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance PAL edition with turbo mode (2xCPU)
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static Spectrum CreateSpectrum48PalTurbo()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL_2_X);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance NTSC edition
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static Spectrum CreateSpectrum48Ntsc()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.NTSC);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance NTSC edition with turbo mode
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static Spectrum CreateSpectrum48NtscTurbo()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.NTSC_2_X);
        }

        /// <summary>
        /// Creates a Spectrum 128K instance
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static Spectrum CreateSpectrum128()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_128, SpectrumModels.PAL);
        }

        /// <summary>
        /// Creates a Spectrum +3E instance
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static Spectrum CreateSpectrumP3E()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_P3_E, SpectrumModels.PAL);
        }

        #region Implementation methods

        #endregion
    }

    /// <summary>
    /// Represents the collection of breakpoints
    /// </summary>
    public sealed class CodeBreakpoints
    {
    }

    /// <summary>
    /// Represents the sound samples of the current frame
    /// </summary>
    public sealed class SoundSamples
    {
    }

    /// <summary>
    /// Represents the beeper samples of the current frame
    /// </summary>
    public sealed class BeeperSamples
    {
    }

    /// <summary>
    /// Gets the current screen rendering status of the machine.
    /// </summary>
    public sealed class ScreenRenderingStatus
    {
    }

    /// <summary>
    /// Represents the current screen's pixels, including the border
    /// </summary>
    public sealed class ScreenBitmap
    {
    }

    /// <summary>
    /// Represents the screen rendering table of the machine
    /// </summary>
    public sealed class ScreenRenderingTable
    {
    }

    /// <summary>
    /// Represents the keyboard of a Spectrum machine
    /// </summary>
    public sealed class KeyboardEmulator
    {
    }

    /// <summary>
    /// Represents a single ROM page of the machine
    /// </summary>
    public sealed class RomPage
    {
    }

    /// <summary>
    /// Represents a single MemoryBank of the machine
    /// </summary>
    public sealed class MemoryBank
    {
    }

    /// <summary>
    /// This class allows to query paging information about the memory
    /// </summary>
    public sealed class MemoryPagingInfo
    {
    }

    /// <summary>
    /// Represents tracking information for the memory state
    /// </summary>
    /// <remarks>
    /// (Such as execution, read, write)
    /// </remarks>
    public sealed class AddressTrackingState
    {
    }

    /// <summary>
    /// This class provides access to the memory contents of the Spectrum virtual 
    /// machine, including reading and writing
    /// </summary>
    public sealed class SpectrumMemoryContents
    {
    }

}