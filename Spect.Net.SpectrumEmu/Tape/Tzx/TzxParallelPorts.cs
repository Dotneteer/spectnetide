namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Identified parallel port types
    /// </summary>
    public enum TzxParallelPorts : byte
    {
        KempstonS = 0x00,
        KempstonE = 0x01,
        ZxSpectrum3P = 0x02,
        Tasman = 0x03,
        DkTronics = 0x04,
        Hilderbay = 0x05,
        InesPrinterface = 0x06,
        ZxLprintInterface3 = 0x07,
        MultiPrint = 0x08,
        OpusDiscovery = 0x09,
        Standard8255 = 0x0A
    }
}