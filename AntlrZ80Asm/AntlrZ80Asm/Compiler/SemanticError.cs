namespace AntlrZ80Asm.Compiler
{
    /// <summary>
    /// This class represents a semantic error
    /// </summary>
    public abstract class SemanticErrorBase : CompilerErrorInfoBase
    {
        protected SemanticErrorBase(int line, int position, string problematic, string message)
        {
            SourceLine = line;
            Position = position;
            ProblematicCode = problematic;
            Message = message;
        }
    }
}