namespace Spect.Net.SpectrumEmu.Abstraction.Devices.Tape
{
    /// <summary>
    /// This enum defines the MIC pulse types according to their widths.
    /// </summary>
    public enum MicPulseType : byte
    {
        /// <summary>No pulse information.</summary>
        None = 0,

        /// <summary>Too short to be a valid pulse.</summary>
        TooShort,

        /// <summary>Too long to be a valid pulse.</summary>
        TooLong,

        /// <summary>PILOT pulse (Length: 2168 tacts).</summary>
        Pilot,

        /// <summary>SYNC1 pulse (Length: 667 tacts).</summary>
        Sync1,

        /// <summary>SYNC2 pulse (Length: 735 tacts).</summary>
        Sync2,

        /// <summary>BIT0 pulse (Length: 855 tacts).</summary>
        Bit0,

        /// <summary>BIT1 pulse (Length: 1710 tacts).</summary>
        Bit1,

        /// <summary>TERM_SYNC pulse (Length: 947 tacts).</summary>
        TermSync
    }
}