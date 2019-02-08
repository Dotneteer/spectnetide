using System.Globalization;
using System.Text;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a PREFIX COMMENT tool command
    /// </summary>
    public class PrefixCommentToolCommand : ToolCommandNode
    {
        /// <summary>
        /// COMMENT address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// GOTO Text
        /// </summary>
        public string Text { get; }

        public PrefixCommentToolCommand(CommandToolParser.PrefixCommentCommandContext context)
        {
            if (context.HEXNUM() != null)
            {
                Address = ushort.Parse(context.HEXNUM().GetText(), NumberStyles.HexNumber);
            }

            if (context.ChildCount < 5) return;

            var sb = new StringBuilder();
            for (var i = 4; i < context.ChildCount; i++)
            {
                sb.Append(context.GetChild(i).GetText());
            }
            Text = sb.ToString();
        }
    }
}