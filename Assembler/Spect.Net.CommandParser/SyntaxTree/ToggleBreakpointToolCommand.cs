using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a TOGGLE BREAKPOINT tool command
    /// </summary>
    public class ToggleBreakpointToolCommand : ToolCommandNode
    {
        /// <summary>
        /// TOGGLE BREAKPOINT address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// TOGGLE BREAKPOINT symbol
        /// </summary>
        public string Symbol { get; }

        public ToggleBreakpointToolCommand(CommandToolParser.ToggleBreakpointCommandContext context)
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