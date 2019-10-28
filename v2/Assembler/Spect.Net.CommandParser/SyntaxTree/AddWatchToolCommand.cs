using System.Text;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents an ADD WATCH tool command
    /// </summary>
    public class AddWatchToolCommand : ToolCommandNode
    {
        /// <summary>
        /// WATCH condition
        /// </summary>
        public string Condition { get; }

        public AddWatchToolCommand(CommandToolParser.AddWatchCommandContext context)
        {
            var sb = new StringBuilder();
            for (var i = 1; i < context.ChildCount; i++)
            {
                sb.Append(context.GetChild(i).GetText());
            }
            Condition = sb.ToString().Trim();
        }
    }
}