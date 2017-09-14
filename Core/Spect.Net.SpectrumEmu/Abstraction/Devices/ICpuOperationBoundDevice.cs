namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This device is bound to a rendering frame of the Spectrum virtual machine
    /// </summary>
    public interface ICpuOperationBoundDevice : IDevice
    {
        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        void OnCpuOperationCompleted();
    }
}