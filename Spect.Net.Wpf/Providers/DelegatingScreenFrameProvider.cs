using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.Wpf.Providers
{
    public class DelegatingScreenFrameProvider: IScreenFrameProvider
    {
        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// The ULA signs that it's time to start a new frame
        /// </summary>
        public void StartNewFrame()
        {
        }

        /// <summary>
        /// Signs that the current frame is rendered and ready to be displayed
        /// </summary>
        /// <param name="frame">The buffer that contains the frame to display</param>
        public void DisplayFrame(byte[] frame)
        {
            Messenger.Default.Send(new DisplayFrameMessage(frame));
        }

        /// <summary>
        /// This message signs that it is time to render the buffer
        /// </summary>
        public class DisplayFrameMessage : MessageBase
        {
            public byte[] Buffer { get; }

            /// <summary>Initializes a new instance of the MessageBase class.</summary>
            public DisplayFrameMessage(byte[] buffer)
            {
                Buffer = buffer;
            }
        }
    }
}