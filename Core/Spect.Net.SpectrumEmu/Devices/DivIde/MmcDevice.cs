using System;
using System.Collections.ObjectModel;
using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.DivIde
{
    /// <summary>
    /// This class represents an MultiMedia Card device
    /// </summary>
    public class MmcDevice: IMmcDevice
    {
        /// <summary>
        /// MMC card size: 64MB
        /// </summary>
        public const int MMC_SIZE = 1024 * 1024 * 64;

        /// <summary>
        /// Size of an MMC block
        /// </summary>
        public const int MMC_BLOCK_SIZE = 512;

        /// <summary>
        /// MMC folder
        /// </summary>
        public const string MMC_FOLDER = @"C:\Temp\MmcFiles";

        /// <summary>
        /// MMC file name within the MMC folder
        /// </summary>
        public const string MMC_FILE = "Card0.mmc";

        /// <summary>
        /// The card selected to work with
        /// </summary>
        public int SelectedCard { get; private set; }

        // --- The OCR - Operation Condition Register
        private readonly byte[] _ocr = { 0x05, 0x00, 0x00, 0x00, 0x00 };

        // --- The CID - Card Identification Register
        private readonly byte[] _cid =
        {
            0x01, (byte) 'S', (byte) 'P',
            (byte) 'e', (byte) 'c', (byte) 't', (byte) 'n', (byte) 'e', (byte) 't',
            0x01, 0x01, 0x01, 0x01, 0x01, 0x7F, 0x80
        };

        // --- CSD - Card Specific Data Register
        private readonly byte[] _csd =
        {
            0x0B, 0x0B, 0x0B, 0x0B,
            0x0B, 0x0B, 0x0B, 0x0B,
            0x0B, 0x0B, 0x0B, 0x0B,
            0x0B, 0x0B, 0x0B, 0x0B
        };

        // --- Stores command parameters
        private readonly byte[] _parametersSent = new byte[6];

        /// <summary>
        /// Gets the OCR values
        /// </summary>
        public ReadOnlyCollection<byte> Ocr => new ReadOnlyCollection<byte>(_ocr);

        /// <summary>
        /// Gets the CSD values
        /// </summary>
        public ReadOnlyCollection<byte> Csd => new ReadOnlyCollection<byte>(_csd);

        // --- Index for reading OCR register values.
        private int _ocrIndex = -1;

        // --- Index for reading CSD register values.
        private int _csdIndex = -1;

        // --- Index for reading CID register values.
        private int _cidIndex = -1;

        // --- Index for reading block values.
        private int _readIndex = -1;

        // --- Index for writing block values.
        private int _writeIndex = -1;

        // --- Is the MMC enabled?
        private bool _enabled = true;

        // --- In in Idle state?
        private bool _isIdle;

        // --- The last command to process
        private byte _lastCommand;

        // --- The index of the command/parameters
        private int _commandIndex;

        // --- Flags if the card is write protected
        private bool _writeProtected = false;

        // --- Start address for data read
        private uint _readAddress;

        // --- Start address for data write
        private uint _writeAddress;

        // --- 512 byte buffer for read/write operations
        private readonly byte[] _buffer = new byte[MMC_BLOCK_SIZE];

        // --- The persistent MMC storage
        private MmcStorage _storage;

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            var filename = Path.Combine(MMC_FOLDER, MMC_FILE);
            _storage = MmcStorage.Create(filename, 32);
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            // --- MMC is ready to receive aand execute a command
            _commandIndex = 0;
            _isIdle = false;

        }

        #region MMC operations

        /// <summary>
        /// Selects an MMC card
        /// </summary>
        /// <param name="value">Selector byte</param>
        public void SelectCard(byte value)
        {
            _isIdle = true;
            _lastCommand = 0x00;
            _commandIndex = 0;
            _readIndex = -1;
            _writeIndex = -1;
            _ocrIndex = -1;
            _csdIndex = -1;
            _cidIndex = -1;
            SelectedCard = value == 0xFE ? 0 : 1;
        }

        /// <summary>
        /// Writes a byte into the MMC control register
        /// </summary>
        /// <param name="value">Value to write into the control register</param>
        public void WriteControlRegister(byte value)
        {
            if (!_enabled || SelectedCard != 0) return;

            // --- Check, if the last command value is received
            if (_commandIndex == 0)
            {
                _lastCommand = value;
                _commandIndex++;
                return;
            }

            // --- We're about to recive command parameters
            switch (_lastCommand)
            {
                // --- GO_IDLE_STATE
                case 0x40:
                    ProcessCommand(() =>
                    {
                        _isIdle = true;
                        _commandIndex = 0;
                    });
                    break;

                // --- SEND_IF_COND
                case 0x48:
                    // --- Intentionally does not do anything
                    break;

                // --- SEND_SCD
                case 0x49:
                    ProcessCommand(() =>
                    {
                        _csdIndex = 0;
                        _commandIndex = 0;
                    });
                    break;

                // --- SEND_CID
                case 0x4A:
                    ProcessCommand(() =>
                    {
                        _cidIndex = 0;
                        _commandIndex = 0;
                    });
                    break;

                // --- STOP_TRANSMISSION
                // --- Terminates a read/write stream/multiple block operation. 
                // --- When CMD12 is used to terminate a read transaction the 
                // --- card will respond with R1. When it is used to stop a 
                // ---- write transaction the card will respond with R1b.
                case 0x4C:
                    ProcessCommand(() =>
                    {
                        _isIdle = true;
                        _commandIndex = 0;
                    });
                    break;

                // --- READ_SINGLE_BLOCK, READ_MULTIPLE_BLOCKS
                case 0x51:
                case 0x52:
                    _parametersSent[_commandIndex - 1] = value;
                    _commandIndex++;
                    if (_commandIndex == 6)
                    {
                        // --- All parameters received, ready to execute the command
                        _commandIndex = 0;
                        _readAddress = Get32BitParameter();
                        _readIndex = 0;
                    }
                    break;

                // --- WRITE_BLOCK
                case 0x58:
                    if (_commandIndex < 5)
                    {
                        // --- Store next parameter byte
                        _parametersSent[_commandIndex - 1] = value;
                    }
                    else if (_commandIndex == 5)
                    {
                        // --- All parameters received, init the write operation
                        _writeAddress = Get32BitParameter();
                        for (var i = 0; i < MMC_BLOCK_SIZE; i++)
                        {
                            _buffer[i] = 0;
                        }
                        _writeIndex = 0;
                    }
                    else if (_commandIndex >= 5 + 2 
                             && _commandIndex <= 5 + 2 + MMC_BLOCK_SIZE - 1)
                    {
                        // --- Write the subsequent byte to the buffer
                        if (_writeProtected) break;

                        _buffer[_commandIndex - (5 + 2)] = value;
                        if (_commandIndex == 5 + 2 + MMC_BLOCK_SIZE - 1)
                        {
                            // --- Last byte of the block, persist it
                            _storage.WriteData((int)_writeAddress, _buffer);
                        }
                    }
                    _commandIndex++;
                    break;

                // --- READ_OCR
                case 0x7A:
                    ProcessCommand(() =>
                    {
                        _ocrIndex = 0;
                        _commandIndex = 0;
                    });
                    break;
            }
        }

        /// <summary>
        /// Reads the value of the control register
        /// </summary>
        /// <returns>Control register value</returns>
        public byte ReadControlRegister()
        {
            if (!_enabled /*|| SelectedCard != 0*/) return 0xFF;

            // --- Idle state always return zero
            if (_isIdle) return 0x00;

            // --- Unhandled commands will return 0xFF
            byte value = 0xFF;
            switch (_lastCommand)
            {
                // --- We need this temporary value to load config.ini
                case 0x00:
                    return 0x00;

                // --- GO_IDLE_STATE
                // --- We're not in idle state, so return 0x01
                case 0x40:
                    return 0x01;

                // --- SEND_IF_COND
                case 0x48:
                    return 0;

                // --- SEND_CSD
                case 0x49:
                    if (_csdIndex >= 0)
                    {
                        if (_csdIndex == 0)
                        {
                            value = 0xFF; // --- No command response
                        }
                        else if (_csdIndex == 1)
                        {
                            value = 0x00; // --- Command response
                        }
                        else if (_csdIndex == 2)
                        {
                            value = 0xFE; 
                        }
                        else if (_csdIndex >= 3 && _csdIndex <= 18)
                        {
                            value = _csd[_csdIndex - 3]; // --- Retrieve the 16 bytes of CSD
                        }
                        else if (_csdIndex >= 19 && _csdIndex <= 20)
                        {
                            value = 0xFF; // --- 0xFF for CRC
                        }

                        _csdIndex++;
                        if (_csdIndex > 20)
                        {
                            // --- The entire CSD has been returned
                            _csdIndex = -1;
                        }
                        return value;
                    }
                    break;

                // --- SEND_CID
                case 0x4A:
                    if (_cidIndex >= 0)
                    {
                        if (_cidIndex == 0)
                        {
                            value = 0xFF; // --- No command response
                        }
                        else if (_cidIndex == 1)
                        {
                            value = 0x00; // --- Command respopnse
                        }
                        else if (_cidIndex == 2)
                        {
                            value = 0xFE;
                        }
                        else if (_cidIndex >= 3 && _cidIndex <= 18)
                        {
                            value = _cid[_cidIndex - 3];
                        }
                        else if (_cidIndex >= 19 && _cidIndex <= 20)
                        {
                            value = 0xFF; // --- Return 0xFF as CRC
                        }

                        _cidIndex++;
                        if (_cidIndex > 20)
                        {
                            // --- The entire CID has been returned
                            _cidIndex = -1;
                        }
                        return value;
                    }
                    break;

                // --- STOP_TRANSMISSION
                case 0x4C:
                    return 0x01;

                // --- READ_SINGLE_BLOCK, READ_MULTIPLE_BLOCKS
                case 0x51:
                case 0x52:
                    if (_readIndex >= 0)
                    {
                        if (_readIndex == 0)
                        {
                            value = 0xFF; // --- No command response
                        }
                        else if (_readIndex == 1)
                        {
                            value = 0x00; // --- Command response
                        }
                        else if (_readIndex == 2)
                        {
                            value = 0xFE;
                        }
                        else if (_readIndex >= 3 && _readIndex <= MMC_BLOCK_SIZE + 2)
                        {
                            value = _storage.ReadData((int) (_readAddress + _readIndex - 3));
                        }
                        else if (_readIndex == MMC_BLOCK_SIZE + 3)
                        {
                            _readIndex++;
                        }
                        _readIndex++;
                        if (_readIndex == MMC_BLOCK_SIZE + 4)
                        {
                            // --- It is time to read the next block
                            _readIndex = 0;
                            _readAddress += MMC_BLOCK_SIZE;
                        }
                        return value;
                    }
                    break;

                // --- WRITE_BLOCK
                case 0x58:
                    if (_writeIndex >= 0)
                    {
                        switch (_writeIndex)
                        {
                            case 0:
                                value = 0xFF;
                                break;
                            case 1:
                                value = 0x00;
                                break;
                            case 2:
                            case 3:
                                value = 0xFF;
                                break;
                            default:
                                value = 0b0000_0101; // Data accepted status
                                break;
                        }
                        _writeIndex++;
                        return value;
                    }
                    break;

                // --- READ_OCR
                case 0x7A:
                    if (_ocrIndex >= 0)
                    {
                        if (_ocrIndex == 0)
                        {
                            value = 0xFF;
                        }
                        else if (_ocrIndex == 1)
                        {
                            value = 0x00;
                        }
                        else if (_ocrIndex >= 2 && _ocrIndex <= 6)
                        {
                            value = _ocr[_ocrIndex-2];
                        }
                        else if (_ocrIndex == 7 || _ocrIndex == 8)
                        {
                            value = 0xFF;
                        }
                        _ocrIndex++;
                        if (_ocrIndex == 9)
                        {
                            _ocrIndex = -1;
                        }
                        return value;
                    }
                    break;
            }
            return value;
        }

        #endregion

        #region Device state management 

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

        #endregion

        #region Helper commands

        /// <summary>
        /// Processes the command passed in the action
        /// </summary>
        /// <param name="action">Command action</param>
        private void ProcessCommand(Action action)
        {
            if (_commandIndex == 5)
            {
                action();
            }
            else
            {
                _commandIndex++;
            }
        }

        /// <summary>
        /// Get a 32-bit integer from the received argumant bytes
        /// </summary>
        /// <returns>32-bit value</returns>
        private uint Get32BitParameter()
        {
            return (uint)(_parametersSent[0] << 24 + _parametersSent[1] << 16
                + _parametersSent[2] << 8 + _parametersSent[3]);
        }

        #endregion
    }
}