namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class represents the states of the virtual machine as 
    /// managed by the SpectrumVmController
    /// </summary>
    public enum VmState
    {
        /// <summary>
        /// The virtual machine has just been created, but has not run yet
        /// </summary>
        None,

        /// <summary>
        /// The virtual machine is successfully started in the background
        /// </summary>
        Running,

        /// <summary>
        /// The pause request has been sent to the virtual machine, 
        /// now it prepares to get paused
        /// </summary>
        Pausing,

        /// <summary>
        /// The virtual machine has been paused
        /// </summary>
        Paused,

        /// <summary>
        /// The stop request has been sent to the virtual machine, 
        /// now it prepares to get stopped
        /// </summary>
        Stopping,

        /// <summary>
        /// The virtual machine has been stopped
        /// </summary>
        Stopped
    }
}