using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a EXCHANGE WATCH tool command
    /// </summary>
    public class ExchangeWatchToolCommand : ToolCommandNode
    {
        /// <summary>
        /// FROM index
        /// </summary>
        public ushort From { get; }

        /// <summary>
        /// TO index
        /// </summary>
        public ushort To { get; }

        public ExchangeWatchToolCommand(CommandToolParser.ExchangeWatchCommandContext context)
        {
            if (context.LITERAL().Length < 2) return;
            From = ProcessNumber(context.LITERAL()[0].GetText());
            To = ProcessNumber(context.LITERAL()[1].GetText());
        }
    }
}