using System.Globalization;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a TOGGLE BREAKPOINT tool command
    /// </summary>
    public class ToggleBreakpointToolCommand : ToolCommandNode
    {
        /// <summary>
        /// GOTO address
        /// </summary>
        public ushort Address { get; }

        public ToggleBreakpointToolCommand(CommandToolParser.ToggleBreakpointCommandContext context)
        {
            if (context.HEXNUM() != null)
            {
                Address = ushort.Parse(context.HEXNUM().GetText(), NumberStyles.HexNumber);
            }
        }
    }
}