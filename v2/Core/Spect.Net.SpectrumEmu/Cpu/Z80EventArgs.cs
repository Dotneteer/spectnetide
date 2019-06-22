using System;

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// Represents an event that is related to a Z80 operation code
    /// </summary>
    public class Z80EventArgs: EventArgs 
    {
        /// <summary>
        /// The CPU that raises the event
        /// </summary>
        public Z80Cpu Cpu { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public Z80EventArgs(Z80Cpu cpu)
        {
            Cpu = cpu;
        }
    }
}