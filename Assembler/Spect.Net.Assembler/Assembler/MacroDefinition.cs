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

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public MacroDefinition(string macroName, int macroDefLine, int macroEndLine)
        {
            MacroName = macroName;
            Section = new DefinitionSection(macroDefLine, macroEndLine);
        }
    }
}