using System;
using System.IO;
using System.Linq;
using System.Windows;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Z80Programs.ExportMemory
{
    /// <summary>
    /// This class contains the code that carries out memory export
    /// </summary>
    public class MemoryExporter
    {
        private const string FILE_EXISTS_MESSAGE = "The memory export file exists in the project. " +
                                                   "Would you like to override it?";

        private const string INVALID_FOLDER_MESSAGE = "The memory export folder specified in the Options dialog " +
                                                      "contains invalid characters or an absolute path. Go to the Options dialog and " +
                                                      "fix the issue so that you can add the exported file to the project.";

        public MemoryExporter(ExportMemoryViewModel exportParams)
        {
            ExportParams = exportParams;
        }

        /// <summary>
        /// Memory export parameters
        /// </summary>
        public ExportMemoryViewModel ExportParams { get; }

        /// <summary>
        /// Exports the memory of the specified virtual machine
        /// </summary>
        /// <param name="spectrumVm"></param>
        public void ExportMemory(ISpectrumVm spectrumVm)
        {
            // --- Parse addresses
            if (!ushort.TryParse(ExportParams.StartAddress, out var startAddress)) return;
            if (!ushort.TryParse(ExportParams.EndAddress, out var endAddress)) return;
            try
            {
                var contents = spectrumVm.MemoryDevice.CloneMemory()
                    .Skip(startAddress)
                    .Take(endAddress - startAddress + 1)
                    .ToArray();
                var dirName = Path.GetDirectoryName(ExportParams.Filename);
                if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                File.WriteAllBytes(ExportParams.Filename, contents);
            }
            catch (Exception ex)
            {
                VsxDialogs.Show($"Error while exporting to file {ExportParams.Filename}: {ex.Message}",
                    "Export disassembly error.", MessageBoxButton.OK, VsxMessageBoxIcon.Error);
            }

            if (!ExportParams.AddToProject) return;

            DiscoveryProject.AddFileToProject(SpectNetPackage.Default.Options.MemoryExportFolder,
                ExportParams.Filename,
                INVALID_FOLDER_MESSAGE, FILE_EXISTS_MESSAGE);
        }
    }
}