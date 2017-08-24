using System;
using System.Globalization;
using AntlrZ80Asm.SyntaxTree;
using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm
{
    /// <summary>
    /// This visitor class processes the elements of the AST parsed by ANTLR
    /// </summary>
    public class Z80AsmVisitor: Z80AsmBaseVisitor<object>
    {
        private string _label;

        /// <summary>
        /// Access the comilation results through this object
        /// </summary>
        public CompilationUnit Compilation { get; } = new CompilationUnit();

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.asmline"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAsmline(Z80AsmParser.AsmlineContext context)
        {
            _label = context.label()?.children?[0].GetText();
            return base.VisitAsmline(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.trivialInstruction"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTrivialInstruction(Z80AsmParser.TrivialInstructionContext context)
        {
            var line = new TrivialInstruction
            {
                Label = _label,
                Mnemonic = context.children[0].NormalizeToken()
            };
            Compilation.Lines.Add(line);
            return base.VisitTrivialInstruction(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.load8BitInstruction"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLoad8BitInstruction(Z80AsmParser.Load8BitInstructionContext context)
        {
            // 'LD' (8-bit-reg) ',' (8-bit-reg)
            var line = new LoadReg8ToReg8Instruction
            {
                Destination = context.children[1].NormalizeToken(),
                Source = context.children[3].NormalizeToken()
            };
            Compilation.Lines.Add(line);
            return line;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.load8BitWithValueInstruction"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLoad8BitWithValueInstruction(Z80AsmParser.Load8BitWithValueInstructionContext context)
        {
            // 'LD' (8-bit-reg) ',' Expression
            var line = new LoadValueToRegInstruction()
            {
                Destination = context.children[1].NormalizeToken(),
                Expression = (ExpressionNode)VisitExpr(context.children[3] 
                    as Z80AsmParser.ExprContext)
            };
            Compilation.Lines.Add(line);
            return line;
        }

        #region Expression handling

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.expr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitExpr(Z80AsmParser.ExprContext context)
        {
            var expr = VisitXorExpr(context.children[0] 
                as Z80AsmParser.XorExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitXorExpr(context.children[nextChildIndex] 
                    as Z80AsmParser.XorExprContext);
                expr = new BitwiseOrOperationNode
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.xorExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitXorExpr(Z80AsmParser.XorExprContext context)
        {
            var expr = VisitAndExpr(context.children[0] 
                as Z80AsmParser.AndExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitAndExpr(context.children[nextChildIndex]
                    as Z80AsmParser.AndExprContext);
                expr = new BitwiseXorOperationNode
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.andExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAndExpr(Z80AsmParser.AndExprContext context)
        {
            var expr = VisitShiftExpr(context.children[0] 
                as Z80AsmParser.ShiftExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitShiftExpr(context.children[nextChildIndex]
                    as Z80AsmParser.ShiftExprContext);
                expr = new BitwiseAndOperationNode
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.shiftExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitShiftExpr(Z80AsmParser.ShiftExprContext context)
        {
            var expr = (ExpressionNode)VisitAddExpr(context.children[0] as Z80AsmParser.AddExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitAddExpr(context.children[nextChildIndex]
                    as Z80AsmParser.AddExprContext);
                var opToken = context.children[nextChildIndex - 1].NormalizeToken();
                var shiftExpr = opToken == "<<" 
                    ? new ShiftLeftOperationNode() 
                    : new ShiftRightOperationNode() as BinaryOperationNode;

                shiftExpr.LeftOperand = expr;
                shiftExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = shiftExpr;
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.addExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAddExpr(Z80AsmParser.AddExprContext context)
        {
            var expr = (ExpressionNode)VisitMultExpr(context.children[0] as Z80AsmParser.MultExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitMultExpr(context.children[nextChildIndex]
                    as Z80AsmParser.MultExprContext);
                var opToken = context.children[nextChildIndex - 1].NormalizeToken();
                var addExpr = opToken == "+"
                    ? new AddOperationNode()
                    : new SubtractOperationNode() as BinaryOperationNode;

                addExpr.LeftOperand = expr;
                addExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = addExpr;
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.multExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMultExpr(Z80AsmParser.MultExprContext context)
        {
            var expr = (ExpressionNode)VisitUnaryExpr(context.children[0] as Z80AsmParser.UnaryExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitUnaryExpr(context.children[nextChildIndex]
                    as Z80AsmParser.UnaryExprContext);
                var opToken = context.children[nextChildIndex - 1].NormalizeToken();
                var multExpr = opToken == "*"
                    ? new MultiplyOperationNode()
                    : (opToken == "/"
                        ? new DivideOperationNode()
                        : new ModuloOperationNode() as BinaryOperationNode);

                multExpr.LeftOperand = expr;
                multExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = multExpr;
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.unaryExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitUnaryExpr(Z80AsmParser.UnaryExprContext context)
        {
            if (context.children[0] is Z80AsmParser.LiteralExprContext litContext)
            {
                return VisitLiteralExpr(litContext);
            }
            if (context.children[0] is Z80AsmParser.SymbolExprContext symbolContext)
            {
                return VisitSymbolExpr(symbolContext);
            }
            if (context.children[0].GetText() == "+")
            {
                return new UnaryPlusNode
                {
                    Operand = (ExpressionNode) VisitUnaryExpr(context.children[1] as Z80AsmParser.UnaryExprContext)
                };
            }
            if (context.children[0].GetText() == "-")
            {
                return new UnaryMinusNode
                {
                    Operand = (ExpressionNode)VisitUnaryExpr(context.children[1] as Z80AsmParser.UnaryExprContext)
                };
            }
            return VisitExpr(context.children[1] as Z80AsmParser.ExprContext);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.literalExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLiteralExpr(Z80AsmParser.LiteralExprContext context)
        {
            var token = context.NormalizeToken();
            if (token == "$")
            {
                return new CurrentAddresNode();
            }

            ushort value;
            if (token.StartsWith("#"))
            {
                value = ushort.Parse(token.Substring(1), NumberStyles.HexNumber);
            }
            else if (token.EndsWith("H", StringComparison.OrdinalIgnoreCase))
            {
                value = ushort.Parse(token.Substring(0, token.Length - 1), 
                    NumberStyles.HexNumber);
            }
            else
            {
                value = ushort.Parse(context.NormalizeToken());
            }
            return new LiteralNode
            {
                LiteralValue = value
            };
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.symbolExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitSymbolExpr(Z80AsmParser.SymbolExprContext context)
        {
            return new IdentifierNode
            {
                SymbolName = context.children[0].NormalizeToken()
            };
        }

        #endregion
    }
}