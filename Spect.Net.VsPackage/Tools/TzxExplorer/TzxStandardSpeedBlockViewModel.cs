using System.Collections.ObjectModel;
using System.ComponentModel;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.VsPackage.Tools.Memory;

namespace Spect.Net.VsPackage.Tools.TzxExplorer
{
    /// <summary>
    /// This class implements the view model for the standard speed TZX data block
    /// </summary>
    public class TzxStandardSpeedBlockViewModel : TzxBlockViewModelBase
    {
        private ushort _pauseAfter;
        private ushort _dataLength;
        private byte[] _data;

        [Description("Pause after this block (in milliseconds")]
        public ushort PauseAfter
        {
            get => _pauseAfter;
            set => Set(ref _pauseAfter, value);
        }

        [Description("Length of data in this block")]
        public ushort DataLenght
        {
            get => _dataLength;
            set => Set(ref _dataLength, value);
        }

        [Description("Data bytes of the block")]
        public byte[] Data
        {
            get => _data;
            set => Set(ref _data, value);
        }

        public ObservableCollection<MemoryLineViewModel> MemoryLines { get; } = new ObservableCollection<MemoryLineViewModel>();

        public TzxStandardSpeedBlockViewModel()
        {
            BlockId = 0x10;
            BlockType = "Standard Speed Data Block";
        }

        public void FromDataBlock(TzxStandardSpeedDataBlock block)
        {
            PauseAfter = block.PauseAfter;
            DataLenght = block.DataLenght;
            Data = block.Data;
            for (var addr = 0; addr < DataLenght + 16; addr += 16)
            {
                var memLine = new MemoryLineViewModel(addr, DataLenght - 1);
                memLine.BindTo(Data);
                MemoryLines.Add(memLine);
            }
        }
    }
}