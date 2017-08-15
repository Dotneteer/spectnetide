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

        /// <summary>
        /// Restores the annotations from the ROM annotation and current project
        /// annotation files.
        /// </summary>
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

        /// <summary>
        /// Saves the contents of annotations
        /// </summary>
        public void SaveAnnotations()
        {
            if (AnnotationInstance == null || CustomAnnotationFile == null) return;

            var annotationData = AnnotationInstance.Serialize();
            File.WriteAllText(CustomAnnotationFile, annotationData);
        }

        /// <summary>
        /// Stores the label in the annotations
        /// </summary>
        /// <param name="address">Label address</param>
        /// <param name="label">Label text</param>
        public void SetLabel(ushort address, string label)
        {
            AnnotationInstance.SetLabel(address, label);
            SaveAnnotations();
        }

        /// <summary>
        /// Stores a comment in annotations
        /// </summary>
        /// <param name="address">Comment address</param>
        /// <param name="comment">Comment text</param>
        public void SetComment(ushort address, string comment)
        {
            AnnotationInstance.SetComment(address, comment);
            SaveAnnotations();
        }

        /// <summary>
        /// Stores a prefix name in this collection
        /// </summary>
        /// <param name="address">Comment address</param>
        /// <param name="comment">Comment text</param>
        public void SetPrefixComment(ushort address, string comment)
        {
            AnnotationInstance.SetPrefixComment(address, comment);
            SaveAnnotations();
        }

    }
}