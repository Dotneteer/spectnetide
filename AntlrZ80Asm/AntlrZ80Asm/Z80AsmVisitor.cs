using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using AntlrZ80Asm.SyntaxTree;
using AntlrZ80Asm.SyntaxTree.Expressions;
using AntlrZ80Asm.SyntaxTree.Operations;
using AntlrZ80Asm.SyntaxTree.Pragmas;
// ReSharper disable PossibleNullReferenceException
// ReSharper disable UsePatternMatching

namespace AntlrZ80Asm
{
    /// <summary>
    /// This visitor class processes the elements of the AST parsed by ANTLR
    /// </summary>
    public class Z80AsmVisitor: Z80AsmBaseVisitor<object>
    {
        private int _sourceLine;
        private int _firstPos;
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
            _label = null;
            _sourceLine = context.Start.Line;
            _firstPos = context.Start.Column;
            return base.VisitAsmline(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.label"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLabel(Z80AsmParser.LabelContext context)
        {
            _label = context.GetChild(0).NormalizeToken();
            return base.VisitLabel(context);
        }

        #region Preprocessor directives

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.directive"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDirective(Z80AsmParser.DirectiveContext context)
        {
            if (context.GetChild(0).NormalizeToken() == "#INCLUDE")
            {
                return AddLine(new IncludeDirective
                {
                    Filename = context.GetChild(1).NormalizeString()
                });
            }
            return AddLine(new Directive
            {
                Mnemonic = context.GetChild(0).NormalizeToken(),
                Identifier = context.ChildCount > 1
                    ? context.GetChild(1).NormalizeToken()
                    : null,
                Expr = context.GetChild(1) is Z80AsmParser.ExprContext
                    ? (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
                    : null
            });
        }

        #endregion

        #region Pragma handling

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.orgPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitOrgPragma(Z80AsmParser.OrgPragmaContext context)
            => AddLine(new OrgPragma
            {
                Expr = (ExpressionNode) VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
            });

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.entPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitEntPragma(Z80AsmParser.EntPragmaContext context)
            => AddLine(new EntPragma
            {
                Expr = (ExpressionNode) VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
            });

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.dispPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDispPragma(Z80AsmParser.DispPragmaContext context)
            => AddLine(new DispPragma
            {
                Expr = (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
            });

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.equPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitEquPragma(Z80AsmParser.EquPragmaContext context)
            => AddLine(new EquPragma
            {
                Expr = (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
            });

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.skipPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitSkipPragma(Z80AsmParser.SkipPragmaContext context)
            => AddLine(new SkipPragma
            {
                Expr = (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext),
                Fill = context.ChildCount > 3
                    ? (ExpressionNode)VisitExpr(context.GetChild(3) as Z80AsmParser.ExprContext)
                    : null
            });

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.defbPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDefbPragma(Z80AsmParser.DefbPragmaContext context)
        {
            var exprs = new List<ExpressionNode>();
            for (var i = 1; i < context.ChildCount; i += 2)
            {
                exprs.Add((ExpressionNode) VisitExpr(context.GetChild(i) as Z80AsmParser.ExprContext));
            }
            return AddLine(new DefbPragma
            {
                Exprs = exprs
            });
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.defwPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDefwPragma(Z80AsmParser.DefwPragmaContext context)
        {
            var exprs = new List<ExpressionNode>();
            for (var i = 1; i < context.ChildCount; i += 2)
            {
                exprs.Add((ExpressionNode)VisitExpr(context.GetChild(i) as Z80AsmParser.ExprContext));
            }
            return AddLine(new DefwPragma
            {
                Exprs = exprs
            });
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.defmPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDefmPragma(Z80AsmParser.DefmPragmaContext context)
            => AddLine(new DefmPragma
            {
                Message = context.GetChild(1).GetText()
            });

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.externPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitExternPragma(Z80AsmParser.ExternPragmaContext context)
            => AddLine(new ExternPragma());

        #endregion

        #region Trivial operations

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.trivialOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTrivialOperation(Z80AsmParser.TrivialOperationContext context)
            => AddLine(new TrivialOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            });

        #endregion

        #region Compound operations

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.compoundOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitCompoundOperation(Z80AsmParser.CompoundOperationContext context)
        {
            var op = new CompoundOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            };

            var operandFound = false;
            foreach (var child in context.children)
            {
                // --- Collect operands
                var operandChild = child as Z80AsmParser.OperandContext;
                if (operandChild != null)
                {
                    if (!operandFound)
                    {
                        op.Operand = (Operand) VisitOperand(operandChild);
                    }
                    else
                    {
                        op.Operand2 = (Operand)VisitOperand(operandChild);
                    }
                    operandFound = true;
                    continue;
                }

                // --- Collect optional condition
                var condChild = child as Z80AsmParser.ConditionContext;
                if (condChild != null)
                {
                    op.Condition = condChild.GetText().NormalizeToken();
                    continue;
                }

                // --- Collect optional bit index
                var exprChild = child as Z80AsmParser.ExprContext;
                if (exprChild != null)
                {
                    op.BitIndex = (ExpressionNode) VisitExpr(exprChild);
                }
            }
            return AddLine(op);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.operand"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitOperand(Z80AsmParser.OperandContext context)
        {
            // --- The context has exactly one child
            var child = context.GetChild(0);
            var op = new Operand();
            if (child is Z80AsmParser.Reg8Context)
            {
                op.Type = OperandType.Reg8;
                op.Register = child.GetText().NormalizeToken();
            }
            else if (child is Z80AsmParser.Reg8IdxContext)
            {
                op.Type = OperandType.Reg8Idx;
                op.Register = child.GetText().NormalizeToken();
            }
            else if (child is Z80AsmParser.Reg8SpecContext)
            {
                op.Type = OperandType.Reg8Spec;
                op.Register = child.GetText().NormalizeToken();
            }
            else if (child is Z80AsmParser.Reg16Context)
            {
                op.Type = OperandType.Reg16;
                op.Register = child.GetText().NormalizeToken();
            }
            else if (child is Z80AsmParser.Reg16IdxContext)
            {
                op.Type = OperandType.Reg16Idx;
                op.Register = child.GetText().NormalizeToken();
            }
            else if (child is Z80AsmParser.Reg16SpecContext)
            {
                op.Type = OperandType.Reg16Spec;
                op.Register = child.GetText().NormalizeToken();
            }
            else if (child is Z80AsmParser.MemIndirectContext)
            {
                var expContext = child.GetChild(1) as Z80AsmParser.ExprContext;
                op.Type = OperandType.MemIndirect;
                op.Expression = (ExpressionNode)VisitExpr(expContext);
            }
            else if (child is Z80AsmParser.RegIndirectContext)
            {
                op.Type = OperandType.RegIndirect;
                op.Register = child.GetText().NormalizeToken();
            }
            else if (child is Z80AsmParser.CPortContext)
            {
                op.Type = OperandType.CPort;
            }
            else if (child is Z80AsmParser.IndexedAddrContext)
            {
                op.Type = OperandType.IndexedAddress;
                var indexedAddrContext = child as Z80AsmParser.IndexedAddrContext;
                if (indexedAddrContext.ChildCount > 3)
                {
                    op.Expression = indexedAddrContext.GetChild(3) is Z80AsmParser.LiteralExprContext
                        ? (ExpressionNode)VisitLiteralExpr(
                            indexedAddrContext.GetChild(3) as Z80AsmParser.LiteralExprContext)
                        : indexedAddrContext.GetChild(3).NormalizeToken() == "["
                            ? (ExpressionNode)VisitExpr(indexedAddrContext.GetChild(4) as Z80AsmParser.ExprContext)
                            : (ExpressionNode)VisitSymbolExpr(
                                indexedAddrContext.GetChild(3) as Z80AsmParser.SymbolExprContext);
                }
                op.Register = indexedAddrContext.GetChild(1).NormalizeToken();
                op.Sign = indexedAddrContext.ChildCount > 3
                    ? indexedAddrContext.GetChild(2).NormalizeToken()
                    : null;
            }
            else if (child is Z80AsmParser.ExprContext)
            {
                op.Type = OperandType.Expr;
                op.Expression = (ExpressionNode)VisitExpr(child as Z80AsmParser.ExprContext);
            }
            return op;
        }

        #endregion

        #region Expression handling

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.expr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitExpr(Z80AsmParser.ExprContext context)
        {
            var expr = (ExpressionNode)VisitOrExpr(context.GetChild(0) as Z80AsmParser.OrExprContext);
            if (context.ChildCount == 1) return expr;

            return new ConditionalExpressionNode
            {
                Condition = expr,
                TrueExpression = (ExpressionNode)VisitExpr(context.GetChild(2) as Z80AsmParser.ExprContext),
                FalseExpression = (ExpressionNode)VisitExpr(context.GetChild(4) as Z80AsmParser.ExprContext)
            };
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.orExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitOrExpr(Z80AsmParser.OrExprContext context)
        {
            var expr = VisitXorExpr(context.GetChild(0)
                as Z80AsmParser.XorExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitXorExpr(context.GetChild(nextChildIndex)
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
            var expr = VisitAndExpr(context.GetChild(0)
                as Z80AsmParser.AndExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitAndExpr(context.GetChild(nextChildIndex)
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
            var expr = VisitEquExpr(context.GetChild(0)
                as Z80AsmParser.EquExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitEquExpr(context.GetChild(nextChildIndex)
                    as Z80AsmParser.EquExprContext);
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
        /// Visit a parse tree produced by <see cref="Z80AsmParser.equExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitEquExpr(Z80AsmParser.EquExprContext context)
        {
            var expr = (ExpressionNode)VisitRelExpr(context.GetChild(0) as Z80AsmParser.RelExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitRelExpr(context.GetChild(nextChildIndex)
                    as Z80AsmParser.RelExprContext);
                var opToken = context.GetChild(nextChildIndex - 1).NormalizeToken();
                var equExpr = opToken == "=="
                    ? new EqualOperationNode()
                    : new NotEqualOperationNode() as BinaryOperationNode;
                equExpr.LeftOperand = expr;
                equExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = equExpr;
                nextChildIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.relExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitRelExpr(Z80AsmParser.RelExprContext context)
        {
            var expr = (ExpressionNode)VisitShiftExpr(context.GetChild(0) as Z80AsmParser.ShiftExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitShiftExpr(context.GetChild(nextChildIndex)
                    as Z80AsmParser.ShiftExprContext);
                var opToken = context.GetChild(nextChildIndex - 1).NormalizeToken();
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
            var expr = (ExpressionNode)VisitAddExpr(context.GetChild(0) as Z80AsmParser.AddExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitAddExpr(context.GetChild(nextChildIndex)
                    as Z80AsmParser.AddExprContext);
                var opToken = context.GetChild(nextChildIndex - 1).NormalizeToken();
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
            var expr = (ExpressionNode)VisitMultExpr(context.GetChild(0) as Z80AsmParser.MultExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitMultExpr(context.GetChild(nextChildIndex)
                    as Z80AsmParser.MultExprContext);
                var opToken = context.GetChild(nextChildIndex - 1).NormalizeToken();
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
            var expr = (ExpressionNode)VisitUnaryExpr(context.GetChild(0) as Z80AsmParser.UnaryExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitUnaryExpr(context.GetChild(nextChildIndex)
                    as Z80AsmParser.UnaryExprContext);
                var opToken = context.GetChild(nextChildIndex - 1).NormalizeToken();
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
            var child0 = context.GetChild(0);
            if (child0 is Z80AsmParser.LiteralExprContext)
            {
                return VisitLiteralExpr(child0 as Z80AsmParser.LiteralExprContext);
            }
            if (child0 is Z80AsmParser.SymbolExprContext)
            {
                return VisitSymbolExpr(child0 as Z80AsmParser.SymbolExprContext);
            }
            if (child0.GetText() == "+")
            {
                return new UnaryPlusNode
                {
                    Operand = (ExpressionNode) VisitUnaryExpr(context.GetChild(1) as Z80AsmParser.UnaryExprContext)
                };
            }
            if (child0.GetText() == "-")
            {
                return new UnaryMinusNode
                {
                    Operand = (ExpressionNode)VisitUnaryExpr(context.GetChild(1) as Z80AsmParser.UnaryExprContext)
                };
            }
            return VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext);
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
                return new CurrentAddressNode();
            }

            ushort value;
            if (token.StartsWith("#"))
            {
                value = ushort.Parse(token.Substring(1), NumberStyles.HexNumber);
            }
            else if (token.EndsWith("H", StringComparison.OrdinalIgnoreCase))
            {
                value = (ushort)int.Parse(token.Substring(0, token.Length - 1), 
                    NumberStyles.HexNumber);
            }
            else if (token.StartsWith("\""))
            {
                value = token == "\\\"" ? '\"' : token[1];
            }
            else
            {
                value = (ushort)int.Parse(context.NormalizeToken());
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
                SymbolName = context.GetChild(0).NormalizeToken()
            };
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Adds an assebmbly line to the compilation
        /// </summary>
        /// <param name="line">Line to add</param>
        /// <returns>The newly added line</returns>
        private SourceLineBase AddLine(SourceLineBase line)
        {
            line.SourceLine = _sourceLine;
            line.Position = _firstPos;
            line.Label = _label;
            Compilation.Lines.Add(line);
            return line;
        }

        #endregion
    }
}