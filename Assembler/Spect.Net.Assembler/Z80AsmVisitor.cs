using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Spect.Net.Assembler.Assembler;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Expressions;
using Spect.Net.Assembler.SyntaxTree.Operations;
using Spect.Net.Assembler.SyntaxTree.Pragmas;
using Spect.Net.Assembler.SyntaxTree.Statements;

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
        private int _firstColumn;
        private int _lastPos;
        private int _firstIndex;
        private int _lastIndex;
        private string _label;
        private string _comment;
        private string _emitIssue;
        private List<string> _macroParamNames;
        private TextSpan _labelSpan;
        private TextSpan _keywordSpan;
        private TextSpan _commentSpan;
        private List<TextSpan> _numbers;
        private List<TextSpan> _identifiers;
        private List<TextSpan> _strings;
        private List<TextSpan> _semiVars;
        private List<TextSpan> _functions;
        private List<TextSpan> _macroParams;
        private List<TextSpan> _statements;
        private List<TextSpan> _operands;
        private List<TextSpan> _mnemonics;

        /// <summary>
        /// Access the compilation results through this object
        /// </summary>
        public CompilationUnit Compilation { get; } = new CompilationUnit();

        /// <summary>
        /// The last assembly line parsed
        /// </summary>
        public object LastAsmLine { get; private set; }

        /// <summary>
        /// This flag indicates that the parses now works on parsing a macro
        /// </summary>
        public bool MacroParsingPhase { get; set; }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.asmline"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAsmline(Z80AsmParser.AsmlineContext context)
        {
            if (IsInvalidContext(context)) return null;

            _label = null;
            _comment = null;
            _labelSpan = null;
            _keywordSpan = null;
            _commentSpan = null;
            _emitIssue = null;
            _numbers = null;
            _identifiers = null;
            _strings = null;
            _functions = null;
            _semiVars = null;
            _macroParams = null;
            _macroParamNames = null;
            _semiVars = null;
            _statements = null;
            _operands = null;
            _mnemonics = null;
            _sourceLine = context.Start.Line;
            _firstColumn = context.Start.Column;
            _firstIndex = context.Start.StartIndex;
            _lastIndex = context.Stop.StopIndex;

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

            if (context.macroParam() != null)
            {
                // --- This line is a macro parameter
                return AddLine(new MacroParamLine(context.macroParam().IDENTIFIER()?.NormalizeToken()), context);
            }

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
                if (context.STRING() != null) AddString(context.STRING());
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
                Message = (ExpressionNode)VisitExpr(context.expr())
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

            string id = null;
            if (context.IDENTIFIER() != null)
            {
                id = context.IDENTIFIER().NormalizeToken();
            }
            else if (context.NEXT() != null)
            {
                id = context.NEXT().NormalizeToken();
            }
            return AddLine(new ModelPragma
            {
                Model = id
            }, context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.alignPragma"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAlignPragma(Z80AsmParser.AlignPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;
            return AddLine(new AlignPragma(
                context.expr() != null
                    ? (ExpressionNode)VisitExpr(context.expr())
                    : null), 
                context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.tracePragma"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTracePragma(Z80AsmParser.TracePragmaContext context)
        {
            if (IsInvalidContext(context)) return null;
            return AddLine(new TracePragma(
                context.TRACEHEX() != null,
                context.expr().Select(ex => (ExpressionNode)VisitExpr(ex)).ToList()),
                context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.rndSeedPragma"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitRndSeedPragma(Z80AsmParser.RndSeedPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;
            return AddLine(new RndSeedPragma(
                    context.expr() != null
                        ? (ExpressionNode)VisitExpr(context.expr())
                        : null),
                context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.defgPragma"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDefgPragma(Z80AsmParser.DefgPragmaContext context)
        {
            if (IsInvalidContext(context)) return null;
            return AddLine(new DefgPragma(
                    context.expr() != null
                        ? (ExpressionNode)VisitExpr(context.expr())
                        : null), 
                context);
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
        /// Visit a parse tree produced by <see cref="Z80AsmParser.trivialNextOperation"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTrivialNextOperation(Z80AsmParser.TrivialNextOperationContext context)
        {
            if (IsInvalidContext(context)) return null;

            return AddLine(new TrivialNextOperation
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

            var operandCount = 0;
            foreach (var child in context.operand())
            {
                // --- Collect operands
                if (operandCount == 0)
                {
                    op.Operand = (Operand)VisitOperand(child);
                    operandCount = 1;
                }
                else if (operandCount == 1)
                {
                    op.Operand2 = (Operand)VisitOperand(child);
                    operandCount = 2;
                }
                else
                {
                    op.Operand3 = (Operand)VisitOperand(child);
                    operandCount = 2;
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
            var op = new Operand();
            ParserRuleContext regContext = null;
            if (context.reg8() != null)
            {
                op.Type = OperandType.Reg8;
                op.Register = context.reg8().NormalizeToken();
                regContext = context.reg8();
            }
            else if (context.reg8Idx() != null)
            {
                op.Type = OperandType.Reg8Idx;
                op.Register = context.reg8Idx().NormalizeToken();
                regContext = context.reg8Idx();
            }
            else if (context.reg8Spec() != null)
            {
                op.Type = OperandType.Reg8Spec;
                op.Register = context.reg8Spec().NormalizeToken();
                regContext = context.reg8Spec();
            }
            else if (context.reg16() != null)
            {
                op.Type = OperandType.Reg16;
                op.Register = context.reg16().NormalizeToken();
                regContext = context.reg16();
            }
            else if (context.reg16Idx() != null)
            {
                op.Type = OperandType.Reg16Idx;
                op.Register = context.reg16Idx().NormalizeToken();
                regContext = context.reg16Idx();
            }
            else if (context.reg16Spec() != null)
            {
                op.Type = OperandType.Reg16Spec;
                op.Register = context.reg16Spec().NormalizeToken();
                regContext = context.reg16Spec();
            }
            else if (context.memIndirect() != null)
            {
                var miContext = context.memIndirect();
                var expContext = miContext.expr();
                op.Type = OperandType.MemIndirect;
                op.Expression = (ExpressionNode)VisitExpr(expContext);
                if (miContext.LPAR() != null) AddOperand(miContext.LPAR());
                if (miContext.RPAR() != null) AddOperand(miContext.RPAR());
            }
            else if (context.regIndirect() != null)
            {
                op.Type = OperandType.RegIndirect;
                op.Register = context.regIndirect().NormalizeToken();
                regContext = context.regIndirect();
            }
            else if (context.cPort() != null)
            {
                op.Type = OperandType.CPort;
                regContext = context.cPort();
            }
            else if (context.indexedAddr() != null)
            {
                op.Type = OperandType.IndexedAddress;
                var idContext = context.indexedAddr();
                regContext = idContext.reg16Idx();
                if (idContext.ChildCount > 3)
                {
                    op.Expression = (ExpressionNode)VisitExpr(idContext.expr());
                }
                op.Register = idContext.reg16Idx().NormalizeToken();
                op.Sign = idContext.ChildCount > 3
                    ? idContext.GetChild(2).NormalizeToken()
                    : null;
                if (idContext.LPAR() != null) AddOperand(idContext.LPAR());
                if (idContext.RPAR() != null) AddOperand(idContext.RPAR());
            }
            else if (context.expr() != null)
            {
                op.Type = OperandType.Expr;
                op.Expression = (ExpressionNode)VisitExpr(context.expr());
            }
            else if (context.condition() != null)
            {
                op.Type = OperandType.Condition;
                op.Condition = context.condition().NormalizeToken();
                regContext = context.condition();
            }
            else if (context.macroParam() != null)
            {
                // --- LREG or HREG with macro parameter
                AddFunction(context);
                AddMacroParam(context.macroParam());
                if (context.macroParam().IDENTIFIER() != null)
                {
                    AddMacroParamName(context.macroParam().IDENTIFIER().NormalizeToken());
                }
            }
            else if (context.reg16Std() != null)
            {
                // --- LREG or HREG with 16-bit register
                AddFunction(context);
                op.Type = OperandType.Reg8;
                op.Register = string.Empty;

                if (context.HREG() != null)
                {
                    regContext = context.reg16Std();
                    switch (context.reg16Std().NormalizeToken())
                    {
                        case "BC":
                            op.Register = "B";
                            break;
                        case "DE":
                            op.Register = "D";
                            break;
                        case "HL":
                            op.Register = "H";
                            break;
                        case "IX":
                            op.Register = "IXH";
                            op.Type = OperandType.Reg8Idx;
                            break;
                        case "IY":
                            op.Register = "IYH";
                            op.Type = OperandType.Reg8Idx;
                            break;
                        default:
                            regContext = null;
                            break;
                    }
                }
                else
                {
                    regContext = context.reg16Std();
                    switch (context.reg16Std().NormalizeToken())
                    {
                        case "BC":
                            op.Register = "C";
                            break;
                        case "DE":
                            op.Register = "E";
                            break;
                        case "HL":
                            op.Register = "L";
                            break;
                        case "IX":
                            op.Register = "IXL";
                            op.Type = OperandType.Reg8Idx;
                            break;
                        case "IY":
                            op.Register = "IYL";
                            op.Type = OperandType.Reg8Idx;
                            break;
                        default:
                            regContext = null;
                            break;
                    }
                }
            }
            else if (context.NONEARG() != null)
            {
                // --- This can happen only as the result of a macro substitution
                op.Type = OperandType.None;
            }

            if (regContext != null)
            {
                AddOperand(regContext);
            }

            return op;
        }

        #endregion

        #region Statement handling

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.statement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitStatement(Z80AsmParser.StatementContext context)
        {
            if (IsInvalidContext(context)) return null;
            _keywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            return base.VisitStatement(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.macroParam"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMacroParam(Z80AsmParser.MacroParamContext context)
        {
            if (IsInvalidContext(context)) return null;
            AddMacroParam(context);
            if (context.IDENTIFIER() != null)
            {
                AddMacroParamName(context.IDENTIFIER().NormalizeToken());
            }
            return null;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.macroInvocation"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMacroInvocation(Z80AsmParser.MacroInvocationContext context)
        {
            if (IsInvalidContext(context)) return null;
            var macroOps = new List<Operand>();
            if (context.macroArgument().Length > 1 
                || context.macroArgument().Length > 0 && context.macroArgument()[0].operand() != null)
            {
                foreach (var arg in context.macroArgument())
                {
                    if (arg.operand() != null)
                    {
                        macroOps.Add((Operand)VisitOperand(arg.operand()));
                    }
                    else
                    {
                        macroOps.Add(new Operand
                        {
                            Type = OperandType.None
                        });
                    }
                }
            }

            _keywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            return AddLine(new MacroInvocation(context.IDENTIFIER().NormalizeToken(), macroOps),
                    context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.macroStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMacroStatement(Z80AsmParser.MacroStatementContext context)
        {
            if (IsInvalidContext(context)) return null;
            foreach (var id in context.IDENTIFIER())
            {
                AddIdentifier(id);
            }
            return AddLine(new MacroStatement(context.IDENTIFIER().Select(id => id.NormalizeToken()).ToList()),
                    context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.macroEndMarker"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMacroEndMarker(Z80AsmParser.MacroEndMarkerContext context)
        {
            return IsInvalidContext(context) 
                ? null 
                : AddLine(new MacroEndStatement(), context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.loopStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLoopStatement(Z80AsmParser.LoopStatementContext context)
        {
            if (IsInvalidContext(context)) return null;
            return AddLine(new LoopStatement((ExpressionNode)VisitExpr(context.expr())), 
                context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.loopEndMarker"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLoopEndMarker(Z80AsmParser.LoopEndMarkerContext context)
        {
            return IsInvalidContext(context)
                ? null
                : AddLine(new LoopEndStatement(), context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.repeatStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitRepeatStatement(Z80AsmParser.RepeatStatementContext context)
        {
            return IsInvalidContext(context)
                ? null
                : AddLine(new RepeatStatement(), context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.untilStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitUntilStatement(Z80AsmParser.UntilStatementContext context)
        {
            if (IsInvalidContext(context)) return null;
            return AddLine(new UntilStatement((ExpressionNode)VisitExpr(context.expr())),
                context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.whileStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitWhileStatement(Z80AsmParser.WhileStatementContext context)
        {
            if (IsInvalidContext(context)) return null;
            return AddLine(new WhileStatement((ExpressionNode)VisitExpr(context.expr())),
                context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.whileEndMarker"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitWhileEndMarker(Z80AsmParser.WhileEndMarkerContext context)
        {
            return IsInvalidContext(context)
                ? null
                : AddLine(new WhileEndStatement(), context);
        }


        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.ifStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitIfStatement(Z80AsmParser.IfStatementContext context)
        {
            if (IsInvalidContext(context)) return null;
            return AddLine(new IfStatement((ExpressionNode)VisitExpr(context.expr())),
                context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.elifStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitElifStatement(Z80AsmParser.ElifStatementContext context)
        {
            if (IsInvalidContext(context)) return null;
            return AddLine(new ElifStatement((ExpressionNode)VisitExpr(context.expr())), 
                context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.elseStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitElseStatement(Z80AsmParser.ElseStatementContext context)
        {
            return IsInvalidContext(context)
                ? null
                : AddLine(new ElseStatement(), context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.endifStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitEndifStatement(Z80AsmParser.EndifStatementContext context)
        {
            return IsInvalidContext(context)
                ? null
                : AddLine(new IfEndStatement(), context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.forStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitForStatement(Z80AsmParser.ForStatementContext context)
        {
            if (IsInvalidContext(context)) return null;
            if (context.IDENTIFIER() != null) AddIdentifier(context.IDENTIFIER());
            if (context.TO() != null) AddStatement(context.TO());
            if (context.STEP() != null) AddStatement(context.STEP());

            var id = context.IDENTIFIER()?.NormalizeToken();
            var fromExpr = context.expr().Length > 0 ? (ExpressionNode) VisitExpr(context.expr()[0]) : null;
            var toExpr = context.expr().Length > 1 ? (ExpressionNode)VisitExpr(context.expr()[1]) : null;
            var stepExpr = context.expr().Length > 2 ? (ExpressionNode)VisitExpr(context.expr()[2]) : null;
            return AddLine(new ForStatement(id, fromExpr, toExpr, stepExpr),
                context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.nextStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitNextStatement(Z80AsmParser.NextStatementContext context)
        {
            return IsInvalidContext(context)
                ? null
                : AddLine(new NextStatement(), context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.breakStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitBreakStatement(Z80AsmParser.BreakStatementContext context)
        {
            return IsInvalidContext(context)
                ? null
                : AddLine(new BreakStatement(), context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.continueStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitContinueStatement(Z80AsmParser.ContinueStatementContext context)
        {
            return IsInvalidContext(context)
                ? null
                : AddLine(new ContinueStatement(), context);
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

            // --- Extract the expression text
            var sb = new StringBuilder(400);
            for (var i = 0; i < context.ChildCount; i++)
            {
                var token = context.GetChild(i).GetText();
                sb.Append(token);
            }

            var expr = (ExpressionNode)VisitOrExpr(context.GetChild(0) as Z80AsmParser.OrExprContext);
            if (context.ChildCount == 1)
            {
                if (expr != null)
                {
                    expr.SourceText = sb.ToString();
                }
                return expr;
            }

            return new ConditionalExpressionNode
            {
                SourceText = sb.ToString(),
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
                BinaryOperationNode equExpr;
                switch (opToken)
                {
                    case "==":
                        equExpr = new EqualOperationNode();
                        break;
                    case "===":
                        equExpr = new CaseInsensitiveEqualOperationNode();
                        break;
                    case "!=":
                        equExpr = new NotEqualOperationNode();
                        break;
                    default:
                        equExpr = new CaseInsensitiveNotEqualOperationNode();
                        break;

                }
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

            if (context.functionInvocation() != null)
            {
                return VisitFunctionInvocation(context.functionInvocation());
            }

            if (context.builtinFunctionInvocation() != null)
            {
                return VisitBuiltinFunctionInvocation(context.builtinFunctionInvocation());
            }

            if (context.literalExpr() != null)
            {
                return VisitLiteralExpr(context.literalExpr());
            }

            if (context.macroParam() != null)
            {
                AddMacroParam(context);
                if (context.macroParam().IDENTIFIER() != null)
                {
                    AddMacroParamName(context.macroParam().IDENTIFIER().NormalizeToken());
                }
                return new MacroParamNode(context.macroParam().IDENTIFIER()?.NormalizeToken());
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
            if (child0.GetText() == "!")
            {
                return new UnaryLogicalNotNode()
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
            if (context.CURADDR() != null)
            {
                AddSemiVar(context);
                return new CurrentAddressNode();
            }

            if (context.CURCNT() != null)
            {
                AddSemiVar(context);
                return new CurrentLoopCounterNode();
            }

            // --- Check for Boolean values
            if (context.BOOLLIT() != null)
            {
                var boolValue = new ExpressionValue(context.BOOLLIT().GetText().ToLower().Contains("t"));
                return new LiteralNode(boolValue);
            }

            // --- Check for real values
            if (context.REALNUM() != null)
            {
                AddNumber(context);
                return double.TryParse(context.REALNUM().GetText(), out var realValue) 
                    ? new LiteralNode(realValue) 
                    : new LiteralNode(ExpressionValue.Error);
            }

            // --- Check for string values
            if (context.STRING() != null)
            {
                AddString(context);
                var stringValue = context.STRING().NormalizeString();
                return new LiteralNode(stringValue);
            }

            ushort value;
            // --- Hexadecimal literals
            if (token.StartsWith("#"))
            {
                AddNumber(context);
                value = ushort.Parse(token.Substring(1), NumberStyles.HexNumber);
            }
            else if (token.StartsWith("0X"))
            {
                AddNumber(context);
                value = ushort.Parse(token.Substring(2), NumberStyles.HexNumber);
            }
            else if (token.StartsWith("$"))
            {
                AddNumber(context);
                value = ushort.Parse(token.Substring(1), NumberStyles.HexNumber);
            }
            else if (token.EndsWith("H", StringComparison.OrdinalIgnoreCase))
            {
                AddNumber(context);
                value = (ushort)int.Parse(token.Substring(0, token.Length - 1),
                    NumberStyles.HexNumber);
            }
            // --- Binary literals
            else if (token.StartsWith("%"))
            {
                AddNumber(context);
                value = (ushort)Convert.ToInt32(token.Substring(1).Replace("_", ""), 2);
            }
            else if (token.StartsWith("0B"))
            {
                AddNumber(context);
                value = (ushort)Convert.ToInt32(token.Substring(2).Replace("_", ""), 2);
            }
            // --- Character literals
            else if (token.StartsWith("\"") || token.StartsWith("'"))
            {
                var charExpr = context.GetText();
                var bytes = Z80Assembler.SpectrumStringToBytes(charExpr.Substring(1, charExpr.Length - 2));
                value = bytes.Count == 0 ? (ushort)0x00 : bytes[0];
            }
            // --- Decimal literals
            else
            {
                AddNumber(context);
                value = (ushort)int.Parse(context.NormalizeToken());
            }

            return new LiteralNode(value);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.symbolExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitSymbolExpr(Z80AsmParser.SymbolExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            AddIdentifier(context);
            return new IdentifierNode
            {
                SymbolName = context.GetChild(0).NormalizeToken()
            };
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.functionInvocation"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitFunctionInvocation(Z80AsmParser.FunctionInvocationContext context)
        {
            if (IsInvalidContext(context)) return null;

            var funcName = context.IDENTIFIER()?.GetText()?.ToLower();
            if (funcName != null)
            {
                AddFunction(context);
            }
            var args = context.expr().Select(expr => (ExpressionNode) VisitExpr(expr)).ToList();
            return new FunctionInvocationNode(funcName, args);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.builtinFunctionInvocation"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitBuiltinFunctionInvocation(Z80AsmParser.BuiltinFunctionInvocationContext context)
        {
            if (IsInvalidContext(context)) return null;
            AddFunction(context);
            string token = null;
            if (context.TEXTOF() != null)
            {
                if (context.macroParam() != null)
                {
                    AddFunction(context);
                    AddMacroParam(context.macroParam());
                    if (context.macroParam().IDENTIFIER() != null)
                    {
                        AddMacroParamName(context.macroParam().IDENTIFIER().NormalizeToken());
                    }
                }
                if (context.mnemonic() != null)
                {
                    AddMnemonics(context.mnemonic());
                    token = context.mnemonic().NormalizeToken();
                }
                else if (context.regsAndConds() != null)
                {
                    AddOperand(context.regsAndConds());
                    token = context.regsAndConds().NormalizeToken();
                }
                return new LiteralNode(token);
            }

            if (context.DEF() != null)
            {
                // --- We do this visit just for syntax highlighting
                if (context.operand() != null)
                {
                    var op = (Operand)VisitOperand(context.operand());
                    if (!MacroParsingPhase && (op.Type != OperandType.Expr || !(op.Expression is MacroParamNode)))
                    {
                        // --- DEF can use only macro parameters during the collection phase
                        _emitIssue = Errors.Z0421;
                    }

                }
                return new LiteralNode(context.operand() != null && context.operand().NONEARG() == null);
            }

            return null;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Checks if the current context is invalid for further visiting
        /// </summary>
        /// <param name="context"></param>
        private bool IsInvalidContext(ITree context) => context == null || context.ChildCount == 0;

        /// <summary>
        /// Adds a new number text span
        /// </summary>
        private void AddNumber(ParserRuleContext context)
        {
            if (_numbers == null) _numbers = new List<TextSpan>();
            _numbers.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new string text span
        /// </summary>
        private void AddString(ParserRuleContext context)
        {
            if (_strings == null) _strings = new List<TextSpan>();
            _strings.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new string text span
        /// </summary>
        private void AddString(ITerminalNode node)
        {
            if (_strings == null) _strings = new List<TextSpan>();
            _strings.Add(new TextSpan(node.Symbol.StartIndex, node.Symbol.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new identifier text span
        /// </summary>
        private void AddIdentifier(ParserRuleContext context)
        {
            if (_identifiers == null) _identifiers = new List<TextSpan>();
            _identifiers.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new identifier text span
        /// </summary>
        private void AddIdentifier(ITerminalNode node)
        {
            if (_identifiers == null) _identifiers = new List<TextSpan>();
            _identifiers.Add(new TextSpan(node.Symbol.StartIndex, node.Symbol.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new statement text span
        /// </summary>
        private void AddStatement(ITerminalNode node)
        {
            if (_statements == null) _statements = new List<TextSpan>();
            _statements.Add(new TextSpan(node.Symbol.StartIndex, node.Symbol.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new function text span
        /// </summary>
        private void AddFunction(ParserRuleContext context)
        {
            if (_functions == null) _functions = new List<TextSpan>();
            _functions.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new semi-variable text span
        /// </summary>
        private void AddSemiVar(ParserRuleContext context)
        {
            if (_semiVars == null) _semiVars = new List<TextSpan>();
            _semiVars.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new semi-variable text span
        /// </summary>
        private void AddMacroParam(ParserRuleContext context)
        {
            if (_macroParams == null) _macroParams = new List<TextSpan>();
            _macroParams.Add(new TextSpan(context.Start.StartIndex, context.Stop.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new semi-variable text span
        /// </summary>
        private void AddMacroParamName(string name)
        {
            if (_macroParamNames == null) _macroParamNames = new List<string>();
            _macroParamNames.Add(name);
        }

        /// <summary>
        /// Adds a new operand text span
        /// </summary>
        private void AddOperand(ParserRuleContext context)
        {
            if (_operands == null) _operands = new List<TextSpan>();
            _operands.Add(new TextSpan(context.Start.StartIndex, context.Stop.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new operand text span
        /// </summary>
        private void AddOperand(ITerminalNode node)
        {
            if (_operands == null) _operands = new List<TextSpan>();
            _operands.Add(new TextSpan(node.Symbol.StartIndex, node.Symbol.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new operand text span
        /// </summary>
        private void AddMnemonics(ParserRuleContext context)
        {
            if (_mnemonics == null) _mnemonics = new List<TextSpan>();
            _mnemonics.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
        }

        /// <summary>
        /// Adds an assebmbly line to the compilation
        /// </summary>
        /// <param name="line">Line to add</param>
        /// <param name="context">The context that generates this line</param>
        /// <returns>The newly added line</returns>
        private SourceLineBase AddLine(SourceLineBase line, ParserRuleContext context)
        {
            line.SourceLine = _sourceLine;
            line.FirstColumn = _firstColumn;
            line.FirstPosition = _firstIndex;
            line.LastPosition = _lastIndex;
            line.ParserException = context.exception;
            line.Label = _label;
            line.LabelSpan = _labelSpan;
            line.KeywordSpan = _keywordSpan;
            line.Numbers = _numbers;
            line.Strings = _strings;
            line.Functions = _functions;
            line.SemiVars = _semiVars;
            line.MacroParams = _macroParams;
            line.MacroParamNames = _macroParamNames;
            line.Identifiers = _identifiers;
            line.Statements = _statements;
            line.Operands = _operands;
            line.Mnemonics = _mnemonics;
            line.Comment = _comment;
            line.CommentSpan = _commentSpan;
            line.EmitIssue = _emitIssue;
            line.InstructionSpan = _keywordSpan != null 
                ? new TextSpan(_keywordSpan.Start, _lastPos) 
                : new TextSpan(_firstColumn, _lastPos);
            Compilation.Lines.Add(line);
            LastAsmLine = line;
            return line;
        }

        #endregion
    }
}