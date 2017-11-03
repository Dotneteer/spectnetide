using System;
using System.Threading;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class implements the base of a controller that can handle the 
    /// Spectrum virtual machine's states
    /// </summary>
    public abstract class SpectrumVmControllerBase : IVmControlLink,
        IDisposable
    {
        // --- Use it to wait for the start of the VM
        private TaskCompletionSource<bool> _vmStarterCompletionSource;

        // --- Use it to wait for the completion of the VM execution cycle
        private TaskCompletionSource<bool> _executionCompletionSource;

        /// <summary>
        /// The Spectrum virtual machine being controlled
        /// </summary>
        public ISpectrumVm SpectrumVm { get; private set; }

        /// <summary>
        /// The current state of the virtual machine
        /// </summary>
        public VmStatus VmState { get; private set; }

        /// <summary>
        /// The cancellation token source to suspend the virtual machine
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        /// <summary>
        /// The ROM provider to use with the VM
        /// </summary>
        public IRomProvider RomProvider { get; set; }

        /// <summary>
        /// The clock provider to use with the VM
        /// </summary>
        public IClockProvider ClockProvider { get; set; }

        /// <summary>
        /// The pixel renderer to use with the VM
        /// </summary>
        public IScreenFrameProvider ScreenFrameProvider { get; set; }

        /// <summary>
        /// The renderer that creates the beeper and tape sound
        /// </summary>
        public IEarBitFrameProvider EarBitFrameProvider { get; set; }

        /// <summary>
        /// The TZX content provider for the tape device
        /// </summary>
        public ITapeContentProvider LoadContentProvider { get; set; }

        /// <summary>
        /// TZX Save provider for the tape device
        /// </summary>
        public ISaveToTapeProvider SaveToTapeProvider { get; set; }

        /// <summary>
        /// The provider for the keyboard
        /// </summary>
        public IKeyboardProvider KeyboardProvider { get; set; }

        /// <summary>
        /// Provider to manage debug information
        /// </summary>
        public ISpectrumDebugInfoProvider DebugInfoProvider { get; set; }

        /// <summary>
        /// Stack debug provider
        /// </summary>
        public IStackDebugSupport StackDebugSupport { get; set; }

        /// <summary>
        /// Signs if keyboard scan is allowed or disabled
        /// </summary>
        public bool AllowKeyboardScan { get; set; }

        /// <summary>
        /// This event is raised whenever the state of the virtual machine changes
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged;

        /// <summary>
        /// You can use this task to wait for the event when the execution cycle 
        /// notifies the machine controller about the start
        /// </summary>
        public Task StarterTask => _vmStarterCompletionSource.Task;

        /// <summary>
        /// You can use this task to wait for the event when the execution cycle 
        /// completes.
        /// </summary>
        public Task CompletionTask => _executionCompletionSource.Task;

        /// <summary>
        /// Instantiates the machine controller
        /// </summary>
        protected SpectrumVmControllerBase()
        {
            VmState = VmStatus.None;
            SpectrumVm = null;
            VmStateChanged += InternalVmStateChanged;
        }

        #region Control methods

        /// <summary>
        /// Starts the virtual machine with the provided options
        /// </summary>
        /// <param name="options"></param>
        public void StartVm(ExecuteCycleOptions options)
        {
            MoveToState(VmStatus.BuildingMachine);
            BuildMachine();

            // --- We allow this event to execute something on the main thread
            // --- right before the execution cycle starts
            MoveToState(VmStatus.BeforeRun);

            // --- Dispose the previous cancellation token, and create a new one
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();

            // --- We use the completion source to sign that the VM's execution cycle is done
            _vmStarterCompletionSource = new TaskCompletionSource<bool>();
            _executionCompletionSource = new TaskCompletionSource<bool>();

            // --- Get the exception of the execution cycle
            Exception exDuringRun = null;

            Task.Factory.StartNew(ExecutionAction,
                CancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Current);

            // --- Execute the VM cycle
            async void ExecutionAction()
            {
                try
                {
                    SpectrumVm.ExecuteCycle(CancellationTokenSource.Token, options);
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    exDuringRun = ex;
                }

                // --- Forget about the cancellation token
                CancellationTokenSource?.Dispose();
                CancellationTokenSource = null;

                // --- Go back to the main thread before setting up new state
                await SwitchToMainThread();

                // --- Calculate next state
                MoveToState(VmState == VmStatus.Stopping || exDuringRun != null 
                    ? VmStatus.Stopped 
                    : VmStatus.Paused);

                // --- Conclude the execution task
                if (exDuringRun == null)
                {
                    _executionCompletionSource.SetResult(true);
                }
                else
                {
                    _executionCompletionSource.SetException(exDuringRun);
                }
            }
        }

        /// <summary>
        /// Pauses the running virtual machine
        /// </summary>
        public void PauseVm()
        {
            // --- Pause only the running machine
            if (VmState != VmStatus.Running) return;
            MoveToState(VmStatus.Pausing);

            CancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Stops the running virtual machine
        /// </summary>
        public void StopVm()
        {
            // --- Stop only running machine    
            if (VmState != VmStatus.Running) return;
            MoveToState(VmStatus.Stopping);

            CancellationTokenSource.Cancel();
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Checks if the caller runs on the main thread
        /// </summary>
        /// <returns></returns>
        protected abstract bool IsOnMainThread();

        /// <summary>
        /// Override this method to provide the way that the controller can 
        /// switch back to the main (UI) thread.
        /// </summary>
        /// <returns></returns>
        protected abstract Task SwitchToMainThread();

        /// <summary>
        /// Moves the virtual machine to the specified new state
        /// </summary>
        /// <param name="newState">New machine state</param>
        protected void MoveToState(VmStatus newState)
        {
            CheckMainThread();
            var oldState = VmState;
            VmState = newState;
            VmStateChanged?.Invoke(this, new VmStateChangedEventArgs(oldState, VmState));
        }

        /// <summary>
        /// Builds the machine that can be started
        /// </summary>
        protected virtual void BuildMachine()
        {
            if (SpectrumVm == null)
            {
                // --- Create the machine on first start
                SpectrumVm = new Spectrum48(
                    RomProvider,
                    ClockProvider,
                    KeyboardProvider,
                    ScreenFrameProvider,
                    EarBitFrameProvider,
                    LoadContentProvider,
                    SaveToTapeProvider,
                    this);
            }

            // --- We either provider out DebugInfoProvider, or use
            // --- the default one
            if (DebugInfoProvider == null)
            {
                DebugInfoProvider = SpectrumVm.DebugInfoProvider;
            }
            else
            {
                SpectrumVm.DebugInfoProvider = DebugInfoProvider;
            }
            // --- Set up stack debug support
            if (StackDebugSupport != null)
            {
                SpectrumVm.Cpu.StackDebugSupport = StackDebugSupport;
                StackDebugSupport.Reset();
            }

            // --- At this point we have a Spectrum VM.
            // --- Let's reset it
            SpectrumVm.Reset();
        }

        #endregion

        #region IVmControlLink implementation 

        /// <summary>
        /// The vm notifies the controller when its execution cycle is activated on
        /// a background thread.
        /// </summary>
        /// <remarks>
        /// We expect that this link is called from a background thread.
        /// </remarks>
        void IVmControlLink.ExecutionCycleStarted()
        {
            Task.Run(async () =>
            {
                await SwitchToMainThread();
                MoveToState(VmStatus.Running);
            });
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Checks if the controller runs on the main thread
        /// </summary>
        private void CheckMainThread()
        {
            if (!IsOnMainThread())
            {
                throw new NotOnMainThreadException();
            }
        }

        /// <summary>
        /// Handles vm state changes internally within the machine controller
        /// </summary>
        private void InternalVmStateChanged(object sender, VmStateChangedEventArgs e)
        {
            if (e.OldState == VmStatus.BeforeRun && e.NewState == VmStatus.Running)
            {
                // --- We have just got the notification from the execution cycle
                // --- about the successful start.
                _vmStarterCompletionSource.SetResult(true);
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Does the lion's share of disposing the asycn command objects carefully
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            CancellationTokenSource?.Dispose();
            VmStateChanged -= InternalVmStateChanged;
        }

        #endregion
    }
}