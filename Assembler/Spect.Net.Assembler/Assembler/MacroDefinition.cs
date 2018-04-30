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
        /// The index of the line that defines the macro
        /// </summary>
        public int MacroDefLine { get; }

        /// <summary>
        /// The index of the line that signs the end of the macro
        /// </summary>
        public int MacroEndLine { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public MacroDefinition(string macroName, int macroDefLine, int macroEndLine)
        {
            MacroName = macroName;
            MacroDefLine = macroDefLine;
            MacroEndLine = macroEndLine;
        }
    }
}