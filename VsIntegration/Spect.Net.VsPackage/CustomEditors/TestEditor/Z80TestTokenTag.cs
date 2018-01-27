using Microsoft.VisualStudio.Text.Tagging;

namespace Spect.Net.VsPackage.CustomEditors.TestEditor
{
    /// <summary>
    /// This class defines the a token tag used in Z80 test language
    /// </summary>
    public class Z80TestTokenTag : ITag
    {
        /// <summary>
        /// The type of the token
        /// </summary>
        public Z80TestTokenType Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Z80TestTokenTag(Z80TestTokenType type)
        {
            Type = type;
        }
    }

    /// <summary>
    /// The available token types in Z80 test language
    /// </summary>
    public enum Z80TestTokenType
    {
        None,
        Keyword,
        Comment,
        Number,
        Identifier,
        Z80Key,
        Breakpoint,
        CurrentBreakpoint
    }
}