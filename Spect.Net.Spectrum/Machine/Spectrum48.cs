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
        private ulong _lastFrameStarts;

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
        public bool IsRunning { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Spectrum48()
        {
            Cpu = new Z80();
            Ula = new UlaChip();
            _memory = new byte[0x10000];
            InitRom("ZXSpectrum48.rom");

            Cpu.ReadMemory = ReadMemory;
            Ula.ScreenRenderer.FetchScreenMemory = ReadMemory;
        }

        public Task RunMachine(CancellationToken token)
        {
            var runnerTask = Task.Run(() =>
            {
                ExecuteCycle(token);
            }, token);
            return runnerTask;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Reset()
        {
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
                var remainder = (Cpu.Ticks - _lastFrameStarts) % (ulong)Ula.DisplayParameters.UlaFrameTactCount;
                Ula.ScreenRenderer.StartNewFrame((int)remainder);
            }
            _lastFrameStarts = Cpu.Ticks;
        }

        public void ExecuteCycle(CancellationToken token, EmulationMode mode = EmulationMode.Continuous)
        {
            while (!token.IsCancellationRequested)
            {
                while (Cpu.Ticks - _lastFrameStarts < (ulong)Ula.DisplayParameters.UlaFrameTactCount)
                {
                    // --- Run a single Z80 instruction
                    // TODO: Implement Z80 execution cycle

                    // --- Run a rendering cycle according to the current CPU tact count
                    if (mode == EmulationMode.SingleZ80Instruction)
                    {
                        break;
                    }
                }

                // --- Start a new frame
                StartNewFrame();
                if (mode == EmulationMode.UntilFrameEnds)
                {
                    break;
                }

                // --- Wait while the real frame time comes
                // TODO: Wait for the time frame's end
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

        #region Helper functions

        public void InitRom(string romResourceName)
        {
            var romBytes = RomHelper.ExtractResourceFile(romResourceName);
            romBytes.CopyTo(_memory, 0);
        }

        #endregion
    }
}