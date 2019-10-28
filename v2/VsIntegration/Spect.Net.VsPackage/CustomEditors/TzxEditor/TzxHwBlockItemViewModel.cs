using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using System;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Represents the view model of a TZX hadrware block.
    /// </summary>
    public class TzxHwBlockItemViewModel
    {
        public string Type { get; }
        public string Id { get; }
        public string TapeInfo { get; }

        public TzxHwBlockItemViewModel(TzxHwInfo hwInfo)
        {
            Type = Enum.GetName(typeof(TzxHwType), hwInfo.HwType);
            switch ((TzxHwType)hwInfo.HwType)
            {
                case TzxHwType.Computer:
                    Id = Enum.GetName(typeof(TzxComputerType), hwInfo.HwId);
                    break;

                case TzxHwType.ExternalStorage:
                    Id = Enum.GetName(typeof(TzxExternalStorageType), hwInfo.HwId);
                    break;

                case TzxHwType.RomOrRamTypeAddOn:
                    Id = Enum.GetName(typeof(TzxRomRamAddOnType), hwInfo.HwId);
                    break;

                case TzxHwType.SoundDevice:
                    Id = Enum.GetName(typeof(TzxSoundDeviceType), hwInfo.HwId);
                    break;

                case TzxHwType.Joystick:
                    Id = Enum.GetName(typeof(TzxJoystickType), hwInfo.HwId);
                    break;

                case TzxHwType.Mouse:
                    Id = Enum.GetName(typeof(TzxMouseType), hwInfo.HwId);
                    break;

                case TzxHwType.OtherController:
                    Id = Enum.GetName(typeof(TzxOtherControllerType), hwInfo.HwId);
                    break;

                case TzxHwType.SerialPort:
                    Id = Enum.GetName(typeof(TzxSerialPortType), hwInfo.HwId);
                    break;

                case TzxHwType.ParallelPort:
                    Id = Enum.GetName(typeof(TzxParallelPortType), hwInfo.HwId);
                    break;

                case TzxHwType.Printer:
                    Id = Enum.GetName(typeof(TzxPrinterType), hwInfo.HwId);
                    break;

                case TzxHwType.Modem:
                    Id = Enum.GetName(typeof(TzxModemTypes), hwInfo.HwId);
                    break;

                case TzxHwType.Digitizer:
                    Id = Enum.GetName(typeof(TzxDigitizerType), hwInfo.HwId);
                    break;

                case TzxHwType.NetworkAdapter:
                    Id = Enum.GetName(typeof(TzxNetworkAdapterType), hwInfo.HwId);
                    break;

                case TzxHwType.Keyboard:
                    Id = Enum.GetName(typeof(TzxKeyboardType), hwInfo.HwId);
                    break;

                case TzxHwType.AdOrDaConverter:
                    Id = Enum.GetName(typeof(TzxAdOrDaConverterType), hwInfo.HwId);
                    break;

                case TzxHwType.EpromProgrammer:
                    Id = Enum.GetName(typeof(TzxEpromProgrammerType), hwInfo.HwId);
                    break;

                case TzxHwType.Graphics:
                    Id = Enum.GetName(typeof(TzxGraphicsType), hwInfo.HwId);
                    break;
                default:
                    Id = "Unknown";
                    break;
            }
            switch (hwInfo.TapeInfo)
            {
                case 0:
                    TapeInfo = "(Works with this hardware)";
                    break;
                case 1:
                    TapeInfo = "(Uses this hardware)";
                    break;
                case 2:
                    TapeInfo = "(Does not use this hardware)";
                    break;
                default:
                    TapeInfo = "(Does not work with this hardware)";
                    break;
            }
        }
    }
}
