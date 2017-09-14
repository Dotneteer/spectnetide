namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// This message signs that the disassembly view should be refreshed.
    /// </summary>
    public class RefreshDisassemblyViewMessage
    {
        /// <summary>
        /// Address to navigate to
        /// </summary>
        public ushort? Address { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public RefreshDisassemblyViewMessage(ushort? address = null)
        {
            Address = address;
        }
    }
}