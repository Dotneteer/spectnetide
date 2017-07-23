using System;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.Wpf.Providers
{
    /// <summary>
    /// This provider reads TZX files from embedded resources in an assembly
    /// </summary>
    public class TzxEmbeddedResourceLoadContentProvider: ITzxLoadContentProvider
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
        /// Gets a binary reader that provider TZX content
        /// </summary>
        /// <returns>BinaryReader instance to obtain the content from</returns>
        public BinaryReader GetTzxContent()
        {
            //const string RESOURCE_NAME = "JetSetWilly.tzx";
            //const string RESOURCE_NAME = "JungleTrouble.tzx";
            //const string RESOURCE_NAME = "Pac-Man.tzx";
            const string RESOURCE_NAME = "Border.tzx";
            var resMan = GetFileResource(ResourceAssembly, RESOURCE_NAME);
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