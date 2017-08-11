using System;
using System.IO;
using Newtonsoft.Json;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.VsPackage.CodeDiscovery
{
    /// <summary>
    /// This class is responsible for handling the annotations within
    /// a code discovery project
    /// </summary>
    public class AnnotationHandler: IDisposable
    {
        public string DisAnnFileName { get; }

        /// <summary>
        /// Th annotations associated with the project
        /// </summary>
        public DisassembyAnnotations Annotations { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public AnnotationHandler(string disAnnFileName)
        {
            DisAnnFileName = disAnnFileName;
            ReloadAnnotations();
        }

        /// <summary>
        /// Realoads the annotations from the Annotations.disann project file
        /// </summary>
        public void ReloadAnnotations()
        {
            if (DisAnnFileName == null) return;
            try
            {
                var disAnnText = File.ReadAllText(DisAnnFileName);
                Annotations = JsonConvert.DeserializeObject<DisassembyAnnotations>(disAnnText);
            }
            catch
            {
                // --- This exception is intentionally ignored
            }
            if (Annotations != null) return;

            // --- Falback: let's fill up the annotation file with valid data
            Annotations = new DisassembyAnnotations();
            SaveAnnotations();
        }

        /// <summary>
        /// Saves the annotation file
        /// </summary>
        public void SaveAnnotations()
        {
            if (Annotations == null) return;
            try
            {
                var serialized = JsonConvert.SerializeObject(Annotations, Formatting.Indented);
                File.WriteAllText(DisAnnFileName, serialized);
            }
            catch
            {
                // --- This exception is intentionally ignored
            }
        }

        /// <summary>
        /// Adds a custom label to the annotations.
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <param name="label">Label name</param>
        /// <returns>
        /// True, if the label has been created, modified, or removed;
        /// otherwise; false.
        /// </returns>
        public bool AddCustomLabel(ushort addr, string label)
        {
            var result = Annotations.CreateCustomLabel(addr, label);
            if (result)
            {
                SaveAnnotations();
            }
            return result;
        }

        /// <summary>
        /// Adds a custom comment to the annotations.
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <param name="comment">Label name</param>
        /// <returns>
        /// True, if the comment has been created, modified, or removed;
        /// otherwise; false.
        /// </returns>
        public bool AddCustomComment(ushort addr, string comment)
        {
            var result = Annotations.CreateCustomComment(addr, comment);
            if (result)
            {
                SaveAnnotations();
            }
            return result;
        }

        /// <summary>
        /// Adds a custom prefix comment to the annotations.
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <param name="comment">Label name</param>
        /// <returns>
        /// True, if the comment has been created, modified, or removed;
        /// otherwise; false.
        /// </returns>
        public bool AddCustomPrefixComment(ushort addr, string comment)
        {
            var result = Annotations.CreateCustomPrefixComment(addr, comment);
            if (result)
            {
                SaveAnnotations();
            }
            return result;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}