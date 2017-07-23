namespace Spect.Net.Wpf.SpectrumControl
{
    /// <summary>
    /// Defines the possible states of the ZS Spectrum VM
    /// </summary>
    public enum SpectrumVmState
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