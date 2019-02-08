using System.Globalization;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a REMOVE BREAKPOINT tool command
    /// </summary>
    public class RemoveBreakpointToolCommand : ToolCommandNode
    {
        /// <summary>
        /// GOTO address
        /// </summary>
        public ushort Address { get; }

        public RemoveBreakpointToolCommand(CommandToolParser.RemoveBreakpointCommandContext context)
        {
            if (context.HEXNUM() != null)
            {
                Address = ushort.Parse(context.HEXNUM().GetText(), NumberStyles.HexNumber);
            }
        }
    }
}