using System.ComponentModel;

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

        public TzxStandardSpeedBlockViewModel()
        {
            BlockId = 0x10;
            BlockType = "Standard Speed Data Block";
        }
    }
}