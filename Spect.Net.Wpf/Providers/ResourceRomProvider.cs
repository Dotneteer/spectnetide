using System;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.Wpf.Providers
{
    /// <summary>
    /// This provider allows to obtain a ZX Spectrum ROM from
    /// an embedded resource.
    /// </summary>
    /// <remarks>
    /// The resource should be embedded into the calling assembly.
    /// </remarks>
    public class ResourceRomProvider: IRomProvider
    {
        /// <summary>
        /// The assembly to check for resources
        /// </summary>
        public Assembly ResourceAssembly { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public ResourceRomProvider(Assembly resourceAssembly = null)
        {
            ResourceAssembly = resourceAssembly ?? typeof(Spectrum48).Assembly;
        }

        /// <summary>
        /// The folder where the ROM files are stored
        /// </summary>
        private const string RESOURCE_FOLDER = "Roms";

        /// <summary>
        /// Gets the content of the ROM specified by its resource name
        /// </summary>
        /// <param name="romResourceName">ROM resource name</param>
        /// <returns>Content of the ROM</returns>
        public RomInfo LoadRom(string romResourceName)
        {
            RomInfo result;

            // --- Obtain the ROM annotations
            var resMan = GetFileResource(ResourceAssembly, romResourceName, ".romann");
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream for the '{romResourceName}' .romann file not found.");
            }
            using (var reader = new StreamReader(resMan))
            {
                var serialized = reader.ReadToEnd();
                result = RomInfo.DeserializeFromJson(serialized);
            }


            // --- Obtain the ROM contents
            resMan = GetFileResource(ResourceAssembly, romResourceName, ".rom");
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream for the '{romResourceName}' .rom file not found.");
            }
            using (var stream = new StreamReader(resMan).BaseStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                result.RomBytes = bytes;
            }

            return result;
        }

        /// <summary>
        /// Gets the full name of the resource
        /// </summary>
        /// <param name="asm">Resource assembly</param>
        /// <param name="resourceName">Resource name</param>
        /// <param name="extension">Resource extension name</param>
        /// <returns></returns>
        private static Stream GetFileResource(Assembly asm, string resourceName, string extension)
        {
            var resourceFullName = $"{asm.GetName().Name}.{RESOURCE_FOLDER}.{resourceName}.{resourceName}{extension}";
            return asm.GetManifestResourceStream(resourceFullName);
        }

        /// <summary>
        /// Nothing to do when the provider is reset
        /// </summary>
        public void Reset()
        {
        }
    }
}