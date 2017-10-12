using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Utility;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.StackTool
{
    /// <summary>
    /// This class provides stack debug support
    /// </summary>
    public class SimpleStackDebugSupport: IStackDebugSupport, IStackEventData
    {
        /// <summary>
        /// Stack Pointer events
        /// </summary>
        public LruList<StackPointerManipulationEvent> StackPointerEvents { get; private set; }

        /// <summary>
        /// Stack content events
        /// </summary>
        public LruList<StackContentManipulationEvent> StackContentEvents { get; private set; }

        /// <summary>
        /// Resets the debug support
        /// </summary>
        public void Reset()
        {
            var package = VsxPackage.GetPackage<SpectNetPackage>();
            StackPointerEvents = new LruList<StackPointerManipulationEvent>(
                package.Options.StackPointerEvents);
            StackContentEvents = new LruList<StackContentManipulationEvent>(
                package.Options.StackManipulationEvents);
        }

        /// <summary>
        /// Records a stack pointer manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordStackPointerManipulationEvent(StackPointerManipulationEvent ev)
        {
            StackPointerEvents.Add(ev);
        }

        /// <summary>
        /// Records a stack content manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordStackContentManipulationEvent(StackContentManipulationEvent ev)
        {
            StackContentEvents.Add(ev);
        }
    }
}