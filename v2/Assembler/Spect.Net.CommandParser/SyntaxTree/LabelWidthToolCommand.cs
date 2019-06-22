using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a LABEL WIDTH tool command
    /// </summary>
    public class LabelWidthToolCommand : ToolCommandNode
    {
        /// <summary>
        /// GOTO address
        /// </summary>
        public ushort Width { get; }

        public LabelWidthToolCommand(CommandToolParser.LabelWidthCommandContext context)
        {
            if (context.LITERAL() == null) return;
            Width = ProcessNumber(context.LITERAL().GetText());
        }
    }
}