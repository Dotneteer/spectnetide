using Spect.Net.CommandParser.Generated;
// ReSharper disable IdentifierTypo

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a SELECT ROM PAGE tool command
    /// </summary>
    public class BankPageToolCommand : ToolCommandNode
    {
        /// <summary>
        /// Selected ROM page number
        /// </summary>
        public ushort Page { get; }

        public BankPageToolCommand(CommandToolParser.BankPageCommandContext context)
        {
            if (context.LITERAL() == null) return;
            Page = ProcessNumber(context.LITERAL().GetText());
        }
    }
}