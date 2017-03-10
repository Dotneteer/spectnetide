namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Identified external storage types
    /// </summary>
    public enum TzxExternalStorageType : byte
    {
        ZxMicroDrive = 0x00,
        OpusDiscovery = 0x01,
        MgtDisciple = 0x02,
        MgtPlusD = 0x03,
        RobotronicsWafaDrive = 0x04,
        TrDosBetaDisk = 0x05,
        ByteDrive = 0x06,
        Watsford = 0x07,
        Fiz = 0x08,
        Radofin = 0x09,
        DidaktikDiskDrive = 0x0A,
        BsDos = 0x0B,
        ZxSpectrumP3DiskDrive = 0x0C,
        JloDiskInterface = 0x0D,
        TimexFdd3000 = 0x0E,
        ZebraDiskDrive = 0x0F,
        RamexMillenia = 0x10,
        Larken = 0x11,
        KempstonDiskInterface = 0x12,
        Sandy = 0x13,
        ZxSpectrumP3EHardDisk = 0x14,
        ZxAtaSp = 0x15,
        DivIde = 0x16,
        ZxCf = 0x17
    }
}