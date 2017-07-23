using System;
using System.CodeDom;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.Wpf
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
        /// The folder where the ROM files are stored
        /// </summary>
        private const string RESOURCE_FOLDER = "Roms";

        /// <summary>
        /// Gets the content of the ROM specified by its resource name
        /// </summary>
        /// <param name="romResourceName">ROM resource name</param>
        /// <param name="asm">
        /// Assembly to check for the resource. If null, the calling assembly
        /// is used
        /// </param>
        /// <returns>Content of the ROM</returns>
        public byte[] LoadRom(string romResourceName, Assembly asm = null)
        {
            return ExtractResourceFile(romResourceName, asm ?? typeof(Spectrum48).Assembly);
        }

        /// <summary>
        /// Extracts the resource file from the calling assembly
        /// </summary>
        /// <param name="resourceName">The name of the resource</param>
        /// <param name="asm">Assembly to check for the resource</param>
        /// <returns>
        /// The contents of the ROM
        /// </returns>
        private byte[] ExtractResourceFile(string resourceName, Assembly asm)
        {
            var resMan = GetFileResource(asm, resourceName);
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream {resourceName} not found.");
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
        /// Gets the full name of the resource
        /// </summary>
        /// <param name="asm">Resource assembly</param>
        /// <param name="resourceName">Resource name</param>
        /// <returns></returns>
        private static Stream GetFileResource(Assembly asm, string resourceName)
        {
            var resourceFullName = $"{asm.GetName().Name}.{RESOURCE_FOLDER}.{resourceName}";
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