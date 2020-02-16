using Microsoft.VisualStudio.Shell;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.VsPackage.Dialogs.Export;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Exports the specified code file as a program
    /// </summary>
    [CommandId(0x0802)]
    public class ExportProgramCommand : CompileCodeCommandBase
    {
        private const string FILE_EXISTS_MESSAGE = "The exported tape file exists in the project. " +
                                                   "Would you like to override it?";

        private const string INVALID_FOLDER_MESSAGE = "The tape folder specified in the Options dialog " +
                                                      "contains invalid characters or an absolute path. Go to the Options dialog and " +
                                                      "fix the issue so that you can add the tape file to the project.";

        private const byte CLEAR_TKN = 0xFD;
        private const byte LOAD_TKN = 0xEF;
        private const byte CODE_TKN = 0xAF;
        private const byte SCREEN_TKN = 0xAA;
        private const byte DQUOTE = 0x22;
        private const byte COLON = 0x3A;
        private const byte COMMA = 0x2C;
        private const byte RAND_TKN = 0xF9;
        private const byte USER_TKN = 0xC0;
        private const byte NUMB_SIGN = 0x0E;
        private const byte NEW_LINE = 0x0D;
        private const byte PAUSE_TKN = 0xF2;
        private const byte POKE_TKN = 0xF4;
        private const byte BORDER_TKN = 0xE7;
        private const int RAMTOP_GAP = 0x100;

        protected bool Success { get; set; }

        /// <summary>
        /// Compiles the Z80 code file
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            // --- Prepare the appropriate file to export
            Success = true;
            GetCodeItem(out var hierarchy, out var itemId);

            // --- Step #1: Compile the code
            if (!await CompileCode())
            {
                Success = false;
                return;
            }

            // --- Step #2: Check for zero code length
            if (Output.Segments.Sum(s => s.EmittedCode.Count) == 0)
            {
                VsxDialogs.Show("The length of the compiled code is 0, " +
                                "so there is no code to export.",
                    "No code to export.");
                Success = false;
                return;
            }

            // --- Step #3: Collect export parameters from the UI
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (DisplayExportParameterDialog(out var vm)) return;

            // --- Step #4: Execute pre-export event
            var codeManager = SpectNetPackage.Default.CodeManager;
            PreexportError = null;
            PostexportError = null;
            var eventOutput = await codeManager.RunPreExportEvent(ItemFullPath, vm.Filename);
            if (eventOutput != null)
            {
                PreexportError = eventOutput;
                CleanupError = await codeManager.RunBuildErrorEvent(ItemFullPath);
                DisplayBuildErrors();
                Success = false;
                return;
            }

            if (vm.Format == ExportFormat.IntelHex)
            {
                // --- Step #5: Export to Intel format
                SaveIntelHexFile(vm.Filename, Output);
            }
            else
            {
                // --- Step #5: Check screen file again
                var useScreenFile = !string.IsNullOrEmpty(vm.ScreenFile) && vm.ScreenFile.Trim().Length > 0;
                if (useScreenFile && !CommonTapeFilePlayer.CheckScreenFile(vm.ScreenFile))
                {
                    VsxDialogs.Show("The specified screen file cannot be read as a ZX Spectrum compatible screen file.",
                        "Screen file error.", MessageBoxButton.OK, VsxMessageBoxIcon.Error);
                    Success = false;
                    return;
                }

                // --- Step #6: Create code segments
                var codeBlocks = CreateTapeBlocks(vm.Name, Output, vm.SingleBlock);
                List<byte[]> screenBlocks = null;
                if (useScreenFile)
                {
                    screenBlocks = CreatScreenBlocks(vm.ScreenFile);
                }

                // --- Step #7: Create Auto Start header block, if required
                var blocksToSave = new List<byte[]>();
                if (!ushort.TryParse(vm.StartAddress, out var startAddress))
                {
                    startAddress = (ushort) ExportStartAddress;
                }

                var autoStartBlocks = CreateAutoStartBlock(
                    vm.Name,
                    useScreenFile,
                    vm.AddPause0,
                    vm.Border,
                    codeBlocks.Count >> 1,
                    startAddress,
                    vm.ApplyClear
                        ? Output.Segments.Min(s => s.StartAddress)
                        : (ushort?) null);
                blocksToSave.AddRange(autoStartBlocks);

                // --- Step #8: Save all the blocks
                if (screenBlocks != null)
                {
                    blocksToSave.AddRange(screenBlocks);
                }

                blocksToSave.AddRange(codeBlocks);
                SaveDataBlocks(vm, blocksToSave);

                if (vm.AddToProject)
                {
                    // --- Step #9: Add the saved item to the project
                    // --- Check path segment names
                    SpectrumProject.AddFileToProject(SpectNetPackage.Default.Options.TapeFolder, vm.Filename,
                        INVALID_FOLDER_MESSAGE, FILE_EXISTS_MESSAGE);
                }
            }

            // --- Run post-export event
            // --- Execute post-build event
            eventOutput = await codeManager.RunPostExportEvent(ItemFullPath, vm.Filename);
            if (eventOutput != null)
            {
                PostexportError = eventOutput;
                CleanupError = await codeManager.RunBuildErrorEvent(ItemFullPath);
                DisplayBuildErrors();
                Success = false;
            }
        }

        /// <summary>
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected override async Task FinallyOnMainThreadAsync()
        {
            await base.FinallyOnMainThreadAsync();
            if (Success && !IsCancelled 
                && SpectNetPackage.Default.Options.ConfirmCodeExport 
                && Output.ErrorCount == 0)
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

            var programName = Path.GetFileNameWithoutExtension(ItemPath) ?? "MyCode";
            var filename = Path.Combine(SpectNetPackage.Default.Options.CodeExportPath, $"{programName}.tzx");
            vm = new ExportZ80ProgramViewModel
            {
                Format = ExportFormat.Tzx,
                Name = programName,
                Filename = filename,
                SingleBlock = true,
                AddToProject = false,
                AutoStart = true,
                ApplyClear = true,
                AddPause0 = false,
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
                            DataLength = (ushort)block.Length
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
        /// Saves the output to Intel HEX file format
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="output">Assembly output to save</param>
        private string SaveIntelHexFile(string filename, AssemblerOutput output)
        {
            const int ROW_LEN = 0x10;
            var hexOut = new StringBuilder(4096);
            foreach (var segment in output.Segments)
            {
                var offset = 0;
                while (offset + ROW_LEN < segment.EmittedCode.Count)
                {
                    // --- Write an entire data row
                    WriteDataRecord(segment, offset, ROW_LEN);
                    offset += ROW_LEN;
                }
                // --- Write the left of the data row
                var leftBytes = segment.EmittedCode.Count - offset;
                WriteDataRecord(segment, offset, leftBytes);
            }
            // --- Write End-Of-File record
            hexOut.AppendLine(":00000001FF");

            // --- Save the data to a file
            var intelHexString = hexOut.ToString();
            if (filename != null)
            {
                File.WriteAllText(filename, intelHexString);
            }
            return intelHexString;

            void WriteDataRecord(BinarySegment segment, int offset, int bytesCount)
            {
                if (bytesCount == 0) return;
                var addr = (ushort)((segment.XorgValue ?? segment.StartAddress) + offset);
                hexOut.Append($":{bytesCount:X2}{addr:X4}00"); // --- Data record header
                var checksum = bytesCount + (addr >> 8) + (addr & 0xFF);
                for (var i = offset; i < offset + bytesCount; i++)
                {
                    var data = segment.EmittedCode[i];
                    checksum += data;
                    hexOut.Append($"{data:X2}");
                }
                var chk = (byte)(256 - (checksum & 0xff));
                hexOut.Append($"{chk:X2}");
                hexOut.AppendLine();
            }
        }

        /// <summary>
        /// Creates tape blocks from the assembler output.
        /// </summary>
        /// <param name="name">Program name</param>
        /// <param name="output">Assembler output</param>
        /// <param name="singleBlock">
        /// Indicates if a single block should be created from all segments
        /// </param>
        /// <returns>The list that contains headers and data blocks to save</returns>
        private List<byte[]> CreateTapeBlocks(string name, AssemblerOutput output, bool singleBlock)
        {
            var result = new List<byte[]>();
            if (output.Segments.Sum(s => s.EmittedCode.Count) == 0)
            {
                // --- No code to return
                return null;
            }

            if (singleBlock)
            {
                // --- Merge all blocks together
                var startAddr = output.Segments.Min(s => s.StartAddress);
                var endAddr = output.Segments.Max(s => s.StartAddress + s.EmittedCode.Count - 1);

                var mergedSegment = new byte[endAddr - startAddr + 3];
                foreach (var segment in output.Segments)
                {
                    segment.EmittedCode.CopyTo(mergedSegment, segment.StartAddress - startAddr + 1);
                }

                // --- The first byte of the merged segment is 0xFF (Data block)
                mergedSegment[0] = 0xff;
                SetTapeCheckSum(mergedSegment);

                // --- Create the single header
                var singleHeader = new SpectrumTapeHeader
                {
                    Type = 3, // --- Code block
                    Name = name,
                    DataLength = (ushort)(mergedSegment.Length - 2),
                    Parameter1 = startAddr,
                    Parameter2 = 0x8000
                };

                // --- Create the two tape blocks (header + data)
                result.Add(singleHeader.HeaderBytes);
                result.Add(mergedSegment);
            }
            else
            {
                // --- Create separate block for each segment
                var segmentIdx = 0;
                foreach (var segment in output.Segments)
                {
                    segmentIdx++;
                    var startAddr = segment.StartAddress;
                    var endAddr = segment.StartAddress + segment.EmittedCode.Count - 1;

                    var codeSegment = new byte[endAddr - startAddr + 3];
                    segment.EmittedCode.CopyTo(codeSegment, segment.StartAddress - startAddr + 1);

                    // --- The first byte of the code segment is 0xFF (Data block)
                    codeSegment[0] = 0xff;
                    SetTapeCheckSum(codeSegment);

                    // --- Create the single header
                    var header = new SpectrumTapeHeader
                    {
                        Type = 3, // --- Code block
                        Name = $"{segmentIdx}_{name}",
                        DataLength = (ushort)(codeSegment.Length - 2),
                        Parameter1 = startAddr,
                        Parameter2 = 0x8000
                    };

                    // --- Create the two tape blocks (header + data)
                    result.Add(header.HeaderBytes);
                    result.Add(codeSegment);
                }
            }
            return result;
        }

        /// <summary>
        /// Sets the tape checksum of the specified byte array.
        /// </summary>
        /// <param name="bytes">Byte array</param>
        /// <remarks>
        /// Checksum is stored in the last item of the byte array,
        /// it is the value of bytes XORed.
        /// </remarks>
        private void SetTapeCheckSum(byte[] bytes)
        {
            var chk = 0x00;
            for (var i = 0; i < bytes.Length - 1; i++)
            {
                chk ^= bytes[i];
            }
            bytes[bytes.Length - 1] = (byte)chk;
        }

        /// <summary>
        /// Creates screen blocks from the specified screen file
        /// </summary>
        /// <param name="screenFile">Screen file name</param>
        /// <returns></returns>
        private List<byte[]> CreatScreenBlocks(string screenFile)
        {
            var result = new List<byte[]>();
            using (var reader = new BinaryReader(File.OpenRead(screenFile)))
            {
                var player = new CommonTapeFilePlayer(reader);
                player.ReadContent();
                result.Add(((ITapeData)player.DataBlocks[0]).Data);
                result.Add(((ITapeData)player.DataBlocks[1]).Data);
            }
            return result;
        }

        /// <summary>
        /// Creates auto start block (header+data) to save 
        /// </summary>
        /// <param name="name">Program name</param>
        /// <param name="useScreenFile">Indicates if a screen file is used</param>
        /// <param name="addPause0">Indicates if a "PAUSE 0" should be added</param>
        /// <param name="borderColor">Border color ("0"-"7")</param>
        /// <param name="blockNo">Number of blocks to load</param>
        /// <param name="startAddr">Auto start address</param>
        /// <param name="clearAddr">Optional CLEAR address</param>
        /// <returns></returns>
        private List<byte[]> CreateAutoStartBlock(string name, bool useScreenFile, bool addPause0, string borderColor, int blockNo, ushort startAddr, ushort? clearAddr = null)
        {
            if (blockNo > 128)
            {
                throw new ArgumentException("The number of blocks cannot be more than 128.", nameof(blockNo));
            }

            var result = new List<byte[]>();

            // --- Step #1: Create the code line for auto start
            var codeLine = new List<byte>(100);
            if (clearAddr.HasValue && clearAddr.Value >= 0x6200)
            {
                // --- Add clear statement
                codeLine.Add(CLEAR_TKN);
                WriteNumber(codeLine, (ushort)(clearAddr.Value - RAMTOP_GAP));
                codeLine.Add(COLON);
            }

            // --- Add optional border color
            if (borderColor != null)
            {
                var border = int.Parse(borderColor);
                codeLine.Add(BORDER_TKN);
                WriteNumber(codeLine, (ushort)border);
                codeLine.Add(COLON);
            }

            // --- Add optional screen loader, 'LOAD "" SCREEN$ : POKE 23739,111
            if (useScreenFile)
            {
                codeLine.Add(LOAD_TKN);
                codeLine.Add(DQUOTE);
                codeLine.Add(DQUOTE);
                codeLine.Add(SCREEN_TKN);
                codeLine.Add(COLON);
                codeLine.Add(POKE_TKN);
                WriteNumber(codeLine, 23739);
                codeLine.Add(COMMA);
                WriteNumber(codeLine, 111);
                codeLine.Add(COLON);
            }

            // --- Add 'LOAD "" CODE' for each block
            for (int i = 0; i < blockNo; i++)
            {
                codeLine.Add(LOAD_TKN);
                codeLine.Add(DQUOTE);
                codeLine.Add(DQUOTE);
                codeLine.Add(CODE_TKN);
                codeLine.Add(COLON);
            }

            // --- Add 'PAUSE 0'
            if (addPause0)
            {
                codeLine.Add(PAUSE_TKN);
                WriteNumber(codeLine, 0);
                codeLine.Add(COLON);
            }

            // --- Add 'RANDOMIZE USR addr'
            codeLine.Add(RAND_TKN);
            codeLine.Add(USER_TKN);
            WriteNumber(codeLine, startAddr);

            // --- Complete the line
            codeLine.Add(NEW_LINE);

            // --- Step #2: Now, complete the data block
            // --- Allocate extra 6 bytes: 1 byte - header, 2 byte - line number
            // --- 2 byte - line length, 1 byte - checksum
            var dataBlock = new byte[codeLine.Count + 6];
            codeLine.CopyTo(dataBlock, 5);
            dataBlock[0] = 0xff;
            // --- Set line number to 10. Line number uses MSB/LSB order
            dataBlock[1] = 0x00;
            dataBlock[2] = 10;
            // --- Set line length
            dataBlock[3] = (byte)codeLine.Count;
            dataBlock[4] = (byte)(codeLine.Count >> 8);
            SetTapeCheckSum(dataBlock);

            // --- Step #3: Create the header
            var header = new SpectrumTapeHeader
            {
                // --- Program block
                Type = 0,
                Name = name,
                DataLength = (ushort)(dataBlock.Length - 2),
                // --- Autostart at Line 10
                Parameter1 = 10,
                // --- Variable area offset
                Parameter2 = (ushort)(dataBlock.Length - 2)
            };

            // --- Step #4: Retrieve the auto start header and data block for save
            result.Add(header.HeaderBytes);
            result.Add(dataBlock);
            return result;

            void WriteNumber(ICollection<byte> codeArray, ushort number)
            {
                // --- Number in string form
                foreach (var ch in number.ToString()) codeArray.Add((byte)ch);
                codeArray.Add(NUMB_SIGN);
                // --- Five bytes as the short form of an integer
                codeArray.Add(0x00);
                codeArray.Add(0x00);
                codeArray.Add((byte)number);
                codeArray.Add((byte)(number >> 8));
                codeArray.Add(0x00);
            }
        }
    }
}