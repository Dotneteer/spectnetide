using System;
using System.Globalization;
using Antlr4.Runtime.Tree;
using Spect.Net.EvalParser.Generated;
using Spect.Net.EvalParser.SyntaxTree;

namespace Spect.Net.EvalParser
{
    /// <summary>
    /// This visitor class processes the elements of the AST parsed by ANTLR
    /// </summary>
    public class Z80EvalVisitor: Z80EvalBaseVisitor<object>
    {
        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.compileUnit"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitCompileUnit(Z80EvalParser.CompileUnitContext context)
        {
            return new Z80ExpressionNode(
                (ExpressionNode)VisitExpr(context.expr()),
                context.formatSpec() != null
                    ? new FormatSpecifierNode(context.formatSpec().GetText().ToUpper())
                    : default(FormatSpecifierNode));
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.expr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitExpr(Z80EvalParser.ExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitOrExpr(context.orExpr());
            if (context.ChildCount == 1) return expr;

            var result = new ConditionalExpressionNode()
            {
                Condition = expr,
                TrueExpression = (ExpressionNode)VisitExpr(context.expr()[0]),
                FalseExpression = (ExpressionNode)VisitExpr(context.expr()[1])
            };
            return result.Condition == null || result.TrueExpression == null || result.FalseExpression == null
                ? null
                : result;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.orExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitOrExpr(Z80EvalParser.OrExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = VisitXorExpr(context.xorExpr()[0]);
            for (var i = 1; i < context.xorExpr().Length; i++)
            {
                var rightExpr = VisitXorExpr(context.xorExpr()[i]);
                var binExpr = new BitwiseOrOperationNode()
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                if (binExpr.LeftOperand == null || binExpr.RightOperand == null)
                {
                    return null;
                }
                expr = binExpr;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.xorExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitXorExpr(Z80EvalParser.XorExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = VisitAndExpr(context.andExpr()[0]);
            for (var i = 1; i < context.andExpr().Length; i++)
            {
                var rightExpr = VisitAndExpr(context.andExpr()[i]);
                var binExpr = new BitwiseXorOperationNode()
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                if (binExpr.LeftOperand == null || binExpr.RightOperand == null)
                {
                    return null;
                }
                expr = binExpr;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.andExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAndExpr(Z80EvalParser.AndExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = VisitEquExpr(context.equExpr()[0]);
            for (var i = 1; i < context.equExpr().Length; i++)
            {
                var rightExpr = VisitEquExpr(context.equExpr()[i]);
                var binExpr = new BitwiseAndOperationNode()
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                if (binExpr.LeftOperand == null || binExpr.RightOperand == null)
                {
                    return null;
                }
                expr = binExpr;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.equExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitEquExpr(Z80EvalParser.EquExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitRelExpr(context.relExpr()[0]);
            var opIndex = 1;
            for (var i = 1; i < context.relExpr().Length; i++)
            {
                var rightExpr = VisitRelExpr(context.relExpr()[i]);
                var opToken = context.GetTokenText(opIndex);
                var equExpr = opToken == "=="
                    ? new EqualOperationNode()
                    : new NotEqualOperationNode() as BinaryOperationNode;
                equExpr.LeftOperand = expr;
                equExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = equExpr;
                opIndex += 2;
                if (equExpr.LeftOperand == null || equExpr.RightOperand == null)
                {
                    return null;
                }
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.relExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitRelExpr(Z80EvalParser.RelExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitShiftExpr(context.shiftExpr()[0]);
            var opIndex = 1;
            for (var i = 1; i < context.shiftExpr().Length; i++)
            {
                var rightExpr = VisitShiftExpr(context.shiftExpr()[i]);
                var opToken = context.GetTokenText(opIndex);
                var relExpr = opToken == "<"
                    ? new LessThanOperationNode()
                    : (opToken == "<="
                        ? new LessThanOrEqualOperationNode()
                        : (opToken == ">"
                            ? new GreaterThanOperationNode()
                            : new GreaterThanOrEqualOperationNode() as BinaryOperationNode));

                relExpr.LeftOperand = expr;
                relExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = relExpr;
                opIndex += 2;
                if (relExpr.LeftOperand == null || relExpr.RightOperand == null)
                {
                    return null;
                }
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.shiftExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitShiftExpr(Z80EvalParser.ShiftExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitAddExpr(context.addExpr()[0]);
            var opIndex = 1;
            for (var i = 1; i < context.addExpr().Length; i++)
            {
                var rightExpr = VisitAddExpr(context.addExpr()[i]);
                var opToken = context.GetTokenText(opIndex);
                var shiftExpr = opToken == "<<"
                    ? new ShiftLeftOperationNode()
                    : new ShiftRightOperationNode() as BinaryOperationNode;
                shiftExpr.LeftOperand = expr;
                shiftExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = shiftExpr;
                opIndex += 2;
                if (shiftExpr.LeftOperand == null || shiftExpr.RightOperand == null)
                {
                    return null;
                }
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.addExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAddExpr(Z80EvalParser.AddExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitMultExpr(context.multExpr()[0]);
            var opIndex = 1;
            for (var i = 1; i < context.multExpr().Length; i++)
            {
                var rightExpr = VisitMultExpr(context.multExpr()[i]);
                var opToken = context.GetTokenText(opIndex);
                var addExpr = opToken == "+"
                    ? new AddOperationNode()
                    : new SubtractOperationNode() as BinaryOperationNode;
                addExpr.LeftOperand = expr;
                addExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = addExpr;
                opIndex += 2;
                if (addExpr.LeftOperand == null || addExpr.RightOperand == null)
                {
                    return null;
                }
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.multExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMultExpr(Z80EvalParser.MultExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitUnaryExpr(context.unaryExpr()[0]);
            var opIndex = 1;
            for (var i = 1; i < context.unaryExpr().Length; i++)
            {
                var rightExpr = VisitUnaryExpr(context.unaryExpr()[i]);
                var opToken = context.GetTokenText(opIndex);
                var multExpr = opToken == "*"
                    ? new MultiplyOperationNode()
                    : (opToken == "/"
                        ? new DivideOperationNode()
                        : new ModuloOperationNode() as BinaryOperationNode);
                multExpr.LeftOperand = expr;
                multExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = multExpr;
                opIndex += 2;
                if (multExpr.LeftOperand == null || multExpr.RightOperand == null)
                {
                    return null;
                }
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.unaryExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitUnaryExpr(Z80EvalParser.UnaryExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            if (context.unaryExpr() != null)
            {
                var unaryExpr = (ExpressionNode)VisitUnaryExpr(context.unaryExpr());
                if (unaryExpr == null)
                {
                    return null;
                }
                var op = context.GetChild(0).GetText();
                switch (op)
                {
                    case "+":
                        return new UnaryPlusNode() { Operand = unaryExpr };
                    case "-":
                        return new UnaryMinusNode() { Operand = unaryExpr };
                    case "~":
                        return new UnaryBitwiseNotNode() { Operand = unaryExpr };
                    case "!":
                        return new UnaryLogicalNotNode() { Operand = unaryExpr };
                }
            }
            if (context.literalExpr() != null)
            {
                return VisitLiteralExpr(context.literalExpr());
            }
            if (context.z80Spec() != null)
            {
                return VisitZ80Spec(context.z80Spec());
            }
            return context.symbolExpr() != null 
                ? VisitSymbolExpr(context.symbolExpr()) 
                : VisitExpr(context.GetChild(1) as Z80EvalParser.ExprContext);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.literalExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLiteralExpr(Z80EvalParser.LiteralExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var token = context.GetText();
            ushort value;
            // --- Hexadecimal literals
            if (token.StartsWith("#"))
            {
                value = ushort.Parse(token.Substring(1), NumberStyles.HexNumber);
            }
            else if (token.StartsWith("0x"))
            {
                value = ushort.Parse(token.Substring(2), NumberStyles.HexNumber);
            }
            else if (token.EndsWith("h", StringComparison.OrdinalIgnoreCase))
            {
                value = (ushort)int.Parse(token.Substring(0, token.Length - 1),
                    NumberStyles.HexNumber);
            }
            // --- Binary literals
            else if (token.StartsWith("%"))
            {
                value = (ushort)Convert.ToInt32(token.Substring(1).Replace("_", ""), 2);
            }
            else if (token.StartsWith("0b"))
            {
                value = (ushort)Convert.ToInt32(token.Substring(2).Replace("_", ""), 2);
            }
            // --- Character literals
            else if (token.StartsWith("'"))
            {
                var charExpr = context.GetText();
                var bytes = SyntaxHelper.SpectrumStringToBytes(charExpr.Substring(1, charExpr.Length - 2));
                value = bytes.Count == 0 ? (ushort)0x00 : bytes[0];
            }
            // --- Decimal literals
            else
            {
                value = int.TryParse(context.GetText(), out var shortVal) ? (ushort)shortVal : (ushort)0;
            }
            return new LiteralNode(value);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.z80Spec"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitZ80Spec(Z80EvalParser.Z80SpecContext context)
        {
            if (IsInvalidContext(context)) return null;

            if (context.reg8() != null || context.reg16() != null)
            {
                return new Z80RegisterNode(context.GetText().ToUpper());
            }
            if (context.flags() != null)
            {
                return new Z80FlagNode(context.GetText().ToUpper());
            }
            if (context.memIndirect() != null)
            {
                var result = new MemoryIndirectNode((ExpressionNode)VisitExpr(context.memIndirect().expr()));
                return context.memIndirect().RSBRAC().Symbol.StartIndex < 0 ? null : result;
            }

            if (context.wordMemIndirect() != null)
            {
                var result = new WordMemoryIndirectNode((ExpressionNode) VisitExpr(context.wordMemIndirect().expr()));
                return context.wordMemIndirect().RCBRAC().Symbol.StartIndex < 0 ? null : result;
            }
            return null;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80EvalParser.symbolExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitSymbolExpr(Z80EvalParser.SymbolExprContext context)
        {
            return IsInvalidContext(context)
                ? null
                : new SymbolNode()
                    {
                        SymbolName = context.GetText()
                    };
        }

        #region Helper methods

        /// <summary>
        /// Checks if the current context is invalid for further visiting
        /// </summary>
        /// <param name="context"></param>
        private bool IsInvalidContext(ITree context) => context == null || context.ChildCount == 0;

        #endregion
    }
}