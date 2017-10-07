namespace Spect.Net.Wpf.Mvvm
{
    /// <summary>
    /// Defines the possible states of the Zx Spectrum virtual machine
    /// </summary>
    public enum VmState
    {
        /// <summary>
        /// The machine is not initialized
        /// </summary>
        None = 0,

        /// <summary>
        /// The machine is stopped; it can be started
        /// </summary>
        Stopped,

        /// <summary>
        /// The machine is running; it can be stopped or paused
        /// </summary>
        Running,

        /// <summary>
        /// The machine is paused and can be resumed
        /// </summary>
        Paused
    }
}