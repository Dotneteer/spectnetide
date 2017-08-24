using System.Collections.Generic;
using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFB pragma
    /// </summary>
    public sealed class DefbPragma : PragmaLine
    {
        /// <summary>
        /// The bytes to define
        /// </summary>
        public List<ExpressionNode> Exprs { get; set; }
    }
}