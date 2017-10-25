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
    }
}