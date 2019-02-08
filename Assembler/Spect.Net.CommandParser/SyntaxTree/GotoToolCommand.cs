using System.Globalization;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a GOTO tool command
    /// </summary>
    public class GotoToolCommand : ToolCommandNode
    {
        /// <summary>
        /// GOTO address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// GOTO symbol
        /// </summary>
        public string Symbol { get; }

        public GotoToolCommand(CommandToolParser.GotoCommandContext context)
        {
            if (context.HEXNUM() != null)
            {
                Address = ushort.Parse(context.HEXNUM().GetText(), NumberStyles.HexNumber);
            }
            else if (context.IDENTIFIER() != null)
            {
                Symbol = context.IDENTIFIER().GetText();
            }
        }
    }
}