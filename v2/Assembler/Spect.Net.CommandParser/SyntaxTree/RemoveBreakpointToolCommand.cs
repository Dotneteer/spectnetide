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

        /// <summary>
        /// BREAKPOINT symbol
        /// </summary>
        public string Symbol { get; }

        public RemoveBreakpointToolCommand(CommandToolParser.RemoveBreakpointCommandContext context)
        {
            if (context.LITERAL() == null) return;
            var type = ProcessId(context.LITERAL().GetText(), out var number, out var symbol);
            if (HasSemanticError) return;

            if (type)
            {
                Symbol = symbol;
            }
            else
            {
                Address = number;
            }
        }
    }
}