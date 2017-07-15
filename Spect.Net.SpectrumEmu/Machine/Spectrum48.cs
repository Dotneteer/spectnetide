using System.Threading;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Devices.Border;
using Spect.Net.SpectrumEmu.Devices.Interrupt;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Keyboard;
using Spect.Net.SpectrumEmu.Providers;
using Spect.Net.Z80Emu.Core;
// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class represents a ZX Spectrum 48 virtual machine
    /// </summary>
    public class Spectrum48
    {
        /// <summary>
        /// Spectrum 48 Memory
        /// </summary>
        private readonly byte[] _memory;

        /// <summary>
        /// The CPU tick at which the last frame rendering started;
        /// </summary>
        private ulong _lastFrameStartCpuTick;

        /// <summary>
        /// The last rendered ULA tact 
        /// </summary>
        private int _lastRenderedUlaTact;

        /// <summary>
        /// The Z80 CPU of the machine
        /// </summary>
        public Z80 Cpu { get; }

        /// <summary>
        /// The clock used within the VM
        /// </summary>
        public IClockProvider Clock { get; }

        /// <summary>
        /// Display parameters of the VM
        /// </summary>
        public IDisplayParameters DisplayPars { get; }

        /// <summary>
        /// Sound parameters of the VM
        /// </summary>
        public IBeeperParameters BeeperPars { get; }

        /// <summary>
        /// The ULA border device used within the VM
        /// </summary>
        public IBorderDevice BorderDevice { get; }

        /// <summary>
        /// The ULA device that renders the VM screen
        /// </summary>
        public IScreenDevice ScreenDevice { get; }

        /// <summary>
        /// The ULA device that can render the VM screen during
        /// a debugging session
        /// </summary>
        public IScreenDevice ShadowScreenDevice { get; }

        /// <summary>
        /// The ULA device that takes care of raising interrupts
        /// </summary>
        public IInterruptDevice InterruptDevice { get; }

        /// <summary>
        /// The current status of the keyboard
        /// </summary>
        public KeyboardStatus KeyboardStatus { get; }

        /// <summary>
        /// The beeper device attached to the VM
        /// </summary>
        public IBeeperDevice BeeperDevice { get; }

        /// <summary>
        /// The tape device attached to the VM
        /// </summary>
        public ITapeDevice TapeDevice { get; }

        /// <summary>
        /// Debug info provider object
        /// </summary>
        public IDebugInfoProvider DebugInfoProvider { get; private set; }

        /// <summary>
        /// The number of frame tact at which the interrupt signal is generated
        /// </summary>
        public virtual int InterruptTact => 32;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Spectrum48(
            IRomProvider romProvider,
            IClockProvider clockProvider,
            IScreenPixelRenderer pixelRenderer,
            IEarBitPulseProcessor earBitPulseProcessor = null,
            ITzxTapeContentProvider tapeContentProvider = null)
        {
            Cpu = new Z80();
            _memory = new byte[0x10000];
            InitRom(romProvider, "ZXSpectrum48.rom");

            Cpu.ReadMemory = ReadMemory;
            Cpu.WriteMemory = WriteMemory;
            Cpu.ReadPort = ReadPort;
            Cpu.WritePort = WritePort;

            Clock = clockProvider;
            DisplayPars = new DisplayParameters();
            BeeperPars = new BeeperParameters();
            BorderDevice = new BorderDevice();
            ScreenDevice = new ScreenDevice(this, pixelRenderer);
            ShadowScreenDevice = new ScreenDevice(this, pixelRenderer);
            BeeperDevice = new BeeperDevice(this, earBitPulseProcessor);
            TapeDevice = new TapeDevice(this, tapeContentProvider);
            DebugInfoProvider = new NoopDebugInfoProvider();

            InterruptDevice = new InterruptDevice(Cpu, InterruptTact);
            KeyboardStatus = new KeyboardStatus();
            ResetUlaTact();
        }

        /// <summary>
        /// Resets the CPU and the ULA chip
        /// </summary>
        public void Reset()
        {
            Cpu.Reset();
            ScreenDevice.Reset();
            BeeperDevice.Reset();
            TapeDevice.Reset();
            ResetUlaTact();
            InterruptDevice.Reset();
        }

        /// <summary>
        /// Gets the current frame tact according to the CPU tick count
        /// </summary>
        public virtual int CurrentFrameTact => (int) (Cpu.Tacts - _lastFrameStartCpuTick);

        /// <summary>
        /// Resets the ULA tact to start screen rendering from the beginning
        /// </summary>
        public void ResetUlaTact()
        {
            _lastRenderedUlaTact = -1;
        }

        /// <summary>
        /// Sets the debug info provider to the specified object
        /// </summary>
        /// <param name="provider">Provider object</param>
        public void SetDebugInfoProvider(IDebugInfoProvider provider)
        {
            DebugInfoProvider = provider;
        }

        /// <summary>
        /// The main execution cycle of the Spectrum VM
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="mode">Execution emulation mode</param>
        /// <param name="stepMode">Debugging execution mode</param>
        /// <return>True, if the cycle completed; false, if it has been cancelled</return>
        public bool ExecuteCycle(CancellationToken token, EmulationMode mode = EmulationMode.Continuous,
            DebugStepMode stepMode = DebugStepMode.StopAtBreakpoint)
        {
            return ExecuteCycle(token, new ExecuteCycleOptions(mode, stepMode, true));
        }

        /// <summary>
        /// The main execution cycle of the Spectrum VM
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="options">Execution options</param>
        /// <return>True, if the cycle completed; false, if it has been cancelled</return>
        public bool ExecuteCycle(CancellationToken token, ExecuteCycleOptions options)
        {
            // --- Prepare the execution
            var cycleStartCounter = Clock.GetCounter();
            var frameSetStartCounter = cycleStartCounter;
            _lastFrameStartCpuTick = Cpu.Tacts;
            if (options.EmulationMode == EmulationMode.Continuous)
            {
                ResetUlaTact();
            }
            var executedInstructionCount = -1;

            // --- Loop #1: Run until cancelled
            while (!token.IsCancellationRequested)
            {
                DebugInfoProvider.CpuTime = 0;
                DebugInfoProvider.ScreenRenderingTime = 0;

                // --- Loop #2: Process instructions and run ULA logic until the frame ends
                while (Cpu.IsInOpExecution || CurrentFrameTact < DisplayPars.UlaFrameTactCount)
                {
                    if (!Cpu.IsInOpExecution)
                    {
                        // --- The next instruction is about to be executed
                        executedInstructionCount++;

                        // --- Check for a debugging stop point
                        if (options.EmulationMode == EmulationMode.Debugger)
                        {
                            if (IsDebugStop(options.DebugStepMode, executedInstructionCount))
                            {
                                // --- At this point, the cycle should be stopped because of debugging reasons
                                // --- The screen should be refreshed
                                ScreenDevice.SignFrameCompleted();
                                return true;
                            }
                        }
                    }

                    // --- Check for interrupt signal generation
                    InterruptDevice.CheckForInterrupt(CurrentFrameTact);

                    // --- Run a single Z80 instruction
                    var cpuStart = Clock.GetCounter();
                    Cpu.ExecuteCpuCycle();
                    DebugInfoProvider.CpuTime += (ulong) (Clock.GetCounter() - cpuStart);

                    if (token.IsCancellationRequested)
                    {
                        return false;
                    }

                    // --- Run a rendering cycle according to the current CPU tact count
                    var lastTact = CurrentFrameTact;
                    var renderStart = Clock.GetCounter();
                    ScreenDevice.RenderScreen(_lastRenderedUlaTact + 1, lastTact);
                    DebugInfoProvider.ScreenRenderingTime += (ulong) (Clock.GetCounter() - renderStart);
                    _lastRenderedUlaTact = lastTact;

                    // --- Exit if the emulation mode specifies so
                    if (options.EmulationMode == EmulationMode.SingleZ80Instruction && !Cpu.IsInOpExecution
                        || options.EmulationMode == EmulationMode.UntilHalt && (Cpu.StateFlags & Z80StateFlags.Halted) != 0)
                    {
                        return true;
                    }

                    // --- Manage the tape device, trigger appropriate modes
                    TapeDevice.SetTapeMode();

                } // -- End Loop #2

                // --- Calculate debug information
                DebugInfoProvider.FrameTime = (ulong) (Clock.GetCounter() - frameSetStartCounter);
                DebugInfoProvider.UtilityTime = DebugInfoProvider.FrameTime - DebugInfoProvider.CpuTime
                                                - DebugInfoProvider.ScreenRenderingTime;
                DebugInfoProvider.CpuTimeInMs = DebugInfoProvider.CpuTime / (double) Clock.GetFrequency() * 1000;
                DebugInfoProvider.ScreenRenderingTimeInMs =
                    DebugInfoProvider.ScreenRenderingTime / (double) Clock.GetFrequency() * 1000;
                DebugInfoProvider.UtilityTimeInMs =
                    DebugInfoProvider.UtilityTime / (double) Clock.GetFrequency() * 1000;
                DebugInfoProvider.FrameTimeInMs = DebugInfoProvider.FrameTime / (double) Clock.GetFrequency() * 1000;

                BeeperDevice.SignFrameCompleted();
                ScreenDevice.SignFrameCompleted();

                // --- Exit if the emulation mode specifies so
                if (options.EmulationMode == EmulationMode.UntilFrameEnds)
                {
                    return true;
                }

                // --- Exit if the emulation mode specifies so
                if (options.EmulationMode == EmulationMode.UntilNextFrame)
                {
                    return true;
                }

                var nextFrameCounter = frameSetStartCounter + Clock.GetFrequency() / (double) DisplayPars.RefreshRate;
                ScreenDevice.SignFrameCompleted();
                Clock.WaitUntil((long) nextFrameCounter, token);

                // --- Start a new frame and carry on
                var remainingTacts = CurrentFrameTact % DisplayPars.UlaFrameTactCount;
                _lastFrameStartCpuTick = Cpu.Tacts - (ulong) remainingTacts;
                ScreenDevice.StartNewFrame();
                ScreenDevice.RenderScreen(0, remainingTacts);
                _lastRenderedUlaTact = remainingTacts;
                frameSetStartCounter = Clock.GetCounter();

                // --- We start a new beeper frame, too
                BeeperDevice.StartNewFrame();

                // --- Reset the interrupt device
                InterruptDevice.Reset();

                // --- Exit if the emulation mode specifies so
                if (options.EmulationMode == EmulationMode.UntilNextFrameCycle)
                {
                    return true;
                }
            } // --- End Loop #1
            return false;
        }

        /// <summary>
        /// Use this method to refresh the shadow screen while a
        /// debugging session is paused
        /// </summary>
        public void RefreshShadowScreen()
        {
            ShadowScreenDevice.StartNewFrame();
            ShadowScreenDevice.RenderScreen(0, DisplayPars.UlaFrameTactCount - 1);
        }

        /// <summary>
        /// Checks whether the execution cycle should be stopped for debugging
        /// </summary>
        /// <param name="stepMode">Debug setp mode</param>
        /// <param name="executedInstructionCount">
        /// The count of instructions already executed in this cycle
        /// </param>
        /// <returns>True, if the execution should be stopped</returns>
        private bool IsDebugStop(DebugStepMode stepMode, int executedInstructionCount)
        {
            // --- No debug provider, no stop
            if (DebugInfoProvider == null) return false;

            // --- In Step-Into mode we always stop when we're about to
            // --- execute the next instruction
            if (stepMode == DebugStepMode.StepInto)
            {
                return executedInstructionCount > 0;
            }

            // --- In Stop-At-Breakpoint mode we stop only if a predefined
            // --- breakpoint is reached
            if (stepMode == DebugStepMode.StopAtBreakpoint
                && DebugInfoProvider.Breakpoints.Contains(Cpu.Registers.PC))
            {
                // --- We do not stop unless we executed at least one instruction
                return executedInstructionCount > 0;
            }

            // --- We're in Step-Over mode
            if (stepMode == DebugStepMode.StepOver)
            {
                if (DebugInfoProvider.ImminentBreakpoint != null)
                {
                    // --- We also stop, if an imminent breakpoint is reached, and also remove
                    // --- this breakpoint
                    if (DebugInfoProvider.ImminentBreakpoint == Cpu.Registers.PC)
                    {
                        DebugInfoProvider.ImminentBreakpoint = null;
                        return true;
                    }
                }
                else
                {
                    var imminentJustCreated = false;

                    // --- We check for a CALL-like instruction
                    var length = Cpu.GetCallInstructionLength();
                    if (length > 0)
                    {
                        // --- Its a CALL-like instraction, create an imminent breakpoint
                        DebugInfoProvider.ImminentBreakpoint = (ushort)(Cpu.Registers.PC + length);
                        imminentJustCreated = true;
                    }

                    // --- We stop, we executed at least one instruction and if there's no imminent 
                    // --- breakpoint or we've just created one
                    if (executedInstructionCount > 0
                        && (DebugInfoProvider.ImminentBreakpoint == null || imminentJustCreated))
                    {
                        return true;
                    }
                }
            }

            // --- In any other case, we carry on
            return false;
        }

        #region Memory access functions

        /// <summary>
        /// Reads a byte from the memory
        /// </summary>
        /// <param name="addr">Memory address to read</param>
        /// <returns>
        /// The byte value read from memory
        /// </returns>
        public virtual byte ReadMemory(ushort addr)
        {
            var value = _memory[addr];
            //if (addr == 0x5C3B)
            //{
            //    Follow(Cpu.Registers.PC, value, "W");
            //}
            if ((addr & 0xC000) == 0x4000)
            {
                Cpu.Delay(ScreenDevice.GetContentionValue((int)(Cpu.Tacts - _lastFrameStartCpuTick)));
            }
            return value;
        }

        /// <summary>
        /// Reads a byte from the memory
        /// </summary>
        /// <param name="addr">Memory address to read</param>
        /// <returns>
        /// The byte value read from memory
        /// </returns>
        public byte UlaReadMemory(ushort addr)
        {
            var value = _memory[(addr & 0x3FFF) + 0x4000];
            return value;
        }

        /// <summary>
        /// Writes a byte to the memory
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Data byte</param>
        public virtual void WriteMemory(ushort addr, byte value)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (addr & 0xC000)
            {
                case 0x0000:
                    // --- ROM cannot be overwritten
                    return;
                case 0x4000:
                    // --- Handle potential memory contention delay
                    Cpu.Delay(ScreenDevice.GetContentionValue((int)(Cpu.Tacts - _lastFrameStartCpuTick)));
                    break;
            }
            _memory[addr] = value;
        }

        #endregion

        #region I/O Access functions

        /// <summary>
        /// Writes the given <paramref name="value" /> to the
        /// given port specified in <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="value">Value to write</param>
        protected virtual void WritePort(ushort addr, byte value)
        {
            if ((addr & 0x0001) == 0)
            {
                BorderDevice.BorderColor = value & 0x07;
                BeeperDevice.ProcessEarBitValue((value & 0x10) != 0);
                TapeDevice.ProcessMicBitValue((value & 0x08) != 0);
            }
        }

        /// <summary>
        /// Reads a byte from the port specified in <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Value read from the port</returns>
        protected virtual byte ReadPort(ushort addr)
        {
            if ((addr & 0x0001) != 0) return 0xFF;

            var portBits = KeyboardStatus.GetLineStatus((byte) (addr >> 8));
            var earBit = TapeDevice.GetEarBit(Cpu.Tacts);
            if (!earBit)
            {
                portBits = (byte) (portBits & 0b1011_1111);
            }
            return portBits;
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Loads the content of the ROM through the specified provider
        /// </summary>
        /// <param name="romProvider">ROM provider instance</param>
        /// <param name="romResourceName">ROM Resource name</param>
        /// <remarks>
        /// The content of the ROM is copied into the memory
        /// </remarks>
        public void InitRom(IRomProvider romProvider, string romResourceName)
        {
            var romBytes = romProvider.LoadRom(romResourceName);
            romBytes?.CopyTo(_memory, 0);
        }

        #endregion

        /// <summary>
        /// This is a no operation debug info provider
        /// </summary>
        private class NoopDebugInfoProvider : IDebugInfoProvider
        {
            /// <summary>
            /// The component provider should be able to reset itself
            /// </summary>
            public void Reset()
            {
            }

            public NoopDebugInfoProvider()
            {
                Breakpoints = new BreakpointCollection();
            }

            /// <summary>
            /// The currently defined breakpoints
            /// </summary>
            public BreakpointCollection Breakpoints { get; }

            /// <summary>
            /// Gets or sets an imminent breakpoint
            /// </summary>
            public ushort? ImminentBreakpoint { get; set; }

            /// <summary>
            /// Entire time spent within a single ULA frame
            /// </summary>
            public ulong FrameTime { get; set; }

            /// <summary>
            /// Time spent with executing CPU instructions
            /// </summary>
            public ulong CpuTime { get; set; }

            /// <summary>
            /// Time spent with screen rendering
            /// </summary>
            public ulong ScreenRenderingTime { get; set; }

            /// <summary>
            /// Time spent with other utility activities
            /// </summary>
            public ulong UtilityTime { get; set; }

            /// <summary>
            /// Entire time spent within a single ULA frame
            /// </summary>
            public double FrameTimeInMs { get; set; }

            /// <summary>
            /// Time spent with executing CPU instructions
            /// </summary>
            public double CpuTimeInMs { get; set; }

            /// <summary>
            /// Time spent with screen rendering
            /// </summary>
            public double ScreenRenderingTimeInMs { get; set; }

            /// <summary>
            /// Time spent with other utility activities
            /// </summary>
            public double UtilityTimeInMs { get; set; }
        }
    }
}