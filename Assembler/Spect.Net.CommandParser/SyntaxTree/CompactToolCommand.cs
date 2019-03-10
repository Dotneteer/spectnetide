using System.Text;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a compact tool command to be rp-parsed
    /// </summary>
    public class CompactToolCommand : ToolCommandNode
    {
        private static readonly string[] s_CommandPrefixes =
        {
            "G", "R", "B", "LW", "L", "C", "D", "P", "J",
            "MB", "MD", "MW", "MS", "MC",
            "MG", "MG1", "MG2", "MG3", "MG4",
            "SB", "TB", "T", "RB", "UB", "RL", "RC", "RB", "RP", "XW", "XD"
        };

        /// <summary>
        /// Command texts after adding separator
        /// </summary>
        public string CommandText { get; }

        public CompactToolCommand(CommandToolParser.CompactCommandContext context)
        {
            if (context.LITERAL() == null) return;
            CommandText = ConvertToCommand(context, s_CommandPrefixes, context.LITERAL().GetText());
        }

        private string ConvertToCommand(CommandToolParser.CompactCommandContext context, string[] prefixList, string text)
        {
            foreach (var prefix in prefixList)
            {
                if (!text.ToUpper().StartsWith(prefix.ToUpper())) continue;
                var argChar = text[prefix.Length];
                if (argChar != '+' && argChar != ':' && !char.IsDigit(argChar)) continue;
                var sb = new StringBuilder($"{prefix} {text.Substring(prefix.Length)}");
                for (var i = 1; i < context.ChildCount; i++)
                {
                    sb.Append(context.GetChild(i).GetText());
                }
                return sb.ToString();
            }
            HasSemanticError = true;
            return text;
        }
    }
}