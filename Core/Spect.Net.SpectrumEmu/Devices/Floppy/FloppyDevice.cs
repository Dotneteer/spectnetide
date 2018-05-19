using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Floppy
{
    /// <summary>
    /// Floppy device emulation
    /// </summary>
    public class FloppyDevice: IFloppyDevice
    {
        private bool _acceptCommand;
        private Command _lastCommand;
        private int _commandBytesReceived;
        private int _resultBytesIndex;
        private byte _st0;
        private byte _st1;
        private byte _st2;
        private byte _st3;
        private byte[] _commandResult;
        private byte _execStatus;

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
        }

        /// <summary>
        /// Stepping Rate for the FDD
        /// </summary>
        public byte StepRateTime { get; private set; }

        /// <summary>
        /// Head Unload Time after read or write operation has occurred
        /// </summary>
        public byte HeadUnloadTime { get; private set; }

        /// <summary>
        /// Head Load Time in the FDD
        /// </summary>
        public byte HeadLoadTime { get; private set; }

        /// <summary>
        /// Non-DMA Mode
        /// </summary>
        public bool NonDmaMode { get; private set; }

        /// <summary>
        /// Represents head 0 or 1
        /// </summary>
        public int HeadNumber { get; private set; }

        /// <summary>
        /// Represents the selected drive
        /// </summary>
        public int SelectedDrive { get; private set; }

        /// <summary>
        /// Gets the current cylinder values
        /// </summary>
        public byte[] CurrentCylinders { get; } = new byte[4];

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            MainStatusRegister = 0b1000_0000;
            _acceptCommand = true;
            _lastCommand = Command.None;
            _commandBytesReceived = 0;
            _st0 = _st1 = _st2 = _st3 = 0x00;
            for (var i = 0; i < 4; i++)
            {
                CurrentCylinders[i] = 0x00;
            }
            _execStatus = 0x80;
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public IDeviceState GetState() => null;

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public void RestoreState(IDeviceState state)
        {
        }

        /// <summary>
        /// The value of the main status register
        /// </summary>
        public byte MainStatusRegister { get; set; }

        /// <summary>
        /// Sets the flag that indicates if an FDD is busy
        /// </summary>
        /// <param name="fdd">FDD index (0..3)</param>
        /// <param name="busy">True: busy; false: accepts commands</param>
        public void SetFddBusy(int fdd, bool busy)
        {
            if (busy)
            {
                MainStatusRegister |= (byte) (1 << (fdd & 0x03));
            }
            else
            {
                MainStatusRegister &= (byte) ~(fdd & 0x03);
            }
        }

        /// <summary>
        /// Sets the flag that indicates if the controller is busy
        /// </summary>
        /// <param name="busy">True: busy; false: accepts commands</param>
        public void SetFdcBusy(bool busy)
        {
            if (busy)
            {
                MainStatusRegister |= 0b0001_0000;
            }
            else
            {
                MainStatusRegister &= 0b1110_1111;
            }
        }

        /// <summary>
        /// Sets the flag that indicates executin mode
        /// </summary>
        /// <param name="exm">Execution mode flag</param>
        public void SetExecutionMode(bool exm)
        {
            if (exm)
            {
                MainStatusRegister |= 0b0010_0000;
            }
            else
            {
                MainStatusRegister &= 0b1101_1111;
            }
        }

        /// <summary>
        /// Sets the Data Input/Output (DIO) flag
        /// </summary>
        /// <param name="dio">DIO flag</param>
        public void SetDioFlag(bool dio)
        {
            if (dio)
            {
                MainStatusRegister |= 0b0100_0000;
            }
            else
            {
                MainStatusRegister &= 0b1011_1111;
            }
        }

        /// <summary>
        /// Sets the Request for Master (RQM) flag
        /// </summary>
        /// <param name="rqm">RQM flag</param>
        public void SetRqmFlag(bool rqm)
        {
            if (rqm)
            {
                MainStatusRegister |= 0b1000_0000;
            }
            else
            {
                MainStatusRegister &= 0b0111_1111;
            }
        }

        /// <summary>
        /// Indicates if I/O direction is out
        /// </summary>
        public bool DirectionOut => (MainStatusRegister & 0b0100_0000) != 0;

        /// <summary>
        /// Sends a command byte to the controller
        /// </summary>
        /// <param name="cmd">Command byte</param>
        public void WriteCommandByte(byte cmd)
        {
            if (_acceptCommand)
            {
                if (cmd == 0x03)
                {
                    _lastCommand = Command.Specify;
                }
                else if (cmd == 0x04)
                {
                    _lastCommand = Command.SenseDriveStatus;
                }
                else if (cmd == 0x07)
                {
                    _lastCommand = Command.Recalibrate;
                }
                else if (cmd == 0x08)
                {
                    _lastCommand = Command.SenseInterruptStatus;
                }
                else if (cmd == 0x0F)
                {
                    _lastCommand = Command.Seek;
                }
                else if ((cmd & 0xBF) == 0x0A)
                {
                    _lastCommand = Command.ReadId;
                }
                else
                {
                    var x = 1;
                }
                _acceptCommand = false;
                _commandBytesReceived = 0;
                return;
            }

            switch (_lastCommand)
            {
                case Command.Specify:
                    if (_commandBytesReceived == 0)
                    {
                        StepRateTime = (byte)(cmd >> 4);
                        HeadUnloadTime = (byte)(cmd & 0x0F);
                        _commandBytesReceived++;
                    }
                    else
                    {
                        HeadLoadTime = (byte) (cmd >> 1);
                        NonDmaMode = (cmd & 0x01) != 0;
                        _acceptCommand = true;
                    }
                    break;

                case Command.SenseDriveStatus:
                    HeadNumber = (cmd >> 2) & 0x01;
                    SelectedDrive = cmd & 0x03;
                    _st3 = (byte)(0b0011_1000 | (HeadNumber == 0 ? 0x04 : 0x00) | (SelectedDrive & 0x03));
                    SendResult(new [] { _st3 });
                    break;

                case Command.Recalibrate:
                    SelectedDrive = cmd & 0x03;
                    CurrentCylinders[SelectedDrive] = 0x00;
                    _acceptCommand = true;
                    break;

                case Command.SenseInterruptStatus:
                    _st0 = (byte)(_execStatus |(HeadNumber == 0 ? 0x04 : 0x00) | (SelectedDrive & 0x03));
                    SendResult(new[] { _st0, CurrentCylinders[SelectedDrive] });
                    _execStatus = 0x80;
                    break;

                case Command.Seek:
                    if (_commandBytesReceived == 0)
                    {
                        HeadNumber = (cmd >> 2) & 0x01;
                        SelectedDrive = cmd & 0x03;
                        _commandBytesReceived++;
                    }
                    else
                    {
                        CurrentCylinders[SelectedDrive] = (byte)(cmd > 79 ? 79 : cmd);
                        _execStatus = 0b0010_0000;
                        _acceptCommand = true;
                    }
                    break;

                default:
                    break;
            }

            if (_acceptCommand)
            {
                _commandBytesReceived = 0;
            }
        }

        /// <summary>
        /// Reads a result byte
        /// </summary>
        /// <returns>Result byte received</returns>
        public byte ReadResultByte()
        {
            if (DirectionOut)
            {
                if (_commandResult != null && _resultBytesIndex < _commandResult.Length)
                {
                    var result = _commandResult[_resultBytesIndex++];
                    if (_resultBytesIndex >= _commandResult.Length)
                    {
                        SetDioFlag(false);
                        _resultBytesIndex = 0;
                        _commandResult = null;
                    }
                    return result;
                }
            }
            return 0xFF;
        }

        /// <summary>
        /// Sets the specified result to be sent
        /// </summary>
        /// <param name="result"></param>
        private void SendResult(byte[] result)
        {
            _acceptCommand = true;
            _resultBytesIndex = 0;
            SetDioFlag(true);
            _commandResult = result;
        }

        private enum Command: byte
        {
            None,
            ReadData,
            ReadDeletedData,
            WriteData,
            WriteDeletedData,
            ReadDiagnostic,
            ReadId,
            FormatTrack,
            ScanEqual,
            ScanLowOrEqual,
            ScanHighOrEqual,
            Recalibrate,
            SenseInterruptStatus,
            Specify,
            SenseDriveStatus,
            Version,
            Seek,
            Invalid
        }
    }
}