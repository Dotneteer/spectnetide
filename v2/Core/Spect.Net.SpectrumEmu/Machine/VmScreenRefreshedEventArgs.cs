using System;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// Arguments for the virtual machine screen refreshed event
    /// </summary>
    public class VmScreenRefreshedEventArgs: EventArgs
    {
        /// <summary>
        /// Screen data buffer
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public VmScreenRefreshedEventArgs(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}