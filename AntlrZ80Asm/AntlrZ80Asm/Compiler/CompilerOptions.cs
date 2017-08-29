using System.Collections.Generic;

namespace AntlrZ80Asm.Compiler
{
    /// <summary>
    /// This class represents the input options of the compiler
    /// </summary>
    public class CompilerOptions
    {
        /// <summary>
        /// Predefined compilation symbols
        /// </summary>
        public List<string> PredefinedSymbols { get; } = new List<string>();

        /// <summary>
        /// The default start address of the compilation
        /// </summary>
        public ushort? DefaultStartAddress { get; set; } = null;
    }
}