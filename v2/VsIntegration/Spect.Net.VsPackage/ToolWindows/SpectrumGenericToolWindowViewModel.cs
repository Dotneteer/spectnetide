using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using System;

// ReSharper disable IdentifierTypo

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class is intended to be the base of all ZX Spectrum related tool window
    /// view models
    /// </summary>
    public class SpectrumGenericToolWindowViewModel : SpectNetToolWindowViewModelBase
    {
        private bool _machineEventsInitialized;
        private bool _refreshInProgress;
        private int _screenRefreshCount;

        // --- Constants used by commands
        protected const string INV_SYNTAX = "Invalid command syntax";
        protected const string INV_S48_COMMAND = "This command cannot be used for a Spectrum 48K model.";
        protected const string INV_RUN_COMMAND = "This command can only be used when the virtual machine is running.";
        protected const string INV_CONTEXT = "This command cannot be used in {0}.";
        protected const string UNDEF_SYMBOL = "This command uses an undefined symbol ({0}).";

        /// <summary>
        /// The aggregated ZX Spectrum view model
        /// </summary>
        public EmulatorViewModel EmulatorViewModel => SpectNetPackage.Default.EmulatorViewModel;

        /// <summary>
        /// Gets the current machine
        /// </summary>
        public SpectrumMachine Machine => SpectNetPackage.Default.EmulatorViewModel.Machine;

        /// <summary>
        /// Gets the current machine state
        /// </summary>
        public VmState MachineState => SpectNetPackage.Default.EmulatorViewModel.MachineState;

        /// <summary>
        /// Gets the SpectrumVm
        /// </summary>
        public ISpectrumVm SpectrumVm => SpectNetPackage.Default.EmulatorViewModel.Machine.SpectrumVm;

        /// <summary>
        /// Gets the #of times the screen has been refreshed
        /// </summary>
        public int ScreenRefreshCount
        {
            get => _screenRefreshCount;
            set => Set(ref _screenRefreshCount, value);
        }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public SpectrumGenericToolWindowViewModel()
        {
            if (IsInDesignMode) return;

            EmulatorViewModel.VmStateChanged += OnInternalVmStateChanged;
            EmulatorViewModel.RenderFrameCompleted += OnInternalRenderFrameCompleted;
            _machineEventsInitialized = true;

            SpectNetPackage.Default.Solution.SolutionOpened += OnInternalSolutionOpened;
            SpectNetPackage.Default.Solution.SolutionClosing += OnInternalSolutionClosing;
            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();
        }

        /// <summary>
        /// Respond to the event when the solution is opened.
        /// </summary>
        private void OnInternalSolutionOpened(object sender, EventArgs e)
        {
            if (!_machineEventsInitialized)
            {
                EmulatorViewModel.VmStateChanged += OnInternalVmStateChanged;
                EmulatorViewModel.RenderFrameCompleted += OnInternalRenderFrameCompleted;
                _machineEventsInitialized = true;
            }
            Initialize();
            OnSolutionOpened();
        }

        /// <summary>
        /// Respond to the event when the solution is closing
        /// </summary>
        private void OnInternalSolutionClosing(object sender, EventArgs e)
        {
            if (_machineEventsInitialized)
            {
                EmulatorViewModel.VmStateChanged -= OnInternalVmStateChanged;
                EmulatorViewModel.RenderFrameCompleted -= OnInternalRenderFrameCompleted;
            }
            OnSolutionClosing();
        }

        /// <summary>
        /// Set the machine status and notify controls
        /// </summary>
        private void OnInternalVmStateChanged(object sender, VmStateChangedEventArgs e)
        {
            OnVmStateChanged(e.OldState, e.NewState);
            switch (MachineState)
            {
                case VmState.Running:
                    if (Machine.IsFirstStart || EmulatorViewModel.JustRestoredState)
                    {
                        if (!EmulatorViewModel.NoToolRefreshMode) OnFirstStart();
                    }
                    if (!EmulatorViewModel.NoToolRefreshMode) OnStart();
                    break;

                case VmState.Paused:
                    if (Machine.IsFirstPause || EmulatorViewModel.JustRestoredState)
                    {
                        if (!EmulatorViewModel.NoToolRefreshMode) OnFirstPaused();
                    }
                    if (!EmulatorViewModel.NoToolRefreshMode) OnPaused();
                    break;

                case VmState.Stopped:
                    if (!EmulatorViewModel.NoToolRefreshMode) OnStopped();
                    break;
            }
        }

        /// <summary>
        /// Respond to the frame refreshed event
        /// </summary>
        private void OnInternalRenderFrameCompleted(object sender, RenderFrameEventArgs e)
        {
            if (_refreshInProgress) return;

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

        /// <summary>
        /// Override this method to initialize this view model
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Override this method to respond opening a solution
        /// </summary>
        protected virtual void OnSolutionOpened()
        {
        }

        /// <summary>
        /// Override this method to respond closing a solution
        /// </summary>
        protected virtual void OnSolutionClosing()
        {
        }

        /// <summary>
        /// Override to define *any* virtual machine state changed
        /// </summary>
        protected virtual void OnVmStateChanged(VmState oldState, VmState newState)
        {
        }

        /// <summary>
        /// Set the machine status when the screen has been refreshed
        /// </summary>
        protected virtual void OnScreenRefreshed()
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
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            SpectNetPackage.Default.Solution.SolutionOpened -= OnInternalSolutionOpened;
            SpectNetPackage.Default.Solution.SolutionClosing -= OnInternalSolutionClosing;
            if (!_machineEventsInitialized) return;

            EmulatorViewModel.VmStateChanged -= OnInternalVmStateChanged;
            EmulatorViewModel.RenderFrameCompleted -= OnInternalRenderFrameCompleted;
            _machineEventsInitialized = false;
        }
    }
}