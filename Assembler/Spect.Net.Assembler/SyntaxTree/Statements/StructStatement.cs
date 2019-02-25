using System;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents a STRUCT statement
    /// </summary>
    public sealed class StructStatement: BlockStatementBase, ILabelSetter
    {
        /// <summary>
        /// Type of end statement
        /// </summary>
        public override Type EndType => typeof(StructEndStatement);

        /// <summary>
        /// Gets the name of the end statement
        /// </summary>
        public override string EndStatementName => "ENDS";
    }
}