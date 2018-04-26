using Microsoft.VisualStudio.Text.Tagging;

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    /// <summary>
    /// This class defines the a token tag used in Z80 assembly
    /// </summary>
    public class Z80AsmTokenTag : ITag
    {
        /// <summary>
        /// The type of the token
        /// </summary>
        public Z80AsmTokenType Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Z80AsmTokenTag(Z80AsmTokenType type)
        {
            Type = type;
        }
    }

    /// <summary>
    /// The available token types in Z80 assembly
    /// </summary>
    public enum Z80AsmTokenType
    {
        None,
        Label,
        Pragma,
        Directive,
        Include,
        Instruction,
        Comment,
        Number,
        Identifier,
        Breakpoint,
        CurrentBreakpoint,
        String,
        Function
    }
}