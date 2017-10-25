using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.Wpf.Mvvm;

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

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Z80CodeManager()
        {
            Package = VsxPackage.GetPackage<SpectNetPackage>();
        }

        /// <summary>
        /// The error list
        /// </summary>
        public ErrorListWindow ErrorList => Package.ErrorList;

        /// <summary>
        /// The full path of the item behind this Z80 program file
        /// </summary>
        protected string ItemPath
        {
            get
            {
                var singleItem = SpectNetPackage.IsSingleProjectItemSelection(out var hierarchy, out var itemId);
                if (!singleItem) return null;

                if (!(hierarchy is IVsProject project)) return null;

                project.GetMkDocument(itemId, out var itemFullPath);
                return itemFullPath;
            }
        }

        /// <summary>
        /// Compile the specified Z80 code file
        /// </summary>
        /// <param name="currentHierarchy">Hierarchy object</param>
        /// <param name="currentItemId">Item ID within the hierarchy</param>
        public AssemblerOutput Compile(IVsHierarchy currentHierarchy, uint currentItemId)
        {
            CurrentHierarchy = currentHierarchy;
            CurrentItemId = currentItemId;

            var compiler = new Z80Assembler();
            return compiler.CompileFile(ItemPath);
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

            var memory = spectrumVm.MemoryDevice.GetMemoryBuffer();
            // --- Go through all code segments and inject them
            foreach (var segment in output.Segments)
            {
                var addr = segment.StartAddress + (segment.Displacement ?? 0);
                foreach (var codeByte in segment.EmittedCode)
                {
                    if (addr >= 0x4000 && addr < memory.Length)
                    {
                        memory[addr++] = codeByte;
                    }
                }
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

                var chk = 0x00;
                for (int i = 0; i < mergedSegment.Length - 1; i++) chk ^= mergedSegment[i];

                // --- The last byte of the merged segment is the checksum
                mergedSegment[mergedSegment.Length - 1] = (byte)chk;

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

                    var chk = 0x00;
                    for (int i = 0; i < codeSegment.Length - 1; i++) chk ^= codeSegment[i];

                    // --- The last byte of the merged segment is the checksum
                    codeSegment[codeSegment.Length - 1] = (byte)chk;

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
        /// <param name="blockNo">Number of blocks to load</param>
        /// <param name="startAddr">Auto start address</param>
        /// <param name="clearAddr">Optional CLEAR address</param>
        /// <returns></returns>
        public List<byte[]> CreateAutoStartBlock(int blockNo, ushort startAddr, ushort? clearAddr = null)
        {
            if (blockNo > 128)
            {
                throw new ArgumentException("The number of blocks cannot be more than 128.", nameof(blockNo));    
            }

            var result = new List<byte[]>();

            var codeLine = new List<byte>(100);
            if (clearAddr.HasValue)
            {
                // --- Add clear statement
                codeLine.Add(CLEAR_TKN);
                WriteNumber(codeLine, clearAddr.Value);
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
    }
}