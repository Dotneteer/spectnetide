using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a semantic error
    /// </summary>
    public abstract class SemanticErrorBase : AssemblerErrorInfoBase
    {
        protected SemanticErrorBase(string errorCode, SourceLineBase line, params object[] parameters)
        {
            ErrorCode = errorCode;
            Line = line.SourceLine;
            Column = line.Position;
            Message = ErrorMessage.GetMessage(ErrorCode, parameters);
        }
    }
}