using System;
using System.Collections.Generic;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// Represents a block statement with the specified end statement
    /// </summary>
    public abstract class BlockStatementBase : StatementBase
    {
        /// <summary>
        /// Type of end statement
        /// </summary>
        public abstract Type EndType { get; }

        /// <summary>
        /// Gets the name of the end statement
        /// </summary>
        public abstract string EndStatementName { get; }

        /// <summary>
        /// Searches the assembly lines for the end of the block
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="currentLineIndex"></param>
        /// <returns></returns>
        public bool SearchForEnd(List<SourceLineBase> lines, ref int currentLineIndex)
        {
            currentLineIndex++;
            while (currentLineIndex < lines.Count)
            {
                var curLine = lines[currentLineIndex];
                if (curLine.GetType() == EndType)
                {
                    // --- We have found the end line
                    return true;
                }
                if (curLine is BlockStatementBase blockStmt)
                {
                    var success = blockStmt.SearchForEnd(lines, ref currentLineIndex);
                    if (!success)
                    {
                        // TODO: Report issue
                        return false;
                    }
                }
                currentLineIndex++;
            }
            return false;
        }
    }
}