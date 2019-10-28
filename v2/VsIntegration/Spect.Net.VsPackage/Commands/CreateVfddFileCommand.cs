using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Floppy;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.VsPackage.Dialogs.Vfdd;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Command;
using System;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command creates a virtual floppy disk file
    /// </summary>
    [CommandId(0x1B00)]
    public class CreateVfddFileCommand: SpectNetCommandBase
    {
        private const string FILE_EXISTS_MESSAGE = "The virtual floppy disk file exists in the project. " +
                                           "Would you like to override it?";

        private const string INVALID_FOLDER_MESSAGE = "The virtual floppy disk folder specified in the Options dialog " +
                                                      "contains invalid characters or an absolute path. Go to the Options dialog and " +
                                                      "fix the issue so that you can add the file to the project.";

        /// <summary>
        /// Eanble this command only if there's any floppy disk present
        /// </summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            mc.Visible = SpectNetPackage.Default.EmulatorViewModel.Machine.SpectrumVm.FloppyDevice is FloppyDevice;
        }

        /// <summary>
        /// Sets the active project to the current project file
        /// </summary>
        protected async override Task ExecuteAsync()
        {
            // --- Collect export parameters from the UI
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (DisplayCreateVfddDialog(out var vm)) return;

            // --- Create a temporary file
            string fullPath;
            try
            {
                var filename = Path.ChangeExtension(Path.GetFileName(vm.Filename), ".vfdd");
                var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                fullPath = Path.Combine(Path.Combine(userPath, "SpectNetFloppies"), filename);
                VirtualFloppyFile.CreateSpectrumFloppyFile(fullPath, vm.Format);
            }
            catch (Exception ex)
            {
                VsxDialogs.Show($"Error: {ex.Message}", "Error creating virtual floppy disk file");
                return;
            }

            // --- Add the temp file to the project
            SpectrumProject.AddFileToProject(SpectNetPackage.Default.Options.VfddFolder, fullPath,
                INVALID_FOLDER_MESSAGE, FILE_EXISTS_MESSAGE);

            // --- Remove the temp file
            try
            {
                File.Delete(fullPath);
            }
            catch (Exception)
            {
                // --- This exception is intentionally ignored
            }
        }

        /// <summary>
        /// Displays the Export Z80 Code dialog to collect parameter data
        /// </summary>
        /// <param name="vm">View model with collected data</param>
        /// <returns>
        /// True, if the user stars export; false, if the export is cancelled
        /// </returns>
        private bool DisplayCreateVfddDialog(out CreateVfddFileViewModel vm)
        {
            var createVfddDialog = new CreateVfddFileDialog()
            {
                HasMaximizeButton = false,
                HasMinimizeButton = false
            };

            vm = new CreateVfddFileViewModel
            {
                Format = FloppyFormat.SpectrumP3,
                Filename = "Floppy.vfdd",
            };

            createVfddDialog.SetVm(vm);
            var accepted = createVfddDialog.ShowModal();
            if (accepted.HasValue && accepted.Value) return false;

            IsCancelled = true;
            return true;
        }

    }
}
