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
        public const string MMC_FOLDER = ".SpectNetIde/Mmc";

        /// <summary>
        /// MMC file name within the MMC folder
        /// </summary>
        public const string MMC_FILE = "Card{0}.mmc";

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
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
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
        }

        /// <summary>
        /// Reads the value of the control register
        /// </summary>
        /// <returns>Control register value</returns>
        public byte ReadControlRegister()
        {
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
    }
}