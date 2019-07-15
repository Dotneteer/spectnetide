namespace Spect.Net.SpectrumEmu.Abstraction.Devices.Tape
{
    /// <summary>
    /// This enumeration defines the phases of the SAVE operation.
    /// </summary>
    public enum SavePhase : byte
    {
        /// <summary>No SAVE operation is in progress.</summary>
        None = 0,

        /// <summary>Emitting PILOT impulses.</summary>
        Pilot,

        /// <summary>Emitting SYNC1 impulse.</summary>
        Sync1,

        /// <summary>Emitting SYNC2 impulse.</summary>
        Sync2,

        /// <summary>Emitting BIT0/BIT1 impulses.</summary>
        Data,

        /// <summary>Unexpected pulse detected.</summary>
        Error
    }
}