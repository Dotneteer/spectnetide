﻿namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This interface represents a renderer that can display a
    /// pixel in a virtual screen device
    /// </summary>
    public interface IScreenFrameProvider: IVmComponentProvider
    {
        /// <summary>
        /// The ULA signs that it's time to start a new frame
        /// </summary>
        void StartNewFrame();

        /// <summary>
        /// Signs that the current frame is rendered and ready to be displayed
        /// </summary>
        /// <param name="frame">The buffer that contains the frame to display</param>
        void DisplayFrame(byte[] frame);
    }
}