using System.Collections.Generic;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFB pragma
    /// </summary>
    public sealed class DefbPragma : PragmaBase, ISupportsFieldAssignment
    {
        /// <summary>
        /// The bytes to define
        /// </summary>
        public List<ExpressionNode> Exprs { get; set; }

        /// <summary>
        /// True indicates that this node is used within a field assignment
        /// </summary>
        public bool IsFieldAssignment { get; set; }
    }
}