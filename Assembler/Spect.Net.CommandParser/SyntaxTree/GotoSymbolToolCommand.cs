using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a GOTO SYMBOL tool command
    /// </summary>
    public class GotoSymbolToolCommand : ToolCommandNode
    {
        /// <summary>
        /// GOTO symbol
        /// </summary>
        public string Symbol { get; }

        public GotoSymbolToolCommand(CommandToolParser.GotoSymbolCommandContext context)
        {
            if (context.IDENTIFIER() != null)
            {
                Symbol = context.IDENTIFIER().GetText();
            }
        }
    }
}