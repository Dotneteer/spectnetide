namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This enumeration represents the state of a ZX Spectrum virtual machine
    /// </summary>
    public enum VmState
    {
        /// <summary>The machine is turned off</summary>
        None,

        /// <summary>The machine is starting (after On, Paused, or Stopped states)</summary>
        Starting,

        /// <summary>The machine has successfully started (after Starting)</summary>
        Running,

        /// <summary>The machine is getting paused (after Running)</summary>
        Pausing,

        /// <summary>The machine has successfully paused (after Running)</summary>
        Paused,

        /// <summary>The machine is getting stopped (after Running)</summary>
        Stopping,

        /// <summary>The machine has successfully stopped (after Stopping)</summary>
        Stopped
    }
}