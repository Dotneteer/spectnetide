using System.Collections.Generic;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents the input options of the compiler
    /// </summary>
    public class AssemblerOptions
    {
        /// <summary>
        /// Predefined compilation symbols
        /// </summary>
        public List<string> PredefinedSymbols { get; } = new List<string>();

        /// <summary>
        /// The default start address of the compilation
        /// </summary>
        public ushort? DefaultStartAddress { get; set; } = null;

        /// <summary>
        /// The default displacement address of the compilation
        /// </summary>
        public int? DefaultDisplacement { get; set; } = null;

        /// <summary>
        /// The current ZX Spectrum model
        /// </summary>
        public SpectrumModelType CurrentModel { get; set; } = SpectrumModelType.Spectrum48;
    }
}