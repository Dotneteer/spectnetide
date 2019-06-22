using System;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents an IF statement
    /// </summary>
    public class IfStatement : BlockStatementBase
    {
        /// <summary>
        /// Type of end statement
        /// </summary>
        public override Type EndType => typeof(IfEndStatement);

        /// <summary>
        /// Gets the name of the end statement
        /// </summary>
        public override string EndStatementName => "ENDIF";

        /// <summary>
        /// Type of IF condition
        /// </summary>
        public IfStatementType Type { get;  }

        /// <summary>
        /// IF expression condition expression
        /// </summary>
        public ExpressionNode Expr { get; }

        /// <summary>
        /// IFUSED/IFNUSED symbol
        /// </summary>
        public IdentifierNode Symbol { get; }

        public IfStatement(IZ80AsmVisitorContext visitorContext, Z80AsmParser.IfStatementContext context)
        {
            if (context.IFSTMT() != null)
            {
                Expr = visitorContext.GetExpression(context.expr());
                Type = IfStatementType.If;
                return;
            }

            Type = context.IFUSED() != null ? IfStatementType.IfUsed : IfStatementType.IfNotUsed;
            Symbol = new IdentifierNode(context.symbol());
        }
    }
}

