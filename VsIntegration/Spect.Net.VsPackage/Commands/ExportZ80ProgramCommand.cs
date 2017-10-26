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
    public class ExportZ80ProgramCommand : Z80CompileCodeCommandBase
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

            var exportDialog = new ExportZ80ProgramDialog();
            exportDialog.HasMaximizeButton = false;
            exportDialog.HasMaximizeButton = false;
            exportDialog.ShowModal();

            var name = "MyCode";
            var format = ExportFormat.Tap;
            var filename = @"C:\Temp\SaveCode.tap";

            // --- Step #3: Create code segments
            var codeBlocks = Package.CodeManager.CreateTapeBlocks(name, Output, true);
            var blocksToSave = new List<byte[]>();

            // --- Step #4: Create Auto Start header block, if required
            if (true)
            {
                var autoStartBlocks = Package.CodeManager.CreateAutoStartBlock(name, codeBlocks.Count >> 1,
                    Output.EntryAddress ?? Output.Segments[0].StartAddress, 0x6400);
                blocksToSave.AddRange(autoStartBlocks);
            }

            // --- Step #5: Save all the blocks
            blocksToSave.AddRange(codeBlocks);

            // --- Create directory
            var dirName = Path.GetDirectoryName(filename);
            if (dirName != null && !Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            // --- Save data blocks
            if (format == ExportFormat.Tzx)
            {
                using (var writer = new BinaryWriter(File.Create(filename)))
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
            else if (format == ExportFormat.Tap)
            {
                using (var writer = new BinaryWriter(File.Create(filename)))
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
            if (Package.Options.ConfirmCodeStart && Output.ErrorCount == 0)
            {
                VsxDialogs.Show("The code has been exported.");
            }
        }
    }
}