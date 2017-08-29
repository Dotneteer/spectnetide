using AntlrZ80Asm.SyntaxTree;

namespace AntlrZ80Asm.Assembler
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