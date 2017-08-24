using System.Collections.Generic;
using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFW pragma
    /// </summary>
    public sealed class DefwPragma : PragmaLine
    {
        /// <summary>
        /// The words to define
        /// </summary>
        public List<ExpressionNode> Exprs { get; set; }
    }
}