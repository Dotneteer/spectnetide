using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents an EXPORT DISASSEMBLY tool command
    /// </summary>
    public class ExportDisassemblyToolCommand : ToolCommandNode
    {
        /// <summary>
        /// FROM index
        /// </summary>
        public ushort From { get; }

        /// <summary>
        /// TO index
        /// </summary>
        public ushort To { get; }

        public ExportDisassemblyToolCommand(CommandToolParser.ExportDisassemblyCommandContext context)
        {
            if (context.LITERAL().Length < 2) return;
            From = ProcessNumber(context.LITERAL()[0].GetText());
            To = ProcessNumber(context.LITERAL()[1].GetText());
        }
    }
}