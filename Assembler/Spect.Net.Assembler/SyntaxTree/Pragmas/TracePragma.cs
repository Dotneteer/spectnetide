using System.Collections.Generic;
using System.Linq;
using Spect.Net.Assembler.Generated;
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

        public TracePragma(IZ80AsmVisitorContext visitorContext, Z80AsmParser.TracePragmaContext context)
        {
            IsHex = context.TRACEHEX() != null;
            Exprs = context.expr() != null 
                ? context.expr().Select(visitorContext.GetExpression).ToList() 
                : new List<ExpressionNode>();
        }
    }
}