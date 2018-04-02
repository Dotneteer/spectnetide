using System;

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// This class represent the arguments of an address-related event
    /// </summary>
    public class AddressEventArgs: EventArgs
    {
        /// <summary>
        /// Address read
        /// </summary>
        public ushort Address { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.EventArgs" /> class.</summary>
        public AddressEventArgs(ushort address)
        {
            Address = address;
        }
    }
}