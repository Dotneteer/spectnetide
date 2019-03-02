namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// Objects implementing this interface have usage information
    /// </summary>
    public interface IHasUsageInfo
    {
        /// <summary>
        /// Signs if the symbol has been used
        /// </summary>
        bool IsUsed { get; set; }
    }
}