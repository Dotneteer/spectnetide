using Spect.Net.CommandParser.Generated;
// ReSharper disable IdentifierTypo

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