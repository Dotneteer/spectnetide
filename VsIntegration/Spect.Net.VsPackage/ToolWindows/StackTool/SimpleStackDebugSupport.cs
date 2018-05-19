using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Utility;

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
            = new LruList<StackPointerManipulationEvent>();

        /// <summary>
        /// Stack content events
        /// </summary>
        public Dictionary<ushort, StackContentManipulationEvent>  StackContentEvents { get; private set; }
            = new Dictionary<ushort, StackContentManipulationEvent>();

        /// <summary>
        /// Resets the debug support
        /// </summary>
        public void Reset()
        {
            var package = SpectNetPackage.Default;
            StackPointerEvents = new LruList<StackPointerManipulationEvent>(
                package.Options.StackPointerEvents);
            StackContentEvents = new Dictionary<ushort, StackContentManipulationEvent>();
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
            // --- Do not trace POP events
            if (ev.Content != null)
            {
                StackContentEvents[ev.SpValue] = ev;
            }
        }
    }
}