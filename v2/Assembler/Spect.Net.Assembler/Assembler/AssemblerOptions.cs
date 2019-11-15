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
        /// The current ZX Spectrum model
        /// </summary>
        public SpectrumModelType CurrentModel { get; set; } = SpectrumModelType.Spectrum48;

        /// <summary>
        /// The maximum number of errors to report within a loop
        /// </summary>
        public int MaxLoopErrorsToReport { get; set; } = 16;

        /// <summary>
        /// Signs that PROC labels and symbols are not locals by default
        /// </summary>
        public bool ProcExplicitLocalsOnly { get; set; } = false;

        /// <summary>
        /// Indicates that assembly symbols should be case sensitively.
        /// </summary>
        public bool UseCaseSensitiveSymbols { get; set; } = false;
    }
}