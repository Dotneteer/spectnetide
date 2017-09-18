using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a syntax error
    /// </summary>
    public class SyntaxError : AssemblerErrorInfoBase
    {
        public SyntaxError(Z80AsmParserErrorInfo syntaxErrorInfo)
        {
            var token = syntaxErrorInfo.Token.Trim();
            ErrorCode = token.Length == 0 ? "Z0101" : "Z0100";
            Line = syntaxErrorInfo.SourceLine;
            Column = syntaxErrorInfo.Position;
            Message = ErrorMessage.GetMessage(ErrorCode, token);
        }
    }
}