namespace Spect.Net.SpectrumEmu.Abstraction.Discovery
{
    /// <summary>
    /// This interface provides information that support debugging branching statements
    /// </summary>
    public interface IBranchDebugSupport
    {
        /// <summary>
        /// Records a branching event
        /// </summary>
        /// <param name="ev">Event information</param>
        void RecordBranchEvent(BranchEvent ev);
    }
}