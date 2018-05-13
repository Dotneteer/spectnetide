using System;
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

        public ForStatement(string forVariable, ExpressionNode from, ExpressionNode to, ExpressionNode step)
        {
            ForVariable = forVariable;
            From = from;
            To = to;
            Step = step;
        }
    }
}