using System;
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
        private int _dataResultBytesIndex;
        private byte _st0;
        private byte _st1;
        private byte _st2;
        private byte _st3;
        private byte[] _parameters;
        private byte[] _commandResult;
        private byte[] _dataToWrite;
        private byte[] _dataResult;
        private byte _execStatus;
        private bool _multiTrack;
        private bool _skipDeletedData;

        private VirtualFloppyFile _floppyFile;

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; set; }

        /// <summary>
        /// Optional logger
        /// </summary>
        public IFloppyDeviceLogger FloppyLogger { get; set; }

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
        /// Represents the selected drive
        /// </summary>
        public int SelectedDrive { get; private set; }

        /// <summary>
        /// Gets the current head values
        /// </summary>
        public byte[] Heads { get; } = new byte[4];

        /// <summary>
        /// Gets the current track values
        /// </summary>
        public byte[] CurrentTracks { get; } = new byte[4];

        /// <summary>
        /// Gets the current sector values
        /// </summary>
        public byte[] CurrentSectors { get; } = new byte[4];

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
                CurrentTracks[i] = 0x00;
                CurrentSectors[i] = 0x00;
            }
            _execStatus = 0xC0;
            _multiTrack = false;
            _skipDeletedData = false;
            _floppyFile = VirtualFloppyFile.OpenOrCreateFloppyFile(@"C:\Temp\FloppyFile\vfddFile.vfdd");
            FloppyLogger?.Trace("Floppy reset");
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
                    _commandBytesReceived = 0;
                    _parameters = new byte[2];
                }
                else if (cmd == 0x04)
                {
                    _lastCommand = Command.SenseDriveStatus;
                    _commandBytesReceived = 0;
                    _parameters = new byte[1];
                }
                else if (cmd == 0x07)
                {
                    _lastCommand = Command.Recalibrate;
                    _commandBytesReceived = 0;
                    _parameters = new byte[1];
                }
                else if (cmd == 0x08)
                {
                    // --- Sense interrupt status, no more command bytes
                    FloppyLogger?.CommandReceived(cmd);
                    _st0 = (byte)(_execStatus | (Heads[SelectedDrive] == 0 ? 0x00 : 0x04) | (SelectedDrive & 0x03));
                    SendResult(new[] { _st0, CurrentTracks[SelectedDrive] });
                    _execStatus = 0x80;
                    return;
                }
                else if (cmd == 0x0F)
                {
                    _lastCommand = Command.Seek;
                    _commandBytesReceived = 0;
                    _parameters = new byte[2];
                }
                else if ((cmd & 0xBF) == 0x0A)
                {
                    _lastCommand = Command.ReadId;
                    _commandBytesReceived = 0;
                    _parameters = new byte[1];
                }
                else if ((cmd & 0x1F) == 0x06)
                {
                    _lastCommand = Command.ReadData;
                    _multiTrack = (cmd & 0x80) != 0;
                    _skipDeletedData = (cmd & 0x20) != 0;
                    _commandBytesReceived = 0;
                    _parameters = new byte[8];
                }
                else if ((cmd & 0xBD) == 0x0D)
                {
                    _lastCommand = Command.FormatTrack;
                    _commandBytesReceived = 0;
                    _parameters = new byte[5];
                }
                else if ((cmd & 0x3F) == 0x05)
                {
                    _lastCommand = Command.WriteData;
                    _multiTrack = (cmd & 0x80) != 0;
                    _commandBytesReceived = 0;
                    _parameters = new byte[8];
                }
                else
                {
                    _lastCommand = Command.None;
                }
                FloppyLogger?.CommandReceived(cmd);
                _acceptCommand = false;
                _commandBytesReceived = 0;
                return;
            }

            switch (_lastCommand)
            {
                case Command.Specify:
                    if (_commandBytesReceived == 0)
                    {
                        _parameters[0] = cmd;
                        StepRateTime = (byte)(cmd >> 4);
                        HeadUnloadTime = (byte)(cmd & 0x0F);
                        _commandBytesReceived++;
                    }
                    else
                    {
                        _parameters[1] = cmd;
                        FloppyLogger?.CommandParamsReceived(_parameters);
                        HeadLoadTime = (byte) (cmd >> 1);
                        NonDmaMode = (cmd & 0x01) != 0;
                        _acceptCommand = true;
                    }
                    break;

                case Command.SenseDriveStatus:
                    _parameters[0] = cmd;
                    FloppyLogger?.CommandParamsReceived(_parameters);
                    SelectedDrive = cmd & 0x03;
                    Heads[SelectedDrive] = (byte)((cmd >> 2) & 0x01);
                    _st3 = (byte)(0b0011_1000 | (Heads[SelectedDrive] == 0 ? 0x00 : 0x04) | (SelectedDrive & 0x03));
                    var result = new[] {_st3};
                    SendResult(result);
                    break;

                case Command.Recalibrate:
                    _parameters[0] = cmd;
                    FloppyLogger?.CommandParamsReceived(_parameters);
                    SelectedDrive = cmd & 0x03;
                    CurrentTracks[SelectedDrive] = 0x00;
                    _acceptCommand = true;
                    _execStatus = 0b0010_0000;
                    break;

                case Command.Seek:
                    if (_commandBytesReceived == 0)
                    {
                        _parameters[0] = cmd;
                        SelectedDrive = cmd & 0x03;
                        Heads[SelectedDrive] = (byte)((cmd >> 2) & 0x01);
                        _commandBytesReceived++;
                    }
                    else
                    {
                        _parameters[1] = cmd;
                        FloppyLogger?.CommandParamsReceived(_parameters);
                        CurrentTracks[SelectedDrive] = (byte)(cmd > 79 ? 79 : cmd);
                        CurrentSectors[SelectedDrive] = 1;
                        _execStatus = 0b0010_0000;
                        _acceptCommand = true;
                    }
                    break;

                case Command.ReadId:
                    _parameters[0] = cmd;
                    FloppyLogger?.CommandParamsReceived(_parameters);
                    SelectedDrive = cmd & 0x03;
                    Heads[SelectedDrive] = (byte)((cmd >> 2) & 0x01);
                    _commandBytesReceived++;
                    _execStatus = 0x00;
                    _st0 = (byte)(_execStatus | (Heads[SelectedDrive] == 0 ? 0x00 : 0x04) | (SelectedDrive & 0x03));
                    _st1 = (byte)(CurrentSectors[SelectedDrive] > 9 ? 0x80 : 0x00);
                    _st2 = 0x00;
                    SendResult(new byte[]
                    {
                        _st0, _st1, _st2,
                        CurrentTracks[SelectedDrive],
                        Heads[SelectedDrive],
                        0x01,
                        0x02
                    }, _dataResult);
                    break;

                case Command.ReadData:
                    if (_commandBytesReceived < 8)
                    {
                        _parameters[_commandBytesReceived++] = cmd;
                    }
                    if (_commandBytesReceived == 8)
                    {
                        FloppyLogger?.CommandParamsReceived(_parameters);
                        SelectedDrive = _parameters[0] & 0x03;
                        Heads[SelectedDrive] = (byte)((_parameters[0] >> 2) & 0x01);
                        CurrentSectors[SelectedDrive] = _parameters[3];

                        // --- Check if ID is the same as the current sector
                        if (_parameters[1] != CurrentTracks[SelectedDrive]
                            || _parameters[2] != Heads[SelectedDrive])
                        {
                            _dataResult = null;
                            _execStatus = 0x40; // --- Command completed abnormally
                            SendDataResult();
                            break;
                        }

                        // --- Read bytes in
                        var length = _parameters[4] == 0 ? _parameters[7] : 512;
                        _dataResult = _floppyFile.ReadData(
                            Heads[SelectedDrive],
                            CurrentTracks[SelectedDrive],
                            CurrentSectors[SelectedDrive],
                            length);
                        _execStatus = 0x20;
                        SendDataResult();
                    }
                    break;

                case Command.FormatTrack:
                    if (_commandBytesReceived < 5)
                    {
                        _parameters[_commandBytesReceived++] = cmd;
                    }
                    if (_commandBytesReceived >= 5)
                    {
                        // --- Receive format parameters
                        FloppyLogger?.CommandParamsReceived(_parameters);
                        SelectedDrive = _parameters[0] & 0x03;
                        Heads[SelectedDrive] = (byte)((_parameters[0] >> 2) & 0x01);

                        var dataBytes = 1 << (_parameters[1] + 7);
                        var sectors = _parameters[2];
                        var filler = _parameters[4];
                        var data = new byte[dataBytes];
                        for (var i = 0; i < dataBytes; i++)
                        {
                            data[i] = filler;
                        }

                        // --- Format the selected track
                        for (var sector = 1; sector <= sectors; sector++)
                        {
                            _floppyFile.WriteData(Heads[SelectedDrive], 
                                CurrentTracks[SelectedDrive],
                                sector,
                                data);
                        }
                        _dataResult = null;
                        _execStatus = 0x00;
                        SendDataResult();
                    }
                    break;

                case Command.WriteData:
                    if (_commandBytesReceived < 8)
                    {
                        _parameters[_commandBytesReceived] = cmd;
                        if (_commandBytesReceived == 7)
                        {
                            FloppyLogger?.CommandParamsReceived(_parameters);
                            SelectedDrive = _parameters[0] & 0x03;
                            Heads[SelectedDrive] = (byte)((_parameters[0] >> 2) & 0x01);
                            CurrentSectors[SelectedDrive] = _parameters[3];
                            var length = _parameters[4] == 0 ? _parameters[7] : 1 << (_parameters[4] + 7);
                            _dataToWrite = new byte[length];

                            // --- Sign that writye position has been reached
                            SetExecutionMode(true);
                        }
                    }
                    else if (_commandBytesReceived < 8 + _dataToWrite.Length)
                    {
                        _dataToWrite[_commandBytesReceived - 8] = cmd;
                        SetExecutionMode(true);
                    }
                    else if (_commandBytesReceived >= 8 + _dataToWrite.Length)
                    {
                        FloppyLogger.DataReceived(_dataToWrite);
                        SetExecutionMode(false);
                        _floppyFile.WriteData(
                            Heads[SelectedDrive],
                            CurrentTracks[SelectedDrive],
                            CurrentSectors[SelectedDrive],
                            _dataToWrite);
                        _execStatus = 0x00;
                        SendDataResult();
                    }
                    _commandBytesReceived++;
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
                if (_dataResult != null && _dataResultBytesIndex < _dataResult.Length)
                {
                    if (_dataResultBytesIndex == _dataResult.Length - 1)
                    {
                        FloppyLogger?.DataSent(_dataResult);
                    }
                    return _dataResult[_dataResultBytesIndex++];
                }
                SetExecutionMode(false);
                if (_commandResult != null && _resultBytesIndex < _commandResult.Length)
                {
                    var result = _commandResult[_resultBytesIndex++];
                    if (_resultBytesIndex >= _commandResult.Length)
                    {
                        FloppyLogger?.ResultSent(_commandResult);
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
        private void SendResult(byte[] result, byte[] dataResult = null)
        {
            _acceptCommand = true;
            _resultBytesIndex = 0;
            _dataResultBytesIndex = 0;
            SetDioFlag(true);
            _commandResult = result;
            _dataResult = dataResult;
            if (_dataResult != null)
            {
                SetExecutionMode(true);
            }
        }

        /// <summary>
        /// Sends the standard data result package
        /// </summary>
        private void SendDataResult()
        {
            _st0 = (byte)(_execStatus | (Heads[SelectedDrive] == 0 ? 0x00 : 0x04) | (SelectedDrive & 0x03));
            _st1 = (byte)(CurrentSectors[SelectedDrive] > 9 ? 0x80 : 0x00);
            _st2 = 0x00;
            SendResult(new byte[]
            {
                _st0, _st1, _st2,
                CurrentTracks[SelectedDrive],
                Heads[SelectedDrive],
                CurrentSectors[SelectedDrive],
                0x02
            }, _dataResult);
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