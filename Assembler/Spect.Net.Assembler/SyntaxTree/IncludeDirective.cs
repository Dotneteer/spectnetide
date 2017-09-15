namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents an #include preprocessor directive
    /// </summary>
    public sealed class IncludeDirective : OperationBase
    {
        /// <summary>
        /// Include file name
        /// </summary>
        public string Filename { get; set; }
    }
}