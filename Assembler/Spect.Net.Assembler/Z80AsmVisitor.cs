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

namespace Spect.Net.Assembler
{
    /// <summary>
    /// This visitor class processes the elements of the AST parsed by ANTLR
    /// </summary>
    public class Z80AsmVisitor : Z80AsmBaseVisitor<object>, IZ80AsmVisitorContext
    {
        #region IZ80AsmVisitorState implementation

        /// <summary>
        /// Source line number
        /// </summary>
        public int CurrentSourceLine { get; set; }

        /// <summary>
        /// Source index of the first column
        /// </summary>
        public int FirstColumn { get; set; }

        /// <summary>
        /// Last position of the processed instruction
        /// </summary>
        public int LastInstructionPos { get; set; }

        /// <summary>
        /// Source index of the beginning of the current line
        /// </summary>
        public int FirstPosition { get; set; }

        /// <summary>
        /// Source index of the beginning of the current line
        /// </summary>
        public int LastPosition { get; set; }

        /// <summary>
        /// The current label
        /// </summary>
        public string CurrentLabel { get; set; }

        /// <summary>
        /// The current comment
        /// </summary>
        public string CurrentComment { get; set; }

        /// <summary>
        /// The issue to emit when the line has been visited
        /// </summary>
        public string IssueToEmit { get; set; }

        /// <summary>
        /// Indicates if the current pragma is in a field assignment
        /// </summary>
        public bool IsFieldAssignment { get; set; }

        /// <summary>
        /// The macro parameter names within the current line
        /// </summary>
        public List<string> MacroParamNames { get; set; }

        /// <summary>
        /// The text span of the current label
        /// </summary>
        public TextSpan LabelSpan { get; set; }

        /// <summary>
        /// The text span of the current keyword
        /// </summary>
        public TextSpan KeywordSpan { get; set; }

        /// <summary>
        /// The text span of the current comment
        /// </summary>
        public TextSpan CommentSpan { get; set; }

        /// <summary>
        /// Number text spans in the current line
        /// </summary>
        public List<TextSpan> NumberSpans { get; set; }

        /// <summary>
        /// Identifier text spans in the current line
        /// </summary>
        public List<TextSpan> IdentifierSpans { get; set; }

        /// <summary>
        /// String text spans in the current line
        /// </summary>
        public List<TextSpan> StringSpans { get; set; }

        /// <summary>
        /// Semi variable text spans in the current line
        /// </summary>
        public List<TextSpan> SemiVarSpans { get; set; }

        /// <summary>
        /// Function text spans in the current line
        /// </summary>
        public List<TextSpan> FunctionSpans { get; set; }

        /// <summary>
        /// Macro parameter text spans in the current line
        /// </summary>
        public List<TextSpan> MacroParamSpans { get; set; }

        /// <summary>
        /// Statement text spans in the current line
        /// </summary>
        public List<TextSpan> StatementSpans { get; set; }

        /// <summary>
        /// Operand text spans in the current line
        /// </summary>
        public List<TextSpan> OperandSpans { get; set; }

        /// <summary>
        /// Mnemonic text spans in the current line
        /// </summary>
        public List<TextSpan> MnemonicSpans { get; set; }

        /// <summary>
        /// Adds a new number text span
        /// </summary>
        public void AddNumber(ParserRuleContext context)
        {
            if (NumberSpans == null) NumberSpans = new List<TextSpan>();
            NumberSpans.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new string text span
        /// </summary>
        public void AddString(ParserRuleContext context)
        {
            if (StringSpans == null) StringSpans = new List<TextSpan>();
            StringSpans.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new string text span
        /// </summary>
        public void AddString(ITerminalNode node)
        {
            if (StringSpans == null) StringSpans = new List<TextSpan>();
            StringSpans.Add(new TextSpan(node.Symbol.StartIndex, node.Symbol.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new identifier text span
        /// </summary>
        public void AddIdentifier(ParserRuleContext context)
        {
            if (IdentifierSpans == null) IdentifierSpans = new List<TextSpan>();
            IdentifierSpans.Add(new TextSpan(context.Start.StartIndex, context.Stop.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new identifier text span
        /// </summary>
        public void AddIdentifier(ITerminalNode node)
        {
            if (IdentifierSpans == null) IdentifierSpans = new List<TextSpan>();
            IdentifierSpans.Add(new TextSpan(node.Symbol.StartIndex, node.Symbol.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new statement text span
        /// </summary>
        public void AddStatement(ITerminalNode node)
        {
            if (StatementSpans == null) StatementSpans = new List<TextSpan>();
            StatementSpans.Add(new TextSpan(node.Symbol.StartIndex, node.Symbol.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new function text span
        /// </summary>
        public void AddFunction(ParserRuleContext context)
        {
            if (FunctionSpans == null) FunctionSpans = new List<TextSpan>();
            FunctionSpans.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new semi-variable text span
        /// </summary>
        public void AddSemiVar(ParserRuleContext context)
        {
            if (SemiVarSpans == null) SemiVarSpans = new List<TextSpan>();
            SemiVarSpans.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new semi-variable text span
        /// </summary>
        public void AddMacroParam(ParserRuleContext context)
        {
            if (MacroParamSpans == null) MacroParamSpans = new List<TextSpan>();
            MacroParamSpans.Add(new TextSpan(context.Start.StartIndex, context.Stop.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new semi-variable text span
        /// </summary>
        public void AddMacroParamName(string name)
        {
            if (MacroParamNames == null) MacroParamNames = new List<string>();
            MacroParamNames.Add(name);
        }

        /// <summary>
        /// Adds a new operand text span
        /// </summary>
        public void AddOperand(ParserRuleContext context)
        {
            if (OperandSpans == null) OperandSpans = new List<TextSpan>();
            OperandSpans.Add(new TextSpan(context.Start.StartIndex, context.Stop.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new operand text span
        /// </summary>
        public void AddOperand(ITerminalNode node)
        {
            if (OperandSpans == null) OperandSpans = new List<TextSpan>();
            OperandSpans.Add(new TextSpan(node.Symbol.StartIndex, node.Symbol.StopIndex + 1));
        }

        /// <summary>
        /// Adds a new operand text span
        /// </summary>
        public void AddMnemonics(ParserRuleContext context)
        {
            if (MnemonicSpans == null) MnemonicSpans = new List<TextSpan>();
            MnemonicSpans.Add(new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1));
        }

        /// <summary>
        /// Gets an expression from the specified context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ExpressionNode GetExpression(IParseTree context)
        {
            return (ExpressionNode) VisitExpr(context as Z80AsmParser.ExprContext);
        }

        #endregion

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

        #region Z80 Assembly line

        public override object VisitAsmline(Z80AsmParser.AsmlineContext context)
        {
            CurrentLabel = null;
            CurrentComment = null;
            LabelSpan = null;
            KeywordSpan = null;
            CommentSpan = null;
            IssueToEmit = null;
            IsFieldAssignment = false;
            NumberSpans = null;
            IdentifierSpans = null;
            StringSpans = null;
            FunctionSpans = null;
            SemiVarSpans = null;
            MacroParamSpans = null;
            MacroParamNames = null;
            SemiVarSpans = null;
            StatementSpans = null;
            OperandSpans = null;
            MnemonicSpans = null;

            if (context?.Start == null || context.Stop == null)
            {
                return null;
            }

            CurrentSourceLine = context.Start.Line;
            FirstColumn = context.Start.Column;
            FirstPosition = context.Start.StartIndex;
            LastPosition = context.Stop.StopIndex;

            object mainInstructionPart = null;

            // --- Obtain label
            var labelCtx = context.label();
            if (labelCtx != null)
            {
                CurrentLabel = labelCtx.GetChild(0).NormalizeToken();
                LabelSpan = new TextSpan(labelCtx.Start.StartIndex, labelCtx.Start.StopIndex + 1);
            }

            // --- Obtain line body/directive
            var lineBodyCtx = context.lineBody();
            if (lineBodyCtx != null)
            {
                // --- Special case, when a macro parameters is used as the main line
                LastInstructionPos = lineBodyCtx.Stop.StopIndex;
                var macroParamCtx = lineBodyCtx.macroParam();
                if (macroParamCtx != null)
                {
                    VisitMacroParam(macroParamCtx);
                    mainInstructionPart = new MacroParamLine(macroParamCtx.IDENTIFIER()?.NormalizeToken());
                }
                else
                {
                    mainInstructionPart = VisitLineBody(context.lineBody());
                }
            }
            else if (context.directive() != null)
            {
                mainInstructionPart = VisitDirective(context.directive());
            }

            // --- Obtain comment
            if (context.comment() != null)
            {
                var commentCtx = context.comment(); 
                CurrentComment = commentCtx.GetText();
                CommentSpan = new TextSpan(commentCtx.Start.StartIndex, commentCtx.Stop.StopIndex + 1);
            }

            // --- Now, we have every part of the line, and create some special main instruction part
            if (context.exception != null)
            {
                mainInstructionPart = new ParserErrorLine();
            }
            else if (mainInstructionPart == null && (CurrentLabel != null || CurrentComment != null))
            {
                // --- Either a label only or a comment only line
                mainInstructionPart = new NoInstructionLine();
            }

            return mainInstructionPart is SourceLineBase sourceLine
                ? AddLine(sourceLine, context)
                : mainInstructionPart;
        }

        #endregion

        #region Preprocessor directives

        public override object VisitDirective(Z80AsmParser.DirectiveContext context)
        {
            KeywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            var mnemonic = context.GetChild(0).NormalizeToken();
            return mnemonic == "#INCLUDE" 
                ? (object) new IncludeDirective(this, context) 
                : new Directive(this, context);
        }

        #endregion

        #region Pragma handling

        public override object VisitPragma(Z80AsmParser.PragmaContext context)
        {
            KeywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            return base.VisitPragma(context);
        }

        public override object VisitByteEmPragma(Z80AsmParser.ByteEmPragmaContext context)
        {
            KeywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            return base.VisitByteEmPragma(context);
        }

        public override object VisitOrgPragma(Z80AsmParser.OrgPragmaContext context) 
            => new OrgPragma(this, context);

        public override object VisitXorgPragma(Z80AsmParser.XorgPragmaContext context)
            => new XorgPragma(this, context);

        public override object VisitEntPragma(Z80AsmParser.EntPragmaContext context) 
            => new EntPragma(this, context);

        public override object VisitXentPragma(Z80AsmParser.XentPragmaContext context) 
            => new XentPragma(this, context);

        public override object VisitDispPragma(Z80AsmParser.DispPragmaContext context)
            => new DispPragma(this, context);

        public override object VisitEquPragma(Z80AsmParser.EquPragmaContext context)
            => new EquPragma(this, context);

        public override object VisitVarPragma(Z80AsmParser.VarPragmaContext context) 
            => new VarPragma(this, context);

        public override object VisitSkipPragma(Z80AsmParser.SkipPragmaContext context) 
            => new SkipPragma(this, context);

        public override object VisitDefbPragma(Z80AsmParser.DefbPragmaContext context) 
            => new DefbPragma(this, context);

        public override object VisitDefwPragma(Z80AsmParser.DefwPragmaContext context) 
            => new DefwPragma(this, context);

        public override object VisitDefmPragma(Z80AsmParser.DefmPragmaContext context) 
            => new DefmnPragma(this, context, false, false);

        public override object VisitDefnPragma(Z80AsmParser.DefnPragmaContext context)
            => new DefmnPragma(this, context, true, false);

        public override object VisitDefcPragma(Z80AsmParser.DefcPragmaContext context)
            => new DefmnPragma(this, context, false, true);

        public override object VisitDefhPragma(Z80AsmParser.DefhPragmaContext context) 
            => new DefhPragma(this, context);

        public override object VisitExternPragma(Z80AsmParser.ExternPragmaContext context) 
            => new ExternPragma();

        public override object VisitDefsPragma(Z80AsmParser.DefsPragmaContext context) 
            => new DefsPragma(this, context);

        public override object VisitFillbPragma(Z80AsmParser.FillbPragmaContext context) 
            => new FillbPragma(this, context);

        public override object VisitFillwPragma(Z80AsmParser.FillwPragmaContext context) 
            => new FillwPragma(this, context);

        public override object VisitModelPragma(Z80AsmParser.ModelPragmaContext context) 
            => new ModelPragma(context);

        public override object VisitAlignPragma(Z80AsmParser.AlignPragmaContext context) 
            => new AlignPragma(this, context);

        public override object VisitTracePragma(Z80AsmParser.TracePragmaContext context) 
            => new TracePragma(this, context);

        public override object VisitRndSeedPragma(Z80AsmParser.RndSeedPragmaContext context) 
            => new RndSeedPragma(this, context);

        public override object VisitDefgPragma(Z80AsmParser.DefgPragmaContext context) 
            => new DefgPragma(context);

        public override object VisitDefgxPragma(Z80AsmParser.DefgxPragmaContext context) 
            => new DefgxPragma(this, context);

        public override object VisitErrorPragma(Z80AsmParser.ErrorPragmaContext context) 
            => new ErrorPragma(this, context);

        public override object VisitIncBinPragma(Z80AsmParser.IncBinPragmaContext context) 
            => new IncludeBinPragma(this, context);

        #endregion

        #region Field assignment

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.fieldAssignment"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitFieldAssignment(Z80AsmParser.FieldAssignmentContext context)
        {
            IsFieldAssignment = true;
            return base.VisitFieldAssignment(context);
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
            KeywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            return base.VisitOperation(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.trivialOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTrivialOperation(Z80AsmParser.TrivialOperationContext context)
        {
            return new TrivialOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            };
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
            return new TrivialNextOperation
            {
                Mnemonic = context.GetChild(0).NormalizeToken()
            };
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.compoundOperation"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitCompoundOperation(Z80AsmParser.CompoundOperationContext context)
        {
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
            return op;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80AsmParser.operand"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitOperand(Z80AsmParser.OperandContext context)
        {
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
            KeywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
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
            AddMacroParam(context);
            if (context.IDENTIFIER() != null)
            {
                AddMacroParamName(context.IDENTIFIER().NormalizeToken());
            }
            return null;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.macroOrStructInvocation"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMacroOrStructInvocation(Z80AsmParser.MacroOrStructInvocationContext context)
        {
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

            KeywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            return new MacroOrStructInvocation(context.IDENTIFIER().NormalizeToken(), macroOps);
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
            foreach (var id in context.IDENTIFIER())
            {
                AddIdentifier(id);
            }
            return new MacroStatement(context.IDENTIFIER().Select(id => id.NormalizeToken()).ToList());
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
            return new MacroEndStatement();
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.structStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitStructStatement(Z80AsmParser.StructStatementContext context)
        {
            return new StructStatement();
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.structEndMarker"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitStructEndMarker(Z80AsmParser.StructEndMarkerContext context)
        {
            return new StructEndStatement();
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.moduleStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitModuleStatement(Z80AsmParser.ModuleStatementContext context)
        {
            if (context.IDENTIFIER() != null)
            {
                AddIdentifier(context.IDENTIFIER());
            }
            return new ModuleStatement(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.moduleEndMarker"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitModuleEndMarker(Z80AsmParser.ModuleEndMarkerContext context)
        {
            return new ModuleEndStatement();
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
            return new LoopStatement((ExpressionNode)VisitExpr(context.expr()));
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
            return new LoopEndStatement();
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.procStatement"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitProcStatement(Z80AsmParser.ProcStatementContext context)
        {
            return new ProcStatement();
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.procEndMarker"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitProcEndMarker(Z80AsmParser.ProcEndMarkerContext context)
        {
            return new ProcEndStatement();
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
            return new RepeatStatement();
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
            return new UntilStatement((ExpressionNode)VisitExpr(context.expr()));
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
            return new WhileStatement((ExpressionNode)VisitExpr(context.expr()));
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
            return new WhileEndStatement();
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
            if (context.IFSTMT() != null)
            {
                return new IfStatement((ExpressionNode)VisitExpr(context.expr()));
            }

            var isIfUsed = context.IFUSED() != null;
            return new IfStatement((IdentifierNode)VisitSymbolExpr(context.symbolExpr()), isIfUsed);
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
            return new ElifStatement((ExpressionNode)VisitExpr(context.expr()));
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
            return new ElseStatement();
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
            return new IfEndStatement();
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
            if (context.IDENTIFIER() != null) AddIdentifier(context.IDENTIFIER());
            if (context.TO() != null) AddStatement(context.TO());
            if (context.STEP() != null) AddStatement(context.STEP());

            var id = context.IDENTIFIER()?.NormalizeToken();
            var fromExpr = context.expr().Length > 0 ? (ExpressionNode) VisitExpr(context.expr()[0]) : null;
            var toExpr = context.expr().Length > 1 ? (ExpressionNode)VisitExpr(context.expr()[1]) : null;
            var stepExpr = context.expr().Length > 2 ? (ExpressionNode)VisitExpr(context.expr()[2]) : null;
            return new ForStatement(id, fromExpr, toExpr, stepExpr);
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
            return new NextStatement();
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
            return new BreakStatement();
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
            return new ContinueStatement();
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
            var expr = (ExpressionNode)VisitMinMaxExpr(context.GetChild(0) as Z80AsmParser.MinMaxExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitMinMaxExpr(context.GetChild(nextChildIndex)
                    as Z80AsmParser.MinMaxExprContext);
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
        /// Visit a parse tree produced by <see cref="Z80AsmParser.minMaxExpr"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMinMaxExpr(Z80AsmParser.MinMaxExprContext context)
        {
            var expr = (ExpressionNode)VisitUnaryExpr(context.GetChild(0) as Z80AsmParser.UnaryExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitUnaryExpr(context.GetChild(nextChildIndex)
                    as Z80AsmParser.UnaryExprContext);
                var opToken = context.GetChild(nextChildIndex - 1).NormalizeToken();
                var multExpr = opToken == "<?"
                    ? new MinOperationNode()
                    : new MaxOperationNode() as BinaryOperationNode;

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

            if (child0 is Z80AsmParser.SymbolExprContext exprContext)
            {
                return VisitSymbolExpr(exprContext);
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
            var token = context.NormalizeToken();
            if (context.CURADDR() != null || context.MULOP() != null || context.DOT() != null)
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
            else if (token.EndsWith("b") || token.EndsWith("B"))
            {
                AddNumber(context);
                value = (ushort)Convert.ToInt32(token.Substring(0, token.Length - 1).Replace("_", ""), 2);
            }
            // --- Octal literals
            else if (token.EndsWith("q") || token.EndsWith("Q") || token.EndsWith("o") || token.EndsWith("O"))
            {
                AddNumber(context);
                value = (ushort)Convert.ToInt32(token.Substring(0, token.Length - 1), 8);
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
            if (context.ChildCount == 0 || context.IDENTIFIER().Length == 0) return null;

            AddIdentifier(context);
            return new IdentifierNode
            {
                StartFromGlobal = context.GetChild(0).GetText() == "::",
                SymbolName = context.IDENTIFIER()[0].NormalizeToken(),
                ScopeSymbolNames = context.IDENTIFIER().Skip(1).Select(i => i.NormalizeToken()).ToList()
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
            AddFunction(context);
            string token = null;
            if (context.TEXTOF() != null || context.LTEXTOF() != null)
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

                if (context.LTEXTOF() != null)
                {
                    token = token.ToLower();
                }
                return new LiteralNode(token);
            }

            if (context.DEF() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(context.operand() != null && context.operand().NONEARG() == null);
            }

            if (context.ISREG8() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(
                    (context.operand()?.reg8() != null 
                        || context.operand()?.reg8Spec() != null
                        || context.operand()?.reg8Idx() != null)
                    && context.operand().NONEARG() == null);
            }

            if (context.ISREG8STD() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(context.operand()?.reg8() != null
                                       && context.operand().NONEARG() == null);
            }

            if (context.ISREG8SPEC() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(context.operand()?.reg8Spec() != null
                    && context.operand().NONEARG() == null);
            }

            if (context.ISREG8IDX() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(context.operand()?.reg8Idx() != null
                    && context.operand().NONEARG() == null);
            }

            if (context.ISREG16() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(
                    (context.operand()?.reg16() != null
                        || context.operand()?.reg16Idx() != null
                        || context.operand()?.reg16Spec() != null)
                    && context.operand().NONEARG() == null);
            }

            if (context.ISREG16IDX() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(context.operand()?.reg16Idx() != null
                    && context.operand().NONEARG() == null);
            }

            if (context.ISREG16STD() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(context.operand()?.reg16() != null
                                       && context.operand().NONEARG() == null);
            }

            if (context.ISREGINDIRECT() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(context.operand()?.regIndirect() != null
                    && context.operand().NONEARG() == null);
            }

            if (context.ISCPORT() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(context.operand()?.cPort() != null
                    && context.operand().NONEARG() == null);
            }

            if (context.ISINDEXEDADDR() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(context.operand()?.indexedAddr() != null
                    && context.operand().NONEARG() == null);
            }

            if (context.ISCONDITION() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(
                    (context.operand()?.condition() != null || context.operand().reg8()?.GetText()?.ToLower() == "c")
                    && context.operand().NONEARG() == null);
            }

            if (context.ISEXPR() != null)
            {
                CheckForMacroParamNode(context);
                return new LiteralNode(context.operand()?.expr() != null
                                       && context.operand().NONEARG() == null);
            }

            return null;
        }

        /// <summary>
        /// Checks if the operand is used as a macro parameter node
        /// </summary>
        /// <param name="context"></param>
        private void CheckForMacroParamNode(Z80AsmParser.BuiltinFunctionInvocationContext context)
        {
            if (context.operand() == null) return;

            var op = (Operand) VisitOperand(context.operand());
            if (!MacroParsingPhase && (op.Type != OperandType.Expr || !(op.Expression is MacroParamNode)))
            {
                // --- Built in function can use only macro parameters during the collection phase
                IssueToEmit = Errors.Z0421;
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Adds an assembly line to the compilation
        /// </summary>
        /// <param name="line">Line to add</param>
        /// <param name="context">The context that generates this line</param>
        /// <returns>The newly added line</returns>
        private SourceLineBase AddLine(SourceLineBase line, ParserRuleContext context)
        {
            line.SourceLine = CurrentSourceLine;
            line.FirstColumn = FirstColumn;
            line.FirstPosition = FirstPosition;
            line.LastPosition = LastPosition;
            line.ParserException = context.exception;
            line.Label = CurrentLabel;
            line.LabelSpan = LabelSpan;
            line.KeywordSpan = KeywordSpan;
            line.NumberSpans = NumberSpans;
            line.StringSpans = StringSpans;
            line.FunctionSpans = FunctionSpans;
            line.SemiVarSpans = SemiVarSpans;
            line.MacroParamSpans = MacroParamSpans;
            line.MacroParamNames = MacroParamNames;
            line.IdentifierSpans = IdentifierSpans;
            line.StatementSpans = StatementSpans;
            line.OperandSpans = OperandSpans;
            line.MnemonicSpans = MnemonicSpans;
            line.Comment = CurrentComment;
            line.CommentSpan = CommentSpan;
            line.IssueToEmit = IssueToEmit;
            line.InstructionSpan = KeywordSpan != null 
                ? new TextSpan(KeywordSpan.Start, LastInstructionPos + 1) 
                : new TextSpan(FirstColumn, FirstColumn);
            Compilation.Lines.Add(line);
            LastAsmLine = line;
            if (line is ISupportsFieldAssignment fieldAssignment)
            {
                fieldAssignment.IsFieldAssignment = IsFieldAssignment;
            }
            return line;
        }

        #endregion
    }
}