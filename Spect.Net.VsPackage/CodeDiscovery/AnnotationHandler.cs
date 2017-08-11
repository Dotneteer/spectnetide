using System;
using System.IO;
using EnvDTE;
using Newtonsoft.Json;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.CodeDiscovery
{
    /// <summary>
    /// This class is responsible for handling the annotations within
    /// a code discovery project
    /// </summary>
    public class AnnotationHandler: IDisposable
    {
        /// <summary>
        /// The project associated with this annotation handler
        /// </summary>
        public Project Project { get; }

        public ProjectItem DisAnnItem { get; }

        public string DisAnnFileName { get; }

        /// <summary>
        /// Th annotations associated with the project
        /// </summary>
        public DisassembyAnnotations Annotations { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public AnnotationHandler(Project project)
        {
            Project = project;

            // --- Identify key project files
            foreach (ProjectItem item in project.ProjectItems)
            {
                if (item.Kind == VsHierarchyTypes.DisannItem)
                {
                    DisAnnItem = item;
                    DisAnnFileName = item.FileNames[0];
                }
            }

            ReloadAnnotations();
        }

        /// <summary>
        /// Realoads the annotations from the Annotations.disann project file
        /// </summary>
        public void ReloadAnnotations()
        {
            if (DisAnnItem == null) return;
            try
            {
                var disAnnText = File.ReadAllText(DisAnnFileName);
                Annotations = JsonConvert.DeserializeObject<DisassembyAnnotations>(disAnnText);
            }
            catch
            {
                // --- This exception is intentionally ignored
            }
            if (Annotations == null)
            {
                // --- Falback: let's fill up the annotation file with valid data
                Annotations = new DisassembyAnnotations();
                SaveAnnotations();
            }
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}