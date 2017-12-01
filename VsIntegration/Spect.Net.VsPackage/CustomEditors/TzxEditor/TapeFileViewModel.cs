using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Devices.Tape.Tap;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.VsPackage.ToolWindows.BasicList;
using Spect.Net.VsPackage.ToolWindows.TapeFileExplorer;
using Spect.Net.VsPackage.Z80Programs.Providers;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// This class represents the view model of the TZX Explorer tool window
    /// </summary>
    public class TapeFileViewModel: EnhancedViewModelBase
    {
        private ObservableCollection<TapeBlockViewModelBase> _blocks;

        /// <summary>
        /// The data blocks of the TZX file
        /// </summary>
        public ObservableCollection<TapeBlockViewModelBase> Blocks
        {
            get => _blocks;
            set => Set(ref _blocks, value);
        }

        /// <summary>
        /// Gets the selected block
        /// </summary>
        public TapeBlockViewModelBase SelectedBlock => _blocks.FirstOrDefault(b => b.IsSelected);

        /// <summary>
        /// Represents the event when a TZX block has been selected
        /// </summary>
        public event EventHandler<TzxBlockSelectedEventArgs> TzxBlockSelected;

        /// <summary>
        /// Command executed when a block is selected
        /// </summary>
        public RelayCommand BlockSelectedCommand { get; }

        public TapeFileViewModel()
        {
            Blocks = new ObservableCollection<TapeBlockViewModelBase>();
            BlockSelectedCommand = new RelayCommand(OnBlockSelected);

            if (!IsInDesignMode) return;

            // ReSharper disable once UseObjectOrCollectionInitializer
            var provider = new VsIntegratedTapeProvider();
            provider.TapeSetName = "Pac-Man.tzx";
            ReadFrom(provider.GetTapeContent());
        }

        /// <summary>
        /// A block has been selected in the block list
        /// </summary>
        private void OnBlockSelected()
        {
            TzxBlockSelected?.Invoke(this, new TzxBlockSelectedEventArgs(SelectedBlock));
        }

        /// <summary>
        /// Reads the content of the TZX file from the specified binary reader
        /// </summary>
        /// <param name="binaryReader">Reader to get the contents</param>
        public void ReadFrom(BinaryReader binaryReader)
        {
            // --- First, let's try with .TZX files
            Blocks.Clear();
            var tzxReader = new TzxReader(binaryReader);
            var found = false;
            try
            {
                found = tzxReader.ReadContent();
            }
            catch (Exception)
            {
                // --- This exception is intentionally ignored
            }

            if (found)
            {
                CollectTzxInfo(tzxReader);
                return;
            }

            // --- Second, let's try .TAP files
            binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
            var tapReader = new TapReader(binaryReader);
            tapReader.ReadContent();

            foreach (var block in tapReader.DataBlocks)
            {
                var spBlockVm = new StandardDataBlockViewModel();
                spBlockVm.FromDataBlock(block);
                if (Blocks.Count > 0)
                {
                    var prevBlock = _blocks[Blocks.Count - 1] as StandardDataBlockViewModel;
                    var isProgram = spBlockVm.IsProgramDataBlock =
                        prevBlock != null
                        && prevBlock.Data[0] == 0x00
                        && prevBlock.Data[1] == 0x00;
                    if (isProgram)
                    {
                        spBlockVm.ProgramList = new BasicListViewModel(spBlockVm.Data, 0x0001,
                            (ushort)(prevBlock.VariablesOffset - 1));
                        spBlockVm.ProgramList.DecodeBasicProgram();
                    }
                }
                Blocks.Add(spBlockVm);
            }
        }

        /// <summary>
        /// Colects .TZX info from the specified TzxReader
        /// </summary>
        /// <param name="tzxReader">TzxReader to collect the info from</param>
        private void CollectTzxInfo(TzxReader tzxReader)
        {
            Blocks.Add(new TzxHeaderBlockViewModel
            {
                MajorVersion = tzxReader.MajorVersion,
                MinorVersion = tzxReader.MinorVersion
            });

            foreach (var block in tzxReader.DataBlocks)
            {
                TapeBlockViewModelBase blockVm;
                switch (block.BlockId)
                {
                    case 0x10:
                        var spBlockVm = new TzxStandardSpeedBlockViewModel();
                        spBlockVm.FromDataBlock((TzxStandardSpeedDataBlock) block);
                        blockVm = spBlockVm;
                        if (Blocks.Count > 0)
                        {
                            var prevBlock = _blocks[Blocks.Count - 1] as TzxStandardSpeedBlockViewModel;
                            var isProgram = spBlockVm.IsProgramDataBlock =
                                prevBlock != null
                                && prevBlock.Data[0] == 0x00
                                && prevBlock.Data[1] == 0x00;
                            if (isProgram)
                            {
                                spBlockVm.ProgramList = new BasicListViewModel(spBlockVm.Data, 0x0001,
                                    (ushort) (prevBlock.VariablesOffset - 1));
                                spBlockVm.ProgramList.DecodeBasicProgram();
                            }
                        }
                        break;

                    case 0x30:
                        var txtBlock = (TzxTextDescriptionDataBlock) block;
                        blockVm = new TzxTextDescriptionBlockViewModel
                        {
                            Text = TzxDataBlockBase.ToAsciiString(txtBlock.Description)
                        };
                        break;

                    case 0x32:
                        var archvm = new TzxArchiveInfoViewModel();
                        archvm.FromDataBlock((TzxArchiveInfoDataBlock) block);
                        blockVm = archvm;
                        break;

                    default:
                        blockVm = new TzxOtherBlockViewModel
                        {
                            BlockId = block.BlockId
                        };
                        break;
                }
                Blocks.Add(blockVm);
            }
        }

        /// <summary>
        /// Reads the content of the TZX file from the specified file
        /// </summary>
        /// <param name="filename">File name to read</param>
        public virtual void ReadFrom(string filename)
        {
            var binaryReader = new BinaryReader(File.Open(filename, FileMode.Open));
            ReadFrom(binaryReader);
        }
    }
}