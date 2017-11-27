using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// It is time to refresh the view of the memory tool window
    /// </summary>
    public class RefreshMemoryViewMessage: MessageBase
    {
        /// <summary>
        /// Optional ddress to navigate to
        /// </summary>
        public ushort? Address { get; }

        /// <summary>Initializes a new instance of the MessageBase class.</summary>
        public RefreshMemoryViewMessage(ushort? address = null)
        {
            Address = address;
        }
    }
}