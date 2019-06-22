using System;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;

namespace Spect.Net.SpectrumEmu.Providers
{
    public class DefaultTapeProvider: VmComponentProviderBase, ITapeProvider
    {
        public const string RESOURCE_FOLDER = "TzxResources";
        public const string DEFAULT_SAVE_FILE_DIR = @"C:\Temp\ZxSpectrumSavedFiles";
        public const string DEFAULT_NAME = "SavedFile";
        public const string DEFAULT_EXT = ".tzx";
        private string _suggestedName;
        private string _fullFileName;
        private int _dataBlockCount;

        /// <summary>
        /// The directory files should be saved to
        /// </summary>
        public string SaveFileFolder { get; }

        /// <summary>
        /// The assembly to search for resources
        /// </summary>
        public Assembly ResourceAssembly { get; }

        public DefaultTapeProvider(Assembly resourceAssembly, string saveFolder = null)
        {
            ResourceAssembly = resourceAssembly;
            SaveFileFolder = string.IsNullOrWhiteSpace(saveFolder) 
                ? DEFAULT_SAVE_FILE_DIR
                : saveFolder;
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
        /// Tha tape set to load the content from
        /// </summary>
        public string TapeSetName { get; set; }

        /// <summary>
        /// Gets a binary reader that provider TZX content
        /// </summary>
        /// <returns>BinaryReader instance to obtain the content from</returns>
        public BinaryReader GetTapeContent()
        {
            var resMan = GetFileResource(ResourceAssembly, TapeSetName);
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream {TapeSetName} not found.");
            }
            var reader = new BinaryReader(resMan);
            return reader;
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

        /// <summary>
        /// Obtains the specified resource stream ot the given assembly
        /// </summary>
        /// <param name="asm">Assembly to get the resource stream from</param>
        /// <param name="resourceName">Resource name</param>
        private static Stream GetFileResource(Assembly asm, string resourceName)
        {
            var resourceFullName = $"{asm.GetName().Name}.{RESOURCE_FOLDER}.{resourceName}";
            return asm.GetManifestResourceStream(resourceFullName);
        }
    }
}