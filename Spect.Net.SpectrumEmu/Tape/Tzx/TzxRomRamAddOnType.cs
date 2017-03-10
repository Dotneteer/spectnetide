namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Identifier ROM or RAM add-on types
    /// </summary>
    public enum TzxRomRamAddOnType : byte
    {
        SamRam = 0x00,
        MultifaceOne = 0x01,
        Multiface128K = 0x02,
        MultifaceP3 = 0x03,
        MultiPrint = 0x04,
        Mb02 = 0x05,
        SoftRom = 0x06,
        Ram1K = 0x07,
        Ram16K = 0x08,
        Ram48K = 0x09,
        Mem8To16KUsed = 0x0A
    }
}