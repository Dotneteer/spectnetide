using System;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class ResourceRomProvider: IRomProvider
    {
        private const string RESOURCE_FOLDER = "Roms";

        public byte[] ExtractResourceFile(string resourceName)
        {
            var callingAsm = Assembly.GetCallingAssembly();
            var resMan = GetFileResource(callingAsm, resourceName);
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

        private Stream GetFileResource(Assembly asm, string resourceName)
        {
            var resourceFullName = $"{asm.GetName().Name}.{RESOURCE_FOLDER}.{resourceName}";
            return asm.GetManifestResourceStream(resourceFullName);
        }

        /// <summary>
        /// Gets the content of the ROM specified by its resource name
        /// </summary>
        /// <param name="romResourceName">ROM resource name</param>
        /// <returns>Content of the ROM</returns>
        public byte[] LoadRom(string romResourceName)
        {
            return ExtractResourceFile(romResourceName);
        }
    }
}