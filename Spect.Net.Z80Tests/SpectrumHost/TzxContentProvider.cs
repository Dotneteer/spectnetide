using System;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Devices;

namespace Spect.Net.Z80Tests.SpectrumHost
{
    /// <summary>
    /// TZX Tape content provider
    /// </summary>
    public class TzxContentProvider: ITzxTapeContentProvider
    {
        /// <summary>
        /// Folder for the test TZX files
        /// </summary>
        private const string RESOURCE_FOLDER = "TzxResources";

        /// <summary>
        /// Resets the tape content
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Gets a binary reader that provider TZX content
        /// </summary>
        /// <returns></returns>
        public BinaryReader GetTzxContent()
        {
            const string RESOURCE_NAME = "JetSetWilly.tzx";
            var callingAsm = Assembly.GetExecutingAssembly();
            var resMan = GetFileResource(callingAsm, RESOURCE_NAME);
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream {RESOURCE_NAME} not found.");
            }
            var reader = new BinaryReader(resMan);
            return reader;
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