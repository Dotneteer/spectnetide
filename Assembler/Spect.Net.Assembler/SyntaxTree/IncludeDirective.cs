using Spect.Net.Assembler.Generated;

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

        public IncludeDirective(IZ80AsmVisitorContext visitorContext, Z80AsmParser.DirectiveContext context):
            base(context)
        {
            if (context.STRING() != null)
            {
                visitorContext.AddString(context.STRING());
            }
            Filename = context.GetChild(1).NormalizeString();
        }
    }
}