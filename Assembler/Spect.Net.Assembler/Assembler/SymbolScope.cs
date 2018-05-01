using System;
using System.Collections.Generic;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a scope where local symbols are declared
    /// </summary>
    public class SymbolScope
    {
        /// <summary>
        /// The symbol table with properly defined symbols
        /// </summary>
        public Dictionary<string, ExpressionValue> Symbols { get; } =
            new Dictionary<string, ExpressionValue>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// The variable table
        /// </summary>
        public Dictionary<string, ExpressionValue> Vars { get; } =
            new Dictionary<string, ExpressionValue>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// The list of fixups to carry out as the last phase of the compilation
        /// </summary>
        public List<FixupEntry> Fixups { get; } = new List<FixupEntry>();
    }
}