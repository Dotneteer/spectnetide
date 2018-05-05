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
        // --- The errors reported in this scope
        private readonly HashSet<string> _errorsReported = new HashSet<string>();

        /// <summary>
        /// Optional owner scope of this local scope
        /// </summary>
        public SymbolScope OwnerScope { get; }

        /// <summary>
        /// Indicates that this scope is for a loop
        /// </summary>
        public bool IsLoopScope { get; set; } = true;

        /// <summary>
        /// The current loop counter in the scope
        /// </summary>
        public int LoopCounter { get; set; } = 0;

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

        /// <summary>
        /// Signs that the specified error has been reported
        /// </summary>
        /// <param name="errorCode"></param>
        public void ReportError(string errorCode)
        {
            _errorsReported.Add(errorCode);
        }

        /// <summary>
        /// Checks if the specified error has been reported
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <returns></returns>
        public bool IsErrorReported(string errorCode) => _errorsReported.Contains(errorCode);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SymbolScope(SymbolScope ownerScope = null)
        {
            OwnerScope = ownerScope;
        }
    }
}