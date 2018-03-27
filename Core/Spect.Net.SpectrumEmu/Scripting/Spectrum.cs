using System.Collections.Generic;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// This class represents a Spectrum virtual machine
    /// </summary>
    public sealed class Spectrum
    {
        #region Machine properties

        /// <summary>
        /// The CPU of the machine
        /// </summary>
        public CpuZ80 Cpu { get; private set; }

        /// <summary>
        /// Provides access to the individual ROM pages of the machine
        /// </summary>
        public IReadOnlyList<RomPage> Roms { get; private set; }

        /// <summary>
        /// Gets the number of ROM pages
        /// </summary>
        public int RomCount => Roms.Count;
        
        /// <summary>
        /// Allows to obtain paging information about the memory
        /// </summary>
        public MemoryPagingInfo PagingInfo { get; private set; }
        
        /// <summary>
        /// The current Contents of the machine's 64K addressable memory
        /// </summary>
        public SpectrumMemoryContents Memory { get; private set; }

        /// <summary>
        /// Provides access to the individual RAM banks of the machine
        /// </summary>
        public IReadOnlyList<MemoryBank> RamBanks { get; private set; }

        /// <summary>
        /// Gets the number of RAM banks
        /// </summary>
        public int RamBankCount => RamBanks.Count;

        /// <summary>
        /// Allows to emulate keyboard keys and query the keyboard state
        /// </summary>
        public KeyboardEmulator Keyboard { get; private set; }

        /// <summary>
        /// Allows read-only access to screen rendering configuration
        /// </summary>
        public ScreenConfiguration ScreenConfiguration { get; private set; }

        /// <summary>
        /// Allows read-only access to the screen rendering table
        /// </summary>
        public ScreenRenderingTable ScreenRenderingTable { get; private set; }

        /// <summary>
        /// A bitmap that represents the current visible screen's pixels, including the border
        /// </summary>
        public ScreenBitmap ScreenBitmap { get; private set; }

        /// <summary>
        /// Gets the current screen rendering status of the machine.
        /// </summary>
        public ScreenRenderingStatus ScreenRenderingStatus { get; private set; }

        /// <summary>
        /// Gets the beeper configuration of the machine
        /// </summary>
        public IAudioConfiguration BeeperConfiguration { get; private set; }

        /// <summary>
        /// Gets the beeper samples of the current rendering frame
        /// </summary>
        public BeeperSamples BeeperSamples { get; private set; }

        /// <summary>
        /// Gets the sound (PSG) configuration of the machine
        /// </summary>
        public IAudioConfiguration SoundConfiguration { get; private set; }

        /// <summary>
        /// Gets the sound (PSG) samples of the current rendering frame
        /// </summary>
        public SoundSamples SoundSamples { get; private set; }

        /// <summary>
        /// The collection of breakpoints
        /// </summary>
        public CodeBreakpoints Breakpoints { get; private set; }

        /// <summary>
        /// Runs until the timeout value specified in milliseconds
        /// ellapses.
        /// </summary>
        /// <remarks>Set this value to zero to infinite timeout</remarks>
        public int TimeoutInMs { get; set; }

        /// <summary>
        /// Runs until the timeout value specified in CPU tact values
        /// ellapses.
        /// </summary>
        /// <remarks>Set this value to zero to infinite timeout</remarks>
        public int TimeoutInTacts { get; set; }

        /// <summary>
        /// Gets the reason that tells why the machine has been stopped or paused
        /// </summary>
        public ExecutionCompletionReason ExecutionCompletionReason { get; private set; }

        /// <summary>
        /// Gets the current state of the machine
        /// </summary>
        public VmState MachineState { get; private set; }
        
        #endregion

        #region Machine control methods

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread.
        /// </summary>
        /// <remarks>The task completes when the machine has been started its execution cycle</remarks>
        public Task Start() => Task.FromResult(0);

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread unless it reaches a breakpoint.
        /// </summary>
        public Task StartDebug() => Task.FromResult(0);

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread until it reaches a 
        /// HALT instruction.
        /// </summary>
        public Task RunUntilHalt() => Task.FromResult(0);

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread until the current
        /// frame is completed.
        /// </summary>
        public Task RunUntilFrameCompletion() => Task.FromResult(0);

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread until the 
        /// CPU reaches the specified termination point.
        /// </summary>
        /// <param name="address">Termination address</param>
        /// <param name="romIndex">The index of the ROM, provided the address is in ROM</param>
        public Task RunUntilTerminationPoint(ushort address, int romIndex = 0) => Task.FromResult(0);

        /// <summary>
        /// Pauses the Spectrum machine.
        /// </summary>
        /// <remarks>
        /// If the machine is paused or stopped, it leaves the machine in its state.
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public Task Pause() => Task.FromResult(0);

        /// <summary>
        /// Stops the Spectrum machine.
        /// </summary>
        /// <remarks>
        /// If the machine is paused or stopped, it leaves the machine in its state.
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public Task Stop() => Task.FromResult(0);

        /// <summary>
        /// Executes the subsequent Z80 instruction.
        /// </summary>
        /// <remarks>
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public Task StepInto() => Task.FromResult(0);

        /// <summary>
        /// Executes the subsequent Z80 CALL, RST, or block instruction entirely.
        /// </summary>
        /// <remarks>
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public Task StepOver() => Task.FromResult(0);

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
        }

        /// <summary>
        /// Calls the code at the specified subroutine start address
        /// </summary>
        /// <param name="startAddress">Subroutine start address</param>
        /// <param name="callStubAddress">Optional address for a call stub</param>
        /// <remarks>
        /// Generates a call stub and uses it to execute the specified subroutine.
        /// </remarks>
        public void CallCode(ushort startAddress, ushort? callStubAddress = null)
        {

        }

        #endregion

        #region Implementation

        #endregion
    }
}