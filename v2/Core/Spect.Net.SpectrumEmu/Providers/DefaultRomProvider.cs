using System;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Providers
{
    /// <summary>
    /// This provider allows to obtain a ZX Spectrum ROM from
    /// an embedded resource.
    /// </summary>
    /// <remarks>
    /// The resource should be embedded into the calling assembly.
    /// </remarks>
    public class DefaultRomProvider : VmComponentProviderBase, IRomProvider
    {
        /// <summary>
        /// The folder where the ROM files are stored
        /// </summary>
        public const string RESOURCE_FOLDER = "Roms";

        /// <summary>
        /// The assembly to check for resources
        /// </summary>
        public Assembly ResourceAssembly { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public DefaultRomProvider(Assembly resourceAssembly = null)
        {
            ResourceAssembly = resourceAssembly ?? GetType().Assembly;
        }

        /// <summary>
        /// Loads the binary contents of the ROM.
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>Binary contents of the ROM</returns>
        public byte[] LoadRomBytes(string romName, int page = -1)
        {
            var resMan = GetFileResource(ResourceAssembly, romName, ".rom", page);
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream for the '{romName}' .rom file not found.");
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
            var resMan = GetFileResource(ResourceAssembly, romName, ".disann", page);
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream for the '{romName}' .disann file not found.");
            }
            using (var reader = new StreamReader(resMan))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets the resource name for the specified ROM annotation
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>ROM annotation resource name</returns>
        public string GetAnnotationResourceName(string romName, int page = -1)
        {
            return GetFullResourceName(ResourceAssembly, romName, ".disann", page);
        }

        /// <summary>
        /// Gets the full name of the resource
        /// </summary>
        /// <param name="asm">Resource assembly</param>
        /// <param name="resourceName">Resource name</param>
        /// <param name="extension">Resource extension name</param>
        /// <param name="page">ROM Page index</param>
        /// <returns></returns>
        private Stream GetFileResource(Assembly asm, string resourceName, string extension, int page)
        {
            var resourceFullName = GetFullResourceName(asm, resourceName, extension, page);
            return asm.GetManifestResourceStream(resourceFullName);
        }

        /// <summary>
        /// Gets the full resource name of the specified ROM resource
        /// </summary>
        /// <param name="asm">Assembly that contains the ROM resource</param>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="extension">Resource extension</param>
        /// <param name="page">ROM Page index</param>
        /// <returns>Full resource name</returns>
        private string GetFullResourceName(Assembly asm, string romName, string extension, int page)
        {
            var resourceName = (HostVm?.RomConfiguration?.NumberOfRoms ?? 1) == 1 || page == -1
                ? romName
                : $"{romName}-{page}";
            return $"{asm.GetName().Name}.{RESOURCE_FOLDER}.{romName}.{resourceName}{extension}";
        }
    }
}