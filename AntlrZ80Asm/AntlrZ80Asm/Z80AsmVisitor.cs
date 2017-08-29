using System;
using System.Collections.Generic;
using System.Globalization;
using Antlr4.Runtime.Tree;
using AntlrZ80Asm.SyntaxTree;
using AntlrZ80Asm.SyntaxTree.Expressions;
using AntlrZ80Asm.SyntaxTree.Operations;
using AntlrZ80Asm.SyntaxTree.Pragmas;
// ReSharper disable PossibleNullReferenceException

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
            => AddLine(new Directive
            {
                Mnemonic = context.GetChild(0).NormalizeToken(),
                Identifier = context.ChildCount > 1
                    ? context.GetChild(1).NormalizeToken()
                    : null
            });

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
                Expr = (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
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

        #region Load operations

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.loadOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLoadOperation(Z80AsmParser.LoadOperationContext context)
        {
            var op = new LoadOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            };
            var child1 = context.GetChild(1);
            var child2 = context.GetChild(2);
            var lastChild = context.GetChild(context.ChildCount - 1);
            var lastChild2 = context.GetChild(context.ChildCount - 2);
            if (lastChild is Z80AsmParser.ExprContext)
            {
                op.SourceOperand = GetExpressionOperand(context, - 1);
                op.DestinationOperand = GetRegisterOperand(context, 1);
            }
            else if (lastChild is Z80AsmParser.IndexedAddrContext)
            {
                op.SourceOperand = GetIndexedAddress(context, - 1);
                op.DestinationOperand = GetRegisterOperand(context, 1);
            }
            else if (child1 is Z80AsmParser.IndexedAddrContext)
            {
                op.SourceOperand = GetRegisterOperand(context, 3);
                op.DestinationOperand = GetIndexedAddress(context, 1);
            }
            else if (child2 is Z80AsmParser.ExprContext)
            {
                op.SourceOperand = GetRegisterOperand(context, 5);
                op.DestinationOperand = GetExpressionOperand(context, 2, AddressingType.AddressIndirection);
            }
            else if (lastChild2 is Z80AsmParser.ExprContext)
            {
                op.SourceOperand = GetExpressionOperand(context, - 2, AddressingType.AddressIndirection);
                op.DestinationOperand = GetRegisterOperand(context, 1);
            }
            else if (child1.GetText() == "(" && child2.NormalizeToken() != "HL")
            {
                op.SourceOperand = GetRegisterOperand(context, 5);
                op.DestinationOperand = GetRegisterOperand(context, 2, AddressingType.RegisterIndirection);
            }
            else if (lastChild.GetText() == ")" && lastChild2.NormalizeToken() != "HL")
            {
                op.SourceOperand = GetRegisterOperand(context, -2, AddressingType.RegisterIndirection);
                op.DestinationOperand = GetRegisterOperand(context, 1);
            }
            else
            {
                op.SourceOperand = lastChild.GetText() == ")"
                    ? GetRegisterOperand(context, -3)
                    : GetRegisterOperand(context, -1);
                op.DestinationOperand = GetRegisterOperand(context, 1);
            }
            return AddLine(op);
        }

        #endregion

        #region Increment and Decrement

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.incDecOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitIncDecOperation(Z80AsmParser.IncDecOperationContext context)
        {
            var op = new IncDecOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            };

            if (context.GetChild(1) is Z80AsmParser.IndexedAddrContext)
            {
                op.Operand = GetIndexedAddress(context, 1);
            }
            else
            {
                op.Operand = GetRegisterOperand(context, 1);
            }
            return AddLine(op);
        }

        #endregion

        #region Exchange operations

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.exchangeOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitExchangeOperation(Z80AsmParser.ExchangeOperationContext context)
            => AddLine(new ExchangeOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken(),
                Destination = context.ChildCount > 4
                    ? $"({context.GetChild(2).NormalizeToken()})"
                    : context.GetChild(1).NormalizeToken(),
                Source = context.ChildCount > 4
                    ? context.GetChild(5).NormalizeToken()
                    : context.GetChild(3).NormalizeToken()
            });

        #endregion

        #region ALU operations

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.aluOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAluOperation(Z80AsmParser.AluOperationContext context)
        {
            var op = new AluOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            };
            switch (op.Mnemonic)
            {
                case "ADD":
                case "ADC":
                case "SBC":
                    var child3 = context.GetChild(3);
                    if (child3 is Z80AsmParser.ExprContext)
                    {
                        op.Operand = GetExpressionOperand(context, 3);
                    }
                    else if (child3 is Z80AsmParser.IndexedAddrContext)
                    {
                        op.Operand = GetIndexedAddress(context, 3);
                    }
                    else
                    {
                        op.Operand = GetRegisterOperand(context, 3);
                        var dest = context.GetChild(1).NormalizeToken();
                        if (dest.Length > 1)
                        {
                            op.Register = dest;
                        }
                    }
                    break;
                default:
                    var child1 = context.GetChild(1);
                    if (child1 is Z80AsmParser.ExprContext)
                    {
                        op.Operand = GetExpressionOperand(context, 1);
                    }
                    else if (child1 is Z80AsmParser.IndexedAddrContext)
                    {
                        op.Operand = GetIndexedAddress(context, 1);
                    }
                    else
                    {
                        op.Operand = GetRegisterOperand(context, 1);
                    }
                    break;
            }
            return AddLine(op);
        }

        #endregion

        #region Stack operations

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.stackOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitStackOperation(Z80AsmParser.StackOperationContext context)
            => AddLine(new StackOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken(),
                Register = context.GetChild(1).NormalizeToken()
            });

        #endregion

        #region Interrupt mode operations

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.interruptOperation"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitInterruptOperation(Z80AsmParser.InterruptOperationContext context)
            => AddLine(new InterruptModeOperation()
            {
                Mnemonic = context.GetChild(0).NormalizeToken(),
                Mode = context.GetChild(1).NormalizeToken()
            });

        #endregion

        #region Control flow operations

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.controlFlowOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitControlFlowOperation(Z80AsmParser.ControlFlowOperationContext context)
        {
            var op = new ControlFlowOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            };
            var child1 = context.GetChild(1);
            var lastChild = context.GetChild(context.ChildCount - 1);
            if (lastChild is Z80AsmParser.ExprContext)
            {
                op.Target = (ExpressionNode) VisitExpr(lastChild as Z80AsmParser.ExprContext);
            }
            if (op.Mnemonic == "JP" || op.Mnemonic == "JR" || op.Mnemonic == "CALL")
            {
                if (context.ChildCount > 2)
                {
                    if (child1.NormalizeToken() == "(")
                    {
                        op.Register = context.GetChild(2).NormalizeToken();
                    }
                    else
                    {
                        op.Condition = child1.NormalizeToken();
                    }
                }
            }
            return AddLine(op);
        }

        #endregion

        #region I/O operations

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.ioOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitIoOperation(Z80AsmParser.IoOperationContext context)
        {
            var op = new IoOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            };
            var lastChild = context.GetChild(context.ChildCount - 1);
            var lastChild2 = context.GetChild(context.ChildCount - 2);
            if (op.Mnemonic == "IN")
            {
                if (lastChild2 is Z80AsmParser.ExprContext)
                {
                    op.Port = (ExpressionNode) VisitExpr(lastChild2 as Z80AsmParser.ExprContext);
                }
                else if (context.GetChild(1).NormalizeToken() != "(")
                {
                    op.Register = context.GetChild(1).NormalizeToken();
                }
            }
            else
            {
                if (context.GetChild(2) is Z80AsmParser.ExprContext)
                {
                    op.Port = (ExpressionNode)VisitExpr(context.GetChild(2) as Z80AsmParser.ExprContext);
                }
                else if (lastChild.NormalizeToken() != "0")
                {
                    op.Register = lastChild.NormalizeToken();
                }
            }
            return AddLine(op);
        }

        #endregion

        #region Bit operations

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.bitOperation"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitBitOperation(Z80AsmParser.BitOperationContext context)
        {
            var op = new BitOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            };
            if (op.Mnemonic == "BIT" || op.Mnemonic == "RES" || op.Mnemonic == "SET")
            {
                op.BitIndex = (ExpressionNode) VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext);
                if (context.GetChild(3) is Z80AsmParser.IndexedAddrContext)
                {
                    var operand = GetIndexedAddress(context, 3);
                    op.IndexRegister = operand.Register;
                    op.Sign = operand.Sign;
                    op.Displacement = operand.Expression;
                }
                else
                {
                    var regOp = GetRegisterOperand(context, 3);
                    op.Register = regOp.Register;
                }
            }
            else
            {
                var regChildIndex = 1;
                if (context.GetChild(1) is Z80AsmParser.IndexedAddrContext)
                {
                    var operand = GetIndexedAddress(context, 1);
                    op.IndexRegister = operand.Register;
                    op.Sign = operand.Sign;
                    op.Displacement = operand.Expression;
                    regChildIndex = 3;
                }
                if (context.ChildCount > regChildIndex)
                {
                    var regOp = GetRegisterOperand(context, regChildIndex);
                    op.Register = regOp.Register;
                }
            }
            return AddLine(op);
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
            var expr = VisitShiftExpr(context.GetChild(0)
                as Z80AsmParser.ShiftExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitShiftExpr(context.GetChild(nextChildIndex)
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

        /// <summary>
        /// Obtains an operand from the specified indexed address context
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="index">The index of the child node to get an indexed address</param>
        /// <returns>Operand object</returns>
        private Operand GetIndexedAddress(IParseTree context, int index)
        {
            if (index < 0)
            {
                index = context.ChildCount + index;
            }
            var indexedAddrContext = context.GetChild(index) as Z80AsmParser.IndexedAddrContext;
            return new Operand
            {
                AddressingType = AddressingType.IndexedAddress,
                Register = indexedAddrContext.GetChild(1).NormalizeToken(),
                Sign = indexedAddrContext.ChildCount > 3 
                    ? indexedAddrContext.GetChild(2).NormalizeToken()
                    : null,
                Expression = indexedAddrContext.ChildCount > 3
                    ? (ExpressionNode)VisitExpr(indexedAddrContext.GetChild(3) as Z80AsmParser.ExprContext)
                    : null
            };
        }

        /// <summary>
        /// Gets a register operand from the specified context
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="index">The index of the child node to get an indexed address</param>
        /// <param name="type">Addressing type</param>
        /// <returns>Operand object</returns>
        private static Operand GetRegisterOperand(IParseTree context, int index, AddressingType type = AddressingType.Register)
        {
            if (index < 0)
            {
                index = context.ChildCount + index;
            }
            var source = context.GetChild(index);
            return new Operand
            {
                AddressingType = type,
                Register = source.GetText() == "("
                    ? "(HL)" : source.NormalizeToken()
            };
        }

        /// <summary>
        /// Gets an expression operand from the specified context
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="index">The index of the child node to get an indexed address</param>
        /// <param name="type">Addressing type</param>
        /// <returns>Operand object</returns>
        private Operand GetExpressionOperand(IParseTree context, int index, AddressingType type = AddressingType.Expression)
        {
            if (index < 0)
            {
                index = context.ChildCount + index;
            }
            var source = context.GetChild(index);
            return new Operand
            {
                AddressingType = type,
                Expression = (ExpressionNode)VisitExpr(source as Z80AsmParser.ExprContext)
            };
        }

        #endregion
    }
}