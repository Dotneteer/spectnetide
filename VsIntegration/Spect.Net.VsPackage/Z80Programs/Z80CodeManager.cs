using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Assembler.Assembler;
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
    }
}