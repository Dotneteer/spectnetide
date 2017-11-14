using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using EnvDTE;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Commands;
using Spect.Net.VsPackage.Z80Programs.Export;
using ExportZ80ProgramDialog = Spect.Net.VsPackage.Z80Programs.Export.ExportZ80ProgramDialog;
using Task = System.Threading.Tasks.Task;

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

            var exportDialog = new ExportZ80ProgramDialog
            {
                HasMaximizeButton = false,
                HasMinimizeButton = false
            };

            var programName = Path.GetFileNameWithoutExtension(CompiledItemPath) ?? "MyCode";
            var filename = Path.Combine(Package.Options.CodeExportPath, $"{programName}.tzx");
            var vm = new ExportZ80ProgramViewModel
            {
                Format = ExportFormat.Tzx,
                Name = programName,
                Filename = filename,
                SingleBlock = true,
                AddToProject = false,
                AutoStart = true,
                ApplyClear = true,
                StartAddress = (Output.EntryAddress ?? Output.Segments[0].StartAddress).ToString()
            };
            exportDialog.SetVm(vm);
            var accepted = exportDialog.ShowModal();
            if (!accepted.HasValue || !accepted.Value)
            {
                IsCancelled = true;
                return;
            }

            // --- Step #3: Create code segments
            var codeBlocks = Package.CodeManager.CreateTapeBlocks(vm.Name, Output, vm.SingleBlock);
            var blocksToSave = new List<byte[]>();

            // --- Step #4: Create Auto Start header block, if required
            if (true)
            {
                if (!ushort.TryParse(vm.StartAddress, out var startAddress))
                {
                    startAddress = Output.EntryAddress ?? Output.Segments[0].StartAddress;
                }
                var autoStartBlocks = Package.CodeManager.CreateAutoStartBlock(
                    vm.Name,
                    codeBlocks.Count >> 1,
                    startAddress,
                    vm.ApplyClear
                        ? Output.Segments.Min(s => s.StartAddress)
                        : (ushort?) null);
                blocksToSave.AddRange(autoStartBlocks);
            }

            // --- Step #5: Save all the blocks
            blocksToSave.AddRange(codeBlocks);

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

            if (!vm.AddToProject) return;

            // --- Step #6: Add the saved item to the project
            // --- Obtain the project and its items
            var project = Package.CodeDiscoverySolution.CurrentProject.Root;
            var projectItems = project.ProjectItems;

            // --- Search for the tape folder (only within the default project items)
            foreach (ProjectItem projItem in projectItems)
            {
                var folder = projItem.Properties.Item("FolderName").Value?.ToString();
                if (string.Compare(folder, Package.Options.TapeFolder, 
                    StringComparison.InvariantCultureIgnoreCase) == 0 )
                {
                    projectItems = projItem.ProjectItems;
                    break;
                }
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
    }
}