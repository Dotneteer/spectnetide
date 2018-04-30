using System.Collections.Generic;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents a MACRO statement
    /// </summary>
    public sealed class MacroStatement: BlockStatementBase<MacroEndStatement>, ILabelSetter
    {
        /// <summary>
        /// Macro argument names
        /// </summary>
        public List<string> Arguments { get; }

        public MacroStatement(List<string> arguments)
        {
            Arguments = arguments;
        }
    }
}