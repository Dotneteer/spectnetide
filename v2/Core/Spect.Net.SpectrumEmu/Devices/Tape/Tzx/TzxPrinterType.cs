﻿namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// Identified printer types
    /// </summary>
    public enum TzxPrinterType : byte
    {
        ZxPrinter = 0x00,
        GenericPrinter = 0x01,
        EpsonCompatible = 0x02
    }
}