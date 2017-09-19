using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a compilation error
    /// </summary>
    public class AssemblerErrorInfo
    {
        /// <summary>
        /// Error code
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Source line of the error
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Position within the source line
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; }

        public AssemblerErrorInfo(Z80AsmParserErrorInfo syntaxErrorInfo)
        {
            var token = syntaxErrorInfo.Token.Trim();
            ErrorCode = token.Length == 0 
                ? Errors.Z0101 
                : Errors.Z0100;
            Line = syntaxErrorInfo.SourceLine;
            Column = syntaxErrorInfo.Position;
            Message = Errors.GetMessage(ErrorCode, token);
        }

        public AssemblerErrorInfo(string errorCode, SourceLineBase line, params object[] parameters)
        {
            ErrorCode = errorCode;
            Line = line.SourceLine;
            Column = line.Position;
            Message = Errors.GetMessage(ErrorCode, parameters);
        }
    }
}