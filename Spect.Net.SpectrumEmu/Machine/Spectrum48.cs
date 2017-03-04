using System.Threading;
using Spect.Net.SpectrumEmu.Keyboard;
using Spect.Net.SpectrumEmu.Ula;
using Spect.Net.Z80Emu.Core;

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
        public UlaClock Clock { get; }

        /// <summary>
        /// Display parameters of the VM
        /// </summary>
        public DisplayParameters DisplayPars { get; }

        /// <summary>
        /// The ULA border device used within the VM
        /// </summary>
        public UlaBorderDevice BorderDevice { get; }

        /// <summary>
        /// The ULA device that renders the VM screen
        /// </summary>
        public UlaScreenDevice ScreenDevice { get; }

        /// <summary>
        /// The ULA device that takes care of raising interrupts
        /// </summary>
        public UlaInterruptDevice InterruptDevice { get; }

        /// <summary>
        /// The current status of the keyboard
        /// </summary>
        public KeyboardStatus KeyboardStatus { get; }

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
            IHighResolutionClockProvider clockProvider,
            IScreenPixelRenderer pixelRenderer)
        {
            Cpu = new Z80();
            _memory = new byte[0x10000];
            InitRom(romProvider, "ZXSpectrum48.rom");

            Cpu.ReadMemory = ReadMemory;
            Cpu.WriteMemory = WriteMemory;
            Cpu.ReadPort = ReadPort;
            Cpu.WritePort = WritePort;

            Clock = new UlaClock(clockProvider);
            DisplayPars = new DisplayParameters();
            BorderDevice = new UlaBorderDevice();
            ScreenDevice = new UlaScreenDevice(DisplayPars, pixelRenderer, BorderDevice, UlaReadMemory);
            // ReSharper disable once VirtualMemberCallInConstructor
            InterruptDevice = new UlaInterruptDevice(Cpu, InterruptTact);
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
            ResetUlaTact();
        }

        /// <summary>
        /// Gets the current frame tact according to the CPU tick count
        /// </summary>
        public int CurrentFrameTact => (int)(Cpu.Ticks - _lastFrameStartCpuTick);

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
            var startCounter = Clock.GetNativeCounter();
            _lastFrameStartCpuTick = Cpu.Ticks;
            if (mode == EmulationMode.Continuous)
            {
                ResetUlaTact();
            }
            var renderedFameCount = 0;
            var executedInstructionCount = -1;

            // --- Run until cancelled
            while (!token.IsCancellationRequested)
            {
                // --- Process instructions and run ULA logic until the frame ends
                while (Cpu.IsInOpExecution || CurrentFrameTact < DisplayPars.UlaFrameTactCount)
                {
                    if (!Cpu.IsInOpExecution)
                    {
                        // --- The next instruction is about to be executed
                        executedInstructionCount++;
                        var imminentBreakpointJustSet = false;

                        // --- Check for a debugging stop point
                        if (mode == EmulationMode.Debugger)
                        {
                            if (stepMode == DebugStepMode.StepOver)
                            {
                                // --- Step-over mode may require in imminent breakpoint
                                imminentBreakpointJustSet = SetImminentBreakpoint();
                            }

                            if (IsDebugStop(stepMode, executedInstructionCount) || imminentBreakpointJustSet)
                            {
                                // --- At this point, the cycle should be stopped because of debugging reasons
                                // --- The screen should be refreshed
                                ScreenDevice.SignFrameReady();

                                // --- Previously set imminent breakpoint should be cleared
                                if (DebugInfoProvider != null && !imminentBreakpointJustSet)
                                {
                                    DebugInfoProvider.ImminentBreakpoint = null;
                                }
                                return true;
                            }
                        }
                    }

                    // --- Check for interrupt signal generation
                    InterruptDevice.CheckForInterrupt(CurrentFrameTact);

                    // --- Run a single Z80 instruction
                    Cpu.ExecuteCpuCycle();
                    if (token.IsCancellationRequested)
                    {
                        return false;
                    }

                    // --- Run a rendering cycle according to the current CPU tact count
                    var lastTact = CurrentFrameTact;
                    ScreenDevice.RenderScreen(_lastRenderedUlaTact + 1, lastTact);
                    _lastRenderedUlaTact = lastTact;

                    // --- Exit if the emulation mode specifies so
                    if (mode == EmulationMode.SingleZ80Instruction && !Cpu.IsInOpExecution
                        || mode == EmulationMode.UntilHalt && Cpu.HALTED)
                    {
                        return true;
                    }
                }

                // --- Now, the entire frame is rendered
                ScreenDevice.SignFrameReady();

                // --- Exit if the emulation mode specifies so
                if (mode == EmulationMode.UntilFrameEnds)
                {
                    return true;
                }

                // --- Wait while the real frame time comes
                var nextFrameCounter = startCounter + (renderedFameCount + 1)
                    * Clock.Frequency/(double)DisplayPars.RefreshRate;
                Clock.WaitUntil((long)nextFrameCounter, token);

                // --- Exit if the emulation mode specifies so
                if (mode == EmulationMode.UntilNextFrame)
                {
                    return true;
                }

                // --- Start a new frame and carry on
                renderedFameCount++;
                var remainingTacts = CurrentFrameTact % DisplayPars.UlaFrameTactCount;
                _lastFrameStartCpuTick = Cpu.Ticks - (ulong)remainingTacts;
                ScreenDevice.StartNewFrame();
                ScreenDevice.RenderScreen(0, remainingTacts);
                _lastRenderedUlaTact = remainingTacts;

                // --- Reset the interrupt device
                InterruptDevice.Reset();

                // --- Exit if the emulation mode specifies so
                if (mode == EmulationMode.UntilNextFrameCycle)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets an imminent breakpoint for the step-over mode
        /// </summary>
        private bool SetImminentBreakpoint()
        {
            if (DebugInfoProvider == null || DebugInfoProvider.ImminentBreakpoint != null) return false;
            var length = Cpu.GetCallInstructionLength();
            if (length == 0) return false;

            DebugInfoProvider.ImminentBreakpoint = (ushort)(Cpu.Registers.PC + length);
            return true;
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

            // --- We do not stop when starting from a breakpoint
            if (executedInstructionCount <= 0) return false;

            // --- In Step-Into mode we stop
            if (stepMode == DebugStepMode.StepInto) return true;

            // --- Always stop at breakpoints
            if (DebugInfoProvider.ImminentBreakpoint == Cpu.Registers.PC) return true;
            if (DebugInfoProvider.Breakpoints.Contains(Cpu.Registers.PC)) return true;

            // --- In Step-Over mode we stop when there's no imminent breakpoint set
            if (stepMode == DebugStepMode.StepOver)
            {
                return DebugInfoProvider.ImminentBreakpoint == null;
            }

            // --- An any other cases we carry on
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
            if ((addr & 0xC000) == 0x4000)
            {
                Cpu.Delay(ScreenDevice.GetContentionValue((int)(Cpu.Ticks - _lastFrameStartCpuTick)));
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
        private byte UlaReadMemory(ushort addr)
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
                    Cpu.Delay(ScreenDevice.GetContentionValue((int)(Cpu.Ticks - _lastFrameStartCpuTick)));
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
            // --- Set border value
            if ((addr & 0x0001) == 0)
            {
                BorderDevice.BorderColor = value & 0x07;
            }
        }

        /// <summary>
        /// Reads a byte from the port specified in <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Value read from the port</returns>
        protected virtual byte ReadPort(ushort addr)
        {
            return (addr & 0x0001) == 0
                ? KeyboardStatus.GetLineStatus((byte)(addr >> 8))
                : (byte) 0xFF;
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
    }
}