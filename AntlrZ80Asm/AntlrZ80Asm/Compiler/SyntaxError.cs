using AntlrZ80Asm.SyntaxTree;

namespace AntlrZ80Asm.Compiler
{
    /// <summary>
    /// This class represents a syntax error
    /// </summary>
    public class SyntaxError : CompilerErrorInfoBase
    {
        public SyntaxError(Z80AsmParserErrorInfo syntaxErrorInfo)
        {
            SourceLine = syntaxErrorInfo.SourceLine;
            Position = syntaxErrorInfo.Position;
            ProblematicCode = syntaxErrorInfo.Token;
            Message = "Syntax Error";
        }
    }
}