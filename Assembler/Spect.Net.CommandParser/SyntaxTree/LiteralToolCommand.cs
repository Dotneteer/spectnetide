using System.Globalization;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a LITERAL tool command
    /// </summary>
    public class LiteralToolCommand : ToolCommandNode
    {
        /// <summary>
        /// LITERAL address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// Associated symbol
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Is automatic symbol?
        /// </summary>
        public bool IsAuto { get; }

        public LiteralToolCommand(CommandToolParser.LiteralCommandContext context)
        {
            if (context.HEXNUM() != null)
            {
                Address = ushort.Parse(context.HEXNUM().GetText(), NumberStyles.HexNumber);
            }

            if (context.IDENTIFIER() != null)
            {
                Symbol = context.IDENTIFIER().GetText().ToUpper();
            }

            if (context.HASH() != null)
            {
                IsAuto = true;
            }
        }
    }
}