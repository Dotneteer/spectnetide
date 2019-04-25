using System;
using Spect.Net.Assembler.Generated;

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

        public CaseInsensitiveNotEqualOperationNode(Z80AsmParser.EquExprContext context, Z80AsmVisitor visitor) 
            : base(context, visitor)
        {
        }
    }
}