using System;
using System.Threading;
using System.Threading.Tasks;
using Spect.Net.Spectrum.Ula;
using Spect.Net.Spectrum.Utilities;
using Spect.Net.Z80Emu.Core;

namespace Spect.Net.Spectrum.Machine
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
        /// The CPU tact at which the last frame has started
        /// </summary>
        private ulong _lastFrameStartTick;

        /// <summary>
        /// The native counter value when the last execution cycle started
        /// </summary>
        private long _executionCycleStarted;

        /// <summary>
        /// The native counter value of the real time ellapsed
        /// since the last frame started
        /// </summary>
        private long _frameEllapsedCounter;

        /// <summary>
        /// This task represents the VM that runs in a working thread
        /// </summary>
        private Task _vmRunnerTask;

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
        /// The ULA chip within the machine
        /// </summary>
        public UlaChip Ula { get; }

        /// <summary>
        /// Indicates whether the machine is currently running
        /// </summary>
        public bool IsRunning => _vmRunnerTask != null;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Spectrum48()
        {
            Cpu = new Z80();
            Ula = new UlaChip();
            _memory = new byte[0x10000];
            InitRom("ZXSpectrum48.rom");

            Cpu.ReadMemory = ReadMemory;
            Cpu.WriteMemory = WriteMemory;
            Cpu.ReadPort = ReadPort;
            Cpu.WritePort = WritePort;

            Ula.ScreenRenderer.FetchScreenMemory = ReadMemory;
        }

        /// <summary>
        /// Runs the virtual machine in a background working task
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <returns>The working task</returns>
        public Task RunMachine(CancellationToken token)
        {
            _vmRunnerTask = Task.Run(() =>
            {
                _frameEllapsedCounter = 0;
                ExecuteCycle(token);
            }, token);
            return _vmRunnerTask;
        }

        /// <summary>
        /// Starts the VM in a background task
        /// </summary>
        public void Start()
        {
            _cancellationSource?.Dispose();
            _cancellationSource = new CancellationTokenSource();
            RunMachine(_cancellationSource.Token);
        }

        /// <summary>
        /// Stops the VM that runs in the background task
        /// </summary>
        public void Stop()
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
            Ula.Reset();
            Cpu.Reset();
        }

        /// <summary>
        /// Starts a new frame with an optional frame reset
        /// </summary>
        /// <param name="withFrameReset">
        /// When true, the previous frame's remainder is not rendered
        /// </param>
        public void StartNewFrame(bool withFrameReset = false)
        {
            if (!withFrameReset)
            {
                var remainder = (Cpu.Ticks - _lastFrameStartTick) % (ulong)Ula.DisplayParameters.UlaFrameTactCount;
                Ula.ScreenRenderer.StartNewFrame((int)remainder);
            }
            _lastFrameStartTick = Cpu.Ticks;
        }

        /// <summary>
        /// The main execution cycle of the Spectrum VM
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="mode">Execution emulation mode</param>
        public void ExecuteCycle(CancellationToken token, EmulationMode mode = EmulationMode.Continuous)
        {
            _executionCycleStarted = Ula.Clock.GetNativeCounter() - _frameEllapsedCounter;
            var renderedFameCount = 0;

            // --- Run until cancelled
            while (!token.IsCancellationRequested)
            {
                // --- Process instructions and run ULA logic until the frame ends
                while (Cpu.IsInOpExecution 
                    || Cpu.Ticks - _lastFrameStartTick < (ulong)Ula.DisplayParameters.UlaFrameTactCount)
                {
                    // --- Run a single Z80 instruction
                    Cpu.ExecuteCpuCycle();

                    // --- Run a rendering cycle according to the current CPU tact count
                    // TODO: Execute the next rendering slot

                    if (mode == EmulationMode.SingleZ80Instruction && !Cpu.IsInOpExecution
                        || mode == EmulationMode.UntilHalt && Cpu.HALTED)
                    {
                        break;
                    }
                }

                if (mode == EmulationMode.UntilFrameEnds)
                {
                    break;
                }

                // --- Wait while the real frame time comes
                var nextFrameCounter = _executionCycleStarted + (renderedFameCount + 1)
                    * Ula.Clock.PerformanceFrequency/(double)Ula.DisplayParameters.RefreshRate;
                Ula.Clock.WaitUntil((long)nextFrameCounter, token);

                if (mode == EmulationMode.UntilNextFrame)
                {
                    break;
                }

                // --- Start a new frame and carry on
                renderedFameCount++;
                StartNewFrame();
            }

            // --- Store the ellapsed frame counter so that the next ExecuteCycle can
            // --- carry on from that point
            var renderedFrameTime = renderedFameCount
                * Ula.Clock.PerformanceFrequency/(double) Ula.DisplayParameters.RefreshRate;
            _frameEllapsedCounter = Ula.Clock.GetNativeCounter() - _executionCycleStarted 
                - (long)renderedFrameTime;
        }

        #region Memory access functions

        /// <summary>
        /// Reads a byte from the memory
        /// </summary>
        /// <param name="addr">Memory address to read</param>
        /// <returns>
        /// The byte value read from memory
        /// </returns>
        protected virtual byte ReadMemory(ushort addr)
        {
            var value = _memory[addr];
            if ((addr & 0xC000) == 0x4000)
            {
                // TODO: Handle memory contention by incrementing the CPU tact count
            }
            return value;
        }

        /// <summary>
        /// Writes a byte to the memory
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Data byte</param>
        protected virtual void WriteMemory(ushort addr, byte value)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (addr & 0xC000)
            {
                case 0x0000:
                    // --- ROM cannot be overwritten
                    return;
                case 0x4000:
                    // TODO: Handle memory contention by incrementing the CPU tact count
                    // --- ROM cannot be overwritten
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

        public void InitRom(string romResourceName)
        {
            var romBytes = RomHelper.ExtractResourceFile(romResourceName);
            romBytes.CopyTo(_memory, 0);
        }

        #endregion
    }
}