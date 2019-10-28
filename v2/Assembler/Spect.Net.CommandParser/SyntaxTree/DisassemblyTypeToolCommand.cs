using System.Text;
using Antlr4.Runtime.Tree;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a DISASSEMBLY TYPE tool command
    /// </summary>
    public class DisassemblyTypeToolCommand : ToolCommandNode
    {
        /// <summary>
        /// Type identifier
        /// </summary>
        public string Type { get; }

        public DisassemblyTypeToolCommand(IParseTree context)
        {
            if (context.ChildCount <= 2) return;
            var sb = new StringBuilder();
            for (var i = 2; i < context.ChildCount; i++)
            {
                sb.Append(context.GetChild(i).GetText());
            }
            Type = sb.ToString().ToUpper();
        }
    }
}