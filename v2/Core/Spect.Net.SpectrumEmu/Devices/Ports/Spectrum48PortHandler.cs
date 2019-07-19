using System;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class handles the standard spectrum port.
    /// </summary>
    public class Spectrum48PortHandler : PortHandlerBase
    {
        private const ushort PORTMASK = 0b0000_0000_0000_0001;
        private const ushort PORT = 0b0000_0000_0000_0000;

        private IZ80Cpu _cpu;
        private IScreenDevice _screenDevice;
        private IBeeperDevice _beeperDevice;
        private IKeyboardDevice _keyboardDevice;
        private ITapeLoadDevice _tapeDevice;
        private bool _isUla3;

        private bool _bit3LastValue;
        private bool _bit4LastValue;
        private long _bit4ChangedFrom0;
        private long _bit4ChangedFrom1;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        /// <param name="parent">Parent device</param>
        public Spectrum48PortHandler(IPortDevice parent) : base(parent, PORTMASK, PORT)
        {
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _isUla3 = hostVm.UlaIssue == "3";
            _cpu = hostVm.Cpu;
            _screenDevice = hostVm.ScreenDevice;
            _beeperDevice = hostVm.BeeperDevice;
            _keyboardDevice = hostVm.KeyboardDevice;
            _tapeDevice = hostVm.TapeDevice;
            _bit3LastValue = true;
            _bit4LastValue = true;
            _bit4ChangedFrom0 = 0;
            _bit4ChangedFrom1 = 0;
        }

        /// <summary>
        /// Handles the read from the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="readValue">The value read from the port</param>
        /// <returns>True, if read handled; otherwise, false</returns>
        public override bool HandleRead(ushort addr, out byte readValue)
        {
            readValue = _keyboardDevice.GetLineStatus((byte)(addr >> 8));
            if (_tapeDevice.IsInLoadMode)
            {
                var earBit = _tapeDevice.GetEarBit(_cpu.Tacts);
                if (!earBit)
                {
                    readValue = (byte)(readValue & 0b1011_1111);
                }
            }
            else
            {
                var bit4Sensed = _bit4LastValue;
                if (!bit4Sensed)
                {
                    var chargeTime = _bit4ChangedFrom1 - _bit4ChangedFrom0;
                    if (chargeTime > 0)
                    {
                        var delay = Math.Min(chargeTime * 4, 2800);
                        bit4Sensed = _cpu.Tacts - _bit4ChangedFrom1 < delay;
                    }
                }
                var bit6Value = (_bit3LastValue || bit4Sensed) ? 0b0100_0000 : 0x00;
                if (_bit3LastValue && !bit4Sensed && _isUla3)
                {
                    bit6Value = 0x00;
                }
                readValue = (byte)((readValue & 0b1011_1111) | bit6Value);
            }
            return true;
        }

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="writeValue">Value to write to the port</param>
        public override void HandleWrite(ushort addr, byte writeValue)
        {
            _screenDevice.BorderColor = writeValue & 0x07;
            _beeperDevice.ProcessEarBitValue(false, (writeValue & 0x10) != 0);
            _tapeDevice.ProcessMicBit((writeValue & 0x08) != 0);

            // --- Set the latest value of bit 3
            _bit3LastValue = (writeValue & 0x08) != 0;

            // --- Manage bit 4 value
            var curBit4 = (writeValue & 0x10) != 0;
            if (!_bit4LastValue && curBit4)
            {
                // --- Bit 4 goers from0 to 1
                _bit4ChangedFrom0 = _cpu.Tacts;
                _bit4LastValue = true;
            }
            else if (_bit4LastValue && !curBit4)
            {
                _bit4ChangedFrom1 = _cpu.Tacts;
                _bit4LastValue = false;
            }
        }
    }
}