using System;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Z80Programs;
using MachineViewModel = Spect.Net.VsPackage.ToolWindows.SpectrumEmulator.MachineViewModel;
// ReSharper disable IdentifierTypo

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class is intended to be the base of all ZX Spectrum related tool window
    /// view models
    /// </summary>
    public class SpectrumGenericToolWindowViewModel : SpectNetPackageToolWindowViewModelBase
    {
        private bool _refreshInProgress;
        private int _screenRefreshCount;

        // --- Constants used by commands
        protected const string INV_S48_COMMAND = "This command cannot be used for a Spectrum 48K model.";
        protected const string INV_RUN_COMMAND = "This command can only be used when the virtual machine is running.";
        protected const string UNDEF_SYMBOL = "This command uses an undefined symbol ({0}).";

        /// <summary>
        /// The aggregated ZX Spectrum view model
        /// </summary>
        public MachineViewModel MachineViewModel => Package.MachineViewModel;

        /// <summary>
        /// Gets the #of times the screen has been refreshed
        /// </summary>
        public int ScreenRefreshCount
        {
            get => _screenRefreshCount;
            set => Set(ref _screenRefreshCount, value);
        }

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine runs
        /// </summary>
        public bool VmRuns => MachineViewModel.MachineState == VmState.Running;

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine is stopped
        /// </summary>
        public bool VmStopped => MachineViewModel.MachineState == VmState.None
            || MachineViewModel.MachineState == VmState.Stopped;

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine is paused
        /// </summary>
        public bool VmPaused => MachineViewModel.MachineState == VmState.Paused;

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine is not stopped
        /// </summary>
        public bool VmNotStopped => !VmStopped;

        /// <summary>
        /// Compiler output
        /// </summary>
        public AssemblerOutput CompilerOutput { get; protected set; }

        /// <summary>
        /// Represents the event when the screen has been refreshed.
        /// </summary>
        public event EventHandler ScreenRefreshed;

        #region Lifecycle methods

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public SpectrumGenericToolWindowViewModel()
        {
            if (IsInDesignMode) return;

            // --- Register messages
            Package.SolutionOpened += OnInternalSolutionOpened;
            Package.SolutionClosed += OnInternalSolutionClosed;
            MachineViewModel.VmStateChanged += OnInternalVmStateChanged;
            MachineViewModel.VmScreenRefreshed += BridgeScreenRefreshed;
            Package.CodeManager.CompilationCompleted += OnCompilationCompleted;

            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (IsInDesignMode) return;

            Package.SolutionOpened -= OnInternalSolutionOpened;
            Package.SolutionClosed -= OnInternalSolutionClosed;
            MachineViewModel.VmStateChanged -= OnInternalVmStateChanged;
            MachineViewModel.VmScreenRefreshed -= BridgeScreenRefreshed;
            Package.CodeManager.CompilationCompleted -= OnCompilationCompleted;
            base.Dispose();
        }

        #endregion

        #region Virtual methods

        /// <summary>
        /// Override this method to initialize this view model
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Override this method to handle the solution opened event
        /// </summary>
        protected virtual void OnSolutionOpened()
        {
            MachineViewModel.VmStateChanged += OnInternalVmStateChanged;
            MachineViewModel.VmScreenRefreshed += BridgeScreenRefreshed;
        }

        /// <summary>
        /// Override this method to handle the solution closed event
        /// </summary>
        protected virtual void OnSolutionClosed()
        {
            MachineViewModel.VmStateChanged -= OnInternalVmStateChanged;
            MachineViewModel.VmScreenRefreshed -= BridgeScreenRefreshed;
        }

        /// <summary>
        /// Override to define *any* virtual machine state changed
        /// </summary>
        protected virtual void OnVmStateChanged(VmState oldState, VmState newState)
        {
        }

        /// <summary>
        /// Override to handle the first start (from stopped state) 
        /// of the virtual machine
        /// </summary>
        protected virtual void OnFirstStart()
        {
        }

        /// <summary>
        /// Override to handle the start of the virtual machine.
        /// </summary>
        /// <remarks>This method is called for the first start, too</remarks>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Override to handle the first paused state
        /// of the virtual machine
        /// </summary>
        protected virtual void OnFirstPaused()
        {
        }

        /// <summary>
        /// Override to handle the paused state of the virtual machine.
        /// </summary>
        /// <remarks>This method is called for the first pause, too</remarks>
        protected virtual void OnPaused()
        {
        }

        /// <summary>
        /// Override to handle the stopped state of the virtual machine.
        /// </summary>
        protected virtual void OnStopped()
        {
        }

        /// <summary>
        /// Set the machine status when the screen has been refreshed
        /// </summary>
        protected virtual void OnScreenRefreshed()
        {
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Catch the event of compilation
        /// </summary>
        private void OnCompilationCompleted(object sender, CompilationCompletedEventArgs e)
        {
            CompilerOutput = e.Output;
        }

        /// <summary>
        /// Handles the solution opened event
        /// </summary>
        private void OnInternalSolutionOpened(object sender, EventArgs args)
        {
            Initialize();
            OnSolutionOpened();
        }

        /// <summary>
        /// Handles the solution closed event
        /// </summary>
        private void OnInternalSolutionClosed(object sender, EventArgs args)
        {
            ViewInitializedWithSolution = false;
        }

        /// <summary>
        /// Set the machine status and notify controls
        /// </summary>
        protected virtual void OnInternalVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            // --- Anyhow, we always provide a way to respond for *any*
            // --- state changes
            OnVmStateChanged(args.OldState, args.NewState);
            if (VmRuns)
            {
                if (MachineViewModel.IsFirstStart || MachineViewModel.JustRestoredState)
                {
                    if (!MachineViewModel.NoToolRefreshMode) OnFirstStart();
                }
                if (!MachineViewModel.NoToolRefreshMode) OnStart();
            }
            else if (VmPaused)
            {
                if (MachineViewModel.IsFirstPause || MachineViewModel.JustRestoredState)
                {
                    if (!MachineViewModel.NoToolRefreshMode) OnFirstPaused();
                }
                if (!MachineViewModel.NoToolRefreshMode) OnPaused();
            }
            else if (VmStopped)
            {
                if (!MachineViewModel.NoToolRefreshMode) OnStopped();
            }
        }

        /// <summary>
        /// This method makes a bridge between the FrameCompleted event of the
        /// virtual machine's screen device and the OnScreenRefreshed method.
        /// </summary>
        private void BridgeScreenRefreshed(object sender, EventArgs e)
        {
            if (_refreshInProgress) return;

            _refreshInProgress = true;
            try
            {
                ScreenRefreshCount++;
                OnScreenRefreshed();
                ScreenRefreshed?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                _refreshInProgress = false;
            }
        }

        #endregion
    }
}