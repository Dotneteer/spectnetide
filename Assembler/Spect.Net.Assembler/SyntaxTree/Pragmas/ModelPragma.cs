namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the MODEL pragma
    /// </summary>
    public sealed class ModelPragma : PragmaBase
    {
        /// <summary>
        /// The Spectrum model to use
        /// </summary>
        public string Model { get; set; }
    }
}