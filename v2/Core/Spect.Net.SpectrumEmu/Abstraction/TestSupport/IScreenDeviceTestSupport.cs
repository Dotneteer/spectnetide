namespace Spect.Net.SpectrumEmu.Abstraction.TestSupport
{
    /// <summary>
    /// This interface defines the operations that support 
    /// the testing of a screen device.
    /// </summary>
    public interface IScreenDeviceTestSupport
    {
        /// <summary>
        /// Fills the entire screen buffer with the specified data.
        /// </summary>
        /// <param name="data">Data to fill the pixel buffer.</param>
        void FillScreenBuffer(byte data);
    }
}