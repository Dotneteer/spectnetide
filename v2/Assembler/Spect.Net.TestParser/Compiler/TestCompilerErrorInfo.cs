using Spect.Net.TestParser.SyntaxTree;

namespace Spect.Net.TestParser.Compiler
{
    /// <summary>
    /// This class represents a compilation error
    /// </summary>
    public class TestCompilerErrorInfo
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

        public TestCompilerErrorInfo(string filename, Z80TestParserErrorInfo syntaxErrorInfo)
        {
            var token = syntaxErrorInfo.Token.Trim();
            ErrorCode = Errors.T0001;
            Line = syntaxErrorInfo.SourceLine;
            Column = syntaxErrorInfo.Position;
            Message = Errors.GetMessage(ErrorCode, token);
            Filename = filename;
        }

        public TestCompilerErrorInfo(string filename, string errorCode, int line, int column, params object[] parameters)
        {
            ErrorCode = errorCode;
            Line = line;
            Column = column;
            Message = Errors.GetMessage(ErrorCode, parameters);
            Filename = filename;
        }
    }
}