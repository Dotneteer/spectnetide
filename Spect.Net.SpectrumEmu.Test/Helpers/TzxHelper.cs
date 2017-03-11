using System;
using System.IO;
using System.Reflection;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class TzxHelper
    {
        private const string RESOURCE_FOLDER = "TzxResources";

        public static BinaryReader GetResourceReader(string resourceName)
        {
            var callingAsm = Assembly.GetCallingAssembly();
            var resMan = GetFileResource(callingAsm, resourceName);
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream {resourceName} not found.");
            }
            var reader = new BinaryReader(resMan);
            return reader;
        }

        private static Stream GetFileResource(Assembly asm, string resourceName)
        {
            var resourceFullName = $"{asm.GetName().Name}.{RESOURCE_FOLDER}.{resourceName}";
            return asm.GetManifestResourceStream(resourceFullName);
        }
    }
}