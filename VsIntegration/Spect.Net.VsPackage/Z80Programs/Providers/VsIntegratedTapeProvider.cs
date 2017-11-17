using System;
using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Z80Programs.Providers
{
    public class VsIntegratedTapeProvider : VmComponentProviderBase, ITapeProvider
    {
        public const string DEFAULT_SAVE_FILE_DIR = @"C:\Temp\ZxSpectrumSavedFiles";
        public const string DEFAULT_NAME = "SavedFile";
        public const string DEFAULT_EXT = ".tzx";
        private string _suggestedName;
        private string _fullFileName;
        private int _dataBlockCount;
        private readonly SpectNetPackage _package;

        /// <summary>
        /// The directory files should be saved to
        /// </summary>
        public string SaveFileFolder => string.IsNullOrWhiteSpace(_package.Options.SaveFileFolder)
            ? DEFAULT_SAVE_FILE_DIR
            : _package.Options.SaveFileFolder;

        /// <summary>
        /// Tha tape set to load the content from
        /// </summary>
        public string TapeSetName { get; set; }

        /// <summary>
        /// Initializes the provider
        /// </summary>
        /// <param name="package">Package instance</param>
        public VsIntegratedTapeProvider(SpectNetPackage package)
        {
            _package = package;
        }

        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public override void Reset()
        {
            _dataBlockCount = 0;
            _suggestedName = null;
            _fullFileName = null;
        }

        /// <summary>
        /// Gets a binary reader that provider TZX content
        /// </summary>
        /// <returns>BinaryReader instance to obtain the content from</returns>
        public BinaryReader GetTapeContent()
        {
            var solution = VsxPackage.GetPackage<SpectNetPackage>().CodeDiscoverySolution;
            var filename = solution?.CurrentProject?.DefaultTapeItem?.Filename
                           ?? solution?.CurrentTzxItem?.Filename
                           ?? solution?.CurrentTapItem?.Filename;
            return filename == null
                ? null
                : new BinaryReader(File.OpenRead(filename));
        }

        /// <summary>
        /// Creates a tape file with the specified name
        /// </summary>
        /// <returns></returns>
        public void CreateTapeFile()
        {
            Reset();
        }

        /// <summary>
        /// This method sets the name of the file according to the 
        /// Spectrum SAVE HEADER information
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            _suggestedName = name;
        }

        /// <summary>
        /// Appends the TZX block to the tape file
        /// </summary>
        /// <param name="block"></param>
        public void SaveTapeBlock(ITapeDataSerialization block)
        {
            if (_dataBlockCount == 0)
            {
                if (!Directory.Exists(SaveFileFolder))
                {
                    Directory.CreateDirectory(SaveFileFolder);
                }
                var baseFileName = $"{_suggestedName ?? DEFAULT_NAME}_{DateTime.Now:yyyyMMdd_HHmmss}{DEFAULT_EXT}";
                _fullFileName = Path.Combine(SaveFileFolder, baseFileName);
                using (var writer = new BinaryWriter(File.Create(_fullFileName)))
                {
                    var header = new TzxHeader();
                    header.WriteTo(writer);
                }
            }
            _dataBlockCount++;

            var stream = File.Open(_fullFileName, FileMode.Append);
            using (var writer = new BinaryWriter(stream))
            {
                block.WriteTo(writer);
            }
        }

        /// <summary>
        /// The tape provider can finalize the tape when all 
        /// TZX blocks are written.
        /// </summary>
        public void FinalizeTapeFile()
        {
        }
    }
}