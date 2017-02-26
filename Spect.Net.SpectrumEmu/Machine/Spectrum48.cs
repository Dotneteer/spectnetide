using System;
using System.Threading;
using System.Threading.Tasks;
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
        /// This task represents the VM that runs in a working thread
        /// </summary>
        private Task _vmRunnerTask;

        /// <summary>
        /// The CPU tick at which the last frame rendering started;
        /// </summary>
        private ulong _lastFrameStartCpuTick;

        /// <summary>
        /// The source of the cancellation token that can cancel the
        /// VM task
        /// </summary>
        private CancellationTokenSource _cancellationSource;

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
        /// The number of frame tact at which the interrupt signal is generated
        /// </summary>
        public virtual int InterruptTact => 32;

        /// <summary>
        /// Indicates whether the machine is currently running
        /// </summary>
        public bool IsRunning => _vmRunnerTask != null;

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
        }

        /// <summary>
        /// Runs the virtual machine in a background working task
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="startAddress">Optional start address</param>
        /// <returns>The working task</returns>
        public Task RunMachine(CancellationToken token, ushort startAddress = 0)
        {
            _vmRunnerTask = Task.Run(() =>
            {
                ExecuteCycle(token);
            }, token);
            return _vmRunnerTask;
        }

        /// <summary>
        /// Starts the VM in a background task
        /// </summary>
        /// <param name="startAddress">Optional start address</param>
        public void Start(ushort startAddress = 0)
        {
            _cancellationSource?.Dispose();
            _cancellationSource = new CancellationTokenSource();
            RunMachine(_cancellationSource.Token);
        }

        /// <summary>
        /// Pauses the VM that runs in the background task
        /// </summary>
        public void Pause()
        {
            _cancellationSource.Cancel();
            _vmRunnerTask.Wait(TimeSpan.FromMilliseconds(100));
            _vmRunnerTask = null;
        }

        /// <summary>
        /// Resets the CPU and the ULA chip
        /// </summary>
        public void Reset()
        {
            if (!IsRunning) return;
            Cpu.Reset();
        }

        /// <summary>
        /// Gets the current frame tact according to the CPU tick count
        /// </summary>
        public int CurrentFrameTact => (int)(Cpu.Ticks - _lastFrameStartCpuTick);

        /// <summary>
        /// The main execution cycle of the Spectrum VM
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="mode">Execution emulation mode</param>
        public void ExecuteCycle(CancellationToken token, EmulationMode mode = EmulationMode.Continuous)
        {
            var startCounter = Clock.GetNativeCounter();
            _lastFrameStartCpuTick = Cpu.Ticks;
            var lastRenderedTact = -1;
            var renderedFameCount = 0;

            // --- Run until cancelled
            while (!token.IsCancellationRequested)
            {
                // --- Process instructions and run ULA logic until the frame ends
                while (Cpu.IsInOpExecution || CurrentFrameTact < DisplayPars.UlaFrameTactCount)
                {
                    // --- Check for interrupt signal generation
                    InterruptDevice.CheckForInterrupt(CurrentFrameTact);

                    // --- Run a single Z80 instruction
                    Cpu.ExecuteCpuCycle();
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    // --- Run a rendering cycle according to the current CPU tact count
                    var lastTact = CurrentFrameTact;
                    ScreenDevice.RenderScreen(lastRenderedTact + 1, lastTact);
                    lastRenderedTact = lastTact;

                    // --- Exit if the emulation mode specifies so
                    if (token.IsCancellationRequested
                        || mode == EmulationMode.SingleZ80Instruction && !Cpu.IsInOpExecution
                        || mode == EmulationMode.UntilHalt && Cpu.HALTED)
                    {
                        return;
                    }
                }

                // --- Now, the entire frame is rendered
                ScreenDevice.SignFrameReady();

                // --- Exit if the emulation mode specifies so
                if (token.IsCancellationRequested 
                    || mode == EmulationMode.UntilFrameEnds)
                {
                    return;
                }

                // --- Wait while the real frame time comes
                var nextFrameCounter = startCounter + (renderedFameCount + 1)
                    * Clock.Frequency/(double)DisplayPars.RefreshRate;
                Clock.WaitUntil((long)nextFrameCounter, token);

                // --- Exit if the emulation mode specifies so
                if (token.IsCancellationRequested
                    || mode == EmulationMode.UntilNextFrame)
                {
                    return;
                }

                // --- Start a new frame and carry on
                renderedFameCount++;
                var remainingTacts = CurrentFrameTact % DisplayPars.UlaFrameTactCount;
                _lastFrameStartCpuTick = Cpu.Ticks - (ulong)remainingTacts;
                ScreenDevice.StartNewFrame();
                ScreenDevice.RenderScreen(0, remainingTacts);
                lastRenderedTact = remainingTacts;

                // --- Reset the interrupt device
                InterruptDevice.Reset();

                // --- Exit if the emulation mode specifies so
                if (token.IsCancellationRequested
                    || mode == EmulationMode.UntilNextFrameCycle)
                {
                    return;
                }
            }
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
            return 0xFF;
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