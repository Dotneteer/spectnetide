using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Models;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.DivIde;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Next;
using Spect.Net.SpectrumEmu.Devices.Ports;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class can be used to manage the lifecycle of a Spectrum machine.
    /// </summary>
    /// <remarks>
    /// Use the methods of this class from the UI thread.
    /// </remarks>
    public class SpectrumMachine: IDisposable
    {
        #region Static data members and their initialization

        // --- Provider factories to create providers when instantiating the machine
        private static readonly Dictionary<Type, Func<object>> s_ProviderFactories = 
            new Dictionary<Type, Func<object>>();

        /// <summary>
        /// Resets the class members when first accessed
        /// </summary>
        static SpectrumMachine()
        {
            Reset();
        }

        /// <summary>
        /// Resets the static members of this class
        /// </summary>
        public static void Reset()
        {
            s_ProviderFactories.Clear();
        }

        /// <summary>
        /// Registers a provider
        /// </summary>
        /// <typeparam name="TProvider">Provider type</typeparam>
        /// <param name="factory">Factory method for the specified provider</param>
        public static void RegisterProvider<TProvider>(Func<TProvider> factory)
            where TProvider : class, IVmComponentProvider
        {
            s_ProviderFactories[typeof(TProvider)] = factory;
        }

        #endregion

        #region Private members

        // --- Runs the Spectrum engine in the background

        #endregion

        #region Public properties

        /// <summary>
        /// The ZX Spectrum virtual machine object
        /// </summary>
        public ISpectrumVm SpectrumVm { get; private set; }

        /// <summary>
        /// Object that provides stack debug support
        /// </summary>
        public IStackDebugSupport StackDebugSupport { get; private set; }

        /// <summary>
        /// The current state of the virtual machine
        /// </summary>
        public VmState VmState { get; private set; }

        /// <summary>
        /// The cancellation token source to suspend or stop the virtual machine
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        /// <summary>
        /// Signs that this is the very first start of the
        /// virtual machine 
        /// </summary>
        public bool IsFirstStart { get; private set; }

        /// <summary>
        /// Signs that this is the very first paused state
        /// of the virtual machine
        /// </summary>
        public bool IsFirstPause { get; private set; }

        /// <summary>
        /// Exception that has been raised during the execution
        /// </summary>
        public Exception ExecutionCycleException { get; private set; }

        /// <summary>
        /// Result of the execution
        /// </summary>
        /// <value>True, if the cycle has been completed; false if it has been cancelled</value>
        public bool ExecutionCycleResult { get; private set; }

        /// <summary>
        /// Has the execution be cancelled
        /// </summary>
        public bool Cancelled { get; private set; }

        /// <summary>
        /// This event is raised whenever the state of the virtual machine changes
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged;

        /// <summary>
        /// This event is raised when the screen of the virtual machine has
        /// been refreshed
        /// </summary>
        public event EventHandler<VmScreenRefreshedEventArgs> VmScreenRefreshed;

        /// <summary>
        /// This event is raised when the engine stops because of an exception
        /// </summary>
        public event EventHandler VmStoppedWithException;

        /// <summary>
        /// The task that represents the completion of the execution cycle
        /// </summary>
        public Task CompletionTask { get; private set; }

        /// <summary>
        /// Function that can switch to the main thread
        /// </summary>
        public Func<Action, Task> ExecuteOnMainThread { get; set; }

        #endregion

        #region Lifecycle methods

        /// <summary>
        /// We do not allow creating a machine without the factory functions
        /// </summary>
        private SpectrumMachine()
        {
        }

        /// <summary>
        /// Creates a SpectrumMachine instance according to the specified edition key
        /// </summary>
        /// <param name="modelKey">Spectrum model key</param>
        /// <param name="editionKey">Edition key (within the model)</param>
        /// <returns>
        /// The newly created machine isntance
        /// </returns>
        public static SpectrumMachine CreateMachine(string modelKey, string editionKey)
        {
            // --- Check input
            if (modelKey == null) throw new ArgumentNullException(nameof(modelKey));
            if (editionKey == null) throw new ArgumentNullException(nameof(editionKey));

            if (!SpectrumModels.StockModels.TryGetValue(modelKey, out var model))
            {
                throw new KeyNotFoundException($"Cannot find a Spectrum model with key '{modelKey}'");
            }

            if (!model.Editions.TryGetValue(editionKey, out var edition))
            {
                throw new KeyNotFoundException($"Cannot find an edition of {modelKey} with key '{editionKey}'");
            }

            // --- Create the selected Spectrum model/edition
            DeviceInfoCollection devices;
            switch (modelKey)
            {
                case SpectrumModels.ZX_SPECTRUM_128:
                    devices = CreateSpectrum128Devices(edition);
                    break;
                case SpectrumModels.ZX_SPECTRUM_P3_E:
                    devices = CreateSpectrumP3Devices(edition);
                    break;
                case SpectrumModels.ZX_SPECTRUM_NEXT:
                    devices = CreateSpectrumNextDevices(edition);
                    break;
                default:
                    devices = CreateSpectrum48Devices(edition);
                    break;
            }

            // --- Setup the machine
            var machine = new SpectrumMachine
            {
                SpectrumVm = new SpectrumEngine(devices),
                VmState = VmState.None
            };
            var debugProvider = GetProvider<ISpectrumDebugInfoProvider>();
            if (debugProvider != null)
            {
                machine.SpectrumVm.DebugInfoProvider = debugProvider;
            }

            var screenDevice = machine.SpectrumVm.ScreenDevice;
            screenDevice.FrameCompleted += (sender, args) =>
            {
                machine.VmScreenRefreshed?.Invoke(machine,
                    new VmScreenRefreshedEventArgs(screenDevice.GetPixelBuffer()));
            };
            return machine;
        }

        /// <summary>
        /// Releases the resources held by this machine.
        /// </summary>
        public async void Dispose()
        {
            if (VmState != VmState.Stopped)
            {
                await Stop();
            }
        }

        #endregion

        #region Setup methods

        /// <summary>
        /// Sets the object that provides stack debug support
        /// </summary>
        /// <param name="stackDebugSupport"></param>
        public void SetStackDebugSupport(IStackDebugSupport stackDebugSupport)
        {
            StackDebugSupport = stackDebugSupport;
        }

        #endregion

        #region Control methods

        /// <summary>
        /// Starts the machine in a background thread.
        /// </summary>
        /// <param name="options">Options to start the machine with.</param>
        /// <remarks>
        /// Reports completion when the machine starts executing its cycles. The machine can
        /// go into Paused or Stopped state, if the execution options allow, for example, 
        /// when it runs to a predefined breakpoint.
        /// </remarks>
        public void Start(ExecuteCycleOptions options)
        {
            if (VmState == VmState.Running) return;

            // --- Prepare the machine to run
            IsFirstStart = VmState == VmState.None || VmState == VmState.Stopped;
            if (IsFirstStart)
            {
                SpectrumVm.Reset();
            }
            SpectrumVm.DebugInfoProvider?.PrepareBreakpoints();

            // --- Dispose the previous cancellation token, and create a new one
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();

            // --- Set up the task that runs the machine
            CompletionTask = new Task(async () =>
                {
                    Cancelled = false;
                    ExecutionCycleResult = false;
                    try
                    {
                        ExecutionCycleResult = SpectrumVm.ExecuteCycle(CancellationTokenSource.Token, options);
                    }
                    catch (TaskCanceledException)
                    {
                        Cancelled = true;
                    }
                    catch (Exception ex)
                    {
                        ExecutionCycleException = ex;
                    }

                    // --- Conclude the execution task
                    await ExecuteOnMainThread(() =>
                    {
                        MoveToState(VmState == VmState.Stopping 
                                    || VmState == VmState.Stopped 
                                    || ExecutionCycleException != null
                            ? VmState.Stopped
                            : VmState.Paused);

                        if (ExecutionCycleException != null)
                        {
                            VmStoppedWithException?.Invoke(this, EventArgs.Empty);
                        }
                    });
                });

            MoveToState(VmState.Running);
            CompletionTask.Start();
        }

        /// <summary>
        /// Pauses the running machine.
        /// </summary>
        /// <remarks>Reports completion when the background execution has been stopped</remarks>
        public async Task Pause()
        {
            if (VmState == VmState.None || VmState == VmState.Stopped) return;

            // --- Prepare the machine to pause
            IsFirstPause = IsFirstStart;
            MoveToState(VmState.Pausing);

            // --- Wait for cancellation
            CancellationTokenSource?.Cancel();
            await CompletionTask;

            // --- Now, it's been paused
            MoveToState(VmState.Paused);
        }

        /// <summary>
        /// Stops the virtual machine
        /// </summary>
        /// <remarks>
        /// Reports completion when the machine reaches a complet stop.
        /// </remarks>
        public async Task Stop()
        {
            // --- Stop only running machine    
            switch (VmState)
            {
                case VmState.Stopped:
                    return;

                case VmState.Paused:
                    // --- The machine is paused, it can be quicky stopped
                    MoveToState(VmState.Stopping);
                    MoveToState(VmState.Stopped);
                    break;

                default:
                    // --- Initiate stop
                    MoveToState(VmState.Stopping);
                    if (CancellationTokenSource == null)
                    {
                        MoveToState(VmState.Stopped);
                    }
                    else
                    {
                        CancellationTokenSource.Cancel();
                        await CompletionTask;
                        MoveToState(VmState.Stopped);
                    }
                    break;
            }
        }

        /// <summary>
        /// Forces a paused state (used for recovering VM state)
        /// </summary>
        public void ForcePausedState()
        {
            if (VmState == VmState.Paused) return;
            if (VmState == VmState.None || VmState == VmState.Stopped)
            {
                IsFirstPause = true;
                MoveToState(VmState.Paused);
            }
        }

        /// <summary>
        /// Forces a screen refresh
        /// </summary>
        public void ForceScreenRefresh()
        {
            VmScreenRefreshed?.Invoke(this, 
                new VmScreenRefreshedEventArgs(SpectrumVm.ScreenDevice.GetPixelBuffer()));
        }

        /// <summary>
        /// Moves the virtual machine to the specified new state
        /// </summary>
        /// <param name="newState">New machine state</param>
        protected void MoveToState(VmState newState)
        {
            var oldState = VmState;
            VmState = newState;
            VmStateChanged?.Invoke(this, new VmStateChangedEventArgs(oldState, newState));
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Gets a provider instance for the specified provider types
        /// </summary>
        /// <typeparam name="TProvider">Service provider type</typeparam>
        /// <param name="optional">In the provider optional?</param>
        /// <exception cref="KeyNotFoundException">
        /// The requested mandatory provider cannot be found
        /// </exception>
        /// <returns>Provider instance, if found; otherwise, null</returns>
        private static TProvider GetProvider<TProvider>(bool optional = true)
            where TProvider: class, IVmComponentProvider
        {
            if (s_ProviderFactories.TryGetValue(typeof(TProvider), out var factory))
            {
                return (TProvider) factory();
            }

            return optional
                ? (TProvider) default
                : throw new KeyNotFoundException($"Cannot find a factory for {typeof(TProvider)}");
        }

        /// <summary>
        /// Create the collection of devices for the Spectrum 48K virtual machine
        /// </summary>
        /// <param name="spectrumConfig">Machine configuration</param>
        /// <returns></returns>
        private static DeviceInfoCollection CreateSpectrum48Devices(SpectrumEdition spectrumConfig)
        {
            return new DeviceInfoCollection
            {
                new CpuDeviceInfo(spectrumConfig.Cpu),
                new RomDeviceInfo(GetProvider<IRomProvider>(false), spectrumConfig.Rom, new SpectrumRomDevice()),
                new MemoryDeviceInfo(spectrumConfig.Memory, new Spectrum48MemoryDevice()),
                new PortDeviceInfo(null, new Spectrum48PortDevice()),
                new ClockDeviceInfo(new ClockProvider()),
                new KeyboardDeviceInfo(GetProvider<IKeyboardProvider>(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, GetProvider<IBeeperProvider>()),
                new TapeDeviceInfo(GetProvider<ITapeProvider>())
            };
        }

        /// <summary>
        /// Create the collection of devices for the Spectrum 48K virtual machine
        /// </summary>
        /// <param name="spectrumConfig">Machine configuration</param>
        /// <returns></returns>
        private static DeviceInfoCollection CreateSpectrum128Devices(SpectrumEdition spectrumConfig)
        {
            return new DeviceInfoCollection
            {
                new CpuDeviceInfo(spectrumConfig.Cpu),
                new RomDeviceInfo(GetProvider<IRomProvider>(false), spectrumConfig.Rom, new SpectrumRomDevice()),
                new MemoryDeviceInfo(spectrumConfig.Memory, new Spectrum128MemoryDevice()),
                new PortDeviceInfo(null, new Spectrum128PortDevice()),
                new ClockDeviceInfo(new ClockProvider()),
                new KeyboardDeviceInfo(GetProvider<IKeyboardProvider>(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, GetProvider<IBeeperProvider>()),
                new TapeDeviceInfo(GetProvider<ITapeProvider>()),
                new SoundDeviceInfo(spectrumConfig.Sound, GetProvider<ISoundProvider>())
            };
        }

        /// <summary>
        /// Create the collection of devices for the Spectrum +3E virtual machine
        /// </summary>
        /// <param name="spectrumConfig">Machine configuration</param>
        /// <returns></returns>
        private static DeviceInfoCollection CreateSpectrumP3Devices(SpectrumEdition spectrumConfig)
        {
            return new DeviceInfoCollection
            {
                new CpuDeviceInfo(spectrumConfig.Cpu),
                new RomDeviceInfo(GetProvider<IRomProvider>(false), spectrumConfig.Rom, new SpectrumRomDevice()),
                new MemoryDeviceInfo(spectrumConfig.Memory, new SpectrumP3MemoryDevice()),
                new PortDeviceInfo(null, new SpectrumP3PortDevice()),
                new ClockDeviceInfo(new ClockProvider()),
                new KeyboardDeviceInfo(GetProvider<IKeyboardProvider>(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, GetProvider<IBeeperProvider>()),
                new TapeDeviceInfo(GetProvider<ITapeProvider>()),
                new SoundDeviceInfo(spectrumConfig.Sound, GetProvider<ISoundProvider>()),
                new FloppyDeviceInfo(new FloppyDevice())
            };
        }

        /// <summary>
        /// Create the collection of devices for the Spectrum Next virtual machine
        /// </summary>
        /// <param name="spectrumConfig">Machine configuration</param>
        /// <returns></returns>
        private static DeviceInfoCollection CreateSpectrumNextDevices(SpectrumEdition spectrumConfig)
        {
            return new DeviceInfoCollection
            {
                new CpuDeviceInfo(spectrumConfig.Cpu),
                new RomDeviceInfo(GetProvider<IRomProvider>(false), spectrumConfig.Rom, new SpectrumRomDevice()),
                new MemoryDeviceInfo(spectrumConfig.Memory, new SpectrumNextMemoryDevice()),
                new PortDeviceInfo(null, new SpectrumNextPortDevice()),
                new ClockDeviceInfo(new ClockProvider()),
                new KeyboardDeviceInfo(GetProvider<IKeyboardProvider>(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, GetProvider<IBeeperProvider>()),
                new TapeDeviceInfo(GetProvider<ITapeProvider>()),
                new SoundDeviceInfo(spectrumConfig.Sound, GetProvider<ISoundProvider>()),
                new NextDeviceInfo(new NextFeatureSetDevice()),
                new DivIdeDeviceInfo(new DivIdeDevice())
            };
        }

        #endregion
    }
}