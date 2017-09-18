namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a compilation error
    /// </summary>
    public abstract class AssemblerErrorInfoBase
    {
        /// <summary>
        /// Error code
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Source line of the error
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Position within the source line
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; protected set; }
    }
}