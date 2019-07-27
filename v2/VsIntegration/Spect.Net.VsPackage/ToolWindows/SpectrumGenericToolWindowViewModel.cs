using System;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.Wpf.Mvvm;

// ReSharper disable IdentifierTypo

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class is intended to be the base of all ZX Spectrum related tool window
    /// view models
    /// </summary>
    public class SpectrumGenericToolWindowViewModel: EnhancedViewModelBase, IDisposable
    {
        private bool _refreshInProgress;
        private int _screenRefreshCount;

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
            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();

        }

        /// <summary>
        /// Set the machine status and notify controls
        /// </summary>
        private void OnInternalVmStateChanged(object sender, VmStateChangedEventArgs e)
        {
            OnVmStateChanged(e.OldState, e.NewState);
        }

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
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            EmulatorViewModel.RenderFrameCompleted -= OnInternalRenderFrameCompleted;
        }
    }
}