using Antlr4.Runtime;
using Shouldly;
using Spect.Net.CommandParser.Generated;
using Spect.Net.CommandParser.SyntaxTree;

namespace Spect.Net.CommandParser.Test
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
        protected virtual ToolCommandNode ParseCommand(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new CommandToolLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CommandToolParser(tokenStream);
            var context = parser.toolCommand();
            var visitor = new CommandToolVisitor();
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return (ToolCommandNode)visitor.VisitToolCommand(context);
        }


    }
}