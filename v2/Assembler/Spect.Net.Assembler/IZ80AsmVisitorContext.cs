using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler
{
    /// <summary>
    /// This interface represents the current state of the Z80AsmVisitor 
    /// </summary>
    public interface IZ80AsmVisitorContext
    {
        /// <summary>
        /// Source line number
        /// </summary>
        int CurrentSourceLine { get; set; }

        /// <summary>
        /// Source index of the first column
        /// </summary>
        int FirstColumn { get; set; }

        /// <summary>
        /// Last position of the processed instruction
        /// </summary>
        int LastInstructionPos { get; set; }

        /// <summary>
        /// Source index of the beginning of the current line
        /// </summary>
        int FirstPosition { get; set; }

        /// <summary>
        /// Source index of the beginning of the current line
        /// </summary>
        int LastPosition { get; set; }

        /// <summary>
        /// The current label
        /// </summary>
        string CurrentLabel { get; set; }

        /// <summary>
        /// The current comment
        /// </summary>
        string CurrentComment { get; set; }

        /// <summary>
        /// The issue to emit when the line has been visited
        /// </summary>
        string IssueToEmit { get; set; }

        /// <summary>
        /// Indicates if the current pragma is in a field assignment
        /// </summary>
        bool IsFieldAssignment { get; set; }

        /// <summary>
        /// The macro parameter names within the current line
        /// </summary>
        List<string> MacroParamNames { get; set; }

        /// <summary>
        /// The text span of the current label
        /// </summary>
        TextSpan LabelSpan { get; set; }

        /// <summary>
        /// The text span of the current keyword
        /// </summary>
        TextSpan KeywordSpan { get; set; }

        /// <summary>
        /// The text span of the current comment
        /// </summary>
        TextSpan CommentSpan { get; set; }

        /// <summary>
        /// Number text spans in the current line
        /// </summary>
        List<TextSpan> NumberSpans { get; set; }

        /// <summary>
        /// Identifier text spans in the current line
        /// </summary>
        List<TextSpan> IdentifierSpans { get; set; }

        /// <summary>
        /// String text spans in the current line
        /// </summary>
        List<TextSpan> StringSpans { get; set; }

        /// <summary>
        /// Semi variable text spans in the current line
        /// </summary>
        List<TextSpan> SemiVarSpans { get; set; }

        /// <summary>
        /// Function text spans in the current line
        /// </summary>
        List<TextSpan> FunctionSpans { get; set; }

        /// <summary>
        /// Macro parameter text spans in the current line
        /// </summary>
        List<TextSpan> MacroParamSpans { get; set; }

        /// <summary>
        /// Statement text spans in the current line
        /// </summary>
        List<TextSpan> StatementSpans { get; set; }

        /// <summary>
        /// Operand text spans in the current line
        /// </summary>
        List<TextSpan> OperandSpans { get; set; }

        /// <summary>
        /// Mnemonic text spans in the current line
        /// </summary>
        List<TextSpan> MnemonicSpans { get; set; }

        /// <summary>
        /// Adds a new number text span
        /// </summary>
        void AddNumber(ParserRuleContext context);

        /// <summary>
        /// Adds a new string text span
        /// </summary>
        void AddString(ParserRuleContext context);

        /// <summary>
        /// Adds a new string text span
        /// </summary>
        void AddString(ITerminalNode node);

        /// <summary>
        /// Adds a new identifier text span
        /// </summary>
        void AddIdentifier(ParserRuleContext context);

        /// <summary>
        /// Adds a new identifier text span
        /// </summary>
        void AddIdentifier(ITerminalNode node);

        /// <summary>
        /// Adds a new statement text span
        /// </summary>
        void AddStatement(ITerminalNode node);

        /// <summary>
        /// Adds a new function text span
        /// </summary>
        void AddFunction(ParserRuleContext context);

        /// <summary>
        /// Adds a new semi-variable text span
        /// </summary>
        void AddSemiVar(ParserRuleContext context);

        /// <summary>
        /// Adds a new semi-variable text span
        /// </summary>
        void AddMacroParam(ParserRuleContext context);

        /// <summary>
        /// Adds a new semi-variable text span
        /// </summary>
        void AddMacroParamName(string name);

        /// <summary>
        /// Adds a new operand text span
        /// </summary>
        void AddOperand(ParserRuleContext context);

        /// <summary>
        /// Adds a new operand text span
        /// </summary>
        void AddOperand(ITerminalNode node);

        /// <summary>
        /// Adds a new operand text span
        /// </summary>
        void AddMnemonics(ParserRuleContext context);

        /// <summary>
        /// Gets an expression from the specified context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        ExpressionNode GetExpression(IParseTree context);

        /// <summary>
        /// Gets an operand from the specified context
        /// </summary>
        /// <param name="context">Context to get the operand from</param>
        /// <returns>Node that represents the operand</returns>
        Operand GetOperand(Z80AsmParser.OperandContext context);
    }
}