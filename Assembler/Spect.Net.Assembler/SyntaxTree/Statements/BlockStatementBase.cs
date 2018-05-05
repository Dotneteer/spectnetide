using System;
using System.Collections.Generic;
using Spect.Net.Assembler.Assembler;

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
        /// <param name="asm">Assembler instance</param>
        /// <param name="lines"></param>
        /// <param name="currentLineIndex"></param>
        /// <param name="endLabel">Optional end label</param>
        /// <returns></returns>
        public bool SearchForEnd(Z80Assembler asm, List<SourceLineBase> lines, ref int currentLineIndex, out string endLabel)
        {
            endLabel = null;
            if (currentLineIndex >= lines.Count)
            {
                return false;
            }

            // --- Store the start line for error reference
            var startLine = lines[currentLineIndex];
            currentLineIndex++;

            // --- Iterate through lines
            while (currentLineIndex < lines.Count)
            {
                var curLine = lines[currentLineIndex];
                if (curLine.GetType() == EndType)
                {
                    // --- We have found the end line, get its label
                    endLabel = curLine.Label ?? endLabel;
                    return true;
                }

                if (curLine is NoInstructionLine noinstrLine)
                {
                    // --- Record the last hanging label
                    endLabel = noinstrLine.Label;
                }
                else
                {
                    endLabel = null;
                    if (curLine is BlockStatementBase blockStmt)
                    {
                        var success = blockStmt.SearchForEnd(asm, lines, ref currentLineIndex, out endLabel);
                        if (!success)
                        {
                            asm.ReportError(Errors.Z0401, startLine, blockStmt.EndStatementName);
                            return false;
                        }
                    }
                }
                currentLineIndex++;
            }
            asm.ReportError(Errors.Z0401, startLine, EndStatementName);
            return false;
        }
    }
}