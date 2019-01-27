using System.Collections.Generic;
using Antlr4.Runtime;
using Shouldly;
using Spect.Net.EvalParser.Generated;
using Spect.Net.EvalParser.SyntaxTree;

namespace Spect.Net.EvalParser.Test
{
    /// <summary>
    /// Helper methods for parser tests
    /// </summary>
    public class ParserTestBed
    {
        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <param name="expectedErrors">Number of errors expected</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        protected virtual Z80ExpressionNode Parse(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80EvalLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80EvalParser(tokenStream);
            var context = parser.compileUnit();
            var visitor = new Z80EvalVisitor();
            var z80Expr = (Z80ExpressionNode)visitor.Visit(context);
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return z80Expr;
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <param name="expectedErrors">Number of errors expected</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        protected virtual ExpressionNode ParseExpr(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80EvalLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80EvalParser(tokenStream);
            var context = parser.expr();
            var visitor = new Z80EvalVisitor();
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return (ExpressionNode)visitor.VisitExpr(context);
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        protected virtual List<Z80EvalParserErrorInfo> ParseWithErrors(string textToParse)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80EvalLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80EvalParser(tokenStream);
            var context = parser.compileUnit();
            var visitor = new Z80EvalVisitor();
            visitor.Visit(context);
            return parser.SyntaxErrors;
        }
    }
}