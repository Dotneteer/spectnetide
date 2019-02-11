using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a REMOVE WATCH tool command
    /// </summary>
    public class RemoveWatchToolCommand : ToolCommandNode
    {
        /// <summary>
        /// WATCH index
        /// </summary>
        public ushort Index { get; }

        public RemoveWatchToolCommand(CommandToolParser.RemoveWatchCommandContext context)
        {
            if (context.LITERAL() == null) return;
            Index = ProcessNumber(context.LITERAL().GetText());
        }
    }
}