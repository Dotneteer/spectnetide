using Spect.Net.CommandParser.Generated;
// ReSharper disable IdentifierTypo

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
            if (context.LITERAL() == null) return;
            var type = ProcessId(context.LITERAL().GetText(), out var hexnum, out var symbol);
            if (HasSemanticError) return;

            if (type)
            {
                Symbol = symbol;
            }
            else
            {
                Address = hexnum;
            }
        }
    }
}