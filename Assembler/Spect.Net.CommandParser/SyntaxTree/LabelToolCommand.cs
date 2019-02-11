using Spect.Net.CommandParser.Generated;
// ReSharper disable IdentifierTypo

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a LABEL tool command
    /// </summary>
    public class LabelToolCommand : ToolCommandNode
    {
        /// <summary>
        /// LABEL address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// LABEL symbol
        /// </summary>
        public string Symbol { get; }

        public LabelToolCommand(CommandToolParser.LabelCommandContext context)
        {
            if (context.LITERAL().Length < 1) return;
            Address = ProcessNumber(context.LITERAL()[0].GetText());
            if (HasSemanticError) return;

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