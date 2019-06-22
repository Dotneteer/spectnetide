namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// Identified sound device types
    /// </summary>
    public enum TzxSoundDeviceType : byte
    {
        ClassicAy = 0x00,
        FullerBox = 0x01,
        CurrahMicroSpeech = 0x02,
        SpectDrum = 0x03,
        MelodikAyAcbStereo = 0x04,
        AyAbcStereo = 0x05,
        RamMusinMachine = 0x06,
        Covox = 0x07,
        GeneralSound = 0x08,
        IntecEdiB8001 = 0x09,
        ZonXAy = 0x0A,
        QuickSilvaAy = 0x0B,
        JupiterAce = 0x0C
    }
}