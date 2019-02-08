using Antlr4.Runtime.Tree;

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

        public BankPageToolCommand(IParseTree context)
        {
            if (context.ChildCount >= 2)
            {
                Page = ushort.Parse(context.GetChild(2).GetText());
            }
        }
    }
}