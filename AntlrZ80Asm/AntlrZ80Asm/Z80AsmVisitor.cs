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
            var expr = VisitXorExpr(context.children[0] as Z80AsmParser.XorExprContext);
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
            return VisitAndExpr(context.children[0] as Z80AsmParser.AndExprContext);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.andExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAndExpr(Z80AsmParser.AndExprContext context)
        {
            return VisitShiftExpr(context.children[0] as Z80AsmParser.ShiftExprContext);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.shiftExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitShiftExpr(Z80AsmParser.ShiftExprContext context)
        {
            return VisitAddExpr(context.children[0] as Z80AsmParser.AddExprContext);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.addExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAddExpr(Z80AsmParser.AddExprContext context)
        {
            return VisitMultExpr(context.children[0] as Z80AsmParser.MultExprContext);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.multExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMultExpr(Z80AsmParser.MultExprContext context)
        {
            return VisitUnExpr(context.children[0] as Z80AsmParser.UnExprContext);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.unExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitUnExpr(Z80AsmParser.UnExprContext context)
        {
            return VisitLiteralExpr(context.children[0] as Z80AsmParser.LiteralExprContext);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.literalExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLiteralExpr(Z80AsmParser.LiteralExprContext context)
        {
            var token = context.NormalizeToken();
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
            return base.VisitSymbolExpr(context);
        }

        #endregion
    }
}