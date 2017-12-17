using System;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Mvvm;
using MachineViewModel = Spect.Net.VsPackage.ToolWindows.SpectrumEmulator.MachineViewModel;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class is intended to be the base of all ZX Spectrum related tool window
    /// view models
    /// </summary>
    public class SpectrumGenericToolWindowViewModel : EnhancedViewModelBase, IDisposable
    {
        private bool _viewInitializedWithSolution;
        private bool _refreshInProgress;
        private int _screenRefreshCount;

        /// <summary>
        /// The hosting package
        /// </summary>
        public SpectNetPackage Package => SpectNetPackage.Default;

        /// <summary>
        /// The aggregated ZX Spectrum view model
        /// </summary>
        public MachineViewModel MachineViewModel => Package.MachineViewModel;

        /// <summary>
        /// This flag shows if this tool window has already been initialized after
        /// opening the solution
        /// </summary>
        public bool ViewInitializedWithSolution
        {
            get => _viewInitializedWithSolution;
            set => Set(ref _viewInitializedWithSolution, value);
        }

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
        public bool VmRuns => MachineViewModel.VmState == VmState.Running;

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine is stopped
        /// </summary>
        public bool VmStopped => MachineViewModel.VmState == VmState.None
            || MachineViewModel.VmState == VmState.BuildingMachine
            || MachineViewModel.VmState == VmState.Stopped;

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine is paused
        /// </summary>
        public bool VmPaused => MachineViewModel.VmState == VmState.Paused;

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine is not stopped
        /// </summary>
        public bool VmNotStopped => !VmStopped;

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine is currently executing
        /// </summary>
        public bool VmNotRuns => MachineViewModel.VmState != VmState.Running;

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

            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsInDesignMode) return;

            Package.SolutionOpened -= OnInternalSolutionOpened;
            Package.SolutionClosed -= OnInternalSolutionClosed;
            MachineViewModel.VmStateChanged -= OnInternalVmStateChanged;
            MachineViewModel.VmScreenRefreshed -= BridgeScreenRefreshed;
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
        /// Set the machnine status and notify controls
        /// </summary>
        protected virtual void OnInternalVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            // --- Anyhow, we always provide a way to respond for *any*
            // --- state changes
            OnVmStateChanged(args.OldState, args.NewState);
            if (VmRuns)
            {
                if (MachineViewModel.IsFirstStart)
                {
                    OnFirstStart();
                }
                OnStart();
            }
            else if (VmPaused)
            {
                if (MachineViewModel.IsFirstPause)
                {
                    OnFirstPaused();
                }
                OnPaused();
            }
            else if (VmStopped)
            {
                OnStopped();
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