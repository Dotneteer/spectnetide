using System;
using System.Windows.Controls;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class is intended to be the base for all window panes that use
    /// the state of the ZX Spectrum virtual machine
    /// </summary>
    /// <typeparam name="TControl"></typeparam>
    /// <typeparam name="TVm"></typeparam>
    public abstract class SpectrumToolWindowPane<TControl, TVm> : ToolWindowPaneBase<TControl, TVm>
        where TControl : ContentControl, ISupportsMvvm<TVm>, new()
        where TVm : EnhancedViewModelBase, new()

    {
        /// <summary>
        /// Prepares window frame events
        /// </summary>
        /// <remarks>
        /// This method is invoked only once during the life cycle of the
        /// tool window.
        /// </remarks>
        protected override void OnCreate()
        {
            base.OnCreate();
            var solution = SpectNetPackage.Default.Solution;
            solution.SolutionOpened += OnInternalSolutionOpened;
            solution.SolutionClosing += OnInternalSolutionClosing;
            SpectNetPackage.Default.EmulatorViewModel.VmStateChanged += VmOnVmStateChanged;
            ChangeCaption();
        }

        /// <summary>
        /// Responds to the solution opened event
        /// </summary>
        private void OnInternalSolutionOpened(object sender, EventArgs e)
        {
            ChangeCaption();
            OnSolutionOpened();
        }

        /// <summary>
        /// Override to respond the solution opened event
        /// </summary>
        protected virtual void OnSolutionOpened()
        {
        }

        /// <summary>
        /// Closes this window whenever the current solution closes
        /// </summary>
        private void OnInternalSolutionClosing(object sender, EventArgs e)
        {
            ClosePane();
            OnSolutionClosed();
        }

        /// <summary>
        /// Override to respond the solution closes event
        /// </summary>
        protected virtual void OnSolutionClosed()
        {
        }

        /// <summary>
        /// Changes the caption whenever the VM state changes
        /// </summary>
        private void VmOnVmStateChanged(object sender, VmStateChangedEventArgs e)
        {
            ChangeCaption();
        }

        /// <summary>
        /// Changes the caption according to machine state
        /// </summary>
        protected void ChangeCaption()
        {
            var vm = SpectNetPackage.Default.EmulatorViewModel;
            var additional = string.Empty;
            switch (vm.MachineState)
            {
                case VmState.None:
                    additional = " (Not Started)";
                    break;
                case VmState.Stopped:
                    additional = " (Stopped)";
                    break;
                case VmState.Paused:
                    additional = " (Paused)";
                    break;
                case VmState.Running:
                    if (vm.Machine.RunsInDebugMode)
                    {
                        additional = " (Debugging)";
                    }
                    break;
            }
            Caption = BaseCaption + additional;
        }

        /// <summary>
        /// Unregisters messages
        /// </summary>
        protected override void OnClose()
        {
            var solution = SpectNetPackage.Default.Solution;
            solution.SolutionOpened -= OnInternalSolutionOpened;
            solution.SolutionClosing -= OnInternalSolutionClosing;
            base.OnClose();
        }
    }
}