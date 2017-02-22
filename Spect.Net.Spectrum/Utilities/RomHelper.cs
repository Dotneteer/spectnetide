using System;
using System.IO;
using System.Reflection;

namespace Spect.Net.Spectrum.Utilities
{
    public static class RomHelper
    {
        private const string RESOURCE_FOLDER = "Roms";

        public static byte[] ExtractResourceFile(string resourceName)
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

        private static Stream GetFileResource(Assembly asm, string resourceName)
        {
            var resourceFullName = $"{asm.GetName().Name}.{RESOURCE_FOLDER}.{resourceName}";
            return asm.GetManifestResourceStream(resourceFullName);
        }
    }
}