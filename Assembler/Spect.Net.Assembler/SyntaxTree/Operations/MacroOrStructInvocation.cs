using System.Collections.Generic;

namespace Spect.Net.Assembler.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents a macro or struct invocation statement
    /// </summary>
    public class MacroOrStructInvocation : StatementBase
    {
        /// <summary>
        /// Name of the macro to invoke
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Actual parameters of the macro invocation
        /// </summary>
        public List<Operand> Parameters { get; }

        public MacroOrStructInvocation(string name, List<Operand> parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }
}