using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Tape;

namespace Spect.Net.SpectrumEmu.Devices
{
    /// <summary>
    /// This class represents the cassette tape device in ZX Spectrum
    /// </summary>
    public class TapeDevice
    {
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
        /// Host ZX Spectrum virtual machine
        /// </summary>
        private readonly Spectrum48 _hostVm;

        /// <summary>
        /// Gets the TZX tape content provider
        /// </summary>
        public ITzxTapeContentProvider ContentProvider { get; }

        /// <summary>
        /// The current operation mode of the tape
        /// </summary>
        public TapeOperationMode CurrentMode { get; private set; }

        /// <summary>
        /// Stores the start CPU tact when either save or load mode commenced
        /// </summary>
        public ulong SaveStartTact { get; private set; }

        /// <summary>
        /// The CPU tact of the last MIC bit activity
        /// </summary>
        public ulong LastMicBitActivityTact { get; private set; }

        /// <summary>
        /// Gets the current state of the MIC bit
        /// </summary>
        public bool MicBitState { get; private set; }

        /// <summary>
        /// The pulses saved during the save mode
        /// </summary>
        public IList<MicBitPulse> SavedPulses { get; }

        /// <summary>
        /// The TzxPlayer that can playback tape content
        /// </summary>
        public TzxPlayer Player { get; private set; }

        /// <summary>
        /// Initializes the tape device for the specified host VM
        /// </summary>
        /// <param name="hostVm">Host ZX spectrum VM</param>
        /// <param name="contentProvider">Tape content provider</param>
        public TapeDevice(Spectrum48 hostVm, ITzxTapeContentProvider contentProvider)
        {
            _hostVm = hostVm;
            ContentProvider = contentProvider;
            Player = null;
            CurrentMode = TapeOperationMode.Passive;
            MicBitState = true;
            SavedPulses = new List<MicBitPulse>();
        }

        /// <summary>
        /// Gets the EAR bit read from the tape
        /// </summary>
        /// <param name="cpuTicks"></param>
        /// <returns></returns>
        public bool GetEarBit(ulong cpuTicks)
        {
            var earBit = true;
            if (CurrentMode == TapeOperationMode.Load)
            {
                earBit = Player?.GetEarBit(cpuTicks) ?? true;
                _hostVm.BeeperDevice.ProcessEarBitValue(earBit);
            };
            return earBit;
        }

        /// <summary>
        /// Puts the device in save mode. From now on, every MIC pulse is recorded
        /// </summary>
        public void EnterSaveMode()
        {
            CurrentMode = TapeOperationMode.Save;
            MicBitState = true;
            LastMicBitActivityTact = SaveStartTact = _hostVm.Cpu.Ticks;
            SavedPulses.Clear();
        }

        /// <summary>
        /// Leaves the save mode. Stops recording MIC pulses
        /// </summary>
        public void LeaveSaveMode()
        {
            CurrentMode = TapeOperationMode.Passive;
        }

        /// <summary>
        /// Puts the device in load mode. From now on, EAR pulses are played by a device
        /// </summary>
        public void EnterLoadMode()
        {
            CurrentMode = TapeOperationMode.Load;
            if (ContentProvider == null) return;

            var contentReader = ContentProvider.GetTzxContent();
            Player = new TzxPlayer(contentReader);
            Player.ReadContent();
            Player.InitPlay(_hostVm.Cpu.Ticks);
        }

        /// <summary>
        /// Leaves the load mode. Stops the device that playes EAR pulses
        /// </summary>
        public void LeaveLoadMode()
        {
            CurrentMode = TapeOperationMode.Passive;
            Player = null;
            ContentProvider?.Reset();
        }

        /// <summary>
        /// Checks the VM to enter into save mode automatically
        /// </summary>
        public void TriggerSaveMode()
        {
            if (CurrentMode != TapeOperationMode.Passive)
            {
                return;
            }
            if (_hostVm.Cpu.Registers.PC == SAVE_BYTES_ROM_ADDRESS)
            {
                EnterSaveMode();
            }
        }

        /// <summary>
        /// Checks the VM to enter into load mode automatically
        /// </summary>
        public void TriggerLoadMode()
        {
            if (CurrentMode != TapeOperationMode.Passive)
            {
                return;
            }
            if (_hostVm.Cpu.Registers.PC == LOAD_START_ROM_ADDRESS)
            {
                EnterLoadMode();
            }
        }

        /// <summary>
        /// Checks is the tape device should return to passive mode
        /// </summary>
        public void TriggerPassiveMode()
        {
            if (CurrentMode == TapeOperationMode.Passive) return;
            var ticks = _hostVm.Cpu.Ticks;
            var error = _hostVm.Cpu.Registers.PC == ERROR_ROM_ADDRESS;
            switch (CurrentMode)
            {
                case TapeOperationMode.Passive:
                    if (error)
                    {
                        LeaveLoadMode();
                    }
                    return;
                case TapeOperationMode.Save:
                    if (error || (int) (ticks - LastMicBitActivityTact) > SAVE_STOP_SILENCE)
                    {
                        LeaveSaveMode();
                    }
                    return;
                case TapeOperationMode.Load:
                    break;
            }
        }

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

            // --- Record the pulse
            var currentTact = _hostVm.Cpu.Ticks;
            SavedPulses.Add(new MicBitPulse
            {
                MicBit = micBit,
                Lenght = (int)(currentTact - LastMicBitActivityTact)
            });
            MicBitState = micBit;
            LastMicBitActivityTact = currentTact;
        }
    }
}