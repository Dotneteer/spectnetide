using System.IO;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// This class is responsible for managing disassembly annotations
    /// </summary>
    public class DisassemblyAnnotationHandler
    {
        /// <summary>
        /// The instance 
        /// </summary>
        public DisassemblyAnnotation AnnotationInstance { get; set; }

        /// <summary>
        /// Gets the name of the file that contains ROM annotations
        /// </summary>
        public string RomAnnotationFile { get; }

        /// <summary>
        /// Gets the name of the file that contains custom annotations
        /// </summary>
        /// <remarks>User annotations are always saved to this file.</remarks>
        public string CustomAnnotationFile { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DisassemblyAnnotationHandler(DisassemblyAnnotation annotationInstance, 
            string romAnnotationFile, string customAnnotationFile)
        {
            AnnotationInstance = annotationInstance;
            RomAnnotationFile = romAnnotationFile;
            CustomAnnotationFile = customAnnotationFile;
        }

        public void RestoreAnnotations()
        {
            if (RomAnnotationFile != null)
            {
                var romSerialized = File.ReadAllText(RomAnnotationFile);
                AnnotationInstance.Merge(DisassemblyAnnotation.Deserialize(romSerialized));
            }
            if (CustomAnnotationFile != null)
            {
                var customSerialized = File.ReadAllText(CustomAnnotationFile);
                AnnotationInstance.Merge(DisassemblyAnnotation.Deserialize(customSerialized));
            }
        }
    }
}