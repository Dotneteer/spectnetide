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
        /// Span of the FOR variable
        /// </summary>
        public TextSpan IdSpan { get; }

        /// <summary>
        /// Span of the TO keyword
        /// </summary>
        public TextSpan ToKeywordSpan { get; }

        /// <summary>
        /// Span of the STEP keyword
        /// </summary>
        public TextSpan StepKeywordSpan { get; }

        /// <summary>
        /// Optional STEP expression
        /// </summary>
        public ExpressionNode Step { get; }

        public ForStatement(TextSpan idSpan, TextSpan toKwd, TextSpan stepKwd, 
            string forVariable, ExpressionNode from, ExpressionNode to, ExpressionNode step)
        {
            IdSpan = idSpan;
            ToKeywordSpan = toKwd;
            StepKeywordSpan = stepKwd;
            ForVariable = forVariable;
            From = from;
            To = to;
            Step = step;
        }
    }
}