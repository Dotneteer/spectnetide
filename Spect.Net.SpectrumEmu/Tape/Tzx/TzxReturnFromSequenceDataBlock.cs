namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This block indicates the end of the Called Sequence.
    /// The next block played will be the block after the last 
    /// CALL block
    /// </summary>
    public class TzxReturnFromSequenceDataBlock : TzxBodylessDataBlockBase
    {
        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x27;
    }
}