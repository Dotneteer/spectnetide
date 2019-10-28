using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Microsoft.VisualStudio.Text;
using Spect.Net.Assembler;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Operations;
using Spect.Net.Assembler.SyntaxTree.Statements;
using Spect.Net.BasicParser.Generated;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Spect.Net.VsPackage.LanguageServices.ZxBasic
{
    /// <summary>
    /// Represents a listener that processes the ZX BASIC grammar
    /// </summary>
    public class ZxBasicParserListener: ZxBasicBaseListener
    {
        // --- Store a reference to the text buffer
        private readonly ITextBuffer _buffer;

        // --- Indicates if an "asm...end asm" section is being processed
        private bool _asmProcessing;

        /// <summary>
        /// The map of tokens for each source code line
        /// </summary>
        public readonly Dictionary<int, List<TokenInfo>> TokenMap
            = new Dictionary<int, List<TokenInfo>>();
        
        public ZxBasicParserListener(ITextBuffer buffer)
        {
            _buffer = buffer;
        }

        public override void ExitBlock_comment([NotNull] ZxBasicParser.Block_commentContext context)
        {
            if (_asmProcessing) return;

            var text = context.GetText();
            var parts = Regex.Split(text, "\r\n");
            var startLine = context.Start.Line;
            var offset = context.Start.Column;
            for (var i = 0; i < parts.Length; i++)
            {
                AddSpan(TokenType.ZxbComment, startLine + i, offset, parts[i].Length);
                offset = 0;
            }
        }

        public override void ExitLine_comment([NotNull] ZxBasicParser.Line_commentContext context)
        {
            if (_asmProcessing) return;

            var text = context.GetText();            
            if (text.ToUpper().StartsWith("REM"))
            {
                AddSpan(TokenType.ZxbStatement, context.Start, 3);
                AddSpan(TokenType.ZxbComment, context.Start.Line, context.Start.Column + 3, context.GetText().Length - 3);
            }
            else
            {
                AddSpan(TokenType.ZxbComment, context.Start, context.GetText().Length);
            }
        }

        public override void ExitConsole([NotNull] ZxBasicParser.ConsoleContext context)
        {
            if (_asmProcessing) return;

            AddSpan(TokenType.ZxbConsole, context);
        }

        public override void ExitPreproc([NotNull] ZxBasicParser.PreprocContext context)
        {
            if (_asmProcessing) return;

            AddSpan(TokenType.ZxbPreProc, context);
        }

        public override void ExitStatement([NotNull] ZxBasicParser.StatementContext context)
        {
            if (_asmProcessing) return;

            AddSpan(TokenType.ZxbStatement, context);
        }

        public override void ExitControl_flow([NotNull] ZxBasicParser.Control_flowContext context)
        {
            if (_asmProcessing) return;

            AddSpan(TokenType.ZxbControlFlow, context);
        }

        public override void ExitFunction([NotNull] ZxBasicParser.FunctionContext context)
        {
            if (_asmProcessing) return;

            AddSpan(TokenType.ZxbFunction, context);
        }

        public override void ExitOperator([NotNull] ZxBasicParser.OperatorContext context)
        {
            if (_asmProcessing) return;

            AddSpan(TokenType.ZxbOperator, context.Start, context.Start.Text.Length);
        }

        public override void ExitType([NotNull] ZxBasicParser.TypeContext context)
        {
            if (_asmProcessing) return;

            AddSpan(TokenType.ZxbType, context);
        }

        public override void ExitIdentifier([NotNull] ZxBasicParser.IdentifierContext context)
        {
            if (_asmProcessing) return;

            AddSpan(TokenType.ZxbIdentifier, context.Start, context.Start.Text.Length);
        }

        public override void ExitNumber([NotNull] ZxBasicParser.NumberContext context)
        {
            if (_asmProcessing) return;

            AddSpan(TokenType.ZxbNumber, context);
        }

        public override void ExitString([NotNull] ZxBasicParser.StringContext context)
        {
            if (_asmProcessing) return;

            AddSpan(TokenType.ZxbString, context);
        }

        public override void ExitLabel([NotNull] ZxBasicParser.LabelContext context)
        {
            if (_asmProcessing) return;

            AddSpan(TokenType.ZxbLabel, context);
        }

        public override void EnterAsm_start([NotNull] ZxBasicParser.Asm_startContext context)
        {
            _asmProcessing = true;
        }

        public override void ExitAsm_end([NotNull] ZxBasicParser.Asm_endContext context)
        {
            _asmProcessing = false;
        }

        public override void ExitAsm_section([NotNull] ZxBasicParser.Asm_sectionContext context)
        {
            // --- Mark the delimiting tokens
            if (context.asm_start() != null)
            {
                AddSpan(TokenType.ZxbAsm, context.asm_start());
            }
            if (context.asm_end() != null)
            {
                AddSpan(TokenType.ZxbAsm, context.asm_end());
            }

            var bodyContext = context.asm_body();
            if (bodyContext == null) return;

            // --- Create the Z80 Asm body text
            var bodyStart = bodyContext.Start.StartIndex;
            var bodyEnd = bodyContext.Stop.StopIndex;
            var startLine = bodyContext.Start.Line;
            var body = bodyEnd > bodyStart
                ? _buffer.CurrentSnapshot.GetText(bodyStart, bodyEnd - bodyStart + 1)
                : string.Empty;

            // --- Parse the embedded Z80 assembly code
            var parts = Regex.Split(body, "\r\n");
            var visitor = Z80AsmVisitor.VisitSource(body);
            var lines = visitor.Compilation.Lines;

            // --- Iterate through lines
            for (var i = 0; i < parts.Length; i++)
            {
                var offset = i == 0 ? bodyContext.Start.Column : 0;
                var asmLineIdx = Z80AsmVisitor.GetAsmLineIndex(lines, i+1);
                if (asmLineIdx == null) continue;

                // --- Handle block comments
                var textOfLine = parts[i];
                var lastStartIndex = 0;
                while (true)
                {
                    var blockBeginsPos = textOfLine.IndexOf("/*", lastStartIndex, StringComparison.Ordinal);
                    if (blockBeginsPos < 0) break;
                    var blockEndsPos = textOfLine.IndexOf("*/", blockBeginsPos, StringComparison.Ordinal);
                    if (blockEndsPos <= blockBeginsPos) break;

                    // --- Block comment found
                    lastStartIndex = blockEndsPos + 2;
                    AddSpan(TokenType.Comment, i + startLine, offset + blockBeginsPos, lastStartIndex - blockBeginsPos);
                }

                // --- Get the parsed line
                var asmLine = lines[asmLineIdx.Value];

                if (asmLine.LabelSpan != null)
                {
                    AddSpan(TokenType.Label, i + startLine, offset, asmLine, asmLine.LabelSpan);
                }

                // --- Create keywords
                if (asmLine.KeywordSpan != null)
                {
                    var type = TokenType.Instruction;
                    switch (asmLine)
                    {
                        case PragmaBase _:
                            type = TokenType.Pragma;
                            break;
                        case Directive _:
                            type = TokenType.Directive;
                            break;
                        case IncludeDirective _:
                            type = TokenType.IncludeDirective;
                            break;
                        case MacroOrStructInvocation _:
                            type = TokenType.MacroInvocation;
                            break;
                        case ModuleStatement _:
                        case ModuleEndStatement _:
                            type = TokenType.Module;
                            break;
                        case StatementBase _:
                            type = TokenType.Statement;
                            break;
                    }

                    // --- Retrieve a pragma/directive/instruction
                    AddSpan(type, i + startLine, offset, asmLine, asmLine.KeywordSpan);
                }

                // --- Create comments
                if (asmLine.CommentSpan != null)
                {
                    AddSpan(TokenType.Comment, i + startLine, offset, asmLine, asmLine.CommentSpan);
                }

                // --- Create numbers
                if (asmLine.NumberSpans != null)
                {
                    foreach (var numberSpan in asmLine.NumberSpans)
                    {
                        AddSpan(TokenType.Number, i + startLine, offset, asmLine, numberSpan);
                    }
                }

                // --- Create strings
                if (asmLine.StringSpans != null)
                {
                    foreach (var stringSpan in asmLine.StringSpans)
                    {
                        AddSpan(TokenType.String, i + startLine, offset, asmLine, stringSpan);
                    }
                }

                // --- Create functions
                if (asmLine.FunctionSpans != null)
                {
                    foreach (var functionSpan in asmLine.FunctionSpans)
                    {
                        AddSpan(TokenType.Function, i + startLine, offset, asmLine, functionSpan);
                    }
                }

                // --- Create semi-variables
                if (asmLine.SemiVarSpans != null)
                {
                    foreach (var semiVarSpan in asmLine.SemiVarSpans)
                    {
                        AddSpan(TokenType.SemiVar, i + startLine, offset, asmLine, semiVarSpan);
                    }
                }

                // --- Create macro parameters
                if (asmLine.MacroParamSpans != null)
                {
                    foreach (var macroParamSpan in asmLine.MacroParamSpans)
                    {
                        AddSpan(TokenType.MacroParam, i + startLine, offset, asmLine, macroParamSpan);
                    }
                }

                // --- Create identifiers
                if (asmLine.IdentifierSpans != null)
                {
                    foreach (var id in asmLine.IdentifierSpans)
                    {
                        AddSpan(TokenType.Identifier, i + startLine, offset, asmLine, id);
                    }
                }

                // --- Create statements
                if (asmLine.StatementSpans != null)
                {
                    foreach (var statement in asmLine.StatementSpans)
                    {
                        AddSpan(TokenType.Statement, i + startLine, offset, asmLine, statement);
                    }
                }

                // --- Create operands
                if (asmLine.OperandSpans != null)
                {
                    foreach (var operand in asmLine.OperandSpans)
                    {
                        AddSpan(TokenType.Operand, i + startLine, offset, asmLine, operand);
                    }
                }

                // --- Create mnemonics
                if (asmLine.MnemonicSpans != null)
                {
                    foreach (var mnemonic in asmLine.MnemonicSpans)
                    {
                        AddSpan(TokenType.Operand, i + startLine, offset, asmLine, mnemonic);
                    }
                }
            }
        }

        private void AddSpan(TokenType type, int line, int startIndex, SourceLineBase asmLine, TextSpan span)
        {
            AddSpan(type, line, startIndex + span.Start - asmLine.FirstPosition + asmLine.FirstColumn, span.End - span.Start);
        }

        private void AddSpan(TokenType type, ParserRuleContext context)
        {
            var firstColumn = context.Start.Column;
            var lastColumn = context.Stop.Column + context.Stop.Text.Length;
            AddSpan(type, context.Start.Line, firstColumn, lastColumn - firstColumn);
        }

        private void AddSpan(TokenType type, IToken startToken, int length)
        {
            if (startToken == null) return;
            var span = new TokenInfo(type, startToken.Column, length);
            AddSpan(startToken.Line, span);
        }

        private void AddSpan(TokenType type, int line, int start, int length)
        {
            AddSpan(line, new TokenInfo(type, start, length));
        }

        private void AddSpan(int line, TokenInfo tokenInfo)
        {
            if (TokenMap.TryGetValue(line, out var tokenList))
            {
                tokenList.Add(tokenInfo);
            }
            else
            {
                TokenMap.Add(line, new List<TokenInfo> { tokenInfo });
            }
        }
    }
}
