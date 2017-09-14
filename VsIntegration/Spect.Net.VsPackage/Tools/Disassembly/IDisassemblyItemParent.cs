using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Mvvm;

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// This interface represents the parent of a disassemblyItem
    /// </summary>
    public interface IDisassemblyItemParent
    {
        /// <summary>
        /// The annotations that help displaying comments and labels
        /// </summary>
        DisassemblyAnnotation Annotations { get; }

        /// <summary>
        /// The machine that provides optional debig information
        /// </summary>
        MachineViewModel MachineViewModel { get; }
    }
}