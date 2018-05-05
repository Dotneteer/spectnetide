using System.Collections.Generic;
using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// Represents the definition of an IF section to process
    /// </summary>
    public class IfDefinition
    {
        /// <summary>
        /// The entire IF section
        /// </summary>
        public DefinitionSection FullSection { get; set; }

        /// <summary>
        /// List of if sections 
        /// </summary>
        public List<IfSection> IfSections { get; } = new List<IfSection>();

        /// <summary>
        /// Optional else section
        /// </summary>
        public IfSection ElseSection { get; set; } = null;
    }

    /// <summary>
    /// Respresents a section of the If definition
    /// </summary>
    public class IfSection
    {
        /// <summary>
        /// The statement of the section
        /// </summary>
        public StatementBase IfStatement { get; }

        /// <summary>
        /// The section boundaries
        /// </summary>
        public DefinitionSection Section { get; }

        public IfSection(StatementBase stmt, int firstLine, int lastLine)
        {
            IfStatement = stmt;
            Section = new DefinitionSection(firstLine, lastLine);
        }
    }
}