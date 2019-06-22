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
        /// Source file in which the error occurred
        /// </summary>
        public string Filename { get; }

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

        public AssemblerErrorInfo(SourceFileItem sourceItem, Z80AsmParserErrorInfo syntaxErrorInfo)
        {
            var token = syntaxErrorInfo.Token.Trim();
            ErrorCode = token.Length == 0
                ? Errors.Z0101
                : Errors.Z0100;
            Line = syntaxErrorInfo.SourceLine;
            Column = syntaxErrorInfo.Position;
            Message = Errors.GetMessage(ErrorCode, token);
            Filename = sourceItem.Filename;
        }

        public AssemblerErrorInfo(SourceFileItem sourceItem, string errorCode, SourceLineBase line, params object[] parameters)
        {
            ErrorCode = errorCode;
            Line = line?.SourceLine ?? 0;
            Column = line?.FirstColumn ?? 0;
            Message = Errors.GetMessage(ErrorCode, parameters);
            Filename = sourceItem?.Filename;
        }
    }
}