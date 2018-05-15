namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents an instruction
    /// </summary>
    public sealed class MacroParamLine : SourceLineBase
    {
        /// <summary>
        /// Identifier of the macro parameter
        /// </summary>
        public string Identifier { get; }

        public MacroParamLine(string identifier)
        {
            Identifier = identifier;
        }
    }
}