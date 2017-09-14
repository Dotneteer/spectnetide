namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// It means that the utility should jump back to the start 
    /// of the loop if it hasn't been run for the specified number 
    /// of times.
    /// </summary>
    public class TzxLoopEndDataBlock : TzxBodylessDataBlockBase
    {
        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x25;
    }
}