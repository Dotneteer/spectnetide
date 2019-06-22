namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// This class represent the arguments of a write event
    /// </summary>
    public class AddressAndDataEventArgs : AddressEventArgs
    {
        /// <summary>
        /// Data written
        /// </summary>
        public byte Data { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.EventArgs" /> class.</summary>
        public AddressAndDataEventArgs(ushort address, byte data): base(address)
        {
            Data = data;
        }
    }
}