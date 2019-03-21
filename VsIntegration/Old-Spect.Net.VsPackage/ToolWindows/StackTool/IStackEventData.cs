using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Utility;

namespace Spect.Net.VsPackage.ToolWindows.StackTool
{
    /// <summary>
    /// This interface defines the access methods to stack event data.
    /// </summary>
    public interface IStackEventData
    {
        /// <summary>
        /// Stack Pointer events
        /// </summary>
        LruList<StackPointerManipulationEvent> StackPointerEvents { get; }

        /// <summary>
        /// Stack content events
        /// </summary>
        Dictionary<ushort, StackContentManipulationEvent> StackContentEvents { get; }
    }
}