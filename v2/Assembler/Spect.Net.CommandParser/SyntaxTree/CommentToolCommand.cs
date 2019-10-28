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
        /// COMMENT symbol
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// COMMENT Text
        /// </summary>
        public string Text { get; }

        public CommentToolCommand(CommandToolParser.CommentCommandContext context)
        {
            var type = ProcessId(context.LITERAL().GetText(), out var number, out var symbol);
            if (HasSemanticError) return;

            if (type)
            {
                Symbol = symbol;
            }
            else
            {
                Address = number;
            }
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