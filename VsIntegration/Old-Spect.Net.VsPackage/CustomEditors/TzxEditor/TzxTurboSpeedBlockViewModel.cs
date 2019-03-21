using System.ComponentModel;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.VsPackage.CustomEditors.RomEditor;
using Spect.Net.VsPackage.ToolWindows.BasicList;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// This class implements the view model for the standard speed TZX data block
    /// </summary>
    public class TzxTurboSpeedBlockViewModel : TapeBlockViewModelBase
    {
        private ushort _pilotPulseLength;
        private ushort _sync1PulseLength;
        private ushort _sync2PulseLength;
        private ushort _zeroBitPulseLength;
        private ushort _oneBitPulseLength;
        private ushort _pilotToneLength;
        private ushort _pauseAfter;
        private byte _lastByteUsedBits;
        private int _dataLength;
        private byte[] _data;
        private MemoryViewModel _memory;

        public MemoryViewModel Memory
        {
            get => _memory;
            set => Set(ref _memory, value);
        }

        /// <summary>
        /// Length of pilot pulse
        /// </summary>
        public ushort PilotPulseLength
        {
            get => _pilotPulseLength;
            set => Set(ref _pilotPulseLength, value);
        }

        /// <summary>
        /// Length of the first sync pulse
        /// </summary>
        public ushort Sync1PulseLength
        {
            get => _sync1PulseLength;
            set => Set(ref _sync1PulseLength, value);
        }

        /// <summary>
        /// Length of the second sync pulse
        /// </summary>
        public ushort Sync2PulseLength
        {
            get => _sync2PulseLength;
            set => Set(ref _sync2PulseLength, value);
        }

        /// <summary>
        /// Length of the zero bit
        /// </summary>
        public ushort ZeroBitPulseLength
        {
            get => _zeroBitPulseLength;
            set => Set(ref _zeroBitPulseLength, value);
        }

        /// <summary>
        /// Length of the one bit
        /// </summary>
        public ushort OneBitPulseLength
        {
            get => _oneBitPulseLength;
            set => Set(ref _oneBitPulseLength, value);
        }

        /// <summary>
        /// Length of the pilot tone
        /// </summary>
        public ushort PilotToneLength
        {
            get => _pilotToneLength;
            set => Set(ref _pilotToneLength, value);
        }

        /// <summary>
        /// Pause after this block
        /// </summary>
        public ushort PauseAfter
        {
            get => _pauseAfter;
            set => Set(ref _pauseAfter, value);
        }

        /// <summary>
        /// Used bits in the last byte (other bits should be 0)
        /// </summary>
        /// <remarks>
        /// (e.g. if this is 6, then the bits used(x) in the last byte are: 
        /// xxxxxx00, where MSb is the leftmost bit, LSb is the rightmost bit)
        /// </remarks>
        public byte LastByteUsedBits
        {
            get => _lastByteUsedBits;
            set => Set(ref _lastByteUsedBits, value);
        }

        /// <summary>
        /// Length of block data
        /// </summary>
        public int DataLength
        {
            get => _dataLength;
            set => Set(ref _dataLength, value);
        }

        /// <summary>
        /// Block Data
        /// </summary>
        public byte[] Data
        {
            get => _data;
            set => Set(ref _data, value);
        }

        public TzxTurboSpeedBlockViewModel()
        {
            BlockId = 0x11;
            BlockType = "Turbo Speed Data Block";
        }

        public void FromDataBlock(TzxTurboSpeedDataBlock block)
        {
            PilotPulseLength = block.PilotPulseLength;
            Sync1PulseLength = block.Sync1PulseLength;
            Sync2PulseLength = block.Sync2PulseLength;
            ZeroBitPulseLength = block.ZeroBitPulseLength;
            OneBitPulseLength = block.OneBitPulseLength;
            PilotToneLength = block.PilotToneLength;
            LastByteUsedBits = block.LastByteUsedBits;
            PauseAfter = block.PauseAfter;
            DataLength = (ushort)block.Data.Length;
            Data = block.Data;
            Memory = new MemoryViewModel
            {
                MemoryBuffer = Data,
                ShowPrompt = DataLength > 0x14,
                AllowDisassembly = DataLength > 0x14
            };
        }
    }
}