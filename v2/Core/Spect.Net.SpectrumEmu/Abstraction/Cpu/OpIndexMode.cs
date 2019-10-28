// ReSharper disable InconsistentNaming
namespace Spect.Net.SpectrumEmu.Abstraction.Cpu
{
    /// <summary>
    /// Signs if the current instruction uses any of the indexed address modes.
    /// </summary>
    public enum OpIndexMode
    {
        /// <summary>Indexed address mode is not used.</summary>
        None = 0,

        /// <summary>Indexed address with IX register.</summary>
        IX,

        /// <summary>Indexed address with IY register.</summary>
        IY
    }
}