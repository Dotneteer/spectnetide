using System.Collections.Generic;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the TRACE and TRACEHEX pragmas
    /// </summary>
    public sealed class TracePragma : PragmaBase
    {
        /// <summary>
        /// Indicates if TRACEHEX is used
        /// </summary>
        public bool IsHex { get;  }

        /// <summary>
        /// The expressions to output
        /// </summary>
        public List<ExpressionNode> Exprs { get;  }

        public TracePragma(bool isHex, List<ExpressionNode> exprs)
        {
            IsHex = isHex;
            Exprs = exprs;
        }
    }
}