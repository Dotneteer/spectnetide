using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a preprocessor error
    /// </summary>
    public class DirectiveError : SemanticErrorBase
    {
        public DirectiveError(OperationBase directive, string message) : 
            base(directive.SourceLine, directive.Position, directive.Mnemonic, message)
        {
        }
    }
}