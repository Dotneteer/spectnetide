using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using EnvDTE;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Spect.Net.Assembler;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Operations;
using Spect.Net.Assembler.SyntaxTree.Statements;
using Spect.Net.VsPackage.ProjectStructure;

#pragma warning disable 649
#pragma warning disable 67
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    /// <summary>
    /// This tagger provides classification tags for the Z80 assembly 
    /// language elements
    /// </summary>
    internal sealed class Z80AsmTokenTagger : ITagger<Z80AsmTokenTag>,
        IDisposable
    {
        internal SpectNetPackage Package => SpectNetPackage.Default;
        internal ITextBuffer SourceBuffer { get; }
        internal string FilePath { get; private set; }

        /// <summary>
        /// Creates the tagger with the specified view and source buffer
        /// </summary>
        /// <param name="sourceBuffer">Source text</param>
        /// <param name="filePath">The file path behind the document</param>
        public Z80AsmTokenTagger(ITextBuffer sourceBuffer, string filePath)
        {
            SourceBuffer = sourceBuffer;
            SourceBuffer.Changed += (sender, args) => { Package.OnTestFileChanged(FilePath); };
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

        /// <summary>
        /// Gets all the tags that intersect the specified spans.
        /// </summary>
        /// <param name="spans">The spans to visit.</param>
        /// <returns>
        /// A <see cref="T:Microsoft.VisualStudio.Text.Tagging.TagSpan`1" /> for each tag.
        /// </returns>
        public IEnumerable<ITagSpan<Z80AsmTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // --- Obtain the breakpoints that may affect this view
            var affectedLines = new List<int>();
            foreach (Breakpoint bp in Package.ApplicationObject.Debugger.Breakpoints)
            {
                if (string.Compare(bp.File, FilePath, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    // --- Breakpoints start lines at 1, ITextBuffer starts from 0
                    affectedLines.Add(bp.FileLine - 1);
                }
            }

            foreach (var curSpan in spans)
            {
                var currentLine = curSpan.Start.GetContainingLine();
                var textOfLine = currentLine.GetText();

                // --- Just to be sure...
                if (textOfLine == null) yield break;

                // --- Let's use the Z80 assembly parser to obtain tags
                var inputStream = new AntlrInputStream(textOfLine);
                var lexer = new Z80AsmLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);
                var parser = new Z80AsmParser(tokenStream);
                var context = parser.asmline();
                var visitor = new Z80AsmVisitor();
                visitor.Visit(context);

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
                    yield return CreateSpan(currentLine, new TextSpan(blockBeginsPos, blockEndsPos + 2), Z80AsmTokenType.Comment);
                }

                // --- No code line, no tagging
                if (!(visitor.LastAsmLine is SourceLineBase asmline)) continue;

                if (asmline is EmittingOperationBase && asmline.InstructionSpan != null)
                {
                    // --- This line contains executable instruction,
                    // --- So it might have a breakpoint
                    if (affectedLines.IndexOf(currentLine.LineNumber) >= 0)
                    {
                        // --- Check for the any preset breakpoint
                        yield return CreateSpan(currentLine, asmline.InstructionSpan, Z80AsmTokenType.Breakpoint);
                    }
                }

                if (asmline.LabelSpan != null)
                {
                    // --- Retrieve a label
                    yield return CreateSpan(currentLine, asmline.LabelSpan, Z80AsmTokenType.Label);
                }

                if (asmline.KeywordSpan != null)
                {
                    var type = Z80AsmTokenType.Instruction;
                    switch (asmline)
                    {
                        case PragmaBase _:
                            type = Z80AsmTokenType.Pragma;
                            break;
                        case Directive _:
                            type = Z80AsmTokenType.Directive;
                            break;
                        case IncludeDirective _:
                            type = Z80AsmTokenType.Include;
                            break;
                        case MacroInvocation _:
                            type = Z80AsmTokenType.MacroInvocation;
                            break;
                        case ModuleStatement _:
                        case ModuleEndStatement _:
                            type = Z80AsmTokenType.ModuleKeyword;
                            break;
                        case StatementBase _:
                            type = Z80AsmTokenType.Statement;
                            break;
                    }

                    // --- Retrieve a pragma/directive/instruction
                    yield return CreateSpan(currentLine, asmline.KeywordSpan, type);
                }

                if (asmline.CommentSpan != null)
                {
                    yield return CreateSpan(currentLine, asmline.CommentSpan, Z80AsmTokenType.Comment);
                }

                if (asmline.Numbers != null)
                {
                    foreach (var numberSpan in asmline.Numbers)
                    {
                        yield return CreateSpan(currentLine, numberSpan, Z80AsmTokenType.Number);
                    }
                }

                if (asmline.Strings != null)
                {
                    foreach (var stringSpan in asmline.Strings)
                    {
                        yield return CreateSpan(currentLine, stringSpan, Z80AsmTokenType.String);
                    }
                }

                if (asmline.Functions != null)
                {
                    foreach (var functionSpan in asmline.Functions)
                    {
                        yield return CreateSpan(currentLine, functionSpan, Z80AsmTokenType.Function);
                    }
                }

                if (asmline.SemiVars != null)
                {
                    foreach (var semiVarSpan in asmline.SemiVars)
                    {
                        yield return CreateSpan(currentLine, semiVarSpan, Z80AsmTokenType.SemiVar);
                    }
                }

                if (asmline.MacroParams != null)
                {
                    foreach (var macroParamSpan in asmline.MacroParams)
                    {
                        yield return CreateSpan(currentLine, macroParamSpan, Z80AsmTokenType.MacroParam);
                    }
                }

                if (asmline.Identifiers != null)
                {
                    foreach (var id in asmline.Identifiers)
                    {
                        yield return CreateSpan(currentLine, id, Z80AsmTokenType.Identifier);
                    }
                }

                if (asmline.Statements != null)
                {
                    foreach (var statement in asmline.Statements)
                    {
                        yield return CreateSpan(currentLine, statement, Z80AsmTokenType.Statement);
                    }
                }

                if (asmline.Operands != null)
                {
                    foreach (var operand in asmline.Operands)
                    {
                        yield return CreateSpan(currentLine, operand, Z80AsmTokenType.Operand);
                    }
                }

                if (asmline.Mnemonics != null)
                {
                    foreach (var mnemonic in asmline.Mnemonics)
                    {
                        yield return CreateSpan(currentLine, mnemonic, Z80AsmTokenType.Instruction);
                    }
                }

            }
        }

        /// <summary>
        /// Creates a snapshot span from an other snapshot span and a text span
        /// </summary>
        private static TagSpan<Z80AsmTokenTag> CreateSpan(ITextSnapshotLine line,
            TextSpan text, Z80AsmTokenType tokenType)
        {
            var tagSpan = new Span(line.Start.Position + text.Start, text.End - text.Start);
            var span = new SnapshotSpan(line.Snapshot, tagSpan);
            var tag = new Z80AsmTokenTag(tokenType);
            return new TagSpan<Z80AsmTokenTag>(span, tag);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            if (Package.CodeDiscoverySolution?.CurrentProject != null)
            {
                Package.CodeDiscoverySolution.CurrentProject.ProjectItemRenamed -= OnProjectItemRenamed;
            }
        }
    }
}

#pragma warning restore 67
#pragma warning restore 649
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
