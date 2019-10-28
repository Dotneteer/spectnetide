using System;
using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFG pragma
    /// </summary>
    public sealed class DefgPragma : PragmaBase, ISupportsFieldAssignment
    {
        /// <summary>
        /// The DEFG pattern value
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// True indicates that this node is used within a field assignment
        /// </summary>
        public bool IsFieldAssignment { get; set; }

        public DefgPragma(Z80AsmParser.DefgPragmaContext context)
        {
            if (context.DGPRAG() == null) return;

            var text = context.DGPRAG().GetText();
            // --- Cut off the pragma token
            var firstSpace = text.IndexOf("\u0020", StringComparison.Ordinal);
            var firstTab = text.IndexOf("\t", StringComparison.Ordinal);
            if (firstSpace < 0 && firstTab < 0)
            {
                // --- This should not happen according to the grammar definition of DGPRAG
                return;
            }
            if (firstTab > 0 && firstTab < firstSpace)
            {
                firstSpace = firstTab;
            }

            // --- Cut off the comment
            var commentIndex = text.IndexOf(";", StringComparison.Ordinal);
            if (commentIndex > 0)
            {
                text = text.Substring(0, commentIndex);
            }
            Pattern = text.Substring(firstSpace + 1).TrimStart();
        }
    }
}