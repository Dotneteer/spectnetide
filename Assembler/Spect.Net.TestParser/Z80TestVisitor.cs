using System;
using System.Globalization;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree;
using Spect.Net.TestParser.SyntaxTree.DataBlock;
using Spect.Net.TestParser.SyntaxTree.Expressions;
using Spect.Net.TestParser.SyntaxTree.TestBlock;

namespace Spect.Net.TestParser
{
    /// <summary>
    /// This visitor class processes the elements of the AST parsed by ANTLR
    /// </summary>
    public class Z80TestVisitor: Z80TestBaseVisitor<object>
    {
        private DataBlock _lastDataBlock;
        private TestBlock _lastTestBlock;
        private IncludeDirective _lastIncludeDirective;
        private MemoryPatternMember _lastMemoryPattern;

        /// <summary>
        /// Access the compilation results through this object
        /// </summary>
        public CompilationUnit Compilation { get; } = new CompilationUnit();

        #region Datablock visitors

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.dataBlock"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDataBlock(Z80TestParser.DataBlockContext context)
        {
            if (IsInvalidContext(context)) return null;

            // --- Datablock boundaries
            var block =_lastDataBlock = new DataBlock();
            Compilation.LanguageBlocks.Add(_lastDataBlock);
            block.Span = new TextSpan(context);

            // --- Tokens
            block.DataKeywordSpan = new TextSpan(context.GetChild(0));
            block.EndKeywordSpan = new TextSpan(context.GetChild(context.ChildCount - 1));
            base.VisitDataBlock(context);
            return block;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.valueDef"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitValueDef(Z80TestParser.ValueDefContext context)
        {
            if (IsInvalidContext(context)) return null;

            var member = new ValueMember
            {
                Span = new TextSpan(context),
                IdSpan = new TextSpan(context.GetChild(0)),
                Id = context.GetChild(0).NormalizeToken(),
                Expr = (ExpressionNode) VisitExpr(context.GetChild(2) as Z80TestParser.ExprContext)
            };
            _lastDataBlock.DataMembers.Add(member);
            return member;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.memPattern"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMemPattern(Z80TestParser.MemPatternContext context)
        {
            if (IsInvalidContext(context)) return null;

            var member = _lastMemoryPattern = new MemoryPatternMember
            {
                Span = new TextSpan(context),
                IdSpan = new TextSpan(context.GetChild(0)),
                Id = context.GetChild(0).NormalizeToken()
            };
            _lastDataBlock.DataMembers.Add(member);
            return base.VisitMemPattern(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.byteSet"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitByteSet(Z80TestParser.ByteSetContext context)
        {
            if (IsInvalidContext(context)) return null;
            
            var byteSet = new BytePattern
            {
                Span = new TextSpan(context),
                KeywordSpan = new TextSpan(context.GetChild(0))
            };
            var exprIndex = 1;
            while (exprIndex < context.ChildCount)
            {
                byteSet.Bytes.Add((ExpressionNode)VisitExpr(context.GetChild(exprIndex) as Z80TestParser.ExprContext));
                exprIndex += 2;
            }
            _lastMemoryPattern.Patterns.Add(byteSet);
            return byteSet;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.wordSet"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitWordSet(Z80TestParser.WordSetContext context)
        {
            if (IsInvalidContext(context)) return null;

            var wordSet = new WordPattern
            {
                Span = new TextSpan(context),
                KeywordSpan = new TextSpan(context.GetChild(0))
            };
            var exprIndex = 1;
            while (exprIndex < context.ChildCount)
            {
                wordSet.Words.Add((ExpressionNode)VisitExpr(context.GetChild(exprIndex) as Z80TestParser.ExprContext));
                exprIndex += 2;
            }
            _lastMemoryPattern.Patterns.Add(wordSet);
            return wordSet;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.text"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitText(Z80TestParser.TextContext context)
        {
            if (IsInvalidContext(context)) return null;

            var text = new TextPattern
            {
                Span = new TextSpan(context),
                KeywordSpan = new TextSpan(context.GetChild(0)),
                StringSpan = new TextSpan(context.GetChild(1)),
                String = context.GetChild(1).NormalizeToken()
            };
            _lastMemoryPattern.Patterns.Add(text);
            return text;
        }

        #endregion

        #region Include directive visitors

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.includeDirective"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitIncludeDirective(Z80TestParser.IncludeDirectiveContext context)
        {
            if (IsInvalidContext(context)) return null;

            // --- IncludeDirective boundaries
            var block = _lastIncludeDirective = new IncludeDirective();
            Compilation.LanguageBlocks.Add(_lastIncludeDirective);
            block.Span = new TextSpan(context);

            // --- Tokens
            block.IncludeKeywordSpan = new TextSpan(context.GetChild(0));
            block.StringSpan = new TextSpan(context.GetChild(1));
            var filename = context.GetChild(1).GetText();
            block.Filename = filename.Substring(1, filename.Length - 2);
            return block;
        }

        #endregion

        #region TestBlock visitors

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.testBlock"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTestBlock(Z80TestParser.TestBlockContext context)
        {
            if (IsInvalidContext(context)) return null;

            // --- TestBlock boundaries
            var block = _lastTestBlock = new TestBlock();
            block.Span = new TextSpan(context);
            Compilation.LanguageBlocks.Add(_lastTestBlock);


            base.VisitTestBlock(context);
            return block;
        }

        #endregion

        #region Expression visitors

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.expr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitExpr(Z80TestParser.ExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitOrExpr(context.GetChild(0) as Z80TestParser.OrExprContext);
            if (context.ChildCount == 1) return expr;

            return new ConditionalExpressionNode(context)
            {
                Condition = expr,
                TrueExpression = (ExpressionNode)VisitExpr(context.GetChild(2) as Z80TestParser.ExprContext),
                FalseExpression = (ExpressionNode)VisitExpr(context.GetChild(4) as Z80TestParser.ExprContext)
            };
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.orExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitOrExpr(Z80TestParser.OrExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = VisitXorExpr(context.GetChild(0) as Z80TestParser.XorExprContext);

            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitXorExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.XorExprContext);
                expr = new BitwiseOrOperationNode(context)
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.xorExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitXorExpr(Z80TestParser.XorExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = VisitAndExpr(context.GetChild(0) as Z80TestParser.AndExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitAndExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.AndExprContext);
                expr = new BitwiseXorOperationNode(context)
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.andExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAndExpr(Z80TestParser.AndExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = VisitEquExpr(context.GetChild(0)
                as Z80TestParser.EquExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitEquExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.EquExprContext);
                expr = new BitwiseAndOperationNode(context)
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.equExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitEquExpr(Z80TestParser.EquExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitRelExpr(context.GetChild(0) as Z80TestParser.RelExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitRelExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.RelExprContext);
                var opToken = context.GetChild(nextChildIndex - 1).NormalizeToken();
                var equExpr = opToken == "=="
                    ? new EqualOperationNode(context)
                    : new NotEqualOperationNode(context) as BinaryOperationNode;
                equExpr.LeftOperand = expr;
                equExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = equExpr;
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.relExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitRelExpr(Z80TestParser.RelExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitShiftExpr(context.GetChild(0) as Z80TestParser.ShiftExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitShiftExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.ShiftExprContext);
                var opToken = context.GetChild(nextChildIndex - 1).NormalizeToken();
                var relExpr = opToken == "<"
                    ? new LessThanOperationNode(context)
                    : (opToken == "<="
                        ? new LessThanOrEqualOperationNode(context)
                        : (opToken == ">"
                            ? new GreaterThanOperationNode(context)
                            : new GreaterThanOrEqualOperationNode(context) as BinaryOperationNode));

                relExpr.LeftOperand = expr;
                relExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = relExpr;
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.shiftExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitShiftExpr(Z80TestParser.ShiftExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitAddExpr(context.GetChild(0) as Z80TestParser.AddExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitAddExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.AddExprContext);
                var opToken = context.GetChild(nextChildIndex - 1).NormalizeToken();
                var shiftExpr = opToken == "<<"
                    ? new ShiftLeftOperationNode(context)
                    : new ShiftRightOperationNode(context) as BinaryOperationNode;

                shiftExpr.LeftOperand = expr;
                shiftExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = shiftExpr;
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.addExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAddExpr(Z80TestParser.AddExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitMultExpr(context.GetChild(0) as Z80TestParser.MultExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitMultExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.MultExprContext);
                var opToken = context.GetChild(nextChildIndex - 1).NormalizeToken();
                var addExpr = opToken == "+"
                    ? new AddOperationNode(context)
                    : new SubtractOperationNode(context) as BinaryOperationNode;

                addExpr.LeftOperand = expr;
                addExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = addExpr;
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.multExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMultExpr(Z80TestParser.MultExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitUnaryExpr(context.GetChild(0) as Z80TestParser.UnaryExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitUnaryExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.UnaryExprContext);
                var opToken = context.GetChild(nextChildIndex - 1).NormalizeToken();
                var multExpr = opToken == "*"
                    ? new MultiplyOperationNode(context)
                    : (opToken == "/"
                        ? new DivideOperationNode(context)
                        : new ModuloOperationNode(context) as BinaryOperationNode);

                multExpr.LeftOperand = expr;
                multExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = multExpr;
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.unaryExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitUnaryExpr(Z80TestParser.UnaryExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var child0 = context.GetChild(0);
            if (child0 == null) return null;

            if (child0 is Z80TestParser.LiteralExprContext)
            {
                return VisitLiteralExpr(child0 as Z80TestParser.LiteralExprContext);
            }
            if (child0 is Z80TestParser.SymbolExprContext)
            {
                return VisitSymbolExpr(child0 as Z80TestParser.SymbolExprContext);
            }
            if (child0.GetText() == "+")
            {
                return new UnaryPlusNode(context)
                {
                    Operand = (ExpressionNode)VisitUnaryExpr(context.GetChild(1) as Z80TestParser.UnaryExprContext)
                };
            }
            if (child0.GetText() == "-")
            {
                return new UnaryMinusNode(context)
                {
                    Operand = (ExpressionNode)VisitUnaryExpr(context.GetChild(1) as Z80TestParser.UnaryExprContext)
                };
            }
            if (child0.GetText() == "~")
            {
                return new UnaryBitwiseNotNode(context)
                {
                    Operand = (ExpressionNode)VisitUnaryExpr(context.GetChild(1) as Z80TestParser.UnaryExprContext)
                };
            }
            if (child0.GetText() == "!")
            {
                return new UnaryLogicalNotNode(context)
                {
                    Operand = (ExpressionNode)VisitUnaryExpr(context.GetChild(1) as Z80TestParser.UnaryExprContext)
                };
            }
            return VisitExpr(context.GetChild(1) as Z80TestParser.ExprContext);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.literalExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLiteralExpr(Z80TestParser.LiteralExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var token = context.NormalizeToken();
            ushort value;
            // --- Hexadecimal literals
            if (token.StartsWith("#"))
            {
                value = ushort.Parse(token.Substring(1), NumberStyles.HexNumber);
            }
            else if (token.StartsWith("0X"))
            {
                value = ushort.Parse(token.Substring(2), NumberStyles.HexNumber);
            }
            else if (token.EndsWith("H", StringComparison.OrdinalIgnoreCase))
            {
                value = (ushort)int.Parse(token.Substring(0, token.Length - 1),
                    NumberStyles.HexNumber);
            }
            // --- Binary literals
            else if (token.StartsWith("%"))
            {
                value = (ushort)Convert.ToInt32(token.Substring(1).Replace("_", ""), 2);
            }
            else if (token.StartsWith("0B"))
            {
                value = (ushort)Convert.ToInt32(token.Substring(2).Replace("_", ""), 2);
            }
            // --- Character literals
            else if (token.StartsWith("\""))
            {
                var charExpr = context.GetText();
                var bytes = SyntaxHelper.SpectrumStringToBytes(charExpr.Substring(1, charExpr.Length - 2));
                value = bytes.Count == 0 ? (ushort)0x00 : bytes[0];
            }
            // --- Decimal literals
            else
            {
                value = (ushort)int.Parse(context.NormalizeToken());
            }
            return new LiteralNode(context)
            {
                LiteralValue = value
            };
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.symbolExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitSymbolExpr(Z80TestParser.SymbolExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            return new IdentifierNode(context)
            {
                SymbolName = context.GetChild(0).NormalizeToken()
            };
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.registerSpec"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitRegisterSpec(Z80TestParser.RegisterSpecContext context)
        {
            if (IsInvalidContext(context)) return null;

            return base.VisitRegisterSpec(context);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Checks if the current context is invalid for further visiting
        /// </summary>
        /// <param name="context"></param>
        private bool IsInvalidContext(ITree context) => context == null || context.ChildCount == 0;

        #endregion

    }
}