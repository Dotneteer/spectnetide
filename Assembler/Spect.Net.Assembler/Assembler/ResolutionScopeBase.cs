using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class is intended to the base of a resolution scope for compiler statements
    /// </summary>
    public abstract class ResolutionScopeBase
    {
        /// <summary>
        /// The statement that starts the resolution scope
        /// </summary>
        public StatementBase StartStatement { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected ResolutionScopeBase(StatementBase startStatement)
        {
            StartStatement = startStatement;
        }
    }
}