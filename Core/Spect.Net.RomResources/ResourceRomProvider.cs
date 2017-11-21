using System;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.RomResources
{
    /// <summary>
    /// This provider allows to obtain a ZX Spectrum ROM from
    /// an embedded resource.
    /// </summary>
    /// <remarks>
    /// The resource should be embedded into the calling assembly.
    /// </remarks>
    public class ResourceRomProvider : VmComponentProviderBase, IRomProvider
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
            ResourceAssembly = resourceAssembly ?? GetType().Assembly;
        }

        /// <summary>
        /// The folder where the ROM files are stored
        /// </summary>
        private const string RESOURCE_FOLDER = "Roms";

        /// <summary>
        /// Loads the binary contents of the ROM.
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>Binary contents of the ROM</returns>
        public byte[] LoadRomBytes(string romName, int page = -1)
        {
            var fullRomName = (HostVm?.RomConfiguration?.NumberOfRoms ?? 1) == 1 || page == -1
                ? romName
                : $"{romName}-{page}";
            var resMan = GetFileResource(ResourceAssembly, fullRomName, ".rom");
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream for the '{fullRomName}' .rom file not found.");
            }
            using (var stream = new StreamReader(resMan).BaseStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        /// <summary>
        /// Loads the annotations of the ROM.
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>Annotations of the ROM in serialized format</returns>
        public string LoadRomAnnotations(string romName, int page = -1)
        {
            var fullRomName = (HostVm?.RomConfiguration?.NumberOfRoms ?? 1) == 1 || page == -1
                ? romName
                : $"{romName}-{page}";
            var resMan = GetFileResource(ResourceAssembly, fullRomName, ".disann");
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream for the '{fullRomName}' .disann file not found.");
            }
            using (var reader = new StreamReader(resMan))
            {
                return reader.ReadToEnd();
            }
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
    }
}