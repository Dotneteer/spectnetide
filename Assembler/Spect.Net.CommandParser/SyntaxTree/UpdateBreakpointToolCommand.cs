using System.Globalization;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a UPDATE BREAKPOINT tool command
    /// </summary>
    public class UpdateBreakpointToolCommand : ToolCommandNode
    {
        /// <summary>
        /// GOTO address
        /// </summary>
        public ushort Address { get; }

        public UpdateBreakpointToolCommand(CommandToolParser.UpdateBreakpointCommandContext context)
        {
            if (context.LITERAL() == null) return;
            Address = ProcessNumber(context.LITERAL().GetText());
        }
    }
}