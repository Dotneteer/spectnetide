using Antlr4.Runtime;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Spect.Net.TestParser;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree;
using Spect.Net.TestParser.SyntaxTree.DataBlock;
using Spect.Net.TestParser.SyntaxTree.Expressions;
using Spect.Net.TestParser.SyntaxTree.TestSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;

namespace Spect.Net.VsPackage.LanguageServices.Z80Test
{
    /// <summary>
    /// This class carries out the classification of the Z80 Unit
    /// Test language text
    /// </summary>
    internal class Z80TestClassifier : IClassifier
    {
        // --- Store registered classification types here
        private readonly IClassificationType
            _keyword,
            _comment,
            _number,
            _identifier,
            _key,
            _breakpoint,
            _currentBreakpoint;

        private readonly ITextBuffer _buffer;
        private bool _isProcessing;
        private Z80TestVisitor _z80TestVisitor;
        private SortedList<int, int> _commentSpans;

        internal Z80TestClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            _buffer = buffer;

            _keyword = registry.GetClassificationType(Z80TestClassificationTypes.Z80_T_KEYWORD);
            _comment = registry.GetClassificationType(Z80TestClassificationTypes.Z80_T_COMMENT);
            _number = registry.GetClassificationType(Z80TestClassificationTypes.Z80_T_NUMBER);
            _identifier = registry.GetClassificationType(Z80TestClassificationTypes.Z80_T_IDENTIFIER);
            _key = registry.GetClassificationType(Z80TestClassificationTypes.Z80_T_KEY);
            _breakpoint = registry.GetClassificationType(Z80TestClassificationTypes.Z80_T_BREAKPOINT);
            _currentBreakpoint = registry.GetClassificationType(Z80TestClassificationTypes.Z80_T_CURRENT_BREAKPOINT);

            ParseDocument();

            _buffer.Changed += OnBufferChanged;
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var list = new List<ClassificationSpan>();

            if (_z80TestVisitor == null || _isProcessing || span.IsEmpty)
            {
                return list;
            }

            // --- We'll use this snapshot to create classifications for
            var currentLine = span.Start.GetContainingLine();
            var textOfLine = currentLine.GetText();
            if (string.IsNullOrWhiteSpace(textOfLine)) return list;

            // --- Visit program elements
            Visit(currentLine, _z80TestVisitor.Compilation, currentLine.LineNumber + 1, list);

            // --- Check for comments
            var commentSpans = GetCommentSpans(currentLine.Start.Position, currentLine.End.Position - 1);
            foreach (var (startPos, endPos) in commentSpans)
            {
                var commentSpan = new Span(startPos, endPos - startPos + 1);
                list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, commentSpan), _comment));
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
                    var lexer = new Z80TestLexer(inputStream);
                    var tokenStream = new CommonTokenStream(lexer);
                    var parser = new Z80TestParser(tokenStream);
                    var context = parser.compileUnit();
                    var commentWalker = new ParseTreeWalker();
                    var parserListener = new Z80TestParserListener(tokenStream);
                    commentWalker.Walk(parserListener, context);
                    _commentSpans = parserListener.CommentSpans;
                    _z80TestVisitor = new Z80TestVisitor();
                    _z80TestVisitor.Visit(context);

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
        /// Visits the compilation unit for tags
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="visitorCompilation">Compilation unit to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, CompilationUnit visitorCompilation, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            foreach (var testSet in visitorCompilation.TestSets)
            {
                Visit(line, testSet, lineNo, collectedSpans);
            }
        }

        /// <summary>
        /// Visits the test set
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">TestSetNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, TestSetNode context, int lineNo,
            List<ClassificationSpan> collectedSpans)
        {
            if (lineNo < context.Span.StartLine || lineNo > context.Span.EndLine) return;
            Visit(line, context.TestSetKeywordSpan, lineNo, collectedSpans, _keyword);
            Visit(line, context.TestSetIdSpan, lineNo, collectedSpans, _identifier);
            Visit(line, context.Sp48ModeSpan, lineNo, collectedSpans, _keyword);
            Visit(line, context.SourceContext, lineNo, collectedSpans);
            Visit(line, context.CallStub, lineNo, collectedSpans);
            Visit(line, context.DataBlock, lineNo, collectedSpans);
            Visit(line, context.Init, lineNo, collectedSpans);
            foreach (var testBlock in context.TestBlocks)
            {
                Visit(line, testBlock, lineNo, collectedSpans);
            }
        }

        /// <summary>
        /// Visits a single test block
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">TestBlockNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, TestBlockNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.TestKeywordSpan, lineNo, collectedSpans, _keyword);
            Visit(line, context.TestIdSpan, lineNo, collectedSpans, _identifier);
            Visit(line, context.CategoryKeywordSpan, lineNo, collectedSpans, _keyword);
            Visit(line, context.CategoryIdSpan, lineNo, collectedSpans, _identifier);
            Visit(line, context.TestOptions, lineNo, collectedSpans);
            Visit(line, context.Setup, lineNo, collectedSpans);
            Visit(line, context.Params, lineNo, collectedSpans);
            Visit(line, context.Arrange, lineNo, collectedSpans);
            Visit(line, context.Act, lineNo, collectedSpans);
            Visit(line, context.Breakpoints, lineNo, collectedSpans);
            foreach (var testCase in context.Cases)
            {
                Visit(line, testCase, lineNo, collectedSpans);
            }
            Visit(line, context.Assert, lineNo, collectedSpans);
            Visit(line, context.Cleanup, lineNo, collectedSpans);
        }

        /// <summary>
        /// Visits breakpoints
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">BreakpointsNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, BreakpointsNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.BreakpointKeywordSpan, lineNo, collectedSpans, _keyword);
            foreach (var expr in context.Expressions)
            {
                Visit(line, expr, lineNo, collectedSpans);
            }
        }

        /// <summary>
        /// Visits assertions
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">AssertNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, AssertNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.AssertKeywordSpan, lineNo, collectedSpans, _keyword);
            foreach (var expr in context.Expressions)
            {
                Visit(line, expr, lineNo, collectedSpans);
            }
        }

        /// <summary>
        /// Visits a single test case
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">TestCaseNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, TestCaseNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.CaseKeywordSpan, lineNo, collectedSpans, _keyword);
            Visit(line, context.PortMockKeywordSpan, lineNo, collectedSpans, _keyword);
            foreach (var expr in context.Expressions)
            {
                Visit(line, expr, lineNo, collectedSpans);
            }
            foreach (var id in context.PortMocks)
            {
                Visit(line, id.Span, lineNo, collectedSpans, _identifier);
            }
        }

        /// <summary>
        /// Visits the test parameters block
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">ParamsNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, ParamsNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.ParamsKeywordSpan, lineNo, collectedSpans, _keyword);
            foreach (var id in context.Ids)
            {
                Visit(line, id.Span, lineNo, collectedSpans, _keyword);
            }
        }

        /// <summary>
        /// Visits the Setup/Cleanup/Act block
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">AssignmentsNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, InvokeCodeNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.KeywordSpan, lineNo, collectedSpans, _keyword);
            Visit(line, context.CallOrStartSpan, lineNo, collectedSpans, _keyword);
            Visit(line, context.StopOrHaltSpan, lineNo, collectedSpans, _keyword);
            Visit(line, context.StartExpr, lineNo, collectedSpans);
            Visit(line, context.StopExpr, lineNo, collectedSpans);
        }

        /// <summary>
        /// Visits the init/arrange block
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">AssignmentsNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, AssignmentsNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.KeywordSpan, lineNo, collectedSpans, _keyword);
            foreach (var asgn in context.Assignments)
            {
                Visit(line, asgn, lineNo, collectedSpans);
            }
        }

        /// <summary>
        /// Visits a single assignment
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">AssignmentNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, AssignmentNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }

            if (context is RegisterAssignmentNode regAsgn)
            {
                Visit(line, regAsgn.RegisterSpan, lineNo, collectedSpans, _key);
                Visit(line, regAsgn.Expr, lineNo, collectedSpans);
            }
            else if (context is FlagAssignmentNode flagAsgn)
            {
                Visit(line, flagAsgn.Span, lineNo, collectedSpans, _key);
            }
            else if (context is MemoryAssignmentNode memAsgn)
            {
                Visit(line, memAsgn.Address, lineNo, collectedSpans);
                Visit(line, memAsgn.Value, lineNo, collectedSpans);
                Visit(line, memAsgn.Length, lineNo, collectedSpans);
            }
        }

        /// <summary>
        /// Visits the data blocks
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">DataBlockNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, DataBlockNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.DataKeywordSpan, lineNo, collectedSpans, _keyword);
            foreach (var member in context.DataMembers)
            {
                Visit(line, member, lineNo, collectedSpans);
            }
        }

        /// <summary>
        /// Visits the data member
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">DataMemberNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, DataMemberNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.IdSpan, lineNo, collectedSpans, _identifier);
            if (context is ValueMemberNode valMember)
            {
                Visit(line, valMember.Expr, lineNo, collectedSpans);
            }
            else if (context is MemoryPatternMemberNode memMember)
            {
                foreach (var pattern in memMember.Patterns)
                {
                    Visit(line, pattern, lineNo, collectedSpans);
                }
            }
            else if (context is PortMockMemberNode portMember)
            {
                Visit(line, portMember.Expr, lineNo, collectedSpans);
                foreach (var pulse in portMember.Pulses)
                {
                    Visit(line, pulse, lineNo, collectedSpans);
                }
            }
        }

        /// <summary>
        /// Visits the memory patterns
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">MemoryPatternNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, MemoryPatternNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.KeywordSpan, lineNo, collectedSpans, _keyword);
            if (context is BytePatternNode bytePattern)
            {
                Visit(line, bytePattern.ByteKeywordSpan, lineNo, collectedSpans, _keyword);
                foreach (var byteVal in bytePattern.Bytes)
                {
                    Visit(line, byteVal, lineNo, collectedSpans);
                }
            }
            else if (context is WordPatternNode wordPattern)
            {
                Visit(line, wordPattern.WordKeywordSpan, lineNo, collectedSpans, _keyword);
                foreach (var wordVal in wordPattern.Words)
                {
                    Visit(line, wordVal, lineNo, collectedSpans);
                }
            }
            else if (context is TextPatternNode textPattern)
            {
                Visit(line, textPattern.TextKeywordSpan, lineNo, collectedSpans, _keyword);
                Visit(line, textPattern.StringSpan, lineNo, collectedSpans, _number);
            }
        }

        /// <summary>
        /// Visits the port pulses
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">PortPulseNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, PortPulseNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.ValueExpr, lineNo, collectedSpans);
            Visit(line, context.Pulse1Expr, lineNo, collectedSpans);
            Visit(line, context.Pulse2Expr, lineNo, collectedSpans);
        }

        /// <summary>
        /// Visits the test options
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">TestOptionsNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, TestOptionsNode context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.WithKeywordSpan, lineNo, collectedSpans, _keyword);
            foreach (var option in context.Options)
            {
                Visit(line, option, lineNo, collectedSpans);
            }
        }

        /// <summary>
        /// Visits a single test option node
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">TestOptionNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, TestOptionNodeBase context, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.OptionKeywordSpan, lineNo, collectedSpans, _keyword);
            if (context is TimeoutTestOptionNode timeoutNode)
            {
                Visit(line, timeoutNode.Expr, lineNo, collectedSpans);
            }
        }

        /// <summary>
        /// Visits an expression node for literals
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="expr">ExpressionNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, ExpressionNode expr, int lineNo, 
            List<ClassificationSpan> collectedSpans)
        {
            if (expr == null
                || lineNo < expr.Span.StartLine
                || lineNo > expr.Span.EndLine)
            {
                return;
            }

            if (expr is ConditionalExpressionNode condExpr)
            {
                Visit(line, condExpr.Condition, lineNo, collectedSpans);
                Visit(line, condExpr.TrueExpression, lineNo, collectedSpans);
                Visit(line, condExpr.FalseExpression, lineNo, collectedSpans);
            }
            if (expr is BinaryOperationNode binExpr)
            {
                Visit(line, binExpr.LeftOperand, lineNo, collectedSpans);
                Visit(line, binExpr.RightOperand, lineNo, collectedSpans);
            }
            else if (expr is UnaryExpressionNode unExpr)
            {
                Visit(line, unExpr.Operand, lineNo, collectedSpans);
            }
            else if (expr is LiteralNode literal)
            {
                Visit(line, literal.Span, lineNo, collectedSpans, _number);
            }
            else if (expr is AddressRangeNode addrExpr)
            {
                Visit(line, addrExpr.StartAddress, lineNo, collectedSpans);
                Visit(line, addrExpr.EndAddress, lineNo, collectedSpans);
            }
            else if (expr is FlagNode flagExpr)
            {
                Visit(line, flagExpr.Span, lineNo, collectedSpans, _key);
            }
            else if (expr is IdentifierNode idExpr)
            {
                Visit(line, idExpr.Span, lineNo, collectedSpans, _identifier);
            }
            else if (expr is MemoryTouchNodeBase reachExpr)
            {
                Visit(line, reachExpr.StartAddress, lineNo, collectedSpans);
                Visit(line, reachExpr.EndAddress, lineNo, collectedSpans);
            }
            else if (expr is RegisterNode regExpr)
            {
                Visit(line, regExpr.Span, lineNo, collectedSpans, _key);
            }
        }

        /// <summary>
        /// Visits the source context
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">SourceContextNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, SourceContextNode context, int lineNo,
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.SourceKeywordSpan, lineNo, collectedSpans, _keyword);
            Visit(line, context.SourceFileSpan, lineNo, collectedSpans, _number);
        }

        /// <summary>
        /// Visits the callstub
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">SourceContextNode to visit</param>
        /// <param name="lineNo">Current line number</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, CallStubNode context, int lineNo,
            List<ClassificationSpan> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.CallStubKeywordSpan, lineNo, collectedSpans, _keyword);
            Visit(line, context.Value, lineNo, collectedSpans);
        }

        /// <summary>
        /// Visits the specified TextSpan and generates a tag, if it matches with the line number
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="textSpan">TextSpan to match</param>
        /// <param name="lineNo">Line number</param>
        /// <param name="collectedSpans">List of spans collected</param>
        /// <param name="spanType">Type of span to create</param>
        private static void Visit(ITextSnapshotLine line, TextSpan? textSpan, int lineNo, 
            ICollection<ClassificationSpan> collectedSpans,
            IClassificationType spanType)
        {
            if (lineNo >= textSpan?.StartLine && lineNo <= textSpan.Value.EndLine)
            {
                collectedSpans.Add(CreateSpan(line, textSpan.Value, spanType));
            }
        }

        /// <summary>
        /// Creates a classification span
        /// </summary>
        /// <param name="line">The snapshot line to use</param>
        /// <param name="text">The text span for the classification span</param>
        /// <param name="type">Type of classification</param>
        /// <returns>The newly created classification span</returns>
        private static ClassificationSpan CreateSpan(ITextSnapshotLine line,
            TextSpan text, IClassificationType type)
        {
            var span = new Span(line.Start.Position + text.StartColumn, text.EndColumn - text.StartColumn + 1);
            return new ClassificationSpan(new SnapshotSpan(line.Snapshot, span), type);
        }

        /// <summary>
        /// Gets the index of the compilation line index from the compilation
        /// </summary>
        /// <param name="lineStartPos">The start line position to check</param>
        /// <param name="lineEndPos">The end line position to check</param>
        /// <returns>Compilation line index</returns>
        private List<(int StartPos, int EndPos)> GetCommentSpans(int lineStartPos, int lineEndPos)
        {
            var result = new List<(int StartPos, int EndPos)>();
            foreach (var commentSpan in _commentSpans)
            {
                if (commentSpan.Key >= lineStartPos && commentSpan.Key <= lineEndPos
                    || commentSpan.Value >= lineStartPos && commentSpan.Value <= lineEndPos
                    || lineStartPos >= commentSpan.Key && lineStartPos <= commentSpan.Value
                    || lineEndPos >= commentSpan.Key && lineEndPos <= commentSpan.Value)
                {
                    result.Add((commentSpan.Key, commentSpan.Value));
                }
            }
            return result;
        }
    }
}