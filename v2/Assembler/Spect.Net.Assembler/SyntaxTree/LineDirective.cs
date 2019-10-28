using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents a #line preprocessor directive
    /// </summary>
    public sealed class LineDirective : OperationBase
    {
        /// <summary>
        /// Optional expression
        /// </summary>
        public ExpressionNode Expr { get; set; }

        /// <summary>
        /// Include file name
        /// </summary>
        public string Filename { get; set; }

        public LineDirective(IZ80AsmVisitorContext visitorContext, Z80AsmParser.DirectiveContext context) :
            base(context)
        {
            if (context.expr() != null)
            {
                Expr = visitorContext.GetExpression(context.expr());
            }

            if (context.STRING() != null)
            {
                visitorContext.AddString(context.STRING());
            }
            Filename = context.STRING().NormalizeString();
        }
    }
}
