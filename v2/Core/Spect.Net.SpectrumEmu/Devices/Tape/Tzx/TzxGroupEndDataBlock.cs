namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// This indicates the end of a group. This block has no body.
    /// </summary>
    public class TzxGroupEndDataBlock : TzxBodylessDataBlockBase
    {
        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x22;
    }
}