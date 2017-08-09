namespace Spect.Net.SpectrumEmu.Abstraction.Discovery
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStackDebugSupport
    {
        /// <summary>
        /// Records a stack pointer manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        void RecordStackPointerManipulationEvent(StackPointerManipulationEvent ev);

        /// <summary>
        /// Records a stack content manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        void RecordStackContentManipulationEvent(StackContentManipulationEvent ev);
    }
}