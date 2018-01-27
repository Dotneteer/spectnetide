﻿using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an abstract unary operation
    /// </summary>
    public abstract class UnaryExpressionNode : ExpressionNode
    {
        /// <summary>
        /// Operand of the unary operation
        /// </summary>
        public ExpressionNode Operand { get; set; }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IExpressionEvaluationContext evalContext)
            => Operand.ReadyToEvaluate(evalContext);

        protected UnaryExpressionNode(ParserRuleContext context) : base(context)
        {
        }
    }
}