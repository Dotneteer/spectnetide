using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// This message signs that a new TZX block has been selected in the TZX Explorer
    /// </summary>
    public class TzxBlockSelectedMessage: MessageBase
    {
        /// <summary>
        /// The selected TZX block
        /// </summary>
        public TapeBlockViewModelBase Block { get; }

        /// <summary>Initializes a new instance of the MessageBase class.</summary>
        public TzxBlockSelectedMessage(TapeFileViewModel sender, TapeBlockViewModelBase block)
        {
            Sender = sender;
            Block = block;
        }
    }
}