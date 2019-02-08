using System.Globalization;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a JUMP tool command
    /// </summary>
    public class JumpToolCommand : ToolCommandNode
    {
        /// <summary>
        /// JUMP address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// JUMP symbol
        /// </summary>
        public string Symbol { get; }

        public JumpToolCommand(CommandToolParser.JumpCommandContext context)
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