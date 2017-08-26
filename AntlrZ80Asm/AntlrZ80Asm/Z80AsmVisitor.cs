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
            _label = context.label().NormalizeToken();
            return base.VisitAsmline(context);
        }

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
        /// Visit a parse tree produced by <see cref="Z80AsmParser.defbPrag"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDefbPrag(Z80AsmParser.DefbPragContext context)
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
        /// Visit a parse tree produced by <see cref="Z80AsmParser.defwPrag"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDefwPrag(Z80AsmParser.DefwPragContext context)
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
        /// Visit a parse tree produced by <see cref="Z80AsmParser.defmPrag"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDefmPrag(Z80AsmParser.DefmPragContext context)
            => AddLine(new DefmPragma
            {
                Message = context.GetChild(1).GetText()
            });

        #endregion

        #region Trivial instructions

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

        #region Load instructions

        ///// <summary>
        ///// Visit a parse tree produced by <see cref="Z80AsmParser.load8BitRegInstruction"/>.
        ///// </summary>
        ///// <param name="context">The parse tree.</param>
        ///// <return>The visitor result.</return>
        //public override object VisitLoad8BitRegInstruction(Z80AsmParser.Load8BitRegInstructionContext context)
        //    => AddLine(new LoadReg8ToReg8Instruction
        //    {
        //        Destination = context.GetChild(1).NormalizeToken(),
        //        Source = context.GetChild(3).NormalizeToken()
        //    });

        ///// <summary>
        ///// Visit a parse tree produced by <see cref="Z80AsmParser.loadRegWithValueInstruction"/>.
        ///// </summary>
        ///// <param name="context">The parse tree.</param>
        ///// <return>The visitor result.</return>
        //public override object VisitLoadRegWithValueInstruction(Z80AsmParser.LoadRegWithValueInstructionContext context)
        //    => AddLine(
        //        new LoadValueToRegInstruction
        //        {
        //            Destination = context.GetChild(1).NormalizeToken(),
        //            Expression = (ExpressionNode)VisitExpr(context.GetChild(3)
        //                as Z80AsmParser.ExprContext)
        //        });

        ///// <summary>
        ///// Visit a parse tree produced by <see cref="Z80AsmParser.loadRegAddrWith8BitRegInstruction"/>.
        ///// </summary>
        ///// <param name="context">The parse tree.</param>
        ///// <return>The visitor result.</return>
        //public override object VisitLoadRegAddrWith8BitRegInstruction(Z80AsmParser.LoadRegAddrWith8BitRegInstructionContext context)
        //    => AddLine(
        //        new LoadReg8ToRegAddrInstruction
        //        {
        //            Destination = context.GetChild(2) is Z80AsmParser.IndexedAddrContext 
        //                ?  null
        //                : context.GetChild(2).NormalizeToken(),
        //            Source = context.GetChild(5).NormalizeToken(),
        //            IndexedAddress = context.GetChild(2) is Z80AsmParser.IndexedAddrContext
        //                ? (IndexedAddress)VisitIndexedAddr(context.GetChild(2) as Z80AsmParser.IndexedAddrContext)
        //                : null
        //        });

        ///// <summary>
        ///// Visit a parse tree produced by <see cref="Z80AsmParser.load8BitRegFromRegAddrInstruction"/>.
        ///// </summary>
        ///// <param name="context">The parse tree.</param>
        ///// <return>The visitor result.</return>
        //public override object VisitLoad8BitRegFromRegAddrInstruction(Z80AsmParser.Load8BitRegFromRegAddrInstructionContext context)
        //    => AddLine(
        //        new LoadReg8FromRegAddrInstruction
        //        {
        //            Destination = context.GetChild(1).NormalizeToken(),
        //            Source = context.GetChild(4) is Z80AsmParser.IndexedAddrContext
        //                ? null
        //                : context.GetChild(4).NormalizeToken(),
        //            IndexedAddress = context.GetChild(4) is Z80AsmParser.IndexedAddrContext
        //                ? (IndexedAddress)VisitIndexedAddr(context.GetChild(4) as Z80AsmParser.IndexedAddrContext)
        //                : null
        //        });

        ///// <summary>
        ///// Visit a parse tree produced by <see cref="Z80AsmParser.loadMemAddrWithRegInstruction"/>.
        ///// </summary>
        ///// <param name="context">The parse tree.</param>
        ///// <return>The visitor result.</return>
        //public override object VisitLoadMemAddrWithRegInstruction(Z80AsmParser.LoadMemAddrWithRegInstructionContext context)
        //    => AddLine(
        //        new LoadRegToMemAddrInstruction
        //        {
        //            Destination = (ExpressionNode)VisitExpr(context.GetChild(2) as Z80AsmParser.ExprContext),
        //            Source = context.GetChild(5).NormalizeToken()
        //        });

        ///// <summary>
        ///// Visit a parse tree produced by <see cref="Z80AsmParser.loadRegFromMemAddrInstruction"/>.
        ///// </summary>
        ///// <param name="context">The parse tree.</param>
        ///// <return>The visitor result.</return>
        //public override object VisitLoadRegFromMemAddrInstruction(Z80AsmParser.LoadRegFromMemAddrInstructionContext context)
        //    => AddLine(
        //        new LoadRegFromMemAddrInstruction
        //        {
        //            Destination = context.GetChild(1).NormalizeToken(),
        //            Source = (ExpressionNode)VisitExpr(context.GetChild(4) as Z80AsmParser.ExprContext)
        //        });

        ///// <summary>
        ///// Visit a parse tree produced by <see cref="Z80AsmParser.indexedAddr"/>.
        ///// </summary>
        ///// <param name="context">The parse tree.</param>
        ///// <return>The visitor result.</return>
        //public override object VisitIndexedAddr(Z80AsmParser.IndexedAddrContext context)
        //    => new IndexedAddress
        //    {
        //        Register = context.GetChild(0).NormalizeToken(),
        //        Sign = context.ChildCount > 1 
        //            ? context.GetChild(1).NormalizeToken() 
        //            : null,
        //        Displacement = context.ChildCount > 1 
        //            ? (ExpressionNode) VisitExpr(context.GetChild(2) as Z80AsmParser.ExprContext)
        //            : null
        //    };

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

            if (context.ChildCount == 2)
            {
                if (context.GetChild(1) is Z80AsmParser.IndexedAddrContext)
                {
                    op.Operand = GetIndexedAddress(context, 1);
                }
                else
                {
                    op.Operand = new Operand
                    {
                        AddressingType = AddressingType.Register,
                        Register = context.GetChild(1).NormalizeToken()
                    };
                }
            }
            else
            {
                op.Operand = new Operand
                {
                    AddressingType = AddressingType.Register,
                    Register = "(HL)"
                };
            }
            return AddLine(op);
        }

        #endregion

        #region Exchange instructions

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

        #region ALU instructions

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.aluOperation"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAluOperation(Z80AsmParser.AluOperationContext context)
        {
            var mnemonic = context.GetChild(0).NormalizeToken();
            switch (mnemonic)
            {
                case "ADD":
                case "ADC":
                case "SBC":

                    break;
                default:
                    break;
            }
            return base.VisitAluOperation(context);
        }


        ///// <summary>
        ///// Visit a parse tree produced by <see cref="Z80AsmParser.aluInstruction"/>.
        ///// </summary>
        ///// <param name="context">The parse tree.</param>
        ///// <return>The visitor result.</return>
        //public override object VisitAluInstruction(Z80AsmParser.AluInstructionContext context)
        //{
        //    var type = context.GetChild(0).NormalizeToken();
        //    string dest = null;
        //    string source;
        //    IndexedAddress addr;
        //    ExpressionNode expr;
        //    switch (type)
        //    {
        //        case "ADD":
        //        case "ADC":
        //        case "SBC":
        //            dest = context.GetChild(1).NormalizeToken();
        //            source = context.ChildCount > 4
        //                ? null
        //                : context.GetChild(3) is Z80AsmParser.ExprContext
        //                    ? null
        //                    : context.GetChild(3).NormalizeToken();
        //            addr = context.ChildCount > 4
        //                ? (IndexedAddress) VisitIndexedAddr(context.GetChild(4) as Z80AsmParser.IndexedAddrContext)
        //                : null;
        //            expr = context.ChildCount > 4
        //                ? null
        //                : context.GetChild(3) is Z80AsmParser.ExprContext
        //                    ? (ExpressionNode)VisitExpr(context.GetChild(3) as Z80AsmParser.ExprContext)
        //                    : null;
        //            break;
        //        default:
        //            source = context.ChildCount > 2
        //                ? null
        //                : context.GetChild(1) is Z80AsmParser.ExprContext
        //                    ? null
        //                    : context.GetChild(1).NormalizeToken();
        //            addr = context.ChildCount > 2
        //                ? (IndexedAddress)VisitIndexedAddr(context.GetChild(2) as Z80AsmParser.IndexedAddrContext)
        //                : null;
        //            expr = context.ChildCount > 2
        //                ? null
        //                : context.GetChild(1) is Z80AsmParser.ExprContext
        //                    ? (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
        //                    : null;
        //            break;
        //    }
        //    return AddLine(new AluInstruction
        //    {
        //        Type = type,
        //        Source = source,
        //        Destination = dest,
        //        IndexedSource = addr,
        //        SourceExpr = expr
        //    });
        //}

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
            if (context.GetChild(0) is Z80AsmParser.LiteralExprContext litContext)
            {
                return VisitLiteralExpr(litContext);
            }
            if (context.GetChild(0) is Z80AsmParser.SymbolExprContext symbolContext)
            {
                return VisitSymbolExpr(symbolContext);
            }
            if (context.GetChild(0).GetText() == "+")
            {
                return new UnaryPlusNode
                {
                    Operand = (ExpressionNode) VisitUnaryExpr(context.GetChild(1) as Z80AsmParser.UnaryExprContext)
                };
            }
            if (context.GetChild(0).GetText() == "-")
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
        private AssemblyLine AddLine(AssemblyLine line)
        {
            line.Label = _label;
            Compilation.Lines.Add(line);
            return line;
        }

        /// <summary>
        /// Obtains an operand from the specified indexed address context
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="i">The index of the child node to get an indexed address</param>
        /// <returns></returns>
        private Operand GetIndexedAddress(IParseTree context, int i)
        {
            var indexedAddrContext = context.GetChild(i) as Z80AsmParser.IndexedAddrContext;
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

        #endregion
    }
}