using System;
using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;

namespace Spect.Net.VsPackage.Machines
{
    /// <summary>
    /// This class implements the tape save provider for a SpectNetIDE project
    /// </summary>
    public class VsIntegratedTapeSaveProvider : VmComponentProviderBase, ITapeSaveProvider
    {
        public const string DEFAULT_SAVE_FILE_DIR = @"C:\Temp\ZxSpectrumSavedFiles";
        public const string DEFAULT_NAME = "SavedFile";
        public const string DEFAULT_EXT = ".tzx";
        private string _suggestedName;
        private string _fullFileName;
        private int _dataBlockCount;

        /// <summary>
        /// The directory files should be saved to
        /// </summary>
        public string SaveFileFolder => string.IsNullOrWhiteSpace(SpectNetPackage.Default.Options.SaveFileFolder)
            ? DEFAULT_SAVE_FILE_DIR
            : SpectNetPackage.Default.Options.SaveFileFolder;

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
        /// Creates a tape file with the specified name.
        /// </summary>
        /// <returns></returns>
        public void CreateTapeFile()
        {
            Reset();
        }

        /// <summary>
        /// This method sets the name of the file according to the 
        /// Spectrum SAVE HEADER information.
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            _suggestedName = name;
        }

        /// <summary>
        /// Appends the tape block to the tape file.
        /// </summary>
        /// <param name="block">Tape block</param>
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
        /// Gets the full file name when the file was saved.
        /// </summary>
        /// <returns>Full file name</returns>
        public string GetFullFileName()
        {
            return _fullFileName;
        }
    }
}