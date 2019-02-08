using System.Globalization;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a RETRIEVE tool command
    /// </summary>
    public class RetrieveToolCommand : ToolCommandNode
    {
        /// <summary>
        /// RETRIEVE address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// GOTO symbol
        /// </summary>
        public string Type { get; }

        public RetrieveToolCommand(CommandToolParser.RetrieveCommandContext context)
        {
            if (context.HEXNUM() != null)
            {
                Address = ushort.Parse(context.HEXNUM().GetText(), NumberStyles.HexNumber);
            }
            Type = context.GetChild(0).GetText().Substring(1).ToUpper();
        }
    }
}