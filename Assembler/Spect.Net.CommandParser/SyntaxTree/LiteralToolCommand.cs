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
        /// Associated symbol
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Is automatic symbol?
        /// </summary>
        public bool IsAuto { get; }

        public LiteralToolCommand(CommandToolParser.LiteralCommandContext context)
        {
            if (context.LITERAL().Length < 1) return;
            Address = ProcessNumber(context.LITERAL()[0].GetText());
            if (HasSemanticError) return;

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
            Symbol = idText;

        }
    }
}