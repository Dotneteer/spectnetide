using AntlrZ80Asm.SyntaxTree;

namespace AntlrZ80Asm.Assembler
{
    /// <summary>
    /// This class represents a syntax error
    /// </summary>
    public class SyntaxError : AssemblerErrorInfoBase
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