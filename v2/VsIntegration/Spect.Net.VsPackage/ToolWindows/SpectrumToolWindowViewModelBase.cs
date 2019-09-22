using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Compilers;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.Wpf.Mvvm;
using System;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class is intended to be the base of all tool window view models that
    /// are connected to the ZX Spectrum emulator.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public abstract class SpectrumToolWindowViewModelBase: EnhancedViewModelBase, IDisposable
    {
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
        /// Indicates if the solution is closing
        /// </summary>
        public bool IsSolutionClosing => SpectNetPackage.Default.Solution.IsSolutionClosing;

        /// <summary>
        /// Compiler output
        /// </summary>
        public AssemblerOutput CompilerOutput { get; protected set; }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public SpectrumToolWindowViewModelBase()
        {
            if (IsInDesignMode) return;

            EmulatorViewModel.VmStateChanged += OnInternalVmStateChanged;
            EmulatorViewModel.RenderFrameCompleted += OnInternalRenderFrameCompleted;
            EmulatorViewModel.MachineInstanceChanged += OnInternalMachineInstanceChanged;
            SpectNetPackage.Default.Solution.SolutionOpened += OnInternalSolutionOpened;
            SpectNetPackage.Default.Solution.SolutionClosing += OnInternalSolutionClosing;
            SpectNetPackage.Default.CodeManager.CompilationCompleted += OnCompilationCompleted;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            SpectNetPackage.Default.Solution.SolutionOpened -= OnInternalSolutionOpened;
            SpectNetPackage.Default.Solution.SolutionClosing -= OnInternalSolutionClosing;
            EmulatorViewModel.VmStateChanged -= OnInternalVmStateChanged;
            EmulatorViewModel.RenderFrameCompleted -= OnInternalRenderFrameCompleted;
            EmulatorViewModel.MachineInstanceChanged -= OnInternalMachineInstanceChanged;
        }

        /// <summary>
        /// Responds to the event when machine instance changes
        /// </summary>
        private void OnInternalMachineInstanceChanged(object sender, MachineInstanceChangedEventArgs e)
        {
            OnMachineInstanceChanged();
        }

        /// <summary>
        /// Respond to the event when the solution is opened.
        /// </summary>
        private void OnInternalSolutionOpened(object sender, EventArgs e)
        {
            OnSolutionOpened();
        }

        /// <summary>
        /// Respond to the event when the solution is closing
        /// </summary>
        private void OnInternalSolutionClosing(object sender, EventArgs e)
        {
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
        /// Responds to the event when compilation has been completed.
        /// </summary>
        private void OnCompilationCompleted(object sender, CompilationCompletedEventArgs e)
        {
            CompilerOutput = e.Output;
        }

        /// <summary>
        /// Override this method to respond the machine instance changes
        /// </summary>
        protected virtual void OnMachineInstanceChanged()
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
    }
}
