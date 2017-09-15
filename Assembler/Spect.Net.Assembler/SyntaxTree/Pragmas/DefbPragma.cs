using System.Collections.Generic;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFB pragma
    /// </summary>
    public sealed class DefbPragma : PragmaBase
    {
        /// <summary>
        /// The bytes to define
        /// </summary>
        public List<ExpressionNode> Exprs { get; set; }
    }
}