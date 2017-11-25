using System.Threading;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;


namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents a Spectrum virtual machine
    /// </summary>
    public interface ISpectrumVm : IFrameBoundDevice
    {
        /// <summary>
        /// The Z80 CPU of the machine
        /// </summary>
        IZ80Cpu Cpu { get; }

        /// <summary>
        /// Gets the frequency of the virtual machine's clock in Hz
        /// </summary>
        int BaseClockFrequency { get; }

        /// <summary>
        /// #of tacts within the frame
        /// </summary>
        int FrameTacts { get; }

        /// <summary>
        /// The current tact within the frame
        /// </summary>
        int CurrentFrameTact { get; }

        /// <summary>
        /// The length of the physical frame in clock counts
        /// </summary>
        double PhysicalFrameClockCount { get; }

        /// <summary>
        /// The number of frame tact at which the interrupt signal is generated
        /// </summary>
        int InterruptTact { get; }

        /// <summary>
        /// The current execution cycle options
        /// </summary>
        ExecuteCycleOptions ExecuteCycleOptions { get; }

        /// <summary>
        /// The memory device used by the virtual machine
        /// </summary>
        IMemoryDevice MemoryDevice { get; }

        /// <summary>
        /// The port device used by the virtual machine
        /// </summary>
        IPortDevice PortDevice { get; }

        /// <summary>
        /// The ULA device that renders the VM screen
        /// </summary>
        IScreenDevice ScreenDevice { get; }

        /// <summary>
        /// The ULA device that takes care of raising interrupts
        /// </summary>
        IInterruptDevice InterruptDevice { get; }

        /// <summary>
        /// The beeper device attached to the VM
        /// </summary>
        IBeeperDevice BeeperDevice { get; }

        /// <summary>
        /// The current status of the keyboard
        /// </summary>
        IKeyboardDevice KeyboardDevice { get; }

        /// <summary>
        /// The tape device attached to the VM
        /// </summary>
        ITapeDevice TapeDevice { get; }

        /// <summary>
        /// Debug info provider object
        /// </summary>
        ISpectrumDebugInfoProvider DebugInfoProvider { get; set; }

        /// <summary>
        /// This property indicates if the machine currently runs the
        /// maskable interrupt method.
        /// </summary>
        bool RunsInMaskableInterrupt { get; }

        /// <summary>
        /// Allows to set a clock frequency multiplier value (1, 2, 4, or 8).
        /// </summary>
        int ClockMultiplier { get; }

        /// <summary>
        /// Collection of RSpectrum devices
        /// </summary>
        DeviceInfoCollection DeviceData { get; }

        /// <summary>
        /// The ROM device used by the virtual machine
        /// </summary>
        IRomDevice RomDevice { get; }

        /// <summary>
        /// The configuration of the ROM
        /// </summary>
        IRomConfiguration RomConfiguration { get; }

        /// <summary>
        /// The ROM provider object
        /// </summary>
        IRomProvider RomProvider { get; }

        /// <summary>
        /// The configuration of the memory
        /// </summary>
        IMemoryConfiguration MemoryConfiguration { get; }

        /// <summary>
        /// The configuration of the screen
        /// </summary>
        ScreenConfiguration ScreenConfiguration { get; }

        /// <summary>
        /// The provider that handles the keyboard
        /// </summary>
        IKeyboardProvider KeyboardProvider { get; }

        /// <summary>
        /// The main execution cycle of the Spectrum VM
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="options">Execution options</param>
        /// <return>True, if the cycle completed; false, if it has been cancelled</return>
        bool ExecuteCycle(CancellationToken token, ExecuteCycleOptions options);

        /// <summary>
        /// Gets the device with the provided type
        /// </summary>
        /// <typeparam name="TDevice"></typeparam>
        /// <returns></returns>
        IDeviceInfo<TDevice, IDeviceConfiguration, IVmComponentProvider> GetDeviceInfo<TDevice>()
            where TDevice : class, IDevice;
    }
}