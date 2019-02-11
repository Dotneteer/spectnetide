using System.Globalization;
using System.Text;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a COMMENT tool command
    /// </summary>
    public class CommentToolCommand : ToolCommandNode
    {
        /// <summary>
        /// COMMENT address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// GOTO Text
        /// </summary>
        public string Text { get; }

        public CommentToolCommand(CommandToolParser.CommentCommandContext context)
        {
            Address = ProcessNumber(context.LITERAL().GetText());
            if (HasSemanticError || context.ChildCount < 5) return;

            var sb = new StringBuilder();
            for (var i = 4; i < context.ChildCount; i++)
            {
                sb.Append(context.GetChild(i).GetText());
            }
            Text = sb.ToString();
        }
    }
}