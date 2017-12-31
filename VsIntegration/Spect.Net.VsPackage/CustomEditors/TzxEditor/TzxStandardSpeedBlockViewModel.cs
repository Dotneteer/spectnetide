using System.ComponentModel;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.VsPackage.CustomEditors.RomEditor;
using Spect.Net.VsPackage.ToolWindows.BasicList;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// This class implements the view model for the standard speed TZX data block
    /// </summary>
    public class TzxStandardSpeedBlockViewModel : TapeBlockViewModelBase
    {
        private ushort _pauseAfter;
        private ushort _dataLength;
        private byte[] _data;
        private bool _isProgramDataBlock;
        private bool _showProgram;
        private BasicListViewModel _programList;
        private MemoryViewModel _memory;

        private string _headerType;
        private bool _isHeaderBlock;
        private string _dataType;
        private string _filename;
        private ushort _dataBlockBytes;
        private int _autoStartLine;
        private bool _hasAutoStartLine;
        private int _variableOffset;
        private bool _hasVariablesOffset;

        public MemoryViewModel Memory
        {
            get => _memory;
            set => Set(ref _memory, value);
        }

        [Description("Pause after this block (in milliseconds")]
        public ushort PauseAfter
        {
            get => _pauseAfter;
            set => Set(ref _pauseAfter, value);
        }

        [Description("Length of data in this block")]
        public ushort DataLength
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

        [Description("Type of the header")]
        public string HeaderType
        {
            get => _headerType;
            set => Set(ref _headerType, value);
        }

        [Description("Indicates if the current block is a header")]
        public bool IsHeaderBlock
        {
            get => _isHeaderBlock;
            set => Set(ref _isHeaderBlock, value);
        }

        [Description("The data block type")]
        public string DataType
        {
            get => _dataType;
            set => Set(ref _dataType, value);
        }

        /// <summary>
        /// Indicates if the currently selected block is a program
        /// </summary>
        public bool IsProgramDataBlock
        {
            get => _isProgramDataBlock;
            set => Set(ref _isProgramDataBlock, value);
        }

        /// <summary>
        /// Indicates that the Basic program is shown
        /// </summary>
        public bool ShowProgram
        {
            get => _showProgram;
            set => Set(ref _showProgram, value);
        }

        /// <summary>
        /// Gets the program list associated with this block
        /// </summary>
        public BasicListViewModel ProgramList
        {
            get => _programList;
            set => Set(ref _programList, value);
        }

        [Description("Filename in the header")]
        public string Filename
        {
            get => _filename;
            set => Set(ref _filename, value);
        }

        [Description("#of bytes in the data block that follows the header")]
        public ushort DataBlockBytes
        {
            get => _dataBlockBytes;
            set => Set(ref _dataBlockBytes, value);
        }

        [Description("The autostart line of a program")]
        public int AutoStartLine
        {
            get => _autoStartLine;
            set => Set(ref _autoStartLine, value);
        }

        [Description("Indicates, if the program has an auto start line number")]
        public bool HasAutoStartLine
        {
            get => _hasAutoStartLine;
            set => Set(ref _hasAutoStartLine, value);
        }

        [Description("The offset of the variables start area after the code")]
        public int VariablesOffset
        {
            get => _variableOffset;
            set => Set(ref _variableOffset, value);
        }

        [Description("Indicates, if the program has variables offset")]
        public bool HasVariablesOffset
        {
            get => _hasVariablesOffset;
            set => Set(ref _hasVariablesOffset, value);
        }

        public TzxStandardSpeedBlockViewModel()
        {
            BlockId = 0x10;
            BlockType = "Standard Speed Data Block";
            if (!IsInDesignMode) return;

            HeaderType = "Header";
            IsHeaderBlock = true;
            DataType = "Program";
            Filename = "Pac-Man";
            DataBlockBytes = 100;
        }

        public void FromDataBlock(ITapeData block)
        {
            PauseAfter = block.PauseAfter;
            DataLength = (ushort)block.Data.Length;
            Data = block.Data;
            Memory = new MemoryViewModel
            {
                MemoryBuffer = Data,
                ShowPrompt = DataLength > 0x14,
                AllowDisassembly = DataLength > 0x14
            };

            // --- Analyze the header's contents
            IsHeaderBlock = Data[0] == 0x00;
            HeaderType = IsHeaderBlock ? "Header" : "Data";
            if (IsHeaderBlock)
            {
                var dataType = Data[1];
                switch (dataType)
                {
                    case 0:
                        DataType = "Program";
                        break;
                    case 1:
                        DataType = "Number array";
                        break;
                    case 2:
                        DataType = "Character array";
                        break;
                    case 3:
                        DataType = "Code";
                        break;
                    default:
                        DataType = "Unknown";
                        break;
                }
                Filename = TzxDataBlockBase.ToAsciiString(Data, 2, 10).TrimEnd();
                DataBlockBytes = (ushort) (Data[12] + (Data[13] << 8));
                AutoStartLine = (ushort)(Data[14] + (Data[15] << 8));
                HasAutoStartLine = AutoStartLine < 32768;
                VariablesOffset = (ushort)(Data[16] + (Data[17] << 8));
                HasVariablesOffset = true;
            }
        }
    }
}