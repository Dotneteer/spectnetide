using System;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Represents the hardware types that can be defined
    /// </summary>
    public enum TzxHwType: byte
    {
        Computer = 0x00,
        ExternalStorage = 0x01,
        RomOrRamTypeAddOn = 0x02,
        SoundDevice = 0x03,
        JoyStick = 0x04,
        Mouse = 0x05,
        OtherController = 0x06,
        SerialPort = 0x07,
        ParallelPort = 0x08,
        Printer = 0x09,
        Modem = 0x0A,
        Digitizer = 0x0B,
        NetworkAdapter = 0x0C,
        Keyboard = 0x0D,
        AdOrDaConverter = 0x0E,
        EpromProgrammer = 0x0F,
        Graphics = 0x10
    }
}