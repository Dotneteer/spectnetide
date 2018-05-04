using System;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents a REPEAT statement
    /// </summary>
    public sealed class RepeatStatement : BlockStatementBase
    {
        /// <summary>
        /// Type of end statement
        /// </summary>
        public override Type EndType => typeof(UntilStatement);

        /// <summary>
        /// Gets the name of the end statement
        /// </summary>
        public override string EndStatementName => "UNTIL";
    }
}