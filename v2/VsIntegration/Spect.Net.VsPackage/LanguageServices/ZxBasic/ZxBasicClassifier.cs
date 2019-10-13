using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Spect.Net.BasicParser;
using Spect.Net.BasicParser.Generated;
using Spect.Net.VsPackage.LanguageServices.Z80Asm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spect.Net.VsPackage.LanguageServices.ZxBasic
{
    internal class ZxBasicClassifier : IClassifier, IDisposable
    {
        // --- Store registered classification types here
        private readonly IClassificationType
            _zxbLabel,
            _zxbKeyword,
            _zxbComment,
            _zxbFunction,
            _zxbOperator,
            _zxbIdentifier,
            _zxbNumber,
            _zxbString,
            _zxbAsm,

            _label,
            _comment,
            _pragma,
            _directive,
            _includeDirective,
            _instruction,
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
        private Dictionary<int, List<TokenInfo>> _tokenMap;
        private readonly object _locker = new object();

        /// <summary>
        /// Initializes the Z80 Assembly language classifier with the specified
        /// attributes
        /// </summary>
        /// <param name="buffer">The text buffer of the Z80 Assembly source code</param>
        /// <param name="registry">The service that manages known classification types</param>
        internal ZxBasicClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            _buffer = buffer;

            _zxbLabel = registry.GetClassificationType(ZxBasicClassificationTypes.ZXB_LABEL);
            _zxbKeyword = registry.GetClassificationType(ZxBasicClassificationTypes.ZXB_KEYWORD);
            _zxbComment = registry.GetClassificationType(ZxBasicClassificationTypes.ZXB_COMMENT);
            _zxbFunction = registry.GetClassificationType(ZxBasicClassificationTypes.ZXB_FUNCTION);
            _zxbOperator = registry.GetClassificationType(ZxBasicClassificationTypes.ZXB_OPERATOR);
            _zxbIdentifier = registry.GetClassificationType(ZxBasicClassificationTypes.ZXB_IDENTIFIER);
            _zxbNumber = registry.GetClassificationType(ZxBasicClassificationTypes.ZXB_NUMBER);
            _zxbString = registry.GetClassificationType(ZxBasicClassificationTypes.ZXB_STRING);
            _zxbAsm = registry.GetClassificationType(ZxBasicClassificationTypes.ZXB_ASM);

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

            lock (_locker)
            {
                if (_tokenMap == null || span.IsEmpty)
                {
                    return list;
                }
            }

            // --- We'll use this snapshot to create classifications for
            var snapshot = span.Snapshot;

            // --- Get the range of lines
            var firstLine = span.Start.GetContainingLine().LineNumber;
            var lastLine = span.End.GetContainingLine().LineNumber;

            for (var line = firstLine; line <= lastLine; line++)
            {
                if (!_tokenMap.TryGetValue(line + 1, out var tokensInLine))
                {
                    continue;
                }

                var lineContents = _buffer.CurrentSnapshot.GetLineFromLineNumber(line);
                foreach (var token in tokensInLine)
                {
                    try
                    {
                        IClassificationType type = null;
                        switch (token.TokenType)
                        {
                            case TokenType.ZxbKeyword:
                                type = _zxbKeyword;
                                break;
                            case TokenType.ZxbComment:
                                type = _zxbComment;
                                break;
                            case TokenType.ZxbFunction:
                                type = _zxbFunction;
                                break;
                            case TokenType.ZxbOperator:
                                type = _zxbOperator;
                                break;
                            case TokenType.ZxbIdentifier:
                                type = _zxbIdentifier;
                                break;
                            case TokenType.ZxbNumber:
                                type = _zxbNumber;
                                break;
                            case TokenType.ZxbString:
                                type = _zxbString;
                                break;
                            case TokenType.ZxbLabel:
                                type = _zxbLabel;
                                break;
                            case TokenType.ZxbAsm:
                                type = _zxbAsm;
                                break;

                            case TokenType.Comment:
                                type = _comment;
                                break;
                            case TokenType.Label:
                                type = _label;
                                break;
                            case TokenType.Instruction:
                                type = _instruction;
                                break;
                            case TokenType.Pragma:
                                type = _pragma;
                                break;
                            case TokenType.Directive:
                                type = _directive;
                                break;
                            case TokenType.IncludeDirective:
                                type = _includeDirective;
                                break;
                            case TokenType.MacroInvocation:
                                type = _macroInvocation;
                                break;
                            case TokenType.Module:
                                type = _module;
                                break;
                            case TokenType.Statement:
                                type = _statement;
                                break;
                            case TokenType.Number:
                                type = _number;
                                break;
                            case TokenType.String:
                                type = _string;
                                break;
                            case TokenType.Function:
                                type = _function;
                                break;
                            case TokenType.SemiVar:
                                type = _semiVar;
                                break;
                            case TokenType.MacroParam:
                                type = _macroParam;
                                break;
                            case TokenType.Identifier:
                                type = _identifier;
                                break;
                            case TokenType.Operand:
                                type = _operand;
                                break;
                        }
                        if (type != null)
                        {
                            var tokenSpan = new Span(lineContents.Start.Position + token.StartColumn, token.Length);
                            var classificationSpan = new ClassificationSpan(new SnapshotSpan(snapshot, tokenSpan), type);
                            list.Add(classificationSpan);
                        }
                    }
                    catch
                    {
                        // --- Any potential exception is intentionally ignored.
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
                    var lexer = new ZxBasicLexer(inputStream);
                    var tokenStream = new CommonTokenStream(lexer);
                    var parser = new ZxBasicParser(tokenStream);
                    var context = parser.compileUnit();
                    var treeWalker = new ParseTreeWalker();
                    var parserListener = new ZxBasicParserListener(tokenStream);
                    treeWalker.Walk(parserListener, context);

                    lock (_locker)
                    {
                        _tokenMap = parserListener.TokenMap;
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

        public void Dispose()
        {
        }
    }
}
