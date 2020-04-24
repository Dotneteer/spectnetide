using EnvDTE;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Spect.Net.Assembler;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Operations;
using Spect.Net.Assembler.SyntaxTree.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spect.Net.VsPackage.LanguageServices.Z80Asm
{
    /// <summary>
    /// This class carries out the classification of the Z80 Assembly language text
    /// </summary>
    internal class Z80AsmClassifier : IClassifier, IDisposable
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
            _module;

        private readonly ITextBuffer _buffer;
        private bool _isProcessing;
        private bool _reParse;
        private Z80AsmVisitor _z80SyntaxTreeVisitor;
        private readonly object _locker = new object();

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

            ParseDocument();

            _buffer.Changed += OnBufferChanged;
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var list = new List<ClassificationSpan>();

            List<SourceLineBase> lines;
            lock (_locker)
            {
                if (_z80SyntaxTreeVisitor == null || span.IsEmpty)
                {
                    return list;
                }
                lines = _z80SyntaxTreeVisitor.Compilation.Lines;
            }

            // --- Get document and breakpoint information
            string filePath = GetFilePath();
            var breakpointLines = new List<int>();
            var package = SpectNetPackage.Default;
            if (package != null)
            {
                foreach (Breakpoint bp in package.ApplicationObject.Debugger.Breakpoints)
                {
                    if (string.Compare(bp.File, filePath, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        // --- Breakpoints start lines at 1, ITextBuffer starts from 0
                        breakpointLines.Add(bp.FileLine);
                    }
                }
            }

            // --- We'll use this snapshot to create classifications for
            var snapshot = span.Snapshot;
            var currentLine = span.Start.GetContainingLine();
            var textOfLine = currentLine.GetText();

            // --- Get line indexes
            var firstLineNo = span.Start.GetContainingLine().LineNumber;
            var lastLineNo = span.End.GetContainingLine().LineNumber;

            int firstAsmLine;
            int lastAsmLine;
            bool partialCompile;
            if (lastLineNo - firstLineNo < 50)
            {
                partialCompile = true;
                lines = new List<SourceLineBase>();
                for (var line = firstLineNo; line <= lastLineNo; line++)
                {
                    var source = span.Snapshot.GetLineFromLineNumber(line).GetText();
                    var visitor = Z80AsmVisitor.VisitSource(source);
                    if (visitor.Compilation.Lines.Count > 0)
                    {
                        lines.Add(visitor.Compilation.Lines[0]);
                    }
                    else
                    {
                        lines.Add(new NoInstructionLine());
                    }
                }
                firstAsmLine = 0;
                lastAsmLine = lines.Count - 1;
            }
            else
            {
                partialCompile = false;
                firstAsmLine = Z80AsmVisitor.GetAsmLineIndex(lines, span.Start.GetContainingLine().LineNumber + 1) ?? 0;
                lastAsmLine = Z80AsmVisitor.GetAsmLineIndex(lines, span.End.GetContainingLine().LineNumber + 1) ?? lines.Count - 1;
            }

            // --- Get the range of lines
            for (var line = firstAsmLine; line <= lastAsmLine; line++)
            {
                var sourceLine = lines[line];
                var startOffset = partialCompile
                    ? span.Snapshot.GetLineFromLineNumber(firstLineNo + line).Start.Position 
                    : 0;

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
                    AddClassificationSpan(list, snapshot,
                        new TextSpan(sourceLine.FirstPosition + blockBeginsPos - 1, sourceLine.FirstPosition + blockEndsPos + 1),
                        _comment, startOffset);
                }

                // --- Create labels
                if (sourceLine.LabelSpan != null)
                {
                    AddClassificationSpan(list, snapshot, sourceLine.LabelSpan, _label, startOffset);
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
                    AddClassificationSpan(list, snapshot, sourceLine.KeywordSpan, type, startOffset);
                }

                // --- Create comments
                if (sourceLine.CommentSpan != null)
                {
                    AddClassificationSpan(list, snapshot, sourceLine.CommentSpan, _comment, startOffset);
                }

                // --- Create numbers
                if (sourceLine.NumberSpans != null)
                {
                    foreach (var numberSpan in sourceLine.NumberSpans)
                    {
                        AddClassificationSpan(list, snapshot, numberSpan, _number, startOffset);
                    }
                }

                // --- Create strings
                if (sourceLine.StringSpans != null)
                {
                    foreach (var stringSpan in sourceLine.StringSpans)
                    {
                        AddClassificationSpan(list, snapshot, stringSpan, _string, startOffset);
                    }
                }

                // --- Create functions
                if (sourceLine.FunctionSpans != null)
                {
                    foreach (var functionSpan in sourceLine.FunctionSpans)
                    {
                        AddClassificationSpan(list, snapshot, functionSpan, _function, startOffset);
                    }
                }

                // --- Create semi-variables
                if (sourceLine.SemiVarSpans != null)
                {
                    foreach (var semiVarSpan in sourceLine.SemiVarSpans)
                    {
                        AddClassificationSpan(list, snapshot, semiVarSpan, _semiVar, startOffset);
                    }
                }

                // --- Create macro parameters
                if (sourceLine.MacroParamSpans != null)
                {
                    foreach (var macroParamSpan in sourceLine.MacroParamSpans)
                    {
                        AddClassificationSpan(list, snapshot, macroParamSpan, _macroParam, startOffset);
                    }
                }

                // --- Create identifiers
                if (sourceLine.IdentifierSpans != null)
                {
                    foreach (var id in sourceLine.IdentifierSpans)
                    {
                        AddClassificationSpan(list, snapshot, id, _identifier, startOffset);
                    }
                }

                // --- Create statements
                if (sourceLine.StatementSpans != null)
                {
                    foreach (var statement in sourceLine.StatementSpans)
                    {
                        AddClassificationSpan(list, snapshot, statement, _statement, startOffset);
                    }
                }

                // --- Create operands
                if (sourceLine.OperandSpans != null)
                {
                    foreach (var operand in sourceLine.OperandSpans)
                    {
                        AddClassificationSpan(list, snapshot, operand, _operand, startOffset);
                    }
                }

                // --- Create mnemonics
                if (sourceLine.MnemonicSpans != null)
                {
                    foreach (var mnemonic in sourceLine.MnemonicSpans)
                    {
                        AddClassificationSpan(list, snapshot, mnemonic, _operand, startOffset);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// This event is fired whenever the classification changes.
        /// </summary>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        /// <summary>
        /// Refreshes the document
        /// </summary>
        public void Refresh()
        {
            var file = GetFilePath();
            var span = new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length);
            var tempEvent = ClassificationChanged;
            tempEvent?.Invoke(this, new ClassificationChangedEventArgs(span));
        }

        /// <summary>
        /// Refreshes the specified line on the screen
        /// </summary>
        /// <param name="lineNo"></param>
        public void RefreshLine(int lineNo)
        {
            if (!_buffer.Properties.TryGetProperty(typeof(ITextView), out ITextView view))
            {
                return;
            }

            var lines = view.VisualSnapshot.Lines;
            var line = lines.FirstOrDefault(a => a.LineNumber == lineNo);
            if (line == null) return;

            var startPosition = line.Start;
            var endPosition = line.EndIncludingLineBreak;

            var span = new SnapshotSpan(view.TextSnapshot, Span.FromBounds(startPosition, endPosition));
            if (view is IWpfTextView wpfTextView)
            {
                wpfTextView.ViewScroller.EnsureSpanVisible(span, EnsureSpanVisibleOptions.AlwaysCenter);
                var firstLine = wpfTextView.TextViewLines.FirstVisibleLine;
                var lastLine = wpfTextView.TextViewLines.LastVisibleLine;
                if (firstLine == null || lastLine == null) return;
                var newSpan = new SnapshotSpan(wpfTextView.TextSnapshot,
                    Span.FromBounds(firstLine.Start, lastLine.EndIncludingLineBreak));
            }
        }

        /// <summary>
        /// Gets the file path of the document associated with this classifier
        /// </summary>
        /// <returns>Full file path</returns>
        public string GetFilePath()
        {
            return (_buffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument docProperty))
                ? docProperty.FilePath
                : null;
        }

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
            if (_isProcessing)
            {
                _reParse = true;
                return;
            }
            _isProcessing = true;
            await DoParse();
            while (_reParse)
            {
                _reParse = false;
                _isProcessing = true;
                await DoParse();
            }

            async Task DoParse()
            {
                await Task.Run(() =>
                {
                    try
                    {
                        // --- Get the entire text of the source code
                        var span = new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length);
                        var source = _buffer.CurrentSnapshot.GetText(span);

                        // --- Let's use the Z80 assembly parser to obtain tags
                        var visitor = Z80AsmVisitor.VisitSource(source);
                        lock (_locker)
                        {
                            _z80SyntaxTreeVisitor = visitor;
                        }

                        // --- Code is parsed, sign the change
                        ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(span));
                    }
                    finally
                    {
                        _isProcessing = false;
                    }
                });
            }
        }

        /// <summary>
        /// Creates a classification span
        /// </summary>
        /// <param name="list">List to add the new classification span to</param>
        /// <param name="snapshot">The snapshot to use</param>
        /// <param name="text">The text span for the classification span</param>
        /// <param name="type">Type of classification</param>
        /// <returns>The newly created classification span</returns>
        private static void AddClassificationSpan(List<ClassificationSpan> list, ITextSnapshot snapshot,
            TextSpan text, IClassificationType type, int startOffset)
        {
            try
            {
                var span = new Span(text.Start + startOffset, text.End - text.Start);
                var classificationSpan = new ClassificationSpan(new SnapshotSpan(snapshot, span), type);
                list.Add(classificationSpan);
            }
            catch
            {
                // --- Any potential exception is intentionally ignored.
            }
        }

        public void Dispose()
        {
        }
    }
}