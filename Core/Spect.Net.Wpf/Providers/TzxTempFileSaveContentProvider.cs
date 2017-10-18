using System;
using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;

namespace Spect.Net.Wpf.Providers
{
    public class TzxTempFileSaveContentProvider: ITzxSaveContentProvider
    {
        public const string SAVE_FILE_DIR = @"C:\Temp\ZxSpectrumSavedFiles";
        public const string DEFAULT_NAME = "SavedFile";
        public const string DEFAULT_EXT = ".tzx";
        private string _suggestedName;
        private string _fullFileName;
        private int _dataBlockCount;

        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public void Reset()
        {
            _dataBlockCount = 0;
            _suggestedName = null;
            _fullFileName = null;
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
        public void SaveTzxBlock(ITapeDataSerialization block)
        {
            if (_dataBlockCount == 0)
            {
                if (!Directory.Exists(SAVE_FILE_DIR))
                {
                    Directory.CreateDirectory(SAVE_FILE_DIR);
                }
                var baseFileName = $"{_suggestedName ?? DEFAULT_NAME}_{DateTime.Now:yyyyMMdd_HHmmss}{DEFAULT_EXT}";
                _fullFileName = Path.Combine(SAVE_FILE_DIR, baseFileName);
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