using System;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx;
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
        private bool _initializedWithSolution;
        private bool _refreshInProgress;
        private bool _vmRuns;
        private bool _vmStopped;
        private bool _vmPaused;
        private bool _vmNotStopped;
        private bool _vmNotRuns;
        private int _screenRefreshCount;

        /// <summary>
        /// The aggregated ZX Spectrum view model
        /// </summary>
        public MachineViewModel MachineViewModel 
            => VsxPackage.GetPackage<SpectNetPackage>().MachineViewModel;

        /// <summary>
        /// This flag shows if this tool window has already been initialized after
        /// opening the solution
        /// </summary>
        public bool InitializedWithSolution
        {
            get => _initializedWithSolution;
            set => Set(ref _initializedWithSolution, value);
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
        public bool VmRuns
        {
            get => _vmRuns;
            set => Set(ref _vmRuns, value);
        }

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine is stopped
        /// </summary>
        public bool VmStopped
        {
            get => _vmStopped;
            set => Set(ref _vmStopped, value);
        }

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine is paused
        /// </summary>
        public bool VmPaused
        {
            get => _vmPaused;
            set => Set(ref _vmPaused, value);
        }

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine is not stopped
        /// </summary>
        public bool VmNotStopped
        {
            get => _vmNotStopped;
            set => Set(ref _vmNotStopped, value);
        }

        /// <summary>
        /// Gets the flag that indicates if the ZX Spectrum virtual machine is currently executing
        /// </summary>
        public bool VmNotRuns
        {
            get => _vmNotRuns;
            set => Set(ref _vmNotRuns, value);
        }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public SpectrumGenericToolWindowViewModel()
        {
            if (IsInDesignMode)
            {
                VmPaused = true;
                VmStopped = false;
                VmNotStopped = true;
                VmRuns = false;
                return;
            }

            // --- Set the initial state of the view model
            VmPaused = false;
            VmStopped = true;
            VmNotStopped = false;
            VmRuns = false;
            VmNotRuns = true;
            EvaluateState();

            // --- Register messages
            Messenger.Default.Register<SolutionOpenedMessage>(this, OnSolutionOpened);
            Messenger.Default.Register<SolutionClosedMessage>(this, OnSolutionClosed);
            MachineViewModel.VmStateChanged += OnInternalVmStateChanged;
            MachineViewModel.VmScreenRefreshed += BridgeScreenRefreshed;
        }

        /// <summary>
        /// Immediately evaluates the state of the Spectrum virtual machine
        /// </summary>
        public void EvaluateState()
        {
            if (MachineViewModel == null) return;

            var state = MachineViewModel.VmState;
            VmPaused = state == VmState.Paused;
            VmStopped = state == VmState.None
                        || state == VmState.BuildingMachine
                        || state == VmState.Stopped;
            VmNotStopped = !VmStopped;
            VmRuns = state == VmState.Running;
            VmNotRuns = !VmRuns;
        }

        /// <summary>
        /// Override this method to handle the solution opened event
        /// </summary>
        protected virtual void OnSolutionOpened(SolutionOpenedMessage msg)
        {
        }

        /// <summary>
        /// Override this method to handle the solution closed event
        /// </summary>
        protected virtual void OnSolutionClosed(SolutionClosedMessage msg)
        {
            InitializedWithSolution = false;
        }

        /// <summary>
        /// Set the machnine status and notify controls
        /// </summary>
        protected virtual void OnInternalVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            EvaluateState();
            OnVmStateChanged(sender, args);
        }

        /// <summary>
        /// Set the machnine status and notify controls
        /// </summary>
        protected virtual void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
        }

        /// <summary>
        /// This method makes a bridge between the FrameCompleted event of the
        /// virtual machine's screen device and the OnScreenRefreshed method.
        /// </summary>
        private void BridgeScreenRefreshed(object sender, EventArgs e)
        {
            if (!_refreshInProgress)
            {
                _refreshInProgress = true;
                try
                {
                    ScreenRefreshCount++;
                    OnScreenRefreshed();
                }
                finally
                {
                    _refreshInProgress = false;
                }
            }
        }

        /// <summary>
        /// Set the machine status when the screen has been refreshed
        /// </summary>
        protected virtual void OnScreenRefreshed()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsInDesignMode) return;

            Messenger.Default.Unregister<SolutionOpenedMessage>(this);
            Messenger.Default.Unregister<SolutionClosedMessage>(this);
            MachineViewModel.VmStateChanged -= OnVmStateChanged;
            MachineViewModel.VmScreenRefreshed -= BridgeScreenRefreshed;
        }
    }
}