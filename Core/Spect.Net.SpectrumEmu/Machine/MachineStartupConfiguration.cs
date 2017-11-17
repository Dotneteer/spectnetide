using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class holds a collection of properties that describe
    /// the startup configuration of a Spectrum virtual machine
    /// </summary>
    public class MachineStartupConfiguration
    {
        /// <summary>
        /// Device information to use for machine startup
        /// </summary>
        public DeviceInfoCollection DeviceData { get; set; }

        /// <summary>
        /// Provider to manage debug information
        /// </summary>
        public ISpectrumDebugInfoProvider DebugInfoProvider { get; set; }

        /// <summary>
        /// Stack debug provider
        /// </summary>
        public IStackDebugSupport StackDebugSupport { get; set; }
    }
}