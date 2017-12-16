using System;
using System.Threading;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

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
        public VmState VmState { get; private set; }

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
        /// The cancellation token source to suspend the virtual machine
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        /// <summary>
        /// The startup configuration of the machine
        /// </summary>
        public MachineStartupConfiguration StartupConfiguration { get; set; }

        /// <summary>
        /// Signs if keyboard scan is allowed or disabled
        /// </summary>
        public bool AllowKeyboardScan { get; set; }

        /// <summary>
        /// This event is raised whenever the state of the virtual machine changes
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged;

        /// <summary>
        /// This event is raised when the screen of the virtual machine has
        /// been refreshed
        /// </summary>
        public event EventHandler<VmScreenRefreshedEventArgs> VmScreenRefreshed;

        /// <summary>
        /// You can use this task to wait for the event when the execution cycle 
        /// notifies the machine controller about the start
        /// </summary>
        public Task StarterTask => _vmStarterCompletionSource?.Task;

        /// <summary>
        /// You can use this task to wait for the event when the execution cycle 
        /// completes.
        /// </summary>
        public Task CompletionTask => _executionCompletionSource?.Task;

        /// <summary>
        /// Instantiates the machine controller
        /// </summary>
        protected SpectrumVmControllerBase()
        {
            VmState = VmState.None;
            SpectrumVm = null;
        }

        #region Control methods

        /// <summary>
        /// Ensures that there's an instance of the Spectrum virtual machine 
        /// ready to control
        /// </summary>
        public void EnsureMachine()
        {
            BuildMachine();
        }

        /// <summary>
        /// Starts the virtual machine with the provided options
        /// </summary>
        /// <param name="options">The execution cycle options to start with</param>
        public void StartVm(ExecuteCycleOptions options)
        {
            if (VmState == VmState.Running) return;

            IsFirstStart = VmState == VmState.None 
                || VmState == VmState.BuildingMachine
                || VmState == VmState.Stopped;
            if (IsFirstStart)
            {
                EnsureMachine();
            }

            // --- We allow this event to execute something on the main thread
            // --- right before the execution cycle starts
            MoveToState(VmState.BeforeRun);

            // --- Dispose the previous cancellation token, and create a new one
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();

            // --- We use the completion source to sign that the VM's execution cycle is done
            _vmStarterCompletionSource = new TaskCompletionSource<bool>();
            _executionCompletionSource = new TaskCompletionSource<bool>();

            // --- Get the exception of the execution cycle
            Exception exDuringRun = null;

            // --- Allow the controller to save its current scheduler context
            SaveMainContext();
            SpectrumVm.DebugInfoProvider?.PrepareBreakpoints();

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
                await ExecuteOnMainThread(() =>
                {
                    // --- Calculate next state
                    MoveToState(VmState == VmState.Stopping || exDuringRun != null
                        ? VmState.Stopped
                        : VmState.Paused);

                    // --- Conclude the execution task
                    if (exDuringRun == null)
                    {
                        _executionCompletionSource.SetResult(true);
                    }
                    else
                    {
                        _executionCompletionSource.SetException(exDuringRun);
                        OnVmStoppedWithException(exDuringRun);
                    }
                });
            }
        }

        /// <summary>
        /// Override this method to handle the exception of the virtual machine
        /// </summary>
        /// <param name="exDuringRun">Exception that caused the vm to stop</param>
        protected virtual void OnVmStoppedWithException(Exception exDuringRun)
        {
        }

        /// <summary>
        /// Pauses the running virtual machine
        /// </summary>
        public void PauseVm()
        {
            // --- Pause only the running machine
            if (VmState != VmState.Running) return;

            IsFirstPause = IsFirstStart;
            MoveToState(VmState.Pausing);
            CancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Stops the running virtual machine
        /// </summary>
        public void StopVm()
        {
            // --- Stop only running machine    
            if (VmState == VmState.Stopped) return;

            if (VmState == VmState.Paused)
            {
                // --- The machine is paused, it can be quicky stopped
                MoveToState(VmState.Stopping);
                MoveToState(VmState.Stopped);
            }
            else
            {
                // --- Initiate stop
                MoveToState(VmState.Stopping);
                CancellationTokenSource?.Cancel();
            }
        }

        #endregion

        #region Overridable methods

        /// <summary>
        /// Override this method to define how to save the main context
        /// </summary>
        public virtual void SaveMainContext() { }

        /// <summary>
        /// Checks if the caller runs on the main thread
        /// </summary>
        /// <returns></returns>
        public abstract bool IsOnMainThread();

        /// <summary>
        /// Override this method to provide the way that the controller can 
        /// switch back to the main (UI) thread.
        /// </summary>
        /// <returns></returns>
        public abstract Task ExecuteOnMainThread(Action action);

        /// <summary>
        /// Moves the virtual machine to the specified new state
        /// </summary>
        /// <param name="newState">New machine state</param>
        protected void MoveToState(VmState newState)
        {
            CheckMainThread();
            var oldState = VmState;
            VmState = newState;
            OnVmStateChanged(oldState, VmState);
            VmStateChanged?.Invoke(this, new VmStateChangedEventArgs(oldState, VmState));
            if (oldState == VmState.BeforeRun && newState == VmState.Running)
            {
                // --- We have just got the notification from the execution cycle
                // --- about the successful start.
                _vmStarterCompletionSource.SetResult(true);
            }
        }

        /// <summary>
        /// Overrid this method to handle vm state changes within the controller
        /// </summary>
        /// <param name="oldState">Old VM state</param>
        /// <param name="newState">New VM state</param>
        protected virtual void OnVmStateChanged(VmState oldState, VmState newState)
        {
        }

        /// <summary>
        /// Builds the machine that can be started
        /// </summary>
        protected virtual void BuildMachine()
        {
            if (SpectrumVm == null)
            {
                MoveToState(VmState.BuildingMachine);

                if (StartupConfiguration == null)
                {
                    throw new InvalidOperationException("You must provide a startup configuration for " +
                        "the virtual machine, it cannot be null");
                }

                // --- Create the machine on first start
                SpectrumVm = new Spectrum48(StartupConfiguration.DeviceData, this);
                SpectrumVm.ScreenDevice.FrameCompleted +=
                    (s, e) => VmScreenRefreshed?.Invoke(s, 
                        new VmScreenRefreshedEventArgs(SpectrumVm.ScreenDevice.GetPixelBuffer()));
            }

            // --- We either provider out DebugInfoProvider, or use
            // --- the default one
            if (StartupConfiguration.DebugInfoProvider == null)
            {
                StartupConfiguration.DebugInfoProvider = SpectrumVm.DebugInfoProvider;
            }
            else
            {
                SpectrumVm.DebugInfoProvider = StartupConfiguration.DebugInfoProvider;
            }
            // --- Set up stack debug support
            if (StartupConfiguration.StackDebugSupport != null)
            {
                SpectrumVm.Cpu.StackDebugSupport = StartupConfiguration.StackDebugSupport;
                StartupConfiguration.StackDebugSupport.Reset();
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
                await ExecuteOnMainThread(() =>
                {
                    MoveToState(VmState.Running);
                });
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
        }

        #endregion
    }
}