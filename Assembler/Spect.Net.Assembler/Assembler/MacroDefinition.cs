using System.Collections.Generic;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents the definition of a macro
    /// </summary>
    public class MacroDefinition
    {
        /// <summary>
        /// The name of the macro
        /// </summary>
        public string MacroName { get; }

        /// <summary>
        /// Macro definition section
        /// </summary>
        public DefinitionSection Section { get; }

        /// <summary>
        /// Macro argument names
        /// </summary>
        public List<string> ArgumentNames { get; }

        /// <summary>
        /// Optional end label of the macro
        /// </summary>
        public string EndLabel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public MacroDefinition(string macroName, int macroDefLine, int macroEndLine, List<string> argNames, string endLabel)
        {
            MacroName = macroName;
            Section = new DefinitionSection(macroDefLine, macroEndLine);
            ArgumentNames = argNames;
            EndLabel = endLabel;
        }
    }
}