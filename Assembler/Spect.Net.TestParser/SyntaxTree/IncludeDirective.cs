namespace Spect.Net.TestParser.SyntaxTree
{
    /// <summary>
    /// Represents an include directive
    /// </summary>
    public class IncludeDirective : LanguageBlockBase
    {
        /// <summary>
        /// The 'include' span
        /// </summary>
        public TextSpan IncludeKeywordSpan { get; set; }

        /// <summary>
        /// Span of the string
        /// </summary>
        public TextSpan StringSpan { get; set; }

        /// <summary>
        /// Include file name
        /// </summary>
        public string Filename { get; set; }
    }
}