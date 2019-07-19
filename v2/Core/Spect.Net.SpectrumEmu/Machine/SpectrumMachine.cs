using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Screen;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Abstraction.Models;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Ports;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class represents a ZX Spectrum virtual machine
    /// </summary>
    public class SpectrumMachine : ISpectrumMachine
    {
        private readonly object _machineStateLocker = new object();
        private VmState _machineState;
        private readonly ISpectrumVm _spectrumVm;
        private readonly IClockProvider _clockProvider;
        private readonly double _physicalFrameClockCount;
        private CancellationTokenSource _cancellationTokenSource;
        private Task<ExecutionCompletionReason> _completionTask;

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
            RegisterDefaultProviders();
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

        /// <summary>
        /// Registers the default providers
        /// </summary>
        public static void RegisterDefaultProviders()
        {
            RegisterProvider<IClockProvider>(() => new DefaultClockProvider());
            RegisterProvider<IRomProvider>(() => new DefaultRomProvider());
            RegisterProvider<IKeyboardProvider>(() => new DefaultKeyboardProvider());
            RegisterProvider<ITapeLoadProvider>(() => new DefaultTapeLoadProvider());
            RegisterProvider<ITapeSaveProvider>(() => new FileBasedTapeSaveProvider());
            RegisterProvider<IBeeperProvider>(() => new NoAudioProvider());
            RegisterProvider<ISoundProvider>(() => new NoAudioProvider());
        }

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
            where TProvider : class, IVmComponentProvider
        {
            if (s_ProviderFactories.TryGetValue(typeof(TProvider), out var factory))
            {
                return (TProvider)factory();
            }

            return optional
                ? (TProvider)default
                : throw new KeyNotFoundException($"Cannot find a factory for {typeof(TProvider)}");
        }

        #endregion

        #region Virtual machine instantiation

        /// <summary>
        /// Creates an instance of the virtual machine
        /// </summary>
        /// <param name="modelKey">The model key of the virtual machine</param>
        /// <param name="editionKey">The edition key of the virtual machine</param>
        /// <param name="devices">Devices to create the machine</param>
        private SpectrumMachine(string modelKey, string editionKey, DeviceInfoCollection devices)
        {
            // --- Store model information
            ModelKey = modelKey;
            EditionKey = editionKey;

            // --- Create the engine and set up properties
            _spectrumVm = new SpectrumEngine(devices);

            Cpu = new CpuZ80(_spectrumVm.Cpu);

            var roms = new List<ReadOnlyMemorySlice>();
            for (var i = 0; i < _spectrumVm.RomConfiguration.NumberOfRoms; i++)
            {
                roms.Add(new ReadOnlyMemorySlice(_spectrumVm.RomDevice.GetRomBytes(i)));
            }
            Roms = new ReadOnlyCollection<ReadOnlyMemorySlice>(roms);

            PagingInfo = new MemoryPagingInfo(_spectrumVm.MemoryDevice);
            Memory = new SpectrumMemoryContents(_spectrumVm.MemoryDevice, _spectrumVm.Cpu);

            var ramBanks = new List<MemorySlice>();
            if (_spectrumVm.MemoryConfiguration.RamBanks != null)
            {
                for (var i = 0; i < _spectrumVm.MemoryConfiguration.RamBanks; i++)
                {
                    ramBanks.Add(new MemorySlice(_spectrumVm.MemoryDevice.GetRamBank(i)));
                }
            }
            RamBanks = new ReadOnlyCollection<MemorySlice>(ramBanks);

            Keyboard = new KeyboardEmulator(_spectrumVm.KeyboardDevice);
            ScreenConfiguration = _spectrumVm.ScreenConfiguration;
            ScreenRenderingTable = new ScreenRenderingTable(_spectrumVm.ScreenDevice);
            ScreenBitmap = new ScreenBitmap(_spectrumVm.ScreenDevice);
            ScreenRenderingStatus = new ScreenRenderingStatus(_spectrumVm);
            BeeperConfiguration = _spectrumVm.AudioConfiguration;
            BeeperSamples = new AudioSamples(_spectrumVm.BeeperDevice);
            SoundConfiguration = _spectrumVm.SoundConfiguration;
            AudioSamples = new AudioSamples(_spectrumVm.SoundDevice);
            Breakpoints = new CodeBreakpoints(_spectrumVm.DebugInfoProvider);

            // --- Hook device events
            _spectrumVm.TapeDevice.LoadCompleted += (s, e) => FastLoadCompleted?.Invoke(s, e);

            // --- Initialize machine state
            _clockProvider = GetProvider<IClockProvider>();
            _physicalFrameClockCount = _clockProvider.GetFrequency() / (double)_spectrumVm.BaseClockFrequency *
                                       ScreenConfiguration.ScreenRenderingFrameTactCount;
            MachineState = VmState.None;
            ExecutionCompletionReason = ExecutionCompletionReason.None;
        }

        /// <summary>
        /// Creates a Spectrum instance with the specified model and edition name
        /// </summary>
        /// <param name="modelKey">Spectrum model name</param>
        /// <param name="editionKey">Edition name</param>
        /// <returns>The newly create Spectrum machine</returns>
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
                default:
                    devices = CreateSpectrum48Devices(edition);
                    break;
            }

            // --- Setup the machine
            var machine = new SpectrumMachine(modelKey, editionKey, devices);
            return machine;
        }

        /// <summary>
        /// Creates a Spectrum 48K instance PAL edition
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumMachine CreateSpectrum48Pal()
        {
            return CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance PAL edition with turbo mode (2xCPU)
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumMachine CreateSpectrum48PalTurbo()
        {
            return CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL_2_X);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance NTSC edition
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumMachine CreateSpectrum48Ntsc()
        {
            return CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.NTSC);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance NTSC edition with turbo mode
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumMachine CreateSpectrum48NtscTurbo()
        {
            return CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.NTSC_2_X);
        }

        /// <summary>
        /// Creates a Spectrum 128K instance
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumMachine CreateSpectrum128()
        {
            return CreateMachine(SpectrumModels.ZX_SPECTRUM_128, SpectrumModels.PAL);
        }

        /// <summary>
        /// Creates a Spectrum +3E instance
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumMachine CreateSpectrumP3E()
        {
            return CreateMachine(SpectrumModels.ZX_SPECTRUM_P3_E, SpectrumModels.PAL);
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
                new KeyboardDeviceInfo(GetProvider<IKeyboardProvider>(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, GetProvider<IBeeperProvider>()),
                new TapeLoadDeviceInfo(GetProvider<ITapeLoadProvider>()),
                new TapeSaveDeviceInfo(GetProvider<ITapeSaveProvider>())
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
                new KeyboardDeviceInfo(GetProvider<IKeyboardProvider>(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, GetProvider<IBeeperProvider>()),
                new TapeLoadDeviceInfo(GetProvider<ITapeLoadProvider>()),
                new TapeSaveDeviceInfo(GetProvider<ITapeSaveProvider>()),
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
                new KeyboardDeviceInfo(GetProvider<IKeyboardProvider>(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, GetProvider<IBeeperProvider>()),
                new TapeLoadDeviceInfo(GetProvider<ITapeLoadProvider>()),
                new TapeSaveDeviceInfo(GetProvider<ITapeSaveProvider>()),
                new SoundDeviceInfo(spectrumConfig.Sound, GetProvider<ISoundProvider>())
            };
        }

        #endregion

        #region Machine properties

        /// <summary>
        /// The current machine's model key.
        /// </summary>
        public string ModelKey { get; }

        /// <summary>
        /// The current machine's edition key.
        /// </summary>
        public string EditionKey { get; }

        /// <summary>
        /// Gets or sets the state of the virtual machine
        /// </summary>
        public VmState MachineState
        {
            get
            {
                lock (_machineStateLocker)
                {
                    return _machineState;
                }
            }

            private set
            {
                VmState oldState;
                lock (_machineStateLocker)
                {
                    if (value == _machineState) return;

                    oldState = _machineState;
                    _machineState = value;
                }
                OnVmStateChanged(new VmStateChangedEventArgs(oldState, value));
            }
        }

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
        /// Signs if the machine runs in debug mode.
        /// </summary>
        public bool RunsInDebugMode { get; private set; }

        /// <summary>
        /// Indicates if fast tape mode is allowed.
        /// </summary>
        public bool FastTapeMode { get; set; }

        /// <summary>
        /// The CPU of the machine
        /// </summary>
        public CpuZ80 Cpu { get; }

        /// <summary>
        /// Provides access to the individual ROM pages of the machine
        /// </summary>
        public IReadOnlyList<ReadOnlyMemorySlice> Roms { get; }

        /// <summary>
        /// Gets the number of ROM pages
        /// </summary>
        public int RomCount => Roms.Count;

        /// <summary>
        /// Allows to obtain paging information about the memory
        /// </summary>
        public MemoryPagingInfo PagingInfo { get; }

        /// <summary>
        /// The current Contents of the machine's 64K addressable memory
        /// </summary>
        public SpectrumMemoryContents Memory { get; }

        /// <summary>
        /// Provides access to the individual RAM banks of the machine
        /// </summary>
        public IReadOnlyList<MemorySlice> RamBanks { get; }

        /// <summary>
        /// Gets the number of RAM banks
        /// </summary>
        public int RamBankCount => RamBanks.Count;

        /// <summary>
        /// Allows to emulate keyboard keys and query the keyboard state
        /// </summary>
        public KeyboardEmulator Keyboard { get; }

        /// <summary>
        /// Allows read-only access to screen rendering configuration
        /// </summary>
        public ScreenConfiguration ScreenConfiguration { get; }

        /// <summary>
        /// Allows read-only access to the screen rendering table
        /// </summary>
        public ScreenRenderingTable ScreenRenderingTable { get; }

        /// <summary>
        /// A bitmap that represents the current visible screen's pixels, including the border
        /// </summary>
        public ScreenBitmap ScreenBitmap { get; }

        /// <summary>
        /// Gets the current screen rendering status of the machine.
        /// </summary>
        public ScreenRenderingStatus ScreenRenderingStatus { get; }

        /// <summary>
        /// Gets the beeper configuration of the machine
        /// </summary>
        public IAudioConfiguration BeeperConfiguration { get; }

        /// <summary>
        /// Gets the beeper samples of the current rendering frame
        /// </summary>
        public AudioSamples BeeperSamples { get; }

        /// <summary>
        /// Gets the sound (PSG) configuration of the machine
        /// </summary>
        public IAudioConfiguration SoundConfiguration { get; }

        /// <summary>
        /// Gets the sound (PSG) samples of the current rendering frame
        /// </summary>
        public AudioSamples AudioSamples { get; }

        /// <summary>
        /// The collection of breakpoints
        /// </summary>
        public CodeBreakpoints Breakpoints { get; }

        /// <summary>
        /// Gets the reason that tells why the machine has been stopped or paused
        /// </summary>
        public ExecutionCompletionReason ExecutionCompletionReason { get; private set; }

        /// <summary>
        /// The number of CPU frame counts since the machine has started.
        /// </summary>
        public int CpuFrameCount { get; private set; }

        /// <summary>
        /// The number of render frame counts since the machine has started.
        /// </summary>
        public int RenderFrameCount { get; private set; }

        /// <summary>
        /// The length of last CPU frame in ticks
        /// </summary>
        public long LastCpuFrameTicks { get; private set; }

        /// <summary>
        /// The length of last render frame in ticks
        /// </summary>
        public long LastRenderFrameTicks { get; private set; }

        #endregion

        #region Virtual machine events

        /// <summary>
        /// This event is raised when the state of the virtual machine has been changed.
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged;

        /// <summary>
        /// This event is executed when it is time to scan the ZX Spectrum keyboard.
        /// </summary>
        public event EventHandler<KeyStatusEventArgs> KeyScanning;

        /// <summary>
        /// This event is executed whenever the CPU frame has been completed.
        /// </summary>
        public event EventHandler<CancelEventArgs> CpuFrameCompleted;

        /// <summary>
        /// This event is executed whenever the render frame has been completed.
        /// </summary>
        public event EventHandler<RenderFrameEventArgs> RenderFrameCompleted;

        /// <summary>
        /// This event is raised when a fast load operation has been completed.
        /// </summary>
        public event EventHandler FastLoadCompleted;

        /// <summary>
        /// This event fires when the virtual machine engine raised an exception.
        /// </summary>
        public event EventHandler<VmExceptionArgs> ExceptionRaised;

        #endregion

        #region Machine control methods

        /// <summary>
        /// Starts the machine in a background thread with the specified options.
        /// </summary>
        /// <remarks>
        /// Reports completion when the machine starts executing its cycles. The machine can
        /// go into Paused or Stopped state, if the execution options allow, for example, 
        /// when it runs to a predefined breakpoint.
        /// </remarks>
        public void StartWithOptions(ExecuteCycleOptions options)
        {
            if (MachineState == VmState.Running) return;

            // --- Prepare the machine to run
            IsFirstStart = MachineState == VmState.None || MachineState == VmState.Stopped;
            _spectrumVm.DebugInfoProvider?.PrepareBreakpoints();
            MachineState = VmState.Starting;
            if (IsFirstStart)
            {
                _spectrumVm.Reset();
                _spectrumVm.Cpu.StackDebugSupport.ClearStepOutStack();
                _spectrumVm.DebugInfoProvider?.ResetHitCounts();
                CpuFrameCount = 0;
                RenderFrameCount = 0;
            }

            // --- Dispose the previous cancellation token, and create a new one
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            // --- Set up the task that runs the machine
            MachineState = VmState.Running;
            Console.WriteLine("Machine is about to start");
            try
            {
                _completionTask = StartAndRun(_cancellationTokenSource.Token, options);
                Console.WriteLine("Machine started");
            }
            catch (TaskCanceledException)
            {
                ExecutionCompletionReason = ExecutionCompletionReason.Cancelled;
            }
            catch (Exception ex)
            {
                ExceptionRaised?.Invoke(this, new VmExceptionArgs(ex));
            }
        }

        /// <summary>
        /// Starts the machine in a background thread in continuous running mode.
        /// </summary>
        /// <remarks>
        /// Reports completion when the machine starts executing its cycles. The machine can
        /// go into Paused or Stopped state, if the execution options allow, for example, 
        /// when it runs to a predefined breakpoint.
        /// </remarks>
        public void Start()
        {
            RunsInDebugMode = false;
            StartWithOptions(new ExecuteCycleOptions(fastTapeMode: FastTapeMode));
        }

        /// <summary>
        /// Pauses the running machine.
        /// </summary>
        /// <remarks>
        /// Reports completion when the background execution has been stopped.
        /// </remarks>
        public async Task Pause()
        {
            if (MachineState == VmState.None || MachineState == VmState.Stopped) return;

            // --- Prepare the machine to pause
            IsFirstPause = IsFirstStart;
            MachineState = VmState.Pausing;
            await CancelVmExecution(VmState.Paused);
        }

        /// <summary>
        /// Stops the Spectrum machine.
        /// </summary>
        /// <remarks>
        /// If the machine is paused or stopped, it leaves the machine in its state.
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public async Task Stop()
        {
            // --- Stop only running machine    
            switch (MachineState)
            {
                case VmState.Stopped:
                    return;

                case VmState.Paused:
                    // --- The machine is paused, it can be quickly stopped
                    MachineState = VmState.Stopping;
                    MachineState = VmState.Stopped;
                    break;

                default:
                    // --- Initiate stop
                    MachineState = VmState.Stopping;
                    if (_cancellationTokenSource == null)
                    {
                        MachineState = VmState.Stopped;
                    }
                    else
                    {
                        await CancelVmExecution(VmState.Stopped);
                    }
                    break;
            }
            RunsInDebugMode = false;
        }

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread unless it reaches a breakpoint.
        /// </summary>
        public void StartDebug()
        {
            RunsInDebugMode = true;
            StartWithOptions(new ExecuteCycleOptions(EmulationMode.Debugger,
                fastTapeMode: FastTapeMode));
        }

        /// <summary>
        /// Executes the subsequent Z80 instruction.
        /// </summary>
        /// <remarks>
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public void StepInto()
        {
            if (MachineState != VmState.Paused) return;
            RunsInDebugMode = true;
            StartWithOptions(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepInto,
                FastTapeMode));
        }

        /// <summary>
        /// Executes the subsequent Z80 CALL, RST, or block instruction entirely.
        /// </summary>
        /// <remarks>
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public void StepOver()
        {
            if (MachineState != VmState.Paused) return;

            RunsInDebugMode = true;
            StartWithOptions(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepOver,
                true));
        }

        /// <summary>
        /// Executes the subsequent Z80 CALL, RST, or block instruction entirely.
        /// </summary>
        /// <remarks>
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public void StepOut()
        {
            if (MachineState != VmState.Paused) return;

            RunsInDebugMode = true;
            StartWithOptions(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepOut,
                true));
        }

        /// <summary>
        /// Starts the virtual machine and runs it until the execution cycle is completed for
        /// a reason.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="options">Virtual machine execution options</param>
        /// <returns>The reason why the execution completed.</returns>
        private async Task<ExecutionCompletionReason> StartAndRun(CancellationToken cancellationToken,
            ExecuteCycleOptions options)
        {
            var lastRunStart = _clockProvider.GetCounter();
            var lastRenderFrameStart = lastRunStart;
            var frameCount = 0;
            var completed = false;
            while (!completed)
            {
                Console.WriteLine("Next cycle starts");
                // --- Execute a single CPU Frame
                var lastCpuFrameStart = _clockProvider.GetCounter();
                var cycleCompleted = _spectrumVm.ExecuteCycle(cancellationToken, options, true);
                LastCpuFrameTicks = _clockProvider.GetCounter() - lastCpuFrameStart;
                if (!cycleCompleted) return ExecutionCompletionReason.Cancelled;

                // --- Check for emulated keys
                CpuFrameCount++;
                var hasEmulatedKey = _spectrumVm.KeyboardProvider?.EmulateKeyStroke();
                if (hasEmulatedKey != true)
                {
                    // --- Keyboard scan
                    var keyStatusArgs = new KeyStatusEventArgs();
                    KeyScanning?.Invoke(this, keyStatusArgs);
                    foreach (var keyStatus in keyStatusArgs.KeyStatusList)
                    {
                        _spectrumVm.KeyboardDevice.SetStatus(keyStatus);
                    }
                }

                // --- Do additional tasks when CPU frame completed
                var cancelArgs = new CancelEventArgs(false);
                CpuFrameCompleted?.Invoke(this, cancelArgs);
                if (cancelArgs.Cancel) return ExecutionCompletionReason.Cancelled;

                switch (_spectrumVm.ExecutionCompletionReason)
                {
                    case ExecutionCompletionReason.TerminationPointReached:
                    case ExecutionCompletionReason.BreakpointReached:
                    case ExecutionCompletionReason.Halted:
                    case ExecutionCompletionReason.Exception:
                        completed = true;
                        break;
                    case ExecutionCompletionReason.RenderFrameCompleted:
                        var lastFrameEnd = _clockProvider.GetCounter();
                        LastRenderFrameTicks = lastFrameEnd - lastRenderFrameStart;
                        lastRenderFrameStart = lastFrameEnd;
                        completed = options.EmulationMode == EmulationMode.UntilRenderFrameEnds;
                        frameCount++;
                        RenderFrameCount++;

                        // --- Do additional task when render frame completed
                        var renderFrameArgs = new RenderFrameEventArgs(_spectrumVm.ScreenDevice.GetPixelBuffer());
                        RenderFrameCompleted?.Invoke(this, renderFrameArgs);
                        if (renderFrameArgs.Cancel)
                        {
                            return ExecutionCompletionReason.Cancelled;
                        }

                        // --- Wait for the next render frame, unless completed
                        if (!completed)
                        {
                            var runInMs = LastRenderFrameTicks * 1000.0 /
                                          _clockProvider.GetFrequency();
                            var waitInTicks = lastRunStart + frameCount * _physicalFrameClockCount
                                - _clockProvider.GetCounter() - _physicalFrameClockCount * 0.2;
                            var waitInMs = 1000.0 * waitInTicks / _clockProvider.GetFrequency();
                            Console.WriteLine($"Wait for next frame: {runInMs}");
                            if (waitInMs > 0)
                            {
                                await Task.Delay((int)waitInMs, cancellationToken);
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    return ExecutionCompletionReason.Cancelled;
                                }
                            }
                            else
                            {
                                await Task.Delay(1, cancellationToken);
                            }
                        }
                        break;
                }
            }

            // --- Done, pass back the reason of completing the run
            return _spectrumVm.ExecutionCompletionReason;
        }

        /// <summary>
        /// Cancels the execution of the virtual machine.
        /// </summary>
        /// <param name="cancelledState">Virtual machine state after cancellation</param>
        private async Task CancelVmExecution(VmState cancelledState)
        {
            try
            {
                // --- Wait for cancellation
                _cancellationTokenSource?.Cancel();
                ExecutionCompletionReason = await _completionTask;
            }
            catch (TaskCanceledException)
            {
                // --- Ok, run successfully cancelled
                ExecutionCompletionReason = ExecutionCompletionReason.Cancelled;
            }
            catch (Exception ex)
            {
                // --- Some other exception raised
                ExceptionRaised?.Invoke(this, new VmExceptionArgs(ex));
            }
            finally
            {
                // --- Now, it's cancelled
                MachineState = cancelledState;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Invokes the VmStateChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnVmStateChanged(VmStateChangedEventArgs e)
        {
            VmStateChanged?.Invoke(this, e);
        }

        #endregion
    }

}