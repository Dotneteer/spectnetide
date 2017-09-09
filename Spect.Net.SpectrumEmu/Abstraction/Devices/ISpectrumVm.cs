using System.Threading;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;
using ZmakCPU = ZXMAK2.Engine.Cpu.Processor.Z80Cpu;


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

        ZmakCPU ZmakCpu { get; }

        /// <summary>
        /// Gets the frequency of the virtual machine's clock in Hz
        /// </summary>
        int ClockFrequeny { get; }

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
        /// Gets the ROM information of the virtual machine
        /// </summary>
        RomInfo RomInfo { get; }

        /// <summary>
        /// The current execution cycle options
        /// </summary>
        ExecuteCycleOptions ExecuteCycleOptions { get; }

        /// <summary>
        /// The memory device used by the virtual machine
        /// </summary>
        ISpectrumMemoryDevice MemoryDevice { get; }

        /// <summary>
        /// The port device used by the virtual machine
        /// </summary>
        ISpectrumPortDevice PortDevice { get; }

        /// <summary>
        /// The device that represents the border
        /// </summary>
        IBorderDevice BorderDevice { get; }

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
        /// The main execution cycle of the Spectrum VM
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="options">Execution options</param>
        /// <return>True, if the cycle completed; false, if it has been cancelled</return>
        bool ExecuteCycle(CancellationToken token, ExecuteCycleOptions options);
    }
}