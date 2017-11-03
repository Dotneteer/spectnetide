namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This interface provides a way the virtual machine can notify its
    /// controller about its internal events
    /// </summary>
    public interface IVmControlLink
    {
        /// <summary>
        /// The vm notifies the controller when its execution cycle is activated on
        /// a background thread.
        /// </summary>
        void ExecutionCycleStarted();
    }
}