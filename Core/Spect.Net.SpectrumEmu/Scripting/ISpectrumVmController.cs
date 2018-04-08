using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Scripting
{
    public interface ISpectrumVmController
    {
        /// <summary>
        /// Signs that this is the very first start of the
        /// virtual machine 
        /// </summary>
        bool IsFirstStart { get; }

        /// <summary>
        /// Signs that this is the very first paused state
        /// of the virtual machine
        /// </summary>
        bool IsFirstPause { get; }

        /// <summary>
        /// Gets the current state of the machine
        /// </summary>
        VmState MachineState { get; }

        /// <summary>
        /// Indicates if the Spectrum virtual machine runs in debug mode
        /// </summary>
        bool RunsInDebugMode { get; }

        /// <summary>
        /// The task that represents the completion of the execution cycle
        /// </summary>
        Task CompletionTask { get; }

        /// <summary>
        /// Sets the debug mode
        /// </summary>
        /// <param name="mode">Treu, if the machine should run in debug mode</param>
        void SetDebugMode(bool mode);

        /// <summary>
        /// Pauses the Spectrum machine.
        /// </summary>
        /// <remarks>
        /// If the machine is paused or stopped, it leaves the machine in its state.
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        Task Pause();

        /// <summary>
        /// Stops the Spectrum machine.
        /// </summary>
        /// <remarks>
        /// If the machine is paused or stopped, it leaves the machine in its state.
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        Task Stop();

        /// <summary>
        /// Starts the machine in a background thread.
        /// </summary>
        /// <param name="options">Options to start the machine with.</param>
        /// <remarks>
        /// Reports completion when the machine starts executing its cycles. The machine can
        /// go into Paused or Stopped state, if the execution options allow, for example, 
        /// when it runs to a predefined breakpoint.
        /// </remarks>
        void Start(ExecuteCycleOptions options);

        /// <summary>
        /// Forces the machine into Paused state
        /// </summary>
        void ForcePausedState();
    }
}