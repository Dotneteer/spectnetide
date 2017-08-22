using AntlrZ80Asm.SyntaxTree;

namespace AntlrZ80Asm
{
    /// <summary>
    /// This visitor class processes the elements of the AST parsed by ANTLR
    /// </summary>
    public class Z80AsmVisitor: Z80AsmBaseVisitor<object>
    {
        private string _label;

        /// <summary>
        /// Access the comilation results through this object
        /// </summary>
        public CompilationUnit Compilation { get; } = new CompilationUnit();

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.asmline"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAsmline(Z80AsmParser.AsmlineContext context)
        {
            _label = context.label()?.children?[0].GetText();
            return base.VisitAsmline(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.trivialInstruction"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTrivialInstruction(Z80AsmParser.TrivialInstructionContext context)
        {
            var line = new TrivialInstruction
            {
                Label = _label,
                Mnemonic = context.children[0].NormalizeToken()
            };
            Compilation.Lines.Add(line);
            return base.VisitTrivialInstruction(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80AsmParser.load8BitInstruction"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLoad8BitInstruction(Z80AsmParser.Load8BitInstructionContext context)
        {
            // 'LD' (8-bit-reg) ',' (8-bit-reg)
            var line = new LoadInstruction
            {
                LoadType = LoadType.Ld8Bit,
                Destination = context.children[1].NormalizeToken(),
                Source = context.children[3].NormalizeToken()
            };
            Compilation.Lines.Add(line);
            return base.VisitLoad8BitInstruction(context);
        }
    }
}