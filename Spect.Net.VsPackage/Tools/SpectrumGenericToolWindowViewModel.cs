using System;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Mvvm;
using Spect.Net.SpectrumEmu.Mvvm.Messages;
using Spect.Net.VsPackage.Vsx;

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
        public MachineViewModel MachineViewModel { get; private set; }

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
            MachineViewModel = VsxPackage.GetPackage<SpectNetPackage>().MachineViewModel;
            VmPaused = false;
            VmStopped = true;
            VmNotStopped = false;
            VmRuns = false;
            VmNotRuns = true;

            // --- Register messages
            Messenger.Default.Register<SolutionOpenedMessage>(this, OnSolutionOpened);
            Messenger.Default.Register<MachineStateChangedMessage>(this, OnVmStateChanged);
            Messenger.Default.Register<MachineScreenRefreshedMessage>(this, OnScreenRefreshed);
        }

        /// <summary>
        /// Immediately evaluates the state of the Spectru virtual machine
        /// </summary>
        public void EvaluateState()
        {
            var state = MachineViewModel.VmState;
            VmPaused = state == VmState.Paused;
            VmStopped = state == VmState.None
                        || state == VmState.Stopped;
            VmNotStopped = !VmStopped;
            VmRuns = !VmStopped && !VmPaused;
            VmNotRuns = VmStopped || VmPaused;
        }

        /// <summary>
        /// Obtain the machine view model from the solution
        /// </summary>
        protected virtual void OnSolutionOpened(SolutionOpenedMessage msg)
        {
            MachineViewModel = VsxPackage.GetPackage<SpectNetPackage>().MachineViewModel;
        }

        /// <summary>
        /// Set the machnine status and notify controls
        /// </summary>
        protected virtual void OnVmStateChanged(MachineStateChangedMessage msg)
        {
            EvaluateState();
            MessengerInstance.Send(new VmStateChangedMessage(msg.OldState, msg.NewState));
        }

        /// <summary>
        /// Set the machine status when the screen has been refreshed
        /// </summary>
        protected virtual void OnScreenRefreshed(MachineScreenRefreshedMessage msg)
        {
            ScreenRefreshCount++;
            MessengerInstance.Send(new ScreenRefreshedMessage());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsInDesignMode) return;

            Messenger.Default.Unregister<SolutionOpenedMessage>(this);
            Messenger.Default.Unregister<MachineStateChangedMessage>(this);
            Messenger.Default.Unregister<MachineScreenRefreshedMessage>(this);
        }
    }
}