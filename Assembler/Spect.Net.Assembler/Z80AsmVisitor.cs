using System;
using System.Collections.Generic;
using System.Globalization;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Spect.Net.Assembler.Assembler;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Expressions;
using Spect.Net.Assembler.SyntaxTree.Operations;
using Spect.Net.Assembler.SyntaxTree.Pragmas;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable UsePatternMatching

namespace Spect.Net.Assembler
{
    /// <summary>
    /// This visitor class processes the elements of the AST parsed by ANTLR
    /// </summary>
    public class Z80AsmVisitor: Z80AsmBaseVisitor<object>
    {
        private int _sourceLine;
        private int _firstPos;
        private int _lastPos;
        private string _label;
        private string _comment;
        private TextSpan _labelSpan;
        private TextSpan _keywordSpan;
        private readonly List<TextSpan> _numbers = new List<TextSpan>();
        private readonly List<TextSpan> _identifiers = new List<TextSpan>();
        private TextSpan _commentSpan;

        /// <summary>
        /// Access the comilation results through this object
        /// </summary>
        public CompilationUnit Compilation { get; } = new CompilationUnit();

        public object LastAsmLine { get; private set; }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.asmline"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAsmline(Z80AsmParser.AsmlineContext context)
        {
            if (IsInvalidContext(context)) return null;

            _label = null;
            _labelSpan = null;
            _keywordSpan = null;
            _numbers.Clear();
            _identifiers.Clear();
            _sourceLine = context.Start.Line;
            _firstPos = context.Start.Column;
            _comment = null;
            _commentSpan = null;

            // --- Obtain comments
            var firstChild = context.GetChild(0);
            var lastChild = context.GetChild(context.ChildCount - 1);
            _lastPos = context.Stop.StopIndex + 1;
            if (lastChild is Z80AsmParser.LabelContext labelContext)
            {
                // --- Handle label-only lines
                VisitLabel(labelContext);
                return AddLine(new NoInstructionLine(), context);
            }

            if (lastChild is Z80AsmParser.CommentContext commentContext)
            {
                _comment = commentContext.GetText();
                if (context.ChildCount == 1)
                {
                    _lastPos = commentContext.Start.StartIndex;
                }
                else
                {
                    var lastContext = context.GetChild(context.ChildCount - 2) as ParserRuleContext;
                    _lastPos = lastContext?.Stop.StopIndex + 1 
                        ?? commentContext.Start.StartIndex;
                }
                _commentSpan = new TextSpan(_lastPos,
                    commentContext.Start.StopIndex + 1);

                if (context.ChildCount == 1)
                {
                    // --- Handle comment-only lines
                    return AddLine(new NoInstructionLine(), context);
                }
                if (context.ChildCount == 2 && firstChild is Z80AsmParser.LabelContext label2Context)
                {
                    // --- Handle label + comment lines
                    VisitLabel(label2Context);
                    return AddLine(new NoInstructionLine(), context);
                }
            }

            var line = base.VisitAsmline(context);

            // --- Let's save lines with parsing errors, too.
            return context.exception != null
                ? AddLine(new ParserErrorLine(), context) 
                : line;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.label"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLabel(Z80AsmParser.LabelContext context)
        {
            if (IsInvalidContext(context)) return null;

            _label = context.GetChild(0).NormalizeToken();
            _labelSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            return context;
        }

        #region Preprocessor directives

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.directive"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDirective(Z80AsmParser.DirectiveContext context)
        {
            if (IsInvalidContext(context)) return null;

            _keywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            var mnemonic = context.GetChild(0).NormalizeToken();
            if (mnemonic == "#INCLUDE")
            {
                return AddLine(new IncludeDirective
                {
                    Mnemonic = mnemonic,
                    Filename = context.GetChild(1).NormalizeString()
                }, context);
            }
            return AddLine(new Directive
            {
                Mnemonic = mnemonic,
                Identifier = context.ChildCount > 1
                    ? context.GetChild(1).NormalizeToken()
                    : null,
                Expr = context.GetChild(1) is Z80AsmParser.ExprContext
                    ? (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
                    : null
            }, context);
        }

        #endregion

        #region Pragma handling

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.pragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitPragma(Z80AsmParser.PragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            _keywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            return base.VisitPragma(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.orgPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitOrgPragma(Z80AsmParser.OrgPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new OrgPragma
            {
                Expr = (ExpressionNode) VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.entPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitEntPragma(Z80AsmParser.EntPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new EntPragma
            {
                Expr = (ExpressionNode) VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.xentPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitXentPragma(Z80AsmParser.XentPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new XentPragma
            {
                Expr = (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.dispPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDispPragma(Z80AsmParser.DispPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new DispPragma
            {
                Expr = (ExpressionNode) VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.equPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitEquPragma(Z80AsmParser.EquPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new EquPragma
            {
                Expr = (ExpressionNode) VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.varPragma"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitVarPragma(Z80AsmParser.VarPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new VarPragma
            {
                Expr = (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.skipPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitSkipPragma(Z80AsmParser.SkipPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new SkipPragma
            {
                Expr = (ExpressionNode) VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext),
                Fill = context.ChildCount > 3
                    ? (ExpressionNode) VisitExpr(context.GetChild(3) as Z80AsmParser.ExprContext)
                    : null
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.defbPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDefbPragma(Z80AsmParser.DefbPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            var exprs = new List<ExpressionNode>();
            for (var i = 1; i < context.ChildCount; i += 2)
            {
                exprs.Add((ExpressionNode) VisitExpr(context.GetChild(i) as Z80AsmParser.ExprContext));
            }
            return AddLine(new DefbPragma
            {
                Exprs = exprs
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.defwPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDefwPragma(Z80AsmParser.DefwPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            var exprs = new List<ExpressionNode>();
            for (var i = 1; i < context.ChildCount; i += 2)
            {
                exprs.Add((ExpressionNode)VisitExpr(context.GetChild(i) as Z80AsmParser.ExprContext));
            }
            return AddLine(new DefwPragma
            {
                Exprs = exprs
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.defmPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDefmPragma(Z80AsmParser.DefmPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new DefmPragma
            {
                Message = context.GetChild(1).GetText()
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.externPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitExternPragma(Z80AsmParser.ExternPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new ExternPragma(), context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.defsPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDefsPragma(Z80AsmParser.DefsPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new DefsPragma
            {
                Expression = (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext)
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.fillbPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitFillbPragma(Z80AsmParser.FillbPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new FillbPragma
            {
                Count = (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext),
                Expression = (ExpressionNode)VisitExpr(context.GetChild(3) as Z80AsmParser.ExprContext)
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.fillwPragma"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitFillwPragma(Z80AsmParser.FillwPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new FillwPragma
            {
                Count = (ExpressionNode)VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext),
                Expression = (ExpressionNode)VisitExpr(context.GetChild(3) as Z80AsmParser.ExprContext)
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.modelPragma"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitModelPragma(Z80AsmParser.ModelPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;
            return AddLine(new ModelPragma
            {
                Model = context.GetChild(1).NormalizeToken()
            }, context);
        }

        #endregion

        #region Operations

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.operation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitOperation(Z80AsmParser.OperationContext context)
        {
            if (IsInvalidContext(context)) return null;

            _keywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            return base.VisitOperation(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.trivialOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTrivialOperation(Z80AsmParser.TrivialOperationContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new TrivialOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.compoundOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitCompoundOperation(Z80AsmParser.CompoundOperationContext context)
        {
            if (IsInvalidContext(context)) return null;

            var op = new CompoundOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            };

            var operandFound = false;
            foreach (var child in context.children)
            {
                // --- Collect operands
                if (child is Z80AsmParser.OperandContext operandChild)
                {
                    if (!operandFound)
                    {
                        op.Operand = (Operand)VisitOperand(operandChild);
                    }
                    else
                    {
                        op.Operand2 = (Operand)VisitOperand(operandChild);
                    }
                    operandFound = true;
                    continue;
                }

                // --- Collect optional condition
                if (child is Z80AsmParser.ConditionContext condChild)
                {
                    op.Condition = condChild.GetText().NormalizeToken();
                    continue;
                }

                // --- Collect optional bit index
                if (child is Z80AsmParser.ExprContext exprChild)
                {
                    op.BitIndex = (ExpressionNode)VisitExpr(exprChild);
                }
            }
            return AddLine(op, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.operand"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitOperand(Z80AsmParser.OperandContext context)
        {
            if (IsInvalidContext(context)) return null;

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
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.expr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitExpr(Z80AsmParser.ExprContext context)
        {
            if (IsInvalidContext(context)) return null;

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
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.orExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitOrExpr(Z80AsmParser.OrExprContext context)
        {
            if (IsInvalidContext(context)) return null;

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
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.xorExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitXorExpr(Z80AsmParser.XorExprContext context)
        {
            if (IsInvalidContext(context)) return null;

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
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.andExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAndExpr(Z80AsmParser.AndExprContext context)
        {
            if (IsInvalidContext(context)) return null;

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
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.equExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitEquExpr(Z80AsmParser.EquExprContext context)
        {
            if (IsInvalidContext(context)) return null;

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
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.relExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitRelExpr(Z80AsmParser.RelExprContext context)
        {
            if (IsInvalidContext(context)) return null;

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
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.shiftExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitShiftExpr(Z80AsmParser.ShiftExprContext context)
        {
            if (IsInvalidContext(context)) return null;

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
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.addExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAddExpr(Z80AsmParser.AddExprContext context)
        {
            if (IsInvalidContext(context)) return null;

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
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.multExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMultExpr(Z80AsmParser.MultExprContext context)
        {
            if (IsInvalidContext(context)) return null;

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
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.unaryExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitUnaryExpr(Z80AsmParser.UnaryExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var child0 = context.GetChild(0);
            if (child0 == null) return null;

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
            if (child0.GetText() == "~")
            {
                return new UnaryBitwiseNotNode()
                {
                    Operand = (ExpressionNode)VisitUnaryExpr(context.GetChild(1) as Z80AsmParser.UnaryExprContext)
                };
            }
            return VisitExpr(context.GetChild(1) as Z80AsmParser.ExprContext);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.literalExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLiteralExpr(Z80AsmParser.LiteralExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var token = context.NormalizeToken();
            if (token == "$")
            {
                return new CurrentAddressNode();
            }

            ushort value;
            // --- Hexadecimal literals
            if (token.StartsWith("#"))
            {
                _numbers.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
                value = ushort.Parse(token.Substring(1), NumberStyles.HexNumber);
            }
            else if (token.StartsWith("0X"))
            {
                _numbers.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
                value = ushort.Parse(token.Substring(2), NumberStyles.HexNumber);
            }
            else if (token.EndsWith("H", StringComparison.OrdinalIgnoreCase))
            {
                _numbers.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
                value = (ushort)int.Parse(token.Substring(0, token.Length - 1),
                    NumberStyles.HexNumber);
            }
            // --- Binary literals
            else if (token.StartsWith("%"))
            {
                _numbers.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
                value = (ushort)Convert.ToInt32(token.Substring(1).Replace("_", ""), 2);
            }
            else if (token.StartsWith("0B"))
            {
                _numbers.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
                value = (ushort)Convert.ToInt32(token.Substring(2).Replace("_", ""), 2);
            }
            // --- Character literals
            else if (token.StartsWith("\""))
            {
                var charExpr = context.GetText();
                var bytes = Z80Assembler.SpectrumStringToBytes(charExpr.Substring(1, charExpr.Length - 2));
                value = bytes.Count == 0 ? (ushort)0x00 : bytes[0];
            }
            // --- Decimal literals
            else
            {
                _numbers.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
                value = (ushort)int.Parse(context.NormalizeToken());
            }
            return new LiteralNode
            {
                LiteralValue = value
            };
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.symbolExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitSymbolExpr(Z80AsmParser.SymbolExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            _identifiers.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
            return new IdentifierNode
            {
                SymbolName = context.GetChild(0).NormalizeToken()
            };
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Checks if the current context is invalid for further visiting
        /// </summary>
        /// <param name="context"></param>
        private bool IsInvalidContext(ITree context) => context == null || context.ChildCount == 0;

        /// <summary>
        /// Adds an assebmbly line to the compilation
        /// </summary>
        /// <param name="line">Line to add</param>
        /// <param name="context">The context that generates this line</param>
        /// <returns>The newly added line</returns>
        private SourceLineBase AddLine(SourceLineBase line, ParserRuleContext context)
        {
            line.SourceLine = _sourceLine;
            line.Position = _firstPos;
            line.ParserException = context.exception;
            line.Label = _label;
            line.LabelSpan = _labelSpan;
            line.KeywordSpan = _keywordSpan;
            line.Numbers = _numbers;
            line.Identifiers = _identifiers;
            line.Comment = _comment;
            line.CommentSpan = _commentSpan;
            if (_keywordSpan != null)
            {
                line.InstructionSpan = new TextSpan(_keywordSpan.Start, _lastPos);
            }
            Compilation.Lines.Add(line);
            LastAsmLine = line;
            return line;
        }

        #endregion
    }
}