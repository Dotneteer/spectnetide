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
        /// RETRIEVE symbol
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// RETRIEVE Type
        /// </summary>
        public string Type { get; }

        public RetrieveToolCommand(CommandToolParser.RetrieveCommandContext context)
        {
            Type = context.GetChild(0).GetText().Substring(1).ToUpper();
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