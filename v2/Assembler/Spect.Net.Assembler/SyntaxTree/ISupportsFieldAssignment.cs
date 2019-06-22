namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This interface signs that a syntax node can be used within a field assignment
    /// </summary>
    public interface ISupportsFieldAssignment
    {
        /// <summary>
        /// True indicates that this node is used within a field assignment
        /// </summary>
        bool IsFieldAssignment { get; set; }
    }
}