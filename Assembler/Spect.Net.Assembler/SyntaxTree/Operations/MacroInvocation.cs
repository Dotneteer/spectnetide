using System.Collections.Generic;

namespace Spect.Net.Assembler.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents a macro invocation statement
    /// </summary>
    public class MacroInvocation : StatementBase
    {
        /// <summary>
        /// Name of the macro to invoke
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Actual parameters of the macro invocation
        /// </summary>
        public List<Operand> Parameters { get; }

        public MacroInvocation(string name, List<Operand> parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }
}