using System;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// These event arguments represent that the top address of a disassembly 
    /// list has been changed
    /// </summary>
    public class TopAddressChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Old address value
        /// </summary>
        public ushort? OldAddress { get; }

        /// <summary>
        /// New address value
        /// </summary>
        public ushort? NewAddress { get; }

        public TopAddressChangedEventArgs(ushort? oldAddress, ushort? newAddress)
        {
            OldAddress = oldAddress;
            NewAddress = newAddress;
        }
    }
}
