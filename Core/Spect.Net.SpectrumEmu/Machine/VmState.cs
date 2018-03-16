namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class represents the states of the virtual machine as 
    /// managed by the SpectrumVmController
    /// </summary>
    public enum VmState
    {
        /// <summary>
        /// The virtual machine has not been created yet.
        /// </summary>
        None,

        /// <summary>
        /// The virtual machine is ready to run
        /// </summary>
        BeforeRun,

        /// <summary>
        /// The virtual machine is successfully started in the background
        /// </summary>
        Running,

        /// <summary>
        /// The pause request has been sent to the virtual machine, now
        /// the controller waits while the machine gets paused
        /// </summary>
        Pausing,

        /// <summary>
        /// The virtual machine has been paused
        /// </summary>
        Paused,

        /// <summary>
        /// The stop request has been sent to the virtual machine, now
        /// the controller waits while the machine gets paused
        /// </summary>
        Stopping,

        /// <summary>
        /// The virtual machine has been stopped
        /// </summary>
        Stopped
    }
}