using System.ComponentModel;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

// ReSharper disable ConvertToAutoProperty

namespace Spect.Net.Wpf.Providers
{
    /// <summary>
    /// This class renders the Spectrum screen into a WriteableBitmap
    /// </summary>
    public class WriteableBitmapRenderer : IFrameRenderer
    {
        private readonly BackgroundWorker _worker;
        private readonly int _frames;

        private byte[] _currentBuffer;

        /// <summary>
        /// The current screen buffer
        /// </summary>
        public byte[] GetCurrentBuffer() => _currentBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public WriteableBitmapRenderer(BackgroundWorker worker)
        {
            _worker = worker;
            _frames = 0;
            Reset();
        }

        #region Implementation of IFrameRenderer

        /// <summary>
        /// Resets the renderer
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
        public void DisplayFrame(byte[] frame)
        {
            _currentBuffer = frame;
            _worker.ReportProgress(_frames + 1);
        }

        #endregion
    }
}