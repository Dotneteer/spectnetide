using Microsoft.VisualStudio.Shell;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.VsPackage.Dialogs.Export;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Command;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Task = System.Threading.Tasks.Task;
// ReSharper disable LocalizableElement

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
        private const byte CODE_TKN = 0xAF;
        private const byte DATA_TKN = 0xE4;
        private const byte FOR_TKN = 0xEB;
        private const byte IF_TKN = 0xFA;
        private const byte GOTO_TKN = 0xEC;
        private const byte LET_TKN = 0xF1;
        private const byte LOAD_TKN = 0xEF;
        private const byte NEXT_TKN = 0xF3;
        private const byte PEEK_TKN = 0xBE;
        private const byte POKE_TKN = 0xF4;
        private const byte READ_TKN = 0xE3;
        private const byte REM_TKN = 0xEA;
        private const byte THEN_TKN = 0xCB;
        private const byte TO_TKN = 0xCC;
        private const byte SCREEN_TKN = 0xAA;
        private const byte DQUOTE = 0x22;
        private const byte STOP_TKN = 0xE2;
        private const byte COLON = 0x3A;
        private const byte COMMA = 0x2C;
        private const byte RAND_TKN = 0xF9;
        private const byte USR_TKN = 0xC0;
        private const byte NUMB_SIGN = 0x0E;
        private const byte NEW_LINE = 0x0D;
        private const byte PAUSE_TKN = 0xF2;
        private const byte BORDER_TKN = 0xE7;

        public const string EXPORT_PATH = @".SpectNetIde\Exports";

        protected bool Success { get; set; }

        /// <summary>
        /// Compiles the Z80 code file
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            // --- Prepare the appropriate file to export
            Success = true;

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

            vm.Name = Path.GetFileNameWithoutExtension(ItemPath) ?? "MyCode";
            var result = ExportCompiledCode(Output, vm);
            if (result != 0)
            {
                VsxDialogs.Show("The specified screen file cannot be read as a ZX Spectrum compatible screen file.",
                    "Screen file error.", MessageBoxButton.OK, VsxMessageBoxIcon.Error);
                Success = false;
                return;
            }



            if (vm.AddToProject)
            {
                // --- Step #9: Add the saved item to the project
                // --- Check path segment names
                SpectrumProject.AddFileToProject(SpectNetPackage.Default.Options.TapeFolder, vm.Filename,
                    INVALID_FOLDER_MESSAGE, FILE_EXISTS_MESSAGE);
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
        /// Exports the specified compiler output
        /// </summary>
        /// <param name="output">Compiler output</param>
        /// <param name="programName">Name of the program</param>
        /// <param name="vm">Export options</param>
        /// <returns></returns>
        public static int ExportCompiledCode(AssemblerOutput output, ExportZ80ProgramViewModel vm)
        {
            vm.Filename = Path.Combine(Path.GetDirectoryName(SpectNetPackage.Default.ActiveProject.Root.FullName), 
                EXPORT_PATH, 
                vm.Name);
            var oldExt = vm.Format;
            vm.Format = ExportFormat.Unknown;
            vm.Format = oldExt;

            if (vm.Format == ExportFormat.IntelHex)
            {
                SaveIntelHexFile(vm.Filename, output);
                return 0;
            }

            // --- Check for screen file error
            var useScreenFile = !string.IsNullOrEmpty(vm.ScreenFile) && vm.ScreenFile.Trim().Length > 0;
            if (useScreenFile && !CommonTapeFilePlayer.CheckScreenFile(vm.ScreenFile))
            {
                return 1;
            }

            // --- Step #6: Create code segments
            var codeBlocks = CreateTapeBlocks(output, vm.Name, vm.SingleBlock);
            List<byte[]> screenBlocks = null;
            if (useScreenFile)
            {
                screenBlocks = CreateScreenBlocks(vm.ScreenFile);
            }

            // --- Step #7: Create Auto Start header block, if required
            var blocksToSave = new List<byte[]>();
            if (!ushort.TryParse(vm.StartAddress, out var startAddress))
            {
                startAddress = (ushort)(output == null
                    ? -1
                    : output.ExportEntryAddress
                        ?? output.EntryAddress
                        ?? output.Segments[0].StartAddress);
            }

            if (vm.AutoStartEnabled)
            {
                var autoStartBlocks = CreateAutoStartBlock(
                    output,
                    vm.Name,
                    useScreenFile,
                    vm.AddPause0,
                    vm.Border,
                    startAddress,
                    vm.ApplyClear
                        ? output.Segments.Min(s => s.StartAddress)
                        : (ushort?)null);
                blocksToSave.AddRange(autoStartBlocks);
            }

            // --- Step #8: Save all the blocks
            if (screenBlocks != null)
            {
                blocksToSave.AddRange(screenBlocks);
            }

            blocksToSave.AddRange(codeBlocks);
            SaveDataBlocks(vm, blocksToSave);
            return 0;
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
                AddToProject = true,
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
        private static void SaveIntelHexFile(string filename, AssemblerOutput output)
        {
            const int rowLen = 0x10;
            var hexOut = new StringBuilder(4096);
            foreach (var segment in output.Segments)
            {
                var offset = 0;
                while (offset + rowLen < segment.EmittedCode.Count)
                {
                    // --- Write an entire data row
                    WriteDataRecord(segment, offset, rowLen);
                    offset += rowLen;
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

            return;

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
        /// <param name="output">Assembler outpuy</param>
        /// <param name="name">Program name</param>
        /// <param name="singleBlock">
        /// Indicates if a single block should be created from all segments
        /// </param>
        /// <returns>The list that contains headers and data blocks to save</returns>
        private static List<byte[]> CreateTapeBlocks(AssemblerOutput output, string name, bool singleBlock)
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
                var endAddr = output.Segments
                    .Where(s => s.Bank == null)
                    .Max(s => s.StartAddress + s.EmittedCode.Count - 1);

                // --- Normal code segments
                var mergedSegment = new byte[endAddr - startAddr + 3];
                foreach (var segment in output.Segments.Where(s => s.Bank == null))
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

                // --- Normal code segments
                foreach (var segment in output.Segments.Where(s => s.Bank == null))
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

            // --- Create blocks for the banks
            foreach (var bankSegment in output.Segments.Where(s => s.Bank != null).OrderBy(s => s.Bank))
            {
                var startAddr = (ushort)(0xC000 + bankSegment.BankOffset);
                var endAddr = startAddr + bankSegment.EmittedCode.Count - 1;

                var codeSegment = new byte[endAddr - startAddr + 3];
                bankSegment.EmittedCode.CopyTo(codeSegment, 1);

                // --- The first byte of the code segment is 0xFF (Data block)
                codeSegment[0] = 0xff;
                SetTapeCheckSum(codeSegment);

                // --- Create the single header
                var header = new SpectrumTapeHeader
                {
                    Type = 3, // --- Code block
                    Name = $"bank{bankSegment.Bank}.code",
                    DataLength = (ushort)(codeSegment.Length - 2),
                    Parameter1 = startAddr,
                    Parameter2 = 0x8000
                };

                // --- Create the two tape blocks (header + data)
                result.Add(header.HeaderBytes);
                result.Add(codeSegment);
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
        private static void SetTapeCheckSum(byte[] bytes)
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
        private static List<byte[]> CreateScreenBlocks(string screenFile)
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
        /// <param name="output">Assembler output</param>
        /// <param name="name">Program name</param>
        /// <param name="useScreenFile">Indicates if a screen file is used</param>
        /// <param name="addPause0">Indicates if a "PAUSE 0" should be added</param>
        /// <param name="borderColor">Border color ("0"-"7")</param>
        /// <param name="startAddr">Auto start address</param>
        /// <param name="clearAddr">Optional CLEAR address</param>
        /// <returns>Block contents</returns>
        private static List<byte[]> CreateAutoStartBlock(AssemblerOutput output, string name,
            bool useScreenFile,
            bool addPause0,
            string borderColor,
            ushort startAddr,
            ushort? clearAddr = null)
        {
            return output.ModelType == SpectrumModelType.Spectrum48
                   || output.Segments.Count(s => s.Bank != null) == 0
                // --- No banks to emit, use the ZX Spectrum 48 auto-loader format
                ? CreateSpectrum48StartBlock(output, name, useScreenFile, addPause0,
                    borderColor, startAddr, clearAddr)
                // --- There are banks to emit, use the ZX Spectrum 128 auto-loader format
                : CreateSpectrum128StartBlock(output, name, useScreenFile, addPause0,
                    borderColor, startAddr, clearAddr);
        }

        /// <summary>
        /// Creates an auto start block for Spectrum 48K
        /// </summary>
        /// <param name="output">Assembler output</param>
        /// <param name="name">Program name</param>
        /// <param name="useScreenFile">Indicates if a screen file is used</param>
        /// <param name="addPause0">Indicates if a "PAUSE 0" should be added</param>
        /// <param name="borderColor">Border color ("0"-"7")</param>
        /// <param name="startAddr">Auto start address</param>
        /// <param name="clearAddr">Optional CLEAR address</param>
        /// <returns>Block contents</returns>
        private static List<byte[]> CreateSpectrum48StartBlock(AssemblerOutput output, string name,
            bool useScreenFile,
            bool addPause0,
            string borderColor,
            ushort startAddr,
            ushort? clearAddr = null)
        {
            var result = new List<byte[]>();

            // --- Step #1: Create the code line for auto start
            var codeLine = new List<byte>(100);
            if (clearAddr.HasValue && clearAddr.Value >= 0x6000)
            {
                // --- Add clear statement
                codeLine.Add(CLEAR_TKN);
                WriteNumber(codeLine, (ushort)(clearAddr.Value - 1));
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

            // --- Add optional screen loader, LET o = PEEK 23739 : LOAD "" SCREEN$ : POKE 23739,111
            if (useScreenFile)
            {
                codeLine.Add(LET_TKN);
                WriteString(codeLine, "o=");
                codeLine.Add(PEEK_TKN);
                WriteNumber(codeLine, 23739);
                codeLine.Add(COLON);
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
            for (var i = 0; i < output.Segments.Count; i++)
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

            // --- Some SCREEN$ related poking
            if (useScreenFile)
            {
                codeLine.Add(POKE_TKN);
                WriteNumber(codeLine, 23739);
                WriteString(codeLine, ",o:");
            }

            // --- Add 'RANDOMIZE USR address'
            codeLine.Add(RAND_TKN);
            codeLine.Add(USR_TKN);
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

                // --- Auto-start at Line 10
                Parameter1 = 10,
                // --- Variable area offset
                Parameter2 = (ushort)(dataBlock.Length - 2)
            };

            // --- Step #4: Retrieve the auto start header and data block for save
            result.Add(header.HeaderBytes);
            result.Add(dataBlock);
            return result;
        }

        /// <summary>
        /// Creates an auto start block for Spectrum 128K
        /// </summary>
        /// <param name="output">Assembler output</param>
        /// <param name="name">Program name</param>
        /// <param name="useScreenFile">Indicates if a screen file is used</param>
        /// <param name="addPause0">Indicates if a "PAUSE 0" should be added</param>
        /// <param name="borderColor">Border color ("0"-"7")</param>
        /// <param name="startAddr">Auto start address</param>
        /// <param name="clearAddr">Optional CLEAR address</param>
        /// <returns>Block contents</returns>
        private static List<byte[]> CreateSpectrum128StartBlock(AssemblerOutput output, string name,
            bool useScreenFile,
            bool addPause0,
            string borderColor,
            ushort startAddr,
            ushort? clearAddr = null)
        {
            var result = new List<byte[]>();

            // --- We keep the code lines here
            var lines = new List<List<byte>>();

            // --- Create placeholder for the paging code (line 10)
            var codeLine = new List<byte>(100) { REM_TKN };
            WriteString(codeLine, "012345678901234567890");
            codeLine.Add(NEW_LINE);
            lines.Add(codeLine);

            // --- Create code for CLEAR/PEEK program address (line 20)
            codeLine = new List<byte>(100);
            if (clearAddr.HasValue && clearAddr.Value >= 0x6000)
            {
                // --- Add clear statement
                codeLine.Add(CLEAR_TKN);
                WriteNumber(codeLine, (ushort)(clearAddr.Value - 1));
                codeLine.Add(COLON);
            }

            // --- Add "LET c=(PEEK 23635 + 256*PEEK 23636)+5
            codeLine.Add(LET_TKN);
            WriteString(codeLine, "c=(");
            codeLine.Add(PEEK_TKN);
            WriteNumber(codeLine, 23635);
            WriteString(codeLine, "+");
            WriteNumber(codeLine, 256);
            WriteString(codeLine, "*");
            codeLine.Add(PEEK_TKN);
            WriteNumber(codeLine, 23636);
            WriteString(codeLine, ")+");
            WriteNumber(codeLine, 5);
            codeLine.Add(NEW_LINE);
            lines.Add(codeLine);

            // --- Setup the machine code
            codeLine = new List<byte>(100) { FOR_TKN };
            WriteString(codeLine, "i=");
            WriteNumber(codeLine, 0);
            codeLine.Add(TO_TKN);
            WriteNumber(codeLine, 20);
            codeLine.Add(COLON);
            codeLine.Add(READ_TKN);
            WriteString(codeLine, "d:");
            codeLine.Add(POKE_TKN);
            WriteString(codeLine, "c+i,d:");
            codeLine.Add(NEXT_TKN);
            WriteString(codeLine, "i");
            codeLine.Add(NEW_LINE);
            lines.Add(codeLine);

            // --- Create code for BORDER/SCREEN and loading normal code blocks (line 30)
            codeLine = new List<byte>(100);
            if (borderColor != null)
            {
                var border = int.Parse(borderColor);
                codeLine.Add(BORDER_TKN);
                WriteNumber(codeLine, (ushort)border);
                codeLine.Add(COLON);
            }

            // --- Add optional screen loader, LET o = PEEK 23739:LOAD "" SCREEN$ : POKE 23739,111
            if (useScreenFile)
            {
                codeLine.Add(LET_TKN);
                WriteString(codeLine, "o=");
                codeLine.Add(PEEK_TKN);
                WriteNumber(codeLine, 23739);
                codeLine.Add(COLON);
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
            for (var i = 0; i < output.Segments.Count(s => s.Bank == null); i++)
            {
                if (i > 0)
                {
                    codeLine.Add(COLON);
                }
                codeLine.Add(LOAD_TKN);
                codeLine.Add(DQUOTE);
                codeLine.Add(DQUOTE);
                codeLine.Add(CODE_TKN);
            }

            codeLine.Add(NEW_LINE);
            lines.Add(codeLine);

            // --- Code for reading banks
            codeLine = new List<byte>(100) { READ_TKN };
            WriteString(codeLine, "b");
            codeLine.Add(NEW_LINE);
            lines.Add(codeLine);

            // --- "IF b = 8 THEN GO TO 80";
            codeLine = new List<byte>(100) { IF_TKN };
            WriteString(codeLine, "b=");
            WriteNumber(codeLine, 8);
            codeLine.Add(THEN_TKN);
            codeLine.Add(GOTO_TKN);
            WriteNumber(codeLine, 80);
            codeLine.Add(NEW_LINE);
            lines.Add(codeLine);

            // --- "POKE 23608,b: RANDOMIZE USR c: LOAD "" CODE: GO TO 50"
            codeLine = new List<byte>(100) { POKE_TKN };
            WriteNumber(codeLine, 23608);
            WriteString(codeLine, ",b:");
            codeLine.Add(RAND_TKN);
            codeLine.Add(USR_TKN);
            WriteString(codeLine, "c:");
            codeLine.Add(LOAD_TKN);
            codeLine.Add(DQUOTE);
            codeLine.Add(DQUOTE);
            codeLine.Add(CODE_TKN);
            codeLine.Add(COLON);
            codeLine.Add(GOTO_TKN);
            WriteNumber(codeLine, 50);
            codeLine.Add(NEW_LINE);
            lines.Add(codeLine);

            // --- PAUSE and START
            codeLine = new List<byte>(100);
            if (addPause0)
            {
                codeLine.Add(PAUSE_TKN);
                WriteNumber(codeLine, 0);
                codeLine.Add(COLON);
            }
            if (useScreenFile)
            {
                codeLine.Add(POKE_TKN);
                WriteNumber(codeLine, 23739);
                WriteString(codeLine, ",o:");
            }

            // --- Add 'RANDOMIZE USR address: STOP'
            codeLine.Add(RAND_TKN);
            codeLine.Add(USR_TKN);
            WriteNumber(codeLine, startAddr);
            codeLine.Add(COLON);
            codeLine.Add(STOP_TKN);
            codeLine.Add(NEW_LINE);
            lines.Add(codeLine);

            // --- Add data lines with the machine code subroutine
            codeLine = new List<byte>(100);
            WriteDataStatement(codeLine, new ushort[]
            {
                243, 58, 92, 91, 230, 248, 71,
                58, 56, 92, 176, 50, 92, 91,
                1, 253, 127, 237, 121, 251, 201
            });
            lines.Add(codeLine);

            // --- Add data lines with used banks and terminating 8
            codeLine = new List<byte>(100);
            var banks = output
                .Segments
                .Where(s => s.Bank != null)
                .Select(s => (ushort)s.Bank)
                .OrderBy(n => n)
                .ToList();
            banks.Add(8);
            WriteDataStatement(codeLine, banks.ToArray());
            lines.Add(codeLine);

            // --- All code lines are set up, create the file blocks
            var dataBlock = CreateDataBlockForCodeLines(lines);
            var header = new SpectrumTapeHeader
            {
                // --- Program block
                Type = 0,
                Name = name,
                DataLength = (ushort)(dataBlock.Length - 2),

                // --- Auto-start at Line 10
                Parameter1 = 10,

                // --- Variable area offset
                Parameter2 = (ushort)(dataBlock.Length - 2)
            };

            // --- Step #4: Retrieve the auto start header and data block for save
            result.Add(header.HeaderBytes);
            result.Add(dataBlock);
            return result;
        }

        /// <summary>
        /// Writes a number out to the BASIC code stream
        /// </summary>
        /// <param name="codeArray">Code array to add the number information to</param>
        /// <param name="number">Number to write out</param>
        private static void WriteNumber(ICollection<byte> codeArray, ushort number)
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

        /// <summary>
        /// Writes string data out to the BASIC code stream
        /// </summary>
        /// <param name="codeArray">Code array to add the number information to</param>
        /// <param name="data">String to write out</param>
        private static void WriteString(ICollection<byte> codeArray, string data)
        {
            foreach (var ch in data) codeArray.Add((byte)ch);
        }

        /// <summary>
        /// Writes a DATA line with the specified array of numbers
        /// </summary>
        /// <param name="codeLine"></param>
        /// <param name="data"></param>
        private static void WriteDataStatement(ICollection<byte> codeLine, ushort[] data)
        {
            codeLine.Add(DATA_TKN);
            var comma = false;
            foreach (var item in data)
            {
                if (comma)
                {
                    WriteString(codeLine, ",");
                }
                WriteNumber(codeLine, item);
                comma = true;
            }
            codeLine.Add(NEW_LINE);
        }

        /// <summary>
        /// Creates a data block for the code lines passed
        /// </summary>
        /// <param name="lines">BASIC code lines</param>
        /// <returns>
        /// Byte array representing the data block with header and checksum info
        /// </returns>
        private static byte[] CreateDataBlockForCodeLines(List<List<byte>> lines)
        {
            var length = lines.Sum(cl => cl.Count + 4) + 2;
            var dataBlock = new byte[length];
            dataBlock[0] = 0xff;
            var index = 1;
            var lineNo = 10;
            foreach (var line in lines)
            {
                // --- Line number in MSB/LSB format
                dataBlock[index++] = (byte)(lineNo >> 8);
                dataBlock[index++] = (byte)lineNo;

                // --- Set line length in LSB/MSB format
                dataBlock[index++] = (byte)line.Count;
                dataBlock[index++] = (byte)(line.Count >> 8);

                // --- Copy the code line
                line.CopyTo(dataBlock, index);

                // --- Move to the next line
                index += line.Count;
                lineNo += 10;
            }
            SetTapeCheckSum(dataBlock);
            return dataBlock;
        }
    }
}