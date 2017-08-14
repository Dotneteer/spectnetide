using System;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.SpectrumControl;

namespace Spect.Net.VsPackage.Tools
{
    /// <summary>
    /// This class is intended to be the base of all ZS Spectrum related tool window
    /// view models
    /// </summary>
    public class SpectrumGenericToolWindowViewModel : EnhancedViewModelBase, IDisposable
    {
        private bool _vmRuns;
        private bool _vmStopped;
        private bool _vmPaused;
        private bool _vmNotStopped;
        private bool _vmNotRuns;
        private int _screenRefreshCount;

        /// <summary>
        /// The aggregated ZX Spectrum view model
        /// </summary>
        public SpectrumVmViewModel SpectrumVmViewModel { get; }

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
            }
            else
            {
                SpectrumVmViewModel = VsxPackage.GetPackage<SpectNetPackage>().SpectrumVmViewModel;
                Messenger.Default.Register<SpectrumVmStateChangedMessage>(this, OnVmStateChanged);
                Messenger.Default.Register<SpectrumScreenRefreshedMessage>(this, OnScreenRefreshed);
                VmPaused = false;
                VmStopped = true;
                VmNotStopped = false;
                VmRuns = false;
                VmNotRuns = true;
            }
        }

        /// <summary>
        /// Set the machnine status
        /// </summary>
        protected virtual void OnVmStateChanged(SpectrumVmStateChangedMessage msg)
        {
            VmPaused = msg.NewState == SpectrumVmState.Paused;
            VmStopped = msg.NewState == SpectrumVmState.None
                        || msg.NewState == SpectrumVmState.Stopped;
            VmNotStopped = !VmStopped;
            VmRuns = !VmStopped && !VmPaused;
            VmNotRuns = VmStopped || VmPaused;
        }

        /// <summary>
        /// Set the machine status when the screen has been refreshed
        /// </summary>
        protected virtual void OnScreenRefreshed(SpectrumScreenRefreshedMessage msg)
        {
            ScreenRefreshCount++;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (!IsInDesignMode)
            {
                Messenger.Default.Unregister<SpectrumVmStateChangedMessage>(this);
                Messenger.Default.Unregister<SpectrumScreenRefreshedMessage>(this);
            }
        }
    }
}