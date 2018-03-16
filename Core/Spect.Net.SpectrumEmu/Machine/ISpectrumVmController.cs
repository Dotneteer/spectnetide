using System;
using System.Threading;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Machine
{
    public interface ISpectrumVmController : IDisposable
    {
        /// <summary>
        /// The Spectrum virtual machine being controlled
        /// </summary>
        ISpectrumVm SpectrumVm { get; }

        /// <summary>
        /// The current state of the virtual machine
        /// </summary>
        VmState VmState { get; }

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
        /// The cancellation token source to suspend the virtual machine
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// The startup configuration of the machine
        /// </summary>
        MachineStartupConfiguration StartupConfiguration { get; set; }

        /// <summary>
        /// Signs if keyboard scan is allowed or disabled
        /// </summary>
        bool AllowKeyboardScan { get; set; }

        /// <summary>
        /// You can use this task to wait for the event when the execution cycle 
        /// notifies the machine controller about the start
        /// </summary>
        Task StarterTask { get; }

        /// <summary>
        /// You can use this task to wait for the event when the execution cycle 
        /// completes.
        /// </summary>
        Task<bool> CompletionTask { get; }

        /// <summary>
        /// This event is raised whenever the state of the virtual machine changes
        /// </summary>
        event EventHandler<VmStateChangedEventArgs> VmStateChanged;

        /// <summary>
        /// This event is raised when the screen of the virtual machine has
        /// been refreshed
        /// </summary>
        event EventHandler<VmScreenRefreshedEventArgs> VmScreenRefreshed;

        /// <summary>
        /// Ensures that there's an instance of the Spectrum virtual machine 
        /// ready to control
        /// </summary>
        void EnsureMachine();

        /// <summary>
        /// Starts the virtual machine with the provided options
        /// </summary>
        /// <param name="options">The execution cycle options to start with</param>
        void StartVm(ExecuteCycleOptions options);

        /// <summary>
        /// Pauses the running virtual machine
        /// </summary>
        void PauseVm();

        /// <summary>
        /// Stops the running virtual machine
        /// </summary>
        void StopVm();

        /// <summary>
        /// Forces a paused state (used for recovering VM state)
        /// </summary>
        void ForcePausedState();

        /// <summary>
        /// Forces a screen refresh
        /// </summary>
        void ForceScreenRefresh();

        /// <summary>
        /// Checks if the caller runs on the main thread
        /// </summary>
        /// <returns></returns>
        bool IsOnMainThread();

        /// <summary>
        /// Override this method to provide the way that the controller can 
        /// switch back to the main (UI) thread.
        /// </summary>
        /// <returns></returns>
        Task ExecuteOnMainThread(Action action);
    }
}