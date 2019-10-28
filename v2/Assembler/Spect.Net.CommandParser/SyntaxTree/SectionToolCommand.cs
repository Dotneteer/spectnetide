using Spect.Net.CommandParser.Generated;
// ReSharper disable IdentifierTypo

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a LABEL tool command
    /// </summary>
    public class SectionToolCommand : ToolCommandNode
    {
        /// <summary>
        /// Start address
        /// </summary>
        public ushort StartAddress { get; }

        /// <summary>
        /// Start symbol
        /// </summary>
        public string StartSymbol { get; }

        /// <summary>
        /// End address
        /// </summary>
        public ushort EndAddress { get; }

        /// <summary>
        /// End symbol
        /// </summary>
        public string EndSymbol { get; }

        /// <summary>
        /// Section type
        /// </summary>
        public string Type { get; }

        public SectionToolCommand(CommandToolParser.SectionCommandContext context)
        {
            Type = context.GetChild(0).GetText().Substring(1).ToUpper();
            if (context.LITERAL().Length < 1) return;
            var type = ProcessId(context.LITERAL()[0].GetText(), out var hexnum, out var symbol);
            if (HasSemanticError) return;

            if (type)
            {
                StartSymbol = symbol;
            }
            else
            {
                StartAddress = hexnum;
            }

            if (context.LITERAL().Length < 2) return;
            type = ProcessId(context.LITERAL()[1].GetText(), out hexnum, out symbol);
            if (HasSemanticError) return;

            if (type)
            {
                EndSymbol = symbol;
            }
            else
            {
                EndAddress = hexnum;
            }
        }
    }
}