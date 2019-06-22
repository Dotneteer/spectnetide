using System;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// Represents a FOR statement
    /// </summary>
    public class ForStatement: BlockStatementBase
    {
        /// <summary>
        /// Type of end statement
        /// </summary>
        public override Type EndType => typeof(NextStatement);

        /// <summary>
        /// Gets the name of the end statement
        /// </summary>
        public override string EndStatementName => "NEXT";

        /// <summary>
        /// The variable of the FOR statement
        /// </summary>
        public string ForVariable { get; }

        /// <summary>
        /// FROM expression
        /// </summary>
        public ExpressionNode From { get; }

        /// <summary>
        /// TO expression
        /// </summary>
        public ExpressionNode To { get; }

        /// <summary>
        /// Optional STEP expression
        /// </summary>
        public ExpressionNode Step { get; }

        public ForStatement(IZ80AsmVisitorContext visitorContext, Z80AsmParser.ForStatementContext context)
        {
            if (context.IDENTIFIER() != null)
            {
                visitorContext.AddIdentifier(context.IDENTIFIER());
                ForVariable = context.IDENTIFIER()?.NormalizeToken();
            }
            if (context.TO() != null)
            {
                visitorContext.AddStatement(context.TO());
            }

            if (context.STEP() != null)
            {
                visitorContext.AddStatement(context.STEP());
            }

            From = context.expr().Length > 0 ? visitorContext.GetExpression(context.expr()[0]) : null;
            To = context.expr().Length > 1 ? visitorContext.GetExpression(context.expr()[1]) : null;
            Step = context.expr().Length > 2 ? visitorContext.GetExpression(context.expr()[2]) : null;
        }
    }
}