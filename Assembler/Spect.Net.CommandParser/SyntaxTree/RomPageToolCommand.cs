using Antlr4.Runtime.Tree;

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

        public RomPageToolCommand(IParseTree context)
        {
            if (context.ChildCount >= 2)
            {
                Page = ushort.Parse(context.GetChild(2).GetText());
            }
        }
    }
}