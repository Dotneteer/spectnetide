using System.ComponentModel;

namespace Spect.Net.VsPackage.VsxLibrary.Output
{
    [DisplayName("SpectNetIDE Output")]
    [AutoActivate(true)]
    [ClearWithSolution(false)]
    public class SpectNetIdeOutputPane : OutputPaneDefinition
    {
    }

    [DisplayName("Z80 Assembler")]
    [AutoActivate(true)]
    [ClearWithSolution(true)]
    public class Z80AssemblerOutputPane : OutputPaneDefinition
    {
    }

    [DisplayName("ZX Spectrum")]
    [AutoActivate(true)]
    [ClearWithSolution(true)]
    public class SpectrumVmOutputPane : OutputPaneDefinition
    {
    }
}