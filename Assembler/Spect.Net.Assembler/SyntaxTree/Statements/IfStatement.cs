using System;
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

        public IfStatement(ExpressionNode expr)
        {
            Type = IfStatementType.If;
            Expr = expr;
        }

        public IfStatement(IdentifierNode symbol, bool isIfused)
        {
            Type = isIfused ? IfStatementType.IfUsed : IfStatementType.IfNotUsed;
            Symbol = symbol;
        }
    }

    /// <summary>
    /// This enumeration represents the type of the IF statement
    /// </summary>
    public enum IfStatementType
    {
        If,
        IfUsed,
        IfNotUsed
    }
}

