using Spect.Net.SpectrumEmu.Abstraction;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Screen;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Abstraction.Models;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Ports;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class represents a ZX Spectrum virtual machine
    /// </summary>
    public class SpectrumMachine : ISpectrumMachine, ISpectrumMachineInternal
    {
        private readonly object _machineStateLocker = new object();
        private VmState _machineState;
        private readonly IClockProvider _clockProvider;
        private readonly double _physicalFrameClockCount;
        private CancellationTokenSource _cancellationTokenSource;
        private Task<ExecutionCompletionReason> _completionTask;

        /// <summary>
        /// Gets the ISpectrumVm instance behind the machine
        /// </summary>
        public ISpectrumVm SpectrumVm { get; }

        #region Static data members and their initialization

        // --- Provider factories to create providers when instantiating the machine
        private static readonly Dictionary<Type, Func<object>> s_ProviderFactories =
            new Dictionary<Type, Func<object>>();
        private static IStackDebugSupport s_StackDebugSupport;

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

        /// <summary>
        /// Registers the object that provides stack debug support
        /// </summary>
        /// <param name="stackDebugSupport">Stack debug support object</param>
        public static void RegisterStackDebugSupport(IStackDebugSupport stackDebugSupport)
        {
            s_StackDebugSupport = stackDebugSupport;
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
            SpectrumVm = new SpectrumEngine(devices);

            Cpu = new CpuZ80(SpectrumVm.Cpu);

            var roms = new List<ReadOnlyMemorySlice>();
            for (var i = 0; i < SpectrumVm.RomConfiguration.NumberOfRoms; i++)
            {
                roms.Add(new ReadOnlyMemorySlice(SpectrumVm.RomDevice.GetRomBytes(i)));
            }
            Roms = new ReadOnlyCollection<ReadOnlyMemorySlice>(roms);

            PagingInfo = new MemoryPagingInfo(SpectrumVm.MemoryDevice);
            Memory = new SpectrumMemoryContents(SpectrumVm.MemoryDevice, SpectrumVm.Cpu);

            var ramBanks = new List<MemorySlice>();
            if (SpectrumVm.MemoryConfiguration.RamBanks != null)
            {
                for (var i = 0; i < SpectrumVm.MemoryConfiguration.RamBanks; i++)
                {
                    ramBanks.Add(new MemorySlice(SpectrumVm.MemoryDevice.GetRamBank(i)));
                }
            }
            RamBanks = new ReadOnlyCollection<MemorySlice>(ramBanks);

            Keyboard = new KeyboardEmulator(SpectrumVm);
            ScreenConfiguration = SpectrumVm.ScreenConfiguration;
            ScreenRenderingTable = new ScreenRenderingTable(SpectrumVm.ScreenDevice);
            ScreenBitmap = new ScreenBitmap(SpectrumVm.ScreenDevice);
            ScreenRenderingStatus = new ScreenRenderingStatus(SpectrumVm);
            BeeperConfiguration = SpectrumVm.AudioConfiguration;
            BeeperSamples = new AudioSamples(SpectrumVm.BeeperDevice);
            BeeperProvider = SpectrumVm.BeeperProvider;
            SoundConfiguration = SpectrumVm.SoundConfiguration;
            AudioSamples = new AudioSamples(SpectrumVm.SoundDevice);
            Breakpoints = new CodeBreakpoints(SpectrumVm.DebugInfoProvider);

            // --- Hook device events
            SpectrumVm.TapeLoadDevice.LoadCompleted += (s, e) => FastLoadCompleted?.Invoke(s, e);
            SpectrumVm.TapeSaveDevice.LeftSaveMode += (s, e) => LeftSaveMode?.Invoke(s, e);

            // --- Initialize machine state
            _clockProvider = GetProvider<IClockProvider>();
            _physicalFrameClockCount = _clockProvider.GetFrequency() / (double)SpectrumVm.BaseClockFrequency *
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
        public static ISpectrumMachine CreateMachine(string modelKey, string editionKey)
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
            var debugProvider = GetProvider<ISpectrumDebugInfoProvider>();
            if (debugProvider != null)
            {
                machine.SpectrumVm.DebugInfoProvider = debugProvider;
            }
            if (s_StackDebugSupport != null)
            {
                machine.SpectrumVm.Cpu.StackDebugSupport = s_StackDebugSupport;
            }
            return machine;
        }

        /// <summary>
        /// Creates a Spectrum 48K instance PAL edition
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static ISpectrumMachine CreateSpectrum48Pal()
        {
            return CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance PAL edition with turbo mode (2xCPU)
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static ISpectrumMachine CreateSpectrum48PalTurbo()
        {
            return CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL_2_X);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance NTSC edition
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static ISpectrumMachine CreateSpectrum48Ntsc()
        {
            return CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.NTSC);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance NTSC edition with turbo mode
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static ISpectrumMachine CreateSpectrum48NtscTurbo()
        {
            return CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.NTSC_2_X);
        }

        /// <summary>
        /// Creates a Spectrum 128K instance
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static ISpectrumMachine CreateSpectrum128()
        {
            return CreateMachine(SpectrumModels.ZX_SPECTRUM_128, SpectrumModels.PAL);
        }

        /// <summary>
        /// Creates a Spectrum +3E instance
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static ISpectrumMachine CreateSpectrumP3E()
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
                new KempstonDeviceInfo(GetProvider<IKempstonProvider>()),
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
                new KempstonDeviceInfo(GetProvider<IKempstonProvider>()),
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
                new KempstonDeviceInfo(GetProvider<IKempstonProvider>()),
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
        /// The CPU tact at which the last execution cycle started
        /// </summary>
        public long LastExecutionStartTact => SpectrumVm.LastExecutionStartTact;

        /// <summary>
        /// Gets the amounf of contention accumulated 
        /// </summary>
        public long ContentionAccumulated => SpectrumVm.ContentionAccumulated;

        /// <summary>
        /// The current screen rendering frame tact
        /// </summary>
        public int CurrentFrameTact => SpectrumVm.CurrentFrameTact;

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
        /// The beeper provider associated with the machine
        /// </summary>
        public IBeeperProvider BeeperProvider { get; }

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

        /// <summary>
        /// Gets the value of the contention accumulated when the 
        /// execution cycle started
        /// </summary>
        public long LastExecutionContentionValue { get; private set; }

        /// <summary>
        /// The task that represents the completion of the execution cycle
        /// </summary>
        public Task CompletionTask => _completionTask;

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

        /// <summary>
        /// This event fires when the virtual machine left the save mode.
        /// </summary>
        public event EventHandler<SaveModeEventArgs> LeftSaveMode;

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
            SpectrumVm.DebugInfoProvider?.PrepareBreakpoints();
            MachineState = VmState.Starting;
            if (IsFirstStart)
            {
                SpectrumVm.Reset();
                SpectrumVm.Cpu.StackDebugSupport.Reset();
                SpectrumVm.DebugInfoProvider?.ResetHitCounts();
                CpuFrameCount = 0;
                RenderFrameCount = 0;
            }

            // --- Dispose the previous cancellation token, and create a new one
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            // --- Set up the task that runs the machine
            MachineState = VmState.Running;
            try
            {
                _completionTask = StartAndRun(_cancellationTokenSource.Token, options);
                _completionTask.GetAwaiter().OnCompleted(async () =>
                {
                    await WaitForPause();
                });
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
            LastExecutionContentionValue = 0L;
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
        public async Task StepInto()
        {
            if (MachineState != VmState.Paused) return;

            RunsInDebugMode = true;
            StartWithOptions(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepInto,
                FastTapeMode));
            await WaitForPause();
        }

        /// <summary>
        /// Executes the subsequent Z80 CALL, RST, or block instruction entirely.
        /// </summary>
        /// <remarks>
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public async Task StepOver()
        {
            if (MachineState != VmState.Paused) return;

            RunsInDebugMode = true;
            StartWithOptions(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepOver,
                true));
            await WaitForPause();
        }

        /// <summary>
        /// Executes the subsequent Z80 CALL, RST, or block instruction entirely.
        /// </summary>
        /// <remarks>
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public async Task StepOut()
        {
            if (MachineState != VmState.Paused) return;

            RunsInDebugMode = true;
            StartWithOptions(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepOut,
                true));
            await WaitForPause();
        }

        /// <summary>
        /// Forces a paused state (used for recovering VM state)
        /// </summary>
        public void ForcePausedState()
        {
            if (MachineState == VmState.Paused) return;
            if (MachineState == VmState.None || MachineState == VmState.Stopped)
            {
                IsFirstPause = true;
                MachineState = VmState.Paused;
            }
        }

        /// <summary>
        /// Waits while the machine pauses or stops
        /// </summary>
        public async Task WaitForPause()
        {
            try
            {
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
                ExecutionCompletionReason = ExecutionCompletionReason.Exception;
                ExceptionRaised?.Invoke(this, new VmExceptionArgs(ex));
            }

            if (ExecutionCompletionReason != ExecutionCompletionReason.Cancelled
                && ExecutionCompletionReason != ExecutionCompletionReason.Exception
                && MachineState != VmState.Stopped)
            {
                MachineState = VmState.Paused;
            }
        }

        /// <summary>
        /// Starts the virtual machine and runs it until the execution cycle is completed for
        /// a reason.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The reason why the execution completed.</returns>
        public async Task RenderShadowScreen(CancellationToken cancellationToken)
        {
            var lastRunStart = _clockProvider.GetCounter();
            var lastRenderFrameStart = lastRunStart;
            SpectrumVm.ShadowScreenDevice.FrameCount = SpectrumVm.ScreenDevice.FrameCount;
            var frameCount = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                var lastFrameEnd = _clockProvider.GetCounter();
                LastRenderFrameTicks = lastFrameEnd - lastRenderFrameStart;
                lastRenderFrameStart = lastFrameEnd;
                frameCount++;

                // --- Do additional task when render frame completed
                var screenTacts = SpectrumVm.ScreenConfiguration.ScreenRenderingFrameTactCount;
                SpectrumVm.ShadowScreenDevice.RenderScreen(0, screenTacts - 1);
                SpectrumVm.ShadowScreenDevice.OnNewFrame();
                var renderFrameArgs = new RenderFrameEventArgs(SpectrumVm.ShadowScreenDevice.GetPixelBuffer());
                RenderFrameCompleted?.Invoke(this, renderFrameArgs);
                if (renderFrameArgs.Cancel)
                {
                    return;
                }

                var waitInTicks = lastRunStart + frameCount * _physicalFrameClockCount
                                  - _clockProvider.GetCounter() - _physicalFrameClockCount * 0.2;
                var waitInMs = 1000.0 * waitInTicks / _clockProvider.GetFrequency();
                if (waitInMs > 0)
                {
                    await Task.Delay((int)waitInMs, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
                else
                {
                    await Task.Delay(1, cancellationToken);
                }
            }
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
            LastExecutionContentionValue = ContentionAccumulated;
            var lastRunStart = _clockProvider.GetCounter();
            var lastRenderFrameStart = lastRunStart;
            var frameCount = 0;
            var completed = false;
            while (!completed)
            {
                // --- Execute a single CPU Frame
                var lastCpuFrameStart = _clockProvider.GetCounter();
                var cycleCompleted = SpectrumVm.ExecuteCycle(cancellationToken, options, true);
                LastCpuFrameTicks = _clockProvider.GetCounter() - lastCpuFrameStart;
                if (!cycleCompleted) return ExecutionCompletionReason.Cancelled;

                // --- Check for emulated keys
                CpuFrameCount++;
                var hasEmulatedKey = SpectrumVm.KeyboardProvider?.EmulateKeyStroke();
                if (hasEmulatedKey != true)
                {
                    // --- Keyboard scan
                    var keyStatusArgs = new KeyStatusEventArgs();
                    KeyScanning?.Invoke(this, keyStatusArgs);
                    foreach (var keyStatus in keyStatusArgs.KeyStatusList)
                    {
                        SpectrumVm.KeyboardDevice.SetStatus(keyStatus);
                    }
                }

                // --- Do additional tasks when CPU frame completed
                var cancelArgs = new CancelEventArgs(false);
                CpuFrameCompleted?.Invoke(this, cancelArgs);
                if (cancelArgs.Cancel) return ExecutionCompletionReason.Cancelled;

                switch (SpectrumVm.ExecutionCompletionReason)
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
                        var renderFrameArgs = new RenderFrameEventArgs(SpectrumVm.ScreenDevice.GetPixelBuffer());
                        RenderFrameCompleted?.Invoke(this, renderFrameArgs);
                        if (renderFrameArgs.Cancel)
                        {
                            return ExecutionCompletionReason.Cancelled;
                        }

                        // --- Wait for the next render frame, unless completed
                        if (!completed)
                        {
                            var waitInTicks = lastRunStart + frameCount * _physicalFrameClockCount
                                - _clockProvider.GetCounter() - _physicalFrameClockCount * 0.2;
                            var waitInMs = 1000.0 * waitInTicks / _clockProvider.GetFrequency();
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
            return SpectrumVm.ExecutionCompletionReason;
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