using System;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.Wpf.Providers
{
    /// <summary>
    /// This provider reads TZX files from embedded resources in an assembly
    /// </summary>
    public class TzxEmbeddedResourceLoadContentProvider: ITapeContentProvider
    {
        /// <summary>
        /// Folder for the test TZX files
        /// </summary>
        private const string RESOURCE_FOLDER = "TzxResources";

        /// <summary>
        /// The assembly to search for resources
        /// </summary>
        public  Assembly ResourceAssembly { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TzxEmbeddedResourceLoadContentProvider(Assembly resourceAssembly)
        {
            ResourceAssembly = resourceAssembly;
        }

        /// <summary>
        /// Resets the tape content
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Tha tape set to load the content from
        /// </summary>
        public string TapeSetName { get; set; }

        /// <summary>
        /// Gets a binary reader that provider TZX content
        /// </summary>
        /// <returns>BinaryReader instance to obtain the content from</returns>
        public BinaryReader GetTapeContent()
        {
            var resMan = GetFileResource(ResourceAssembly, TapeSetName);
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream {TapeSetName} not found.");
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