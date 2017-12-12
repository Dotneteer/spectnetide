using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EnvDTE;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Commands;
using Spect.Net.VsPackage.Z80Programs.Export;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Export a Z80 program command
    /// </summary>
    [CommandId(0x0802)]
    public class ExportZ80ProgramCommand : Z80CompileCodeCommandBase
    {
        /// <summary>
        /// Compiles the Z80 code file
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            // --- Prepare the appropriate file to export
            GetCodeItem(out var hierarchy, out var itemId);

            // --- Step #1: Compile the code
            if (!CompileCode(hierarchy, itemId)) return;

            // --- Step #2: Check for zero code length
            if (Output.Segments.Sum(s => s.EmittedCode.Count) == 0)
            {
                VsxDialogs.Show("The lenght of the compiled code is 0, " +
                                "so there is no code to export.",
                    "No code to export.");
                return;
            }

            // --- Step #2: Collect export parameters from the UI
            await SwitchToMainThreadAsync();
            if (DisplayExportParameterDialog(out var vm)) return;

            // --- Step #3: Create code segments
            var codeBlocks = Package.CodeManager.CreateTapeBlocks(vm.Name, Output, vm.SingleBlock);

            // --- Step #4: Create Auto Start header block, if required
            var blocksToSave = new List<byte[]>();
            if (!ushort.TryParse(vm.StartAddress, out var startAddress))
            {
                startAddress = (ushort)ExportStartAddress;
            }
            var autoStartBlocks = Package.CodeManager.CreateAutoStartBlock(
                vm.Name,
                codeBlocks.Count >> 1,
                startAddress,
                vm.ApplyClear
                    ? Output.Segments.Min(s => s.StartAddress)
                    : (ushort?) null);
            blocksToSave.AddRange(autoStartBlocks);

            // --- Step #5: Save all the blocks
            blocksToSave.AddRange(codeBlocks);
            SaveDataBlocks(vm, blocksToSave);

            if (!vm.AddToProject) return;

            // --- Step #6: Add the saved item to the project
            // --- Check path segment names
            AddExportedFileToProject(vm);
        }

        /// <summary>
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected override void FinallyOnMainThread()
        {
            base.FinallyOnMainThread();
            if (!IsCancelled && Package.Options.ConfirmCodeExport && Output.ErrorCount == 0)
            {
                VsxDialogs.Show("The code has been exported.");
            }
        }

        /// <summary>
        /// Displays the Export Z80 Code dialog to collect parameter data
        /// </summary>
        /// <param name="vm">View model with collected data</param>
        /// <returns>
        /// True, if the user stars export; false, if the export is cancelled
        /// </returns>
        private bool DisplayExportParameterDialog(out ExportZ80ProgramViewModel vm)
        {
            var exportDialog = new ExportZ80ProgramDialog
            {
                HasMaximizeButton = false,
                HasMinimizeButton = false
            };

            var programName = Path.GetFileNameWithoutExtension(CompiledItemPath) ?? "MyCode";
            var filename = Path.Combine(Package.Options.CodeExportPath, $"{programName}.tzx");
            vm = new ExportZ80ProgramViewModel
            {
                Format = ExportFormat.Tzx,
                Name = programName,
                Filename = filename,
                SingleBlock = true,
                AddToProject = false,
                AutoStart = true,
                ApplyClear = true,
                StartAddress = ExportStartAddress.ToString()
            };
            exportDialog.SetVm(vm);
            var accepted = exportDialog.ShowModal();
            if (!accepted.HasValue || !accepted.Value)
            {
                IsCancelled = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Save data blocks
        /// </summary>
        /// <param name="vm">Export parameters</param>
        /// <param name="blocksToSave">Collection of data blocks to save</param>
        private static void SaveDataBlocks(ExportZ80ProgramViewModel vm, IEnumerable<byte[]> blocksToSave)
        {
            // --- Create directory
            var dirName = Path.GetDirectoryName(vm.Filename);
            if (dirName != null && !Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            // --- Save data blocks
            if (vm.Format == ExportFormat.Tzx)
            {
                using (var writer = new BinaryWriter(File.Create(vm.Filename)))
                {
                    var header = new TzxHeader();
                    header.WriteTo(writer);

                    foreach (var block in blocksToSave)
                    {
                        var tzxBlock = new TzxStandardSpeedDataBlock
                        {
                            Data = block,
                            DataLength = (ushort) block.Length
                        };
                        tzxBlock.WriteTo(writer);
                    }
                }
            }
            else
            {
                using (var writer = new BinaryWriter(File.Create(vm.Filename)))
                {
                    foreach (var block in blocksToSave)
                    {
                        writer.Write((ushort) block.Length);
                        writer.Write(block);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the exported file to the project structure
        /// </summary>
        /// <param name="vm">Export parameters</param>
        private void AddExportedFileToProject(ExportZ80ProgramViewModel vm)
        {
            var folderSegments = Package.Options.TapeFolder.Split(new[] {'/', '\\'},
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in folderSegments)
            {
                bool valid;
                try
                {
                    valid = !Path.IsPathRooted(segment);
                }
                catch
                {
                    valid = false;
                }
                if (!valid)
                {
                    VsxDialogs.Show("The tape folder specified in the Options dialog " +
                                    "contains invalid characters or an absolute path. Go to the Options dialog and " +
                                    "fix the issue so that you can add the tape file to the project.",
                        "Invalid characters in path");
                    return;
                }
            }

            // --- Obtain the project and its items
            var project = Package.CodeDiscoverySolution.CurrentProject.Root;
            var projectItems = project.ProjectItems;
            var currentIndex = 0;
            var find = true;
            while (currentIndex < folderSegments.Length)
            {
                // --- Find or create folder segments
                var segment = folderSegments[currentIndex];
                if (find)
                {
                    // --- We are in "find" mode
                    var found = false;
                    // --- Search for the folder segment
                    foreach (ProjectItem projItem in projectItems)
                    {
                        var folder = projItem.Properties.Item("FolderName").Value?.ToString();
                        if (string.Compare(folder, segment, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            // --- We found the folder, we'll go no with search within this segment
                            projectItems = projItem.ProjectItems;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        // --- Move to "create" mode
                        find = false;
                    }
                }
                if (!find)
                {
                    // --- We're in create mode, add and locate the new folder segment
                    var found = false;
                    projectItems.AddFolder(segment);
                    var parent = projectItems.Parent;
                    if (parent is Project projectType)
                    {
                        projectItems = projectType.ProjectItems;
                    }
                    else if (parent is ProjectItem itemType)
                    {
                        projectItems = itemType.ProjectItems;
                    }
                    foreach (ProjectItem projItem in projectItems)
                    {
                        var folder = projItem.Properties.Item("FolderName").Value?.ToString();
                        if (string.Compare(folder, segment, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            // --- We found the folder, we'll go no with search within this segment
                            projectItems = projItem.ProjectItems;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        // --- This should not happen...
                        VsxDialogs.Show($"The folder segment {segment} could not be created.",
                            "Adding project item failed");
                        return;
                    }
                }

                // --- Move to the next segment
                currentIndex++;
            }

            // --- Check if that filename exists within the project folder
            var tempFile = Path.GetFileName(vm.Filename);
            ProjectItem toDelete = null;
            foreach (ProjectItem projItem in projectItems)
            {
                var file = Path.GetFileName(projItem.FileNames[0]);
                if (string.Compare(file, tempFile,
                        StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var answer = VsxDialogs.Show("The exported tape file exists in the project. " +
                                                 "Would you like to override it?",
                        "File already exists",
                        MessageBoxButton.YesNo, VsxMessageBoxIcon.Question, 1);
                    if (answer == VsxDialogResult.No)
                    {
                        return;
                    }
                    toDelete = projItem;
                    break;
                }
            }

            // --- Remove existing file
            toDelete?.Delete();

            // --- Add the item to the appropriate item
            projectItems.AddFromFileCopy(vm.Filename);

            // --- Refresh the solution's content
            Package.CodeDiscoverySolution.CurrentProject.CollectItems();
        }
    }
}