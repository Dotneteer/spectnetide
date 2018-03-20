using System;
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
        /// MMC folder
        /// </summary>
        public const string MMC_FOLDER = @"C:\Temp\MmcFiles";

        /// <summary>
        /// MMC file name within the MMC folder
        /// </summary>
        public const string MMC_FILE = "Card0.mmc";

        /// <summary>
        /// The selected MMC card
        /// </summary>
        private int _cardSelected = 0;

        // --- The OCR - Operation Condition Register
        private byte[] _ocr = { 0x05, 0x00, 0x00, 0x00, 0x00 };

        // --- The CID - Card Identification Register
        private byte[] _cid =
        {
            0x01, (byte) 'S', (byte) 'P',
            (byte) 'e', (byte) 'c', (byte) 't', (byte) 'n', (byte) 'e', (byte) 't',
            0x01, 0x01, 0x01, 0x01, 0x01, 0x7F, 0x80
        };

        // --- CSD - Card Specific Data Register
        private byte[] _csd =
        {
            0x0B, 0x0B, 0x0B, 0x0B,
            0x0B, 0x0B, 0x0B, 0x0B,
            0x0B, 0x0B, 0x0B, 0x0B,
            0x0B, 0x0B, 0x0B, 0x0B
        };

        // --- Stores command parameters
        private byte[] _parametersSent = new byte[6];

        // --- Index for reading OCR register values. 0 means that all values are set/received
        private int _ocrIndex = -1;

        // --- Index for reading CSD register values. 0 means that all values are set/received
        private int _csdIndex = -1;

        // --- Index for reading CID register values. 0 means that all values are set/received
        private int _cidIndex = -1;

        // --- Index for reading block values. 0 means that all values are set/received
        private int _readIndex = -1;

        // --- Index for writing block values. 0 means that all values are set/received
        private int _writeIndex = -1;

        // --- Is the MMC enabled?
        private bool _enabled = true;

        // --- In in Idle state?
        private bool _isIdle = false;

        // --- The last command to process
        private byte _lastCommand = 0x00;

        // --- The index of the command/parameters
        private int _commandIndex = 0x00;

        // --- Signs if the content should be flushed to the disk
        private bool _flushToDisk = false;

        // --- Flags if the card is write protected
        private bool _writeProtected = false;

        // --- Flags if writes are to be flushed to the disk
        private bool _persistentWrite = true;

        // --- Start address for data read
        private uint _readAddress;

        // --- Start address for data write
        private uint _writeAddress;

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
        }

        /// <summary>
        /// Writes a byte into the MMC control register
        /// </summary>
        /// <param name="value">Value to write into the control register</param>
        public void WriteControlRegister(byte value)
        {
            if (!_enabled || _cardSelected != 0) return;

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
                        _parametersSent[_commandIndex - 1] = value;
                    }
                    else if (_commandIndex == 5)
                    {
                        _writeAddress = Get32BitParameter();
                        _writeIndex = 0;
                    }
                    else if (_commandIndex >= 5 + 2 && _commandIndex <= 5 + 2 + 512 - 1)
                    {
                        // TODO: Check for read-onlyness
                        _storage.WriteData((int)(_writeAddress + _commandIndex - (5+2)), value);
                    }
                    _commandIndex++;
                    break;

                // --- READ_OCR
                case 0x7A:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Reads the value of the control register
        /// </summary>
        /// <returns>Control register value</returns>
        public byte ReadControlRegister()
        {
            if (!_enabled || _cardSelected != 0) return 0xFF;

            // --- Idle state always return zero
            if (_isIdle) return 0x00;

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
                    // TODO: Implement it
                    return 0xFF;

                // --- SEND_CID
                case 0x4A:
                    // TODO: Implement it
                    return 0xFF;

                // --- READ_SINGLE_BLOCK
                case 0x51:
                    // TODO: Implement it
                    return 0xFF;

                // --- READ_MULTIPLE_BLOCKS
                case 0x52:
                    // TODO: Implement it
                    return 0xFF;

                // --- WRITE_BLOCK
                case 0x58:
                    // TODO: Implement it
                    return 0xFF;

                // --- READ_OCR
                case 0x7A:
                    // TODO: Implement it
                    return 0xFF;
            }
            return 0xFF;
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

        private uint Get32BitParameter()
        {
            return (uint)(_parametersSent[0] << 24 + _parametersSent[1] << 16
                + _parametersSent[2] << 8 + _parametersSent[3]);
        }

        #endregion
    }
}