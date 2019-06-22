using Antlr4.Runtime.Tree;
using Spect.Net.CommandParser.Generated;
// ReSharper disable IdentifierTypo

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a SELECT ROM PAGE tool command
    /// </summary>
    public class RomPageToolCommand : ToolCommandNode
    {
        /// <summary>
        /// Selected ROM page number
        /// </summary>
        public ushort Page { get; }

        public RomPageToolCommand(CommandToolParser.RomPageCommandContext context)
        {
            if (context.LITERAL() == null) return;
            Page = ProcessNumber(context.LITERAL().GetText());
        }
    }
}