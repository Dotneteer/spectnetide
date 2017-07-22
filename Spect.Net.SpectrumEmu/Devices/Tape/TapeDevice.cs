using Spect.Net.SpectrumEmu.Abstraction;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Devices.Tape
{
    /// <summary>
    /// This class represents the cassette tape device in ZX Spectrum
    /// </summary>
    public class TapeDevice : ICpuOperationBoundDevice, ITapeDevice
    {
        private IBeeperDevice _beeperDevice;
        private IZ80Cpu _cpu;
        private int _pilotPulseCount;
        private int _bitOffset;
        private byte _dataByte;
        private int _dataLength;
        private byte[] _dataBuffer;
        private int _dataBlockCount;
        private MicPulseType _prevDataPulse;

        /// <summary>
        /// Number of tacts after save mod can be exited automatically
        /// </summary>
        public const int SAVE_STOP_SILENCE = 17_500_000;

        /// <summary>
        /// The address of the SAVE_BYTES routine in the Spectrum ROM
        /// </summary>
        public const ushort SAVE_BYTES_ROM_ADDRESS = 0x4C2;

        /// <summary>
        /// The address of the LOAD_START routine in the Spectrum ROM
        /// </summary>
        public const ushort LOAD_START_ROM_ADDRESS = 0x56C;

        /// <summary>
        /// The address of the ERROR routine in the Spectrum ROM
        /// </summary>
        public const ushort ERROR_ROM_ADDRESS = 0x0008;

        /// <summary>
        /// The maximum distance between two scans of the EAR bit
        /// </summary>
        public const int MAX_TACT_JUMP = 10000;

        /// <summary>
        /// The width tolerance of save pulses
        /// </summary>
        public const int SAVE_PULSE_TOLERANCE = 24;

        /// <summary>
        /// Minimum number of pilot pulses before SYNC1
        /// </summary>
        public const int MIN_PILOT_PULSE_COUNT = 3000;

        /// <summary>
        /// Lenght of the data buffer to allocate for the SAVE operation
        /// </summary>
        public const int DATA_BUFFER_LENGTH = 0x1_0000;

        /// <summary>
        /// Gets the TZX tape content provider
        /// </summary>
        public ITzxTapeContentProvider ContentProvider { get; }

        /// <summary>
        /// Gets the TZX Save provider
        /// </summary>
        public ITzxSaveProvider SaveProvider { get; }

        /// <summary>
        /// The current operation mode of the tape
        /// </summary>
        public TapeOperationMode CurrentMode { get; private set; }

        /// <summary>
        /// Stores the start CPU tact when either save or load mode commenced
        /// </summary>
        public long SaveStartTact { get; private set; }

        /// <summary>
        /// The CPU tact of the last MIC bit activity
        /// </summary>
        public long LastMicBitActivityTact { get; private set; }

        /// <summary>
        /// Gets the current state of the MIC bit
        /// </summary>
        public bool MicBitState { get; private set; }

        /// <summary>
        /// The current phase of the SAVE operation
        /// </summary>
        public SavePhase SavePhase { get; private set; }

        /// <summary>
        /// The TzxPlayer that can playback tape content
        /// </summary>
        public TzxPlayer Player { get; private set; }

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
            _cpu = hostVm.Cpu;
            _beeperDevice = hostVm.BeeperDevice;
            Reset();
        }

        /// <summary>
        /// Initializes the tape device for the specified host VM
        /// </summary>
        /// <param name="contentProvider">Tape content provider</param>
        /// <param name="saveProvider">Save provider for the tape</param>
        public TapeDevice(ITzxTapeContentProvider contentProvider, ITzxSaveProvider saveProvider)
        {
            ContentProvider = contentProvider;
            SaveProvider = saveProvider;
        }

        /// <summary>
        /// Resets the tape device
        /// </summary>
        public void Reset()
        {
            ContentProvider?.Reset();
            Player = null;
            CurrentMode = TapeOperationMode.Passive;
            SavePhase = SavePhase.None;
            MicBitState = true;
        }

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        public void OnCpuOperationCompleted()
        {
            SetTapeMode();
        }

        #region Manage tape modes

        /// <summary>
        /// Sets the current tape mode according to the current PC register
        /// and the MIC bit state
        /// </summary>
        public void SetTapeMode()
        {
            var ticks = _cpu.Tacts;
            var error = _cpu.Registers.PC == ERROR_ROM_ADDRESS;
            switch (CurrentMode)
            {
                case TapeOperationMode.Passive:
                    if (_cpu.Registers.PC == LOAD_START_ROM_ADDRESS)
                    {
                        EnterLoadMode();
                    }
                    else if (_cpu.Registers.PC == SAVE_BYTES_ROM_ADDRESS)
                    {
                        EnterSaveMode();
                    }
                    return;
                case TapeOperationMode.Save:
                    if (error || (int)(ticks - LastMicBitActivityTact) > SAVE_STOP_SILENCE)
                    {
                        LeaveSaveMode();
                    }
                    return;
                case TapeOperationMode.Load:
                    if (Player.Eof || error)
                    {
                        LeaveLoadMode();
                    }
                    return;
            }
        }

        /// <summary>
        /// Puts the device in save mode. From now on, every MIC pulse is recorded
        /// </summary>
        private void EnterSaveMode()
        {
            CurrentMode = TapeOperationMode.Save;
            SavePhase = SavePhase.None;
            MicBitState = true;
            LastMicBitActivityTact = SaveStartTact = _cpu.Tacts;
            _pilotPulseCount = 0;
            _prevDataPulse = MicPulseType.None;
            _dataBlockCount = 0;
            SaveProvider?.CreateTapeFile();
        }

        /// <summary>
        /// Leaves the save mode. Stops recording MIC pulses
        /// </summary>
        private void LeaveSaveMode()
        {
            CurrentMode = TapeOperationMode.Passive;
        }

        /// <summary>
        /// Puts the device in load mode. From now on, EAR pulses are played by a device
        /// </summary>
        private void EnterLoadMode()
        {
            CurrentMode = TapeOperationMode.Load;
            if (ContentProvider == null) return;

            var contentReader = ContentProvider.GetTzxContent();
            Player = new TzxPlayer(contentReader);
            Player.ReadContent();
            Player.InitPlay(_cpu.Tacts);
        }

        /// <summary>
        /// Leaves the load mode. Stops the device that playes EAR pulses
        /// </summary>
        private void LeaveLoadMode()
        {
            CurrentMode = TapeOperationMode.Passive;
            Player = null;
            ContentProvider?.Reset();
        }

        #endregion

        #region Read bits during LOAD

        /// <summary>
        /// Gets the EAR bit read from the tape
        /// </summary>
        /// <param name="cpuTicks"></param>
        /// <returns></returns>
        public bool GetEarBit(long cpuTicks)
        {
            if (CurrentMode != TapeOperationMode.Load)
            {
                return true;
            }
            var earBit = Player?.GetEarBit(cpuTicks) ?? true;
            _beeperDevice.ProcessEarBitValue(earBit);
            return earBit;
        }

        #endregion

        #region Persist bits during SAVE

        /// <summary>
        /// Processes the the change of the MIC bit
        /// </summary>
        /// <param name="micBit"></param>
        public void ProcessMicBitValue(bool micBit)
        {
            if (CurrentMode != TapeOperationMode.Save
                || MicBitState == micBit)
            {
                return;
            }

            var length = _cpu.Tacts - LastMicBitActivityTact;

            // --- Classify the pulse by its width
            var pulse = MicPulseType.None;
            if (length >= TzxStandardSpeedDataBlock.BIT_0_PL - SAVE_PULSE_TOLERANCE
                && length <= TzxStandardSpeedDataBlock.BIT_0_PL + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.Bit0;
            }
            else if (length >= TzxStandardSpeedDataBlock.BIT_1_PL - SAVE_PULSE_TOLERANCE
                && length <= TzxStandardSpeedDataBlock.BIT_1_PL + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.Bit1;
            }
            if (length >= TzxStandardSpeedDataBlock.PILOT_PL - SAVE_PULSE_TOLERANCE
                && length <= TzxStandardSpeedDataBlock.PILOT_PL + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.Pilot;
            }
            else if (length >= TzxStandardSpeedDataBlock.SYNC_1_PL - SAVE_PULSE_TOLERANCE
                     && length <= TzxStandardSpeedDataBlock.SYNC_1_PL + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.Sync1;
            }
            else if (length >= TzxStandardSpeedDataBlock.SYNC_2_PL - SAVE_PULSE_TOLERANCE
                     && length <= TzxStandardSpeedDataBlock.SYNC_2_PL + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.Sync2;
            }
            else if (length >= TzxStandardSpeedDataBlock.TERM_SYNC - SAVE_PULSE_TOLERANCE
                     && length <= TzxStandardSpeedDataBlock.TERM_SYNC + SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.TermSync;
            }
            else if (length < TzxStandardSpeedDataBlock.SYNC_1_PL - SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.TooShort;
            }
            else if (length > TzxStandardSpeedDataBlock.PILOT_PL + 2 * SAVE_PULSE_TOLERANCE)
            {
                pulse = MicPulseType.TooLong;
            }

            MicBitState = micBit;
            LastMicBitActivityTact = _cpu.Tacts;

            // --- Lets process the pulse according to the current SAVE phase and pulse width
            var nextPhase = SavePhase.Error;
            switch (SavePhase)
            {
                case SavePhase.None:
                    if (pulse == MicPulseType.TooShort || pulse == MicPulseType.TooLong)
                    {
                        nextPhase = SavePhase.None;
                    }
                    else if (pulse == MicPulseType.Pilot)
                    {
                        _pilotPulseCount = 1;
                        nextPhase = SavePhase.Pilot;
                    }
                    break;
                case SavePhase.Pilot:
                    if (pulse == MicPulseType.Pilot)
                    {
                        _pilotPulseCount++;
                        nextPhase = SavePhase.Pilot;
                    }
                    else if (pulse == MicPulseType.Sync1 && _pilotPulseCount >= MIN_PILOT_PULSE_COUNT)
                    {
                        nextPhase = SavePhase.Sync1;
                    }
                    break;
                case SavePhase.Sync1:
                    if (pulse == MicPulseType.Sync2)
                    {
                        nextPhase = SavePhase.Sync2;
                    }
                    break;
                case SavePhase.Sync2:
                    if (pulse == MicPulseType.Bit0 || pulse == MicPulseType.Bit1)
                    {
                        // --- Next pulse starts data, prepare for receiving it
                        _prevDataPulse = pulse;
                        nextPhase = SavePhase.Data;
                        _bitOffset = 0;
                        _dataByte = 0;
                        _dataLength = 0;
                        _dataBuffer = new byte[DATA_BUFFER_LENGTH];
                    }
                    break;
                case SavePhase.Data:
                    if (pulse == MicPulseType.Bit0 || pulse == MicPulseType.Bit1)
                    {
                        if (_prevDataPulse == MicPulseType.None)
                        {
                            // --- We are waiting for the second half of the bit pulse
                            _prevDataPulse = pulse;
                            nextPhase = SavePhase.Data;
                        }
                        else if (_prevDataPulse == pulse)
                        {
                            // --- We received a full valid bit pulse
                            nextPhase = SavePhase.Data;
                            _prevDataPulse = MicPulseType.None;

                            // --- Add this bit to the received data
                            _bitOffset++;
                            _dataByte = (byte)(_dataByte * 2 + (pulse == MicPulseType.Bit0 ? 0 : 1));
                            if (_bitOffset == 8)
                            {
                                // --- We received a full byte
                                _dataBuffer[_dataLength++] = _dataByte;
                                _dataByte = 0;
                                _bitOffset = 0;
                            }
                        }
                    }
                    else if (pulse == MicPulseType.TermSync)
                    {
                        // --- We received the terminating pulse, the datablock has been completed
                        nextPhase = SavePhase.None;
                        _dataBlockCount++;

                        // --- Create and save the data block
                        //var dataBlock = new TzxStandardSpeedDataBlock
                        //{
                            
                        //}
                        //SaveProvider.SaveTzxBlock();
                    }
                    break;
            }
            SavePhase = nextPhase;
        }

        #endregion
    }
}