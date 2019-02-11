using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a REMOVE BREAKPOINT tool command
    /// </summary>
    public class RemoveBreakpointToolCommand : ToolCommandNode
    {
        /// <summary>
        /// BREAKPOINT Address
        /// </summary>
        public ushort Address { get; }

        public RemoveBreakpointToolCommand(CommandToolParser.RemoveBreakpointCommandContext context)
        {
            if (context.LITERAL() == null) return;
            Address = ProcessNumber(context.LITERAL().GetText());
        }
    }
}