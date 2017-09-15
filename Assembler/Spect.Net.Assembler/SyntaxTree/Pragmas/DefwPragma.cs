using System.Collections.Generic;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFW pragma
    /// </summary>
    public sealed class DefwPragma : PragmaBase
    {
        /// <summary>
        /// The words to define
        /// </summary>
        public List<ExpressionNode> Exprs { get; set; }
    }
}