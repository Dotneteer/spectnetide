using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a UPDATE BREAKPOINT tool command
    /// </summary>
    public class UpdateBreakpointToolCommand : ToolCommandNode
    {
        /// <summary>
        /// BREAKPOINT address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// BREAKPOINT symbol
        /// </summary>
        public string Symbol { get; }

        public UpdateBreakpointToolCommand(CommandToolParser.UpdateBreakpointCommandContext context)
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