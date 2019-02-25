namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a structure
    /// </summary>
    public class StructDefinition
    {
        /// <summary>
        /// The name of the macro
        /// </summary>
        public string StructName { get; }

        /// <summary>
        /// Macro definition section
        /// </summary>
        public DefinitionSection Section { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public StructDefinition(string structName, int macroDefLine, int macroEndLine)
        {
            StructName = structName;
            Section = new DefinitionSection(macroDefLine, macroEndLine);
        }
    }
}