using System;
using System.IO;
using System.Reflection;

namespace Spect.Net.Z80DisAsm.Test.Helpers
{
    public static class FileHelper
    {
        private const string RESOURCE_FOLDER = "Roms";

        public static byte[] ExtractResourceFile(string resourceName)
        {
            var resMan = GetFileResource(resourceName);
            if (resMan == null)
            {
                throw new InvalidOperationException($"Iput stream {resourceName} not found");
            }
            using (var stream = new StreamReader(resMan).BaseStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        private static Stream GetFileResource(string resourceName)
        {
            var resourceFullName = $"{Assembly.GetExecutingAssembly().GetName().Name}.{RESOURCE_FOLDER}.{resourceName}";
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceFullName);
        }
    }
}