using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Export a Z80 program command
    /// </summary>
    [CommandId(0x0802)]
    public class ExportZ80ProgramCommand : Z80CompileCodeCommandBaseBase
    {
        /// <summary>
        /// Compiles the Z80 code file
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            // --- Step #1: Compile the code
            if (!CompileCode()) return;

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
            var vm = new ExportZ80ProgramViewModel
            {
                Format = ExportFormat.Tap,
                Name = Path.GetFileNameWithoutExtension(ItemPath) ?? "MyCode",
                Filename = @"C:\Temp\ExportedFile.tap",
                SingleBlock = true,
                AddToProject = true,
                AutoStart = true,
                ApplyClear = true
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
                var autoStartBlocks = Package.CodeManager.CreateAutoStartBlock(
                    vm.Name, 
                    codeBlocks.Count >> 1,
                    Output.EntryAddress ?? Output.Segments[0].StartAddress, 
                    vm.ApplyClear 
                        ? Output.Segments.Min(s => s.StartAddress) 
                        : (ushort?)null );
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
                        writer.Write((ushort)block.Length);
                        writer.Write(block);
                    }
                }
            }
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