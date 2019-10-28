using System;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Devices.Tape;

namespace Spect.Net.SpectrumEmu.Test.Devices.Tape
{
    /// <summary>
    /// This class provides helper functions for testing TzxPlayer
    /// </summary>
    public static class CommonTapeFilePlayerHelper
    {
        /// <summary>
        /// Folder for the test TAP files
        /// </summary>
        /// <summary>
        /// Creates a new player for the specified resource
        /// </summary>
        /// <param name="tapResource"></param>
        /// <returns></returns>
        public static CommonTapeFilePlayer CreatePlayer(string tapResource)
        {
            var tzxReader = GetResourceReader(tapResource);
            var player = new CommonTapeFilePlayer(tzxReader);
            player.ReadContent();
            return player;
        }

        /// <summary>
        /// Obtains the specified resource stream
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        private static BinaryReader GetResourceReader(string resourceName)
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

        /// <summary>
        /// Obtains the specified resource stream ot the given assembly
        /// </summary>
        /// <param name="asm">Assembly to get the resource stream from</param>
        /// <param name="resourceName">Resource name</param>
        private static Stream GetFileResource(Assembly asm, string resourceName)
        {
            var resourceFullName = $"{asm.GetName().Name}.{resourceName}";
            return asm.GetManifestResourceStream(resourceFullName);
        }
    }
}