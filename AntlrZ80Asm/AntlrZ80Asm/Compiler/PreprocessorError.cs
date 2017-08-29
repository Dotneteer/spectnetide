using AntlrZ80Asm.SyntaxTree;

namespace AntlrZ80Asm.Compiler
{
    /// <summary>
    /// This class represents a preprocessor error
    /// </summary>
    public class PreprocessorError : SemanticErrorBase
    {
        public PreprocessorError(OperationBase directive, string message) : 
            base(directive.SourceLine, directive.Position, directive.Mnemonic, message)
        {
        }
    }
}