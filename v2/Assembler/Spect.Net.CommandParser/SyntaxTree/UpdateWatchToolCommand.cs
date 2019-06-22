using System.Text;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents an UPDATE WATCH tool command
    /// </summary>
    public class UpdateWatchToolCommand : ToolCommandNode
    {
        /// <summary>
        /// WATCH index
        /// </summary>
        public ushort Index { get; }

        /// <summary>
        /// WATCH condition
        /// </summary>
        public string Condition { get; }

        public UpdateWatchToolCommand(CommandToolParser.UpdateWatchCommandContext context)
        {
            if (context.LITERAL() == null) return;
            Index = ProcessNumber(context.LITERAL().GetText());
            if (context.ChildCount < 2) return;

            var sb = new StringBuilder();
            var isCompact = !string.IsNullOrWhiteSpace(context.GetChild(1).GetText());
            for (var i = isCompact ? 2 : 3; i < context.ChildCount; i++)
            {
                sb.Append(context.GetChild(i).GetText());
            }
            Condition = sb.ToString().Trim();
        }
    }
}