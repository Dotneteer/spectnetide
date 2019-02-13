using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a LITERAL tool command
    /// </summary>
    public class LiteralToolCommand : ToolCommandNode
    {
        /// <summary>
        /// LITERAL address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// LITERAL symbol
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Associated symbol
        /// </summary>
        public string LiteralName { get; }

        /// <summary>
        /// Is automatic symbol?
        /// </summary>
        public bool IsAuto { get; }

        public LiteralToolCommand(CommandToolParser.LiteralCommandContext context)
        {
            if (context.LITERAL().Length < 1) return;
            var type = ProcessId(context.LITERAL()[0].GetText(), out var number, out var symbol);
            if (HasSemanticError) return;

            if (type)
            {
                Symbol = symbol;
            }
            else
            {
                Address = number;
            }

            if (context.HASH() != null)
            {
                IsAuto = true;
            }
            if (context.LITERAL().Length < 2) return;

            var idText = context.LITERAL()[1].GetText();
            if (char.IsDigit(idText[0]))
            {
                HasSemanticError = true;
                return;
            }
            LiteralName = idText;
        }
    }
}