using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Tape
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

        private readonly Spectrum48 _hostVm;

        /// <summary>
        /// The current operation mode of the tape
        /// </summary>
        public TapeOperationMode CurrentMode { get; private set; }

        /// <summary>
        /// Stores the start CPU tact when either save or load mode commenced
        /// </summary>
        public ulong StartTact { get; private set; }

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
        /// Initializes the tape device for the specified host VM
        /// </summary>
        /// <param name="hostVm"></param>
        public TapeDevice(Spectrum48 hostVm)
        {
            _hostVm = hostVm;
            CurrentMode = TapeOperationMode.Passive;
            MicBitState = true;
            SavedPulses = new List<MicBitPulse>();
        }

        /// <summary>
        /// Puts the device in save mode. From now on, every MIC pulse is recorded
        /// </summary>
        public void EnterSaveMode()
        {
            CurrentMode = TapeOperationMode.Save;
            MicBitState = true;
            LastMicBitActivityTact = StartTact = _hostVm.Cpu.Ticks;
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
            CurrentMode = TapeOperationMode.Passive;
            StartTact = _hostVm.Cpu.Ticks;
            LastMicBitActivityTact = StartTact;
        }

        /// <summary>
        /// Leaves the load mode. Stops the device that playes EAR pulses
        /// </summary>
        public void LeaveLoadMode()
        {
            CurrentMode = TapeOperationMode.Passive;
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
        /// Checks is the tape device should return to passive mode
        /// </summary>
        public void TriggerPassiveMode()
        {
            switch (CurrentMode)
            {
                case TapeOperationMode.Passive:
                    return;
                case TapeOperationMode.Save:
                    if ((int) (_hostVm.Cpu.Ticks - LastMicBitActivityTact) > SAVE_STOP_SILENCE)
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