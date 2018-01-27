using System.Collections.Generic;
using Antlr4.Runtime;
using Shouldly;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.Test
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
        protected virtual Z80TestVisitor Parse(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.compileUnit();
            var visitor = new Z80TestVisitor();
            visitor.Visit(context);
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);

            return visitor;
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
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.expr();
            var visitor = new Z80TestVisitor();
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return (ExpressionNode) visitor.VisitExpr(context);
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        protected virtual List<Z80TestParserErrorInfo> ParseWithErrors(string textToParse)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.compileUnit();
            var visitor = new Z80TestVisitor();
            visitor.Visit(context);
            return parser.SyntaxErrors;
        }
    }
}