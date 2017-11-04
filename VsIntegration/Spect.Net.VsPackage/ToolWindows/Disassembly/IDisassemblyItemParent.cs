using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.Wpf.Mvvm;
using MachineViewModel = Spect.Net.VsPackage.ToolWindows.SpectrumEmulator.MachineViewModel;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
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