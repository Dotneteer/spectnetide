using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// This class implements a disassembler provider that obtains the
    /// label name from the ZX Spectrum 48 ROM
    /// </summary>
    public class Spectrum48RomLabelProvider: ISpectrum48RomLabelProvider
    {
        private readonly DisassemblyAnnotation _annotation;

        /// <summary>Initializes a new instance of this provider.</summary>
        public Spectrum48RomLabelProvider(DisassemblyAnnotation annotation)
        {
            _annotation = annotation;
        }

        /// <summary>
        /// Gets the label for the specified address from the ZX Spectrum 48 ROM
        /// </summary>
        /// <param name="address">ROM address</param>
        /// <returns>Label, if found; otherwise, null</returns>
        public string GetSpectrum48Label(ushort address)
        {
            return _annotation == null 
                ? null 
                : (_annotation.Labels.TryGetValue(address, out var label) 
                    ? label : null);
        }
    }
}