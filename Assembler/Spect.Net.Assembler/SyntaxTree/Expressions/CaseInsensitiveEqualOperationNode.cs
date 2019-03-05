using System;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the case-insensitive 'equal' operation
    /// </summary>
    public sealed class CaseInsensitiveEqualOperationNode : EqualOperationNode
    {

        /// <summary>
        /// String comparison to apply
        /// </summary>
        public override StringComparison Comparison => StringComparison.InvariantCultureIgnoreCase;

        public CaseInsensitiveEqualOperationNode(object leftOperand = null, object rightOperand = null)
            : base(leftOperand, rightOperand)
        {
        }
    }
}