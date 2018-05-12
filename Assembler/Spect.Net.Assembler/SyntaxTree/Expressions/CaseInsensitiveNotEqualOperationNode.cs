using System;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the case-insensitive 'not equal' operation
    /// </summary>
    public class CaseInsensitiveNotEqualOperationNode : NotEqualOperationNode
    {
        /// <summary>
        /// String comparison to apply
        /// </summary>
        public override StringComparison Comparison => StringComparison.InvariantCultureIgnoreCase;
    }
}