namespace Spect.Net.SpectrumEmu.Abstraction.Cpu
{
    /// <summary>
    /// Operation Prefix Mode.
    /// </summary>
    public enum OpPrefixMode : byte
    {
        /// <summary>No operation prefix.</summary>
        None = 0,

        /// <summary>Extended mode (0xED prefix).</summary>
        Extended,

        /// <summary>Bit operations mode (0xCB prefix).</summary>
        Bit
    }
}