using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;
// ReSharper disable ArgumentsStyleLiteral

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// This class represents a Spectrum virtual machine
    /// </summary>
    public sealed class SpectrumVm: IDisposable
    {
        private const ushort DEFAULT_CALL_STUB_ADDRESS = 0x5BA0;

        private readonly ISpectrumVm _spectrumVm;
        private CancellationTokenSource _cancellationTokenSource;

        #region Machine properties

        /// <summary>
        /// The key of the Spectrum model
        /// </summary>
        public string ModelKey { get; }

        /// <summary>
        /// The edition key of the model
        /// </summary>
        public string EditionKey { get; }

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
        public SoundSamples BeeperSamples { get; }

        /// <summary>
        /// Gets the sound (PSG) configuration of the machine
        /// </summary>
        public IAudioConfiguration SoundConfiguration { get; }

        /// <summary>
        /// Gets the sound (PSG) samples of the current rendering frame
        /// </summary>
        public SoundSamples SoundSamples { get; }

        /// <summary>
        /// The collection of breakpoints
        /// </summary>
        public CodeBreakpoints Breakpoints { get; }

        /// <summary>
        /// Runs until the timeout value specified in milliseconds
        /// ellapses.
        /// </summary>
        /// <remarks>Set this value to zero to infinite timeout</remarks>
        public long TimeoutInMs
        {
            get => 1000 * TimeoutTacts / _spectrumVm.BaseClockFrequency / _spectrumVm.ClockMultiplier;
            set => TimeoutTacts = value * _spectrumVm.BaseClockFrequency * _spectrumVm.ClockMultiplier / 1000;
        }

        /// <summary>
        /// Runs until the timeout value specified in CPU tact values
        /// ellapses.
        /// </summary>
        /// <remarks>Set this value to zero to infinite timeout</remarks>
        public long TimeoutTacts { get; set; }

        /// <summary>
        /// Indicates if the machine runs in real time mode
        /// </summary>
        public bool RealTimeMode { get; set; }

        /// <summary>
        /// Gets the reason that tells why the machine has been stopped or paused
        /// </summary>
        public ExecutionCompletionReason ExecutionCompletionReason { get; private set; }

        /// <summary>
        /// Gets the current state of the machine
        /// </summary>
        public VmState MachineState { get; private set; }

        /// <summary>
        /// Indicates if the Spectrum virtual machine runs in debug mode
        /// </summary>
        public bool RunsInDebugMode { get; private set; }

        /// <summary>
        /// The task that represents the completion of the execution cycle
        /// </summary>
        public Task CompletionTask { get; private set; }

        #endregion

        #region Lifecycle methods

        /// <summary>
        /// Creates an instance of the virtual machine
        /// </summary>
        /// <param name="modelKey">The model key of the virtual machine</param>
        /// <param name="editionKey">The edition key of the virtual machine</param>
        /// <param name="devices">Devices to create the machine</param>
        internal SpectrumVm(string modelKey, string editionKey, DeviceInfoCollection devices)
        {
            ModelKey = modelKey;
            EditionKey = editionKey;
            RealTimeMode = false;

            _spectrumVm = new SpectrumEngine(devices);
            Cpu = new CpuZ80(_spectrumVm.Cpu);

            var roms = new List<ReadOnlyMemorySlice>();
            for (var i = 0; i < _spectrumVm.RomConfiguration.NumberOfRoms; i++)
            {
                roms.Add(new ReadOnlyMemorySlice(_spectrumVm.RomDevice.GetRomBytes(i)));
            }
            Roms = new ReadOnlyCollection<ReadOnlyMemorySlice>(roms);

            PagingInfo = new MemoryPagingInfo(_spectrumVm.MemoryDevice);
            Memory = new SpectrumMemoryContents(_spectrumVm.MemoryDevice);

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
            BeeperSamples = new SoundSamples(_spectrumVm.BeeperDevice);
            SoundConfiguration = _spectrumVm.SoundConfiguration;
            SoundSamples = new SoundSamples(_spectrumVm.SoundDevice);
            Breakpoints = new CodeBreakpoints(_spectrumVm.DebugInfoProvider);

            MachineState = VmState.None;
            ExecutionCompletionReason = ExecutionCompletionReason.None;
            IsFirstStart = false;
            IsFirstPause = false;
            TimeoutTacts = 0;
            _spectrumVm.ScreenDevice.FrameCompleted += OnScreenFrameCompleted;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public async void Dispose()
        {
            if (MachineState != VmState.Stopped)
            {
                await Stop();
            }
            _spectrumVm.ScreenDevice.FrameCompleted -= OnScreenFrameCompleted;
        }

        /// <summary>
        /// Raises the VmFrameCompleted event
        /// </summary>
        private void OnScreenFrameCompleted(object sender, EventArgs eventArgs)
        {
            VmFrameCompleted?.Invoke(sender, eventArgs);
        }

        #endregion

        #region Machine control methods and properties

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
        /// Starts the Spectrum machine and runs it on a background thread.
        /// </summary>
        /// <remarks>The task completes when the machine has been started its execution cycle</remarks>
        public Task Start() => Start(new ExecuteCycleOptions(
            timeoutTacts: TimeoutTacts,
            fastTapeMode: true,
            hiddenMode: !RealTimeMode));

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread unless it reaches a breakpoint.
        /// </summary>
        public async Task StartDebug()
        {
            RunsInDebugMode = true;
            await Start(new ExecuteCycleOptions(EmulationMode.Debugger,
                timeoutTacts: TimeoutTacts,
                fastTapeMode: true,
                hiddenMode: !RealTimeMode));
        }

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread until it reaches a 
        /// HALT instruction.
        /// </summary>
        public Task RunUntilHalt() => Start(new ExecuteCycleOptions(
            EmulationMode.UntilHalt,
            timeoutTacts: TimeoutTacts,
            fastTapeMode: true,
            hiddenMode: !RealTimeMode));

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread until the current
        /// frame is completed.
        /// </summary>
        public Task RunUntilFrameCompletion() => Start(new ExecuteCycleOptions(
            EmulationMode.UntilFrameEnds,
            timeoutTacts: TimeoutTacts,
            fastTapeMode: true,
            hiddenMode: !RealTimeMode));

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread until the 
        /// CPU reaches the specified termination point.
        /// </summary>
        /// <param name="address">Termination address</param>
        /// <param name="romIndex">The index of the ROM, provided the address is in ROM</param>
        public Task RunUntilTerminationPoint(ushort address, int romIndex = 0) => Start(
            new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                terminationRom: romIndex,
                terminationPoint: address,
                timeoutTacts: TimeoutTacts,
                fastTapeMode: true,
                hiddenMode: !RealTimeMode));

        /// <summary>
        /// Pauses the Spectrum machine.
        /// </summary>
        /// <remarks>
        /// If the machine is paused or stopped, it leaves the machine in its state.
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public async Task Pause()
        {
            if (MachineState == VmState.None || MachineState == VmState.Stopped) return;

            // --- Prepare the machine to pause
            IsFirstPause = IsFirstStart;
            MoveToState(VmState.Pausing);

            // --- Wait for cancellation
            _cancellationTokenSource?.Cancel();
            await CompletionTask;

            // --- Now, it's been paused
            MoveToState(VmState.Paused);
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
                    // --- The machine is paused, it can be quicky stopped
                    MoveToState(VmState.Stopping);
                    MoveToState(VmState.Stopped);
                    break;

                default:
                    // --- Initiate stop
                    MoveToState(VmState.Stopping);
                    if (_cancellationTokenSource == null)
                    {
                        MoveToState(VmState.Stopped);
                    }
                    else
                    {
                        _cancellationTokenSource.Cancel();
                        await CompletionTask;
                        MoveToState(VmState.Stopped);
                    }
                    break;
            }
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
            await Start(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepInto,
                timeoutTacts: TimeoutTacts,
                fastTapeMode: true,
                hiddenMode: !RealTimeMode));
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
            await Start(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepOver,
                timeoutTacts: TimeoutTacts,
                fastTapeMode: true,
                hiddenMode: !RealTimeMode));
        }

        #endregion

        #region Machine state file function

        /// <summary>
        /// Saves the state of the Spectrum machine into the specified file
        /// </summary>
        /// <param name="filename">Machine state file name</param>
        public Task SaveMachineStateTo(string filename) => Task.FromResult(0);

        /// <summary>
        /// Restores the machine state from the specified file
        /// </summary>
        /// <param name="filename">Machine state file name</param>
        public Task RestoreMachineState(string filename) => Task.FromResult(0);

        #endregion

        #region Code manipulation function

        /// <summary>
        /// Injects the code into the RAM of the machine
        /// </summary>
        /// <param name="address">Start address of the code</param>
        /// <param name="codeArray">Code bytes</param>
        public void InjectCode(ushort address, byte[] codeArray)
        {
            if (MachineState != VmState.Paused)
            {
                throw new InvalidOperationException(
                    "The virtual machine must be in Paused state to allow code injection.");
            }
            if (_spectrumVm is ISpectrumVmRunCodeSupport runSupport)
            {
                // --- Go through all code segments and inject them
                runSupport.InjectCodeToMemory(address, codeArray);
            }
        }

        /// <summary>
        /// Calls the code at the specified subroutine start address
        /// </summary>
        /// <param name="startAddress">Subroutine start address</param>
        /// <param name="callStubAddress">Optional address for a call stub</param>
        /// <remarks>
        /// Generates a call stub and uses it to execute the specified subroutine.
        /// </remarks>
        public async Task CallCode(ushort startAddress, ushort? callStubAddress = null)
        {
            // --- Just for extra safety
            if (!(_spectrumVm is ISpectrumVmRunCodeSupport runSupport))
            {
                return;
            }

            // --- Set the call stub address
            if (callStubAddress == null)
            {
                callStubAddress = DEFAULT_CALL_STUB_ADDRESS;
            }

            // --- Create the call stub
            runSupport.InjectCodeToMemory(callStubAddress.Value, new byte[]
            {
                0xCD,
                (byte)startAddress,
                (byte)(startAddress >> 8)
            });
            var runOptions = new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                timeoutTacts: TimeoutTacts,
                terminationPoint: (ushort)(callStubAddress + 3),
                hiddenMode: true);

            // --- Jump to call stub
            Cpu.PC = callStubAddress.Value;
            await Start(runOptions);
        }

        #endregion

        #region Events

        /// <summary>
        /// This event is raised whenever the state of the virtual machine changes
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged;

        /// <summary>
        /// This event is raised when the engine stops because of an exception
        /// </summary>
        public event EventHandler<VmStoppedWithExceptionEventArgs> VmStoppedWithException;

        /// <summary>
        /// This event is raised when a screen rendering frame is completed
        /// </summary>
        public event EventHandler VmFrameCompleted;

        #endregion

        #region Implementation

        /// <summary>
        /// Starts the machine in a background thread.
        /// </summary>
        /// <param name="options">Options to start the machine with.</param>
        /// <remarks>
        /// Reports completion when the machine starts executing its cycles. The machine can
        /// go into Paused or Stopped state, if the execution options allow, for example, 
        /// when it runs to a predefined breakpoint.
        /// </remarks>
        private async Task Start(ExecuteCycleOptions options)
        {
            if (MachineState == VmState.BeforeRun || MachineState == VmState.Running) return;

            // --- Prepare the machine to run
            IsFirstStart = MachineState == VmState.None || MachineState == VmState.Stopped;
            if (IsFirstStart)
            {
                _spectrumVm.Reset();
            }
            MoveToState(VmState.BeforeRun);
            _spectrumVm.DebugInfoProvider?.PrepareBreakpoints();

            // --- Dispose the previous cancellation token, and create a new one
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            // --- Set up the task that runs the machine
            CompletionTask = new Task(() =>
            {
                ExecutionCompletionReason = ExecutionCompletionReason.None;
                Exception cycleException = null;
                try
                {
                    _spectrumVm.ExecuteCycle(_cancellationTokenSource.Token, options);
                    ExecutionCompletionReason = _spectrumVm.ExecutionCompletionReason;
                }
                catch (TaskCanceledException)
                {
                    ExecutionCompletionReason = ExecutionCompletionReason.Cancelled;
                }
                catch (Exception ex)
                {
                    cycleException = ex;
                    ExecutionCompletionReason = ExecutionCompletionReason.Exception;
                }

                // --- Conclude the execution task
                MoveToState(MachineState == VmState.Stopping
                            || MachineState == VmState.Stopped
                            || cycleException != null
                    ? VmState.Stopped
                    : VmState.Paused);

                if (cycleException != null)
                {
                    VmStoppedWithException?.Invoke(this,
                        new VmStoppedWithExceptionEventArgs(cycleException));
                }
            });

            // --- Start the task that ingnites the machine and waits for its start
            await Task.Run(
                () =>
                {
                    CompletionTask.Start();
                    _spectrumVm.StartedSignal.WaitOne(TimeSpan.FromMilliseconds(1000));
                });

            // --- Now, the machine has been started
            if (!CompletionTask.IsCompleted)
            {
                MoveToState(VmState.Running);
            }
        }

        /// <summary>
        /// Moves the virtual machine to the specified new state
        /// </summary>
        /// <param name="newState">New machine state</param>
        private void MoveToState(VmState newState)
        {
            var oldState = MachineState;
            MachineState = newState;
            VmStateChanged?.Invoke(this, new VmStateChangedEventArgs(oldState, newState));
        }

        #endregion
    }
}