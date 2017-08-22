using Antlr4.Runtime;

namespace AntlrZ80Asm.Test
{
    /// <summary>
    /// Helper methods for parser tests
    /// </summary>
    public class TestBed
    {
        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        protected virtual Z80AsmVisitor Parse(string textToParse)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80AsmLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80AsmParser(tokenStream);
            var context = parser.compileUnit();
            var visitor = new Z80AsmVisitor();
            visitor.Visit(context);

            return visitor;
        }
    }
}