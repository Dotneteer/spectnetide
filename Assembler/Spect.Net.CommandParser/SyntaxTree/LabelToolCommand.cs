using System.Globalization;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a LABEL tool command
    /// </summary>
    public class LabelToolCommand : ToolCommandNode
    {
        /// <summary>
        /// GOTO address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// GOTO symbol
        /// </summary>
        public string Symbol { get; }

        public LabelToolCommand(CommandToolParser.LabelCommandContext context)
        {
            if (context.HEXNUM() != null)
            {
                Address = ushort.Parse(context.HEXNUM().GetText(), NumberStyles.HexNumber);
            }

            if (context.IDENTIFIER() != null)
            {
                Symbol = context.IDENTIFIER().GetText();
            }
        }
    }
}