namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This enumeration represents the contention type of memory
    /// </summary>
    public enum MemoryContentionType
    {
        /// <summary>No contended memory</summary>
        None,

        /// <summary>ULA-type memory contention</summary>
        Ula,

        /// <summary>Gate-array-type memory contention</summary>
        GateArray,

        /// <summary>Spectrum Next type memory contention</summary>
        Next
    }
}