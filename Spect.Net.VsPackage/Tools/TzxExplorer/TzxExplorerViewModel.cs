using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.Providers;

namespace Spect.Net.VsPackage.Tools.TzxExplorer
{
    /// <summary>
    /// This class represents the view model of the TZX Explorer tool window
    /// </summary>
    public class TzxExplorerViewModel: EnhancedViewModelBase
    {
        private string _fileName;
        private string _latestPath;
        private bool _loaded;

        private ObservableCollection<TzxBlockViewModelBase> _blocks;

        /// <summary>
        /// The full name of the selected TZX file
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set => Set(ref _fileName, value);
        }

        /// <summary>
        /// The last path used to select a TZX file
        /// </summary>
        public string LatestPath
        {
            get => _latestPath;
            set => Set(ref _latestPath, value);
        }

        /// <summary>
        /// The data blocks of the TZX file
        /// </summary>
        public ObservableCollection<TzxBlockViewModelBase> Blocks
        {
            get => _blocks;
            set => Set(ref _blocks, value);
        }

        /// <summary>
        /// Indicates if a file is loaded
        /// </summary>
        public bool Loaded
        {
            get => _loaded;
            set => Set(ref _loaded, value);
        }

        public RelayCommand BlockSelectedCommand { get; }

        public TzxExplorerViewModel()
        {
            FileName = null;
            LatestPath = null;
            Blocks = new ObservableCollection<TzxBlockViewModelBase>();
            BlockSelectedCommand = new RelayCommand(OnBlockSelected);

            if (!IsInDesignMode) return;

            // ReSharper disable once UseObjectOrCollectionInitializer
            var provider = new TzxEmbeddedResourceLoadContentProvider(Assembly.GetExecutingAssembly());
            provider.TapeSetName = FileName = "Pac-Man.tzx";
            ReadFrom(provider.GetTzxContent());
        }

        /// <summary>
        /// Gets the selected block
        /// </summary>
        public TzxBlockViewModelBase SelectedBlock => _blocks.FirstOrDefault(b => b.IsSelected);

        /// <summary>
        /// A block has been selected in the block list
        /// </summary>
        private void OnBlockSelected()
        {
            var selected = SelectedBlock;
        }

        /// <summary>
        /// Reads the content of the TZX file from the specified binary reader
        /// </summary>
        /// <param name="binaryReader">Reader to get the contents</param>
        public void ReadFrom(BinaryReader binaryReader)
        {
            var tzxContent = new TzxReader(binaryReader);
            tzxContent.ReadContent();

            // --- Move TZX data into the view model
            Blocks.Clear();
            Blocks.Add(new TzxHeaderBlockViewModel
            {
                MajorVersion = tzxContent.MajorVersion,
                MinorVersion = tzxContent.MinorVersion
            });

            foreach (var block in tzxContent.DataBlocks)
            {
                TzxBlockViewModelBase blockVm;
                switch (block.BlockId)
                {
                    case 0x10:
                        var spBlock = (TzxStandardSpeedDataBlock)block;
                        blockVm = new TzxStandardSpeedBlockViewModel
                        {
                            PauseAfter = spBlock.PauseAfter,
                            DataLenght = spBlock.DataLenght,
                            Data = spBlock.Data
                        };
                        break;

                    case 0x30:
                        var txtBlock = (TzxTextDescriptionDataBlock)block;
                        blockVm = new TzxTextDescriptionBlockViewModel
                        {
                            Text = TzxDataBlockBase.ToAsciiString(txtBlock.Description)
                        };
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
            Loaded = true;
        }

        /// <summary>
        /// Reads the content of the TZX file from the specified file
        /// </summary>
        /// <param name="filename">File name to read</param>
        public void ReadFrom(string filename)
        {
            var binaryReader = new BinaryReader(File.Open(filename, FileMode.Open));
            ReadFrom(binaryReader);
        }
    }
}