using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Spect.Net.VsPackage.Z80Programs
{
    /// <summary>
    /// This class is responsible for managing Z80 program files
    /// </summary>
    public class Z80CodeManager
    {
        private const byte CLEAR_TKN = 0xFD;
        private const byte LOAD_TKN = 0xEF;
        private const byte CODE_TKN = 0xAF;
        private const byte DQUOTE = 0x22;
        private const byte COLON = 0x3A;
        private const byte RAND_TKN = 0xF9;
        private const byte USER_TKN = 0xC0;
        private const byte NUMB_SIGN = 0x0E;
        private const byte NEW_LINE = 0x0D;
        private const int RAMTOP_GAP = 0x100;

        /// <summary>
        /// The package that host the project
        /// </summary>
        public SpectNetPackage Package { get; }

        /// <summary>
        /// The hierarchy information of the associated item
        /// </summary>
        public IVsHierarchy CurrentHierarchy { get; private set; }

        /// <summary>
        /// The Id information of the associated item
        /// </summary>
        public uint CurrentItemId { get; private set; }

        /// <summary>
        /// Signs that compilation is in progress
        /// </summary>
        public bool CompilatioInProgress { get; set; }

        /// <summary>
        /// This event signs that code has been injected into the virtual machine.
        /// </summary>
        public event EventHandler CodeInjected;

        /// <summary>
        /// Signs that the annotation file has been changed
        /// </summary>
        public event EventHandler AnnotationFileChanged; 

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Z80CodeManager()
        {
            Package = SpectNetPackage.Default;
        }

        /// <summary>
        /// The error list
        /// </summary>
        public ErrorListWindow ErrorList => Package.ErrorList;

        /// <summary>
        /// Compile the specified Z80 code file
        /// </summary>
        /// <param name="hierarchy">Hierarchy object</param>
        /// <param name="itemId">Item ID within the hierarchy</param>
        /// <param name="options">Assembler options to use</param>
        public AssemblerOutput Compile(IVsHierarchy hierarchy, uint itemId, AssemblerOptions options)
        {
            CurrentHierarchy = hierarchy;
            CurrentItemId = itemId;

            var compiler = new Z80Assembler();
            if (!(hierarchy is IVsProject project)) return null;
            project.GetMkDocument(itemId, out var itemFullPath);
            return compiler.CompileFile(itemFullPath, options);
        }

        /// <summary>
        /// Injects the code into the Spectrum virtual machine's memory
        /// </summary>
        /// <param name="output"></param>
        public void InjectCodeIntoVm(AssemblerOutput output)
        {
            // --- Do not inject faulty code
            if (output == null || output.ErrorCount > 0)
            {
                return;
            }

            // --- Do not inject code if memory is not available
            var spectrumVm = Package.MachineViewModel.SpectrumVm;
            if (Package.MachineViewModel.VmState != VmState.Paused 
                || spectrumVm?.MemoryDevice == null)
            {
                return;
            }

            if (spectrumVm is ISpectrumVmRunCodeSupport runSupport)
            {
                // --- Go through all code segments and inject them
                foreach (var segment in output.Segments)
                {
                    var addr = segment.StartAddress + (segment.Displacement ?? 0);
                    runSupport.InjectCodeToMemory((ushort)addr, segment.EmittedCode);
                }
                
                // --- Prepare the machine for RUN mode
                runSupport.PrepareRunMode();
                CodeInjected?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the annotation file changed message
        /// </summary>
        public void RaiseAnnotationFileChanged()
        {
            AnnotationFileChanged?.Invoke(this, EventArgs.Empty);
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
        public List<byte[]> CreateTapeBlocks(string name, AssemblerOutput output, bool singleBlock)
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
                        DataLength = (ushort)(codeSegment.Length-2),
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
        /// Creates auto start block (header+data) to save 
        /// </summary>
        /// <param name="name">Program name</param>
        /// <param name="blockNo">Number of blocks to load</param>
        /// <param name="startAddr">Auto start address</param>
        /// <param name="clearAddr">Optional CLEAR address</param>
        /// <returns></returns>
        public List<byte[]> CreateAutoStartBlock(string name, int blockNo, ushort startAddr, ushort? clearAddr = null)
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

            // --- Add 'LOAD "" CODE' for each block
            for (int i = 0; i < blockNo; i++)
            {
                codeLine.Add(LOAD_TKN);
                codeLine.Add(DQUOTE);
                codeLine.Add(DQUOTE);
                codeLine.Add(CODE_TKN);
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
            // --- 2 byte - line lenght, 1 byte - checksum
            var dataBlock = new byte[codeLine.Count + 6];
            codeLine.CopyTo(dataBlock, 5);
            dataBlock[0] = 0xff;
            // --- Set line number to 10. Line number uses MSB/LSB order
            dataBlock[1] = 0x00; 
            dataBlock[2] = 10;
            // --- Set line length
            dataBlock[3] = (byte) codeLine.Count;
            dataBlock[4] = (byte) (codeLine.Count >> 8);
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
                foreach(var ch in number.ToString()) codeArray.Add((byte)ch);
                codeArray.Add(NUMB_SIGN);
                // --- Five bytes as the short form of an integer
                codeArray.Add(0x00);
                codeArray.Add(0x00);
                codeArray.Add((byte)number);
                codeArray.Add((byte)(number >>8));
                codeArray.Add(0x00);
            }
        }

        /// <summary>
        /// Sets the tape checksum of the specified byte array.
        /// </summary>
        /// <param name="bytes">Byte array</param>
        /// <remarks>
        /// Checksum is stored in the last item of the byte array,
        /// it is the value of bytes XORed.
        /// </remarks>
        public void SetTapeCheckSum(byte[] bytes)
        {
            var chk = 0x00;
            for (var i = 0; i < bytes.Length - 1; i++)
            {
                chk ^= bytes[i];
            }
            bytes[bytes.Length - 1] = (byte)chk;
        }
    }
}