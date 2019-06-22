using System;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents a WHILE statement
    /// </summary>
    public sealed class WhileStatement : BlockStatementBase
    {
        /// <summary>
        /// Type of end statement
        /// </summary>
        public override Type EndType => typeof(WhileEndStatement);

        /// <summary>
        /// Gets the name of the end statement
        /// </summary>
        public override string EndStatementName => "ENDW/WEND";

        /// <summary>
        /// Loop expression
        /// </summary>
        public ExpressionNode Expr { get; }

        public WhileStatement(IZ80AsmVisitorContext visitorContext, Z80AsmParser.WhileStatementContext context)
        {
            Expr = visitorContext.GetExpression(context.expr());
        }
    }
}