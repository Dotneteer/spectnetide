using System;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// Represents a selected disassembly item
    /// </summary>
    public class DisassemblyItemSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// The selected item
        /// </summary>
        public DisassemblyItemViewModel Selected { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public DisassemblyItemSelectedEventArgs(DisassemblyItemViewModel selected)
        {
            Selected = selected;
        }
    }
}
