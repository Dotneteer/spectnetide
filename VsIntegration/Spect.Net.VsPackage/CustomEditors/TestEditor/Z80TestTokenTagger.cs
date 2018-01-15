using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Spect.Net.TestParser;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree;
using Spect.Net.TestParser.SyntaxTree.DataBlock;
using Spect.Net.TestParser.SyntaxTree.Expressions;
using Spect.Net.TestParser.SyntaxTree.TestSet;
using Spect.Net.VsPackage.ProjectStructure;

// ReSharper disable StringIndexOfIsCultureSpecific.1

#pragma warning disable 649
#pragma warning disable 67

namespace Spect.Net.VsPackage.CustomEditors.TestEditor
{
    public class Z80TestTokenTagger : ITagger<Z80TestTokenTag>
    {
        internal SpectNetPackage Package => SpectNetPackage.Default;
        internal ITextBuffer SourceBuffer { get; }
        internal string FilePath { get; private set; }

        /// <summary>
        /// Creates the tagger with the specified view and source buffer
        /// </summary>
        /// <param name="sourceBuffer">Source text</param>
        /// <param name="filePath">The file path behind the document</param>
        public Z80TestTokenTagger(ITextBuffer sourceBuffer, string filePath)
        {
            SourceBuffer = sourceBuffer;
            FilePath = filePath;
            Package.SolutionOpened += (s, e) =>
            {
                Package.CodeDiscoverySolution.CurrentProject.ProjectItemRenamed += OnProjectItemRenamed;
            };
        }

        /// <summary>
        /// Let's follow file name changes
        /// </summary>
        private void OnProjectItemRenamed(object sender, ProjectItemRenamedEventArgs args)
        {
            if (args.OldName == FilePath)
            {
                FilePath = args.NewName;
            }
        }

        /// <summary>
        /// Occurs when tags are added to or removed from the provider.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>Gets all the tags that intersect the specified spans. </summary>
        /// <returns>A <see cref="T:Microsoft.VisualStudio.Text.Tagging.TagSpan`1" /> for each tag.</returns>
        /// <param name="spans">The spans to visit.</param>
        public IEnumerable<ITagSpan<Z80TestTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // --- Just for the sake of being thorough...
            if (spans.Count <= 0)
            {
                yield break;
            }

            // --- Obtain and parse the entire snapshot
            var source = spans[0].Snapshot.GetText();
            var inputStream = new AntlrInputStream(source);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.compileUnit();
            var visitor = new Z80TestVisitor();
            visitor.Visit(context);

            foreach (var curSpan in spans)
            {
                var currentLine = curSpan.Start.GetContainingLine();
                var lineNo = currentLine.LineNumber;
                var textOfLine = currentLine.GetText();
                if (string.IsNullOrWhiteSpace(textOfLine)) continue;

                var collectedSpans = new List<TagSpan<Z80TestTokenTag>>();
                Visit(currentLine, visitor.Compilation, lineNo + 1, collectedSpans);

                foreach (var span in collectedSpans)
                {
                    yield return span;
                }
            }
        }

        /// <summary>
        /// Visits the compilation unit for tags
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="visitorCompilation">Compilation unit to visit</param>
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, CompilationUnit visitorCompilation, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
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
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, TestSetNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (lineNo < context.Span.StartLine || lineNo > context.Span.EndLine) return;
            Visit(line, context.TestSetKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            Visit(line, context.TestSetIdSpan, lineNo, collectedSpans, Z80TestTokenType.Identifier);
            Visit(line, context.MachineContext, lineNo, collectedSpans);
            Visit(line, context.SourceContext, lineNo, collectedSpans);
            Visit(line, context.TestOptions, lineNo, collectedSpans);
            Visit(line, context.DataBlock, lineNo, collectedSpans);
            Visit(line, context.Init, lineNo, collectedSpans);
            Visit(line, context.Setup, lineNo, collectedSpans);
            Visit(line, context.Cleanup, lineNo, collectedSpans);
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
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, TestBlockNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.TestKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            Visit(line, context.TestIdSpan, lineNo, collectedSpans, Z80TestTokenType.Identifier);
            Visit(line, context.CategoryKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            Visit(line, context.CategoryIdSpan, lineNo, collectedSpans, Z80TestTokenType.Identifier);
            Visit(line, context.TestOptions, lineNo, collectedSpans);
            Visit(line, context.Params, lineNo, collectedSpans);
            Visit(line, context.Arrange, lineNo, collectedSpans);
            Visit(line, context.Act, lineNo, collectedSpans);
            foreach (var testCase in context.Cases)
            {
                Visit(line, testCase, lineNo, collectedSpans);
            }
            Visit(line, context.Assert, lineNo, collectedSpans);
        }

        /// <summary>
        /// Visits assertions
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">AssertNode to visit</param>
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, AssertNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.AssertKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
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
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, TestCaseNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.CaseKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            Visit(line, context.PortMockKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            foreach (var expr in context.Expressions)
            {
                Visit(line, expr, lineNo, collectedSpans);
            }
            foreach (var id in context.PortMocks)
            {
                Visit(line, id.Span, lineNo, collectedSpans, Z80TestTokenType.Identifier);
            }
        }

        /// <summary>
        /// Visits the test parameters block
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">ParamsNode to visit</param>
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spans found</param>
        private void Visit(ITextSnapshotLine line, ParamsNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.ParamsKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            foreach (var id in context.Ids)
            {
                Visit(line, id.Span, lineNo, collectedSpans, Z80TestTokenType.Identifier);
            }
        }

        /// <summary>
        /// Visits the Setup/Cleanup/Act block
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">AssignmentsNode to visit</param>
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, InvokeCodeNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.KeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            Visit(line, context.CallOrStartSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            Visit(line, context.StopOrHaltSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            Visit(line, context.StartExpr, lineNo, collectedSpans);
            Visit(line, context.StopExpr, lineNo, collectedSpans);
        }

        /// <summary>
        /// Visits the init/arrange block
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">AssignmentsNode to visit</param>
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, AssignmentsNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.KeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
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
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, AssignmentNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }

            if (context is RegisterAssignmentNode regAsgn)
            {
                Visit(line, regAsgn.RegisterSpan, lineNo, collectedSpans, Z80TestTokenType.Z80Key);
                Visit(line, regAsgn.Expr, lineNo, collectedSpans);
            }
            else if (context is FlagAssignmentNode flagAsgn)
            {
                Visit(line, flagAsgn.Span, lineNo, collectedSpans, Z80TestTokenType.Z80Key);
            }
            else if (context is MemoryAssignmentNode memAsgn)
            {
                Visit(line, memAsgn.Address, lineNo, collectedSpans);
                Visit(line, memAsgn.Value, lineNo, collectedSpans);
                Visit(line, memAsgn.Lenght, lineNo, collectedSpans);
            }
        }

        /// <summary>
        /// Visits the data blocks
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">DataBlockNode to visit</param>
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, DataBlockNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.DataKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
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
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, DataMemberNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.IdSpan, lineNo, collectedSpans, Z80TestTokenType.Identifier);
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
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, MemoryPatternNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.KeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            if (context is BytePatternNode bytePattern)
            {
                Visit(line, bytePattern.ByteKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
                foreach (var byteVal in bytePattern.Bytes)
                {
                    Visit(line, byteVal, lineNo, collectedSpans);
                }
            }
            else if (context is WordPatternNode wordPattern)
            {
                Visit(line, wordPattern.WordKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
                foreach (var wordVal in wordPattern.Words)
                {
                    Visit(line, wordVal, lineNo, collectedSpans);
                }
            }
            else if (context is TextPatternNode textPattern)
            {
                Visit(line, textPattern.TextKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
                Visit(line, textPattern.StringSpan, lineNo, collectedSpans, Z80TestTokenType.Number);
            }
        }

        /// <summary>
        /// Visits the port pulses
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">PortPulseNode to visit</param>
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, PortPulseNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
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
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, TestOptionsNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.WithKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
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
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, TestOptionNode context, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.OptionKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
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
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, ExpressionNode expr, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans)
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
                Visit(line, literal.Span, lineNo, collectedSpans, Z80TestTokenType.Number);
            }
            else if (expr is AddressRangeNode addrExpr)
            {
                Visit(line, addrExpr.StartAddress, lineNo, collectedSpans);
                Visit(line, addrExpr.EndAddress, lineNo, collectedSpans);
            }
            else if (expr is FlagNode flagExpr)
            {
                Visit(line, flagExpr.Span, lineNo, collectedSpans, Z80TestTokenType.Z80Key);
            }
            else if (expr is IdentifierNode idExpr)
            {
                Visit(line, idExpr.Span, lineNo, collectedSpans, Z80TestTokenType.Identifier);
            }
            else if (expr is ReachRangeNode reachExpr)
            {
                Visit(line, reachExpr.StartAddress, lineNo, collectedSpans);
                Visit(line, reachExpr.EndAddress, lineNo, collectedSpans);
            }
            else if (expr is RegisterNode regExpr)
            {
                Visit(line, regExpr.Span, lineNo, collectedSpans, Z80TestTokenType.Z80Key);
            }
        }

        /// <summary>
        /// Visits the source context
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">SourceContextNode to visit</param>
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, SourceContextNode context, int lineNo, 
            List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.SourceKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            Visit(line, context.SourceFileSpan, lineNo, collectedSpans, Z80TestTokenType.Number);
        }

        /// <summary>
        /// Visits the test set
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="context">MachineContextNode to visit</param>
        /// <param name="lineNo">Current line numer</param>
        /// <param name="collectedSpans">Collection of spancs found</param>
        private void Visit(ITextSnapshotLine line, MachineContextNode context, int lineNo, 
            List<TagSpan<Z80TestTokenTag>> collectedSpans)
        {
            if (context == null
                || lineNo < context.Span.StartLine
                || lineNo > context.Span.EndLine)
            {
                return;
            }
            Visit(line, context.MachineKeywordSpan, lineNo, collectedSpans, Z80TestTokenType.Keyword);
            Visit(line, context.IdSpan, lineNo, collectedSpans, Z80TestTokenType.Identifier);
        }

        /// <summary>
        /// Visits the specified TextSpand and generates a tag, if it matches with the line number
        /// </summary>
        /// <param name="line">Line to add the tag for</param>
        /// <param name="textSpan">TextSpan to match</param>
        /// <param name="lineNo">Line number</param>
        /// <param name="collectedSpans">List of spans collected</param>
        /// <param name="spanType">Type of span to create</param>
        private void Visit(ITextSnapshotLine line, TextSpan? textSpan, int lineNo, List<TagSpan<Z80TestTokenTag>> collectedSpans, 
            Z80TestTokenType spanType)
        {
            if (lineNo >= textSpan?.StartLine && lineNo <= textSpan.Value.EndLine)
            {
                collectedSpans.Add(CreateSpan(line, textSpan.Value, spanType));
            }
        }

        /// <summary>
        /// Creates a snaphot span from an other snapshot span and a text span
        /// </summary>
        private static TagSpan<Z80TestTokenTag> CreateSpan(ITextSnapshotLine line,
            TextSpan text, Z80TestTokenType tokenType)
        {
            var tagSpan = new Span(line.Start.Position + text.StartColumn, text.EndColumn - text.StartColumn + 1);
            var span = new SnapshotSpan(line.Snapshot, tagSpan);
            var tag = new Z80TestTokenTag(tokenType);
            return new TagSpan<Z80TestTokenTag>(span, tag);
        }
    }
}

#pragma warning restore 67
#pragma warning restore 649
