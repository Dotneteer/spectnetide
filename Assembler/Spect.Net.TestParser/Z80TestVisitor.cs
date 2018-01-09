using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree;
using Spect.Net.TestParser.SyntaxTree.DataBlock;
using Spect.Net.TestParser.SyntaxTree.TestBlock;

namespace Spect.Net.TestParser
{
    /// <summary>
    /// This visitor class processes the elements of the AST parsed by ANTLR
    /// </summary>
    public class Z80TestVisitor: Z80TestBaseVisitor<object>
    {
        private DataBlock _lastDataBlock;
        private TestBlock _lastTestBlock;
        private IncludeDirective _lastIncludeDirective;

        /// <summary>
        /// Access the compilation results through this object
        /// </summary>
        public CompilationUnit Compilation { get; } = new CompilationUnit();

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.dataBlock"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDataBlock(Z80TestParser.DataBlockContext context)
        {
            // --- Datablock boundaries
            var block =_lastDataBlock = new DataBlock();
            Compilation.LanguageBlocks.Add(_lastDataBlock);
            block.Span = new TextSpan(context);

            // --- Tokens
            var token = context.GetChild(0);
            return base.VisitDataBlock(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.includeDirective"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitIncludeDirective(Z80TestParser.IncludeDirectiveContext context)
        {
            // --- IncludeDirective boundaries
            var block = _lastIncludeDirective = new IncludeDirective();
            Compilation.LanguageBlocks.Add(_lastIncludeDirective);
            block.Span = new TextSpan(context);

            // --- Tokens
            block.IncludeKeywordSpan = new TextSpan(context.GetChild(0));
            block.StringSpan = new TextSpan(context.GetChild(1));
            var filename = context.GetChild(1).GetText();
            block.Filename = filename.Substring(1, filename.Length - 2);
            return block;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.testBlock"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTestBlock(Z80TestParser.TestBlockContext context)
        {
            var block = _lastTestBlock = new TestBlock();
            block.Span = new TextSpan(context);
            Compilation.LanguageBlocks.Add(_lastTestBlock);
            return base.VisitTestBlock(context);
        }
    }
}