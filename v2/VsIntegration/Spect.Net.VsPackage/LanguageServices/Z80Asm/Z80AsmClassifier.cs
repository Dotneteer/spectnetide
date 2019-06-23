using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Spect.Net.Assembler;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Operations;
using Spect.Net.Assembler.SyntaxTree.Statements;

namespace Spect.Net.VsPackage.LanguageServices.Z80Asm
{
    /// <summary>
    /// This class carries out the classification of the Z80 Assembly language text
    /// </summary>
    internal class Z80AsmClassifier: IClassifier
    {
        // --- Store registered classification types here
        private readonly IClassificationType
            _label,
            _pragma,
            _directive,
            _includeDirective,
            _instruction,
            _comment,
            _number,
            _identifier,
            _string,
            _function,
            _macroParam,
            _statement,
            _macroInvocation,
            _operand,
            _semiVar,
            _module,
            _breakpoint,
            _currentBreakpoint;

        private readonly ITextBuffer _buffer;
        private bool _isProcessing;
        private Z80AsmVisitor _z80SyntaxTreeVisitor;

        /// <summary>
        /// Initializes the Z80 Assembly language classifier with the specified
        /// attributes
        /// </summary>
        /// <param name="buffer">The text buffer of the Z80 Assembly source code</param>
        /// <param name="registry">The service that manages known classification types</param>
        internal Z80AsmClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            _buffer = buffer;

            _label = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_LABEL);
            _pragma = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_PRAGMA);
            _directive = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_DIRECTIVE);
            _includeDirective = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_INCLUDE_DIRECTIVE);
            _instruction = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_INSTRUCTION);
            _comment = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_COMMENT);
            _number = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_NUMBER);
            _identifier = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_IDENTIFIER);
            _string = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_STRING);
            _function = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_FUNCTION);
            _macroParam = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_MACRO_PARAM);
            _statement = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_STATEMENT);
            _macroInvocation = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_MACRO_INVOCATION);
            _operand = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_OPERAND);
            _semiVar = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_SEMI_VAR);
            _module = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_MODULE);
            _breakpoint = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_BREAKPOINT);
            _currentBreakpoint = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_CURRENT_BREAKPOINT);

            ParseDocument();

            _buffer.Changed += OnBufferChanged;
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var list = new List<ClassificationSpan>();

            if (_z80SyntaxTreeVisitor == null || _isProcessing || span.IsEmpty)
            {
                return list;
            }

            // --- We'll use this snapshot to create classifications for
            var snapshot = span.Snapshot;
            var currentLine = span.Start.GetContainingLine();
            var textOfLine = currentLine.GetText();

            // --- Get the range of lines
            var firstLine = GetAsmLineIndex(span.Start.GetContainingLine().LineNumber + 1) ?? 0;
            var lastLine = GetAsmLineIndex(span.End.GetContainingLine().LineNumber + 1) ?? _z80SyntaxTreeVisitor.Compilation.Lines.Count - 1;

            for (var line = firstLine; line <= lastLine; line++)
            {
                var sourceLine = _z80SyntaxTreeVisitor.Compilation.Lines[line];

                // --- Check for a block comment
                var lastStartIndex = 0;
                while (true)
                {
                    var blockBeginsPos = textOfLine.IndexOf("/*", lastStartIndex, StringComparison.Ordinal);
                    if (blockBeginsPos < 0) break;
                    var blockEndsPos = textOfLine.IndexOf("*/", blockBeginsPos, StringComparison.Ordinal);
                    if (blockEndsPos <= blockBeginsPos) break;

                    // --- Block comment found
                    lastStartIndex = blockEndsPos + 2;
                    list.Add(CreateClassificationSpan(snapshot, 
                        new TextSpan(sourceLine.FirstPosition + blockBeginsPos - 1, sourceLine.FirstPosition + blockEndsPos + 1), 
                        _comment));
                }

                // --- Create labels
                if (sourceLine.LabelSpan != null)
                {
                    list.Add(CreateClassificationSpan(snapshot, sourceLine.LabelSpan, _label));
                }

                // --- Create keywords
                if (sourceLine.KeywordSpan != null)
                {
                    var type = _instruction;
                    switch (sourceLine)
                    {
                        case PragmaBase _:
                            type = _pragma;
                            break;
                        case Directive _:
                            type = _directive;
                            break;
                        case IncludeDirective _:
                            type = _includeDirective;
                            break;
                        case MacroOrStructInvocation _:
                            type = _macroInvocation;
                            break;
                        case ModuleStatement _:
                        case ModuleEndStatement _:
                            type = _module;
                            break;
                        case StatementBase _:
                            type = _statement;
                            break;
                    }

                    // --- Retrieve a pragma/directive/instruction
                    list.Add(CreateClassificationSpan(snapshot, sourceLine.KeywordSpan, type));
                }

                // --- Create comments
                if (sourceLine.CommentSpan != null)
                {
                    list.Add(CreateClassificationSpan(snapshot, sourceLine.CommentSpan, _comment));
                }

                // --- Create numbers
                if (sourceLine.NumberSpans != null)
                {
                    foreach (var numberSpan in sourceLine.NumberSpans)
                    {
                        list.Add(CreateClassificationSpan(snapshot, numberSpan, _number));
                    }
                }

                // --- Create strings
                if (sourceLine.StringSpans != null)
                {
                    foreach (var stringSpan in sourceLine.StringSpans)
                    {
                        list.Add(CreateClassificationSpan(snapshot, stringSpan, _string));
                    }
                }

                // --- Create functions
                if (sourceLine.FunctionSpans != null)
                {
                    foreach (var functionSpan in sourceLine.FunctionSpans)
                    {
                        list.Add(CreateClassificationSpan(snapshot, functionSpan, _function));
                    }
                }

                // --- Create semi-variables
                if (sourceLine.SemiVarSpans != null)
                {
                    foreach (var semiVarSpan in sourceLine.SemiVarSpans)
                    {
                        list.Add(CreateClassificationSpan(snapshot, semiVarSpan, _semiVar));
                    }
                }

                // --- Create macro parameters
                if (sourceLine.MacroParamSpans != null)
                {
                    foreach (var macroParamSpan in sourceLine.MacroParamSpans)
                    {
                        list.Add(CreateClassificationSpan(snapshot, macroParamSpan, _macroParam));
                    }
                }

                // --- Create identifiers
                if (sourceLine.IdentifierSpans != null)
                {
                    foreach (var id in sourceLine.IdentifierSpans)
                    {
                        list.Add(CreateClassificationSpan(snapshot, id, _identifier));
                    }
                }

                // --- Create statements
                if (sourceLine.StatementSpans != null)
                {
                    foreach (var statement in sourceLine.StatementSpans)
                    {
                        list.Add(CreateClassificationSpan(snapshot, statement, _statement));
                    }
                }

                // --- Create operands
                if (sourceLine.OperandSpans != null)
                {
                    foreach (var operand in sourceLine.OperandSpans)
                    {
                        list.Add(CreateClassificationSpan(snapshot, operand, _operand));
                    }
                }

                // --- Create mnemonics
                if (sourceLine.MnemonicSpans != null)
                {
                    foreach (var mnemonic in sourceLine.MnemonicSpans)
                    {
                        list.Add(CreateClassificationSpan(snapshot, mnemonic, _operand));
                    }
                }

                if (sourceLine is EmittingOperationBase && sourceLine.InstructionSpan != null)
                {
                    // --- This line contains executable instruction,
                    // --- So it might have a breakpoint
                    // TODO: Handle potential breakpoint
                }

                // TODO: Handle current breakpoint
            }

            return list;
        }

        /// <summary>
        /// This event is fired whenever the classification changes.
        /// </summary>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        /// <summary>
        /// Re-parses the document whenever it changes.
        /// </summary>
        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            ParseDocument();
        }

        /// <summary>
        /// Parses the entire source code 
        /// </summary>
        private async void ParseDocument()
        {
            // --- Do not start parsing over existing parsing
            if (_isProcessing) return;
            _isProcessing = true;

            await Task.Run(() =>
            {
                try
                {
                    // --- Get the entire text of the source code
                    var span = new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length);
                    var source = _buffer.CurrentSnapshot.GetText(span);

                    // --- Let's use the Z80 assembly parser to obtain tags
                    var inputStream = new AntlrInputStream(source);
                    var lexer = new Z80AsmLexer(inputStream);
                    var tokenStream = new CommonTokenStream(lexer);
                    var parser = new Z80AsmParser(tokenStream);
                    var context = parser.compileUnit();
                    _z80SyntaxTreeVisitor = new Z80AsmVisitor(inputStream);
                    _z80SyntaxTreeVisitor.Visit(context);

                    // --- Code is parsed, sign the change
                    ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(span));
                }
                finally
                {
                    _isProcessing = false;
                }
            });
        }

        /// <summary>
        /// Gets the index of the compilation line index from the compilation
        /// </summary>
        /// <param name="lineNo">The source code's line number</param>
        /// <returns>Compilation line index</returns>
        private int? GetAsmLineIndex(int lineNo)
        {
            var lines = _z80SyntaxTreeVisitor.Compilation.Lines;
            var lower = 0;
            var upper = lines.Count - 1;
            while (lower <= upper)
            {
                var idx = (lower + upper) / 2;
                var sourceLineNo = lines[idx].SourceLine;
                if (sourceLineNo == lineNo)
                {
                    return idx;
                }
                if (sourceLineNo > lineNo)
                {
                    upper = idx - 1;
                }
                else
                {
                    lower = idx + 1;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a classification span
        /// </summary>
        /// <param name="snapshot">The snapshot to use</param>
        /// <param name="text">The text span for the classification span</param>
        /// <param name="type">Type of classification</param>
        /// <returns>The newly created classification span</returns>
        private static ClassificationSpan CreateClassificationSpan(ITextSnapshot snapshot,
            TextSpan text, IClassificationType type)
        {
            var span = new Span(text.Start, text.End - text.Start);
            return new ClassificationSpan(new SnapshotSpan(snapshot, span), type);
        }
    }
}