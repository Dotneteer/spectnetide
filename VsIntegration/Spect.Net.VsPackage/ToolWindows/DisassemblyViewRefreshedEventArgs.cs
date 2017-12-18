using System;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class represents the argument of the event when an address-aware
    /// view has been refreshed.
    /// </summary>
    public class DisassemblyViewRefreshedEventArgs: EventArgs
    {
        /// <summary>
        /// Optional memory address to refresh;
        /// </summary>
        public ushort? Address { get; }

        /// <summary>
        /// Action to invoke before refreshing the disassembly view
        /// </summary>
        public Action RefreshAction { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public DisassemblyViewRefreshedEventArgs(ushort? address = null, Action refreshAction = null)
        {
            Address = address;
            RefreshAction = refreshAction;
        }
    }
}