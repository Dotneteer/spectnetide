using System;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows
{
    public abstract class SpectrumToolWindowPane<TControl, TVm>: VsxToolWindowPane<TControl, TVm>
        where TControl : ContentControl, ISupportsMvvm<TVm>, new()
        where TVm : ViewModelBase, new()

    {
        private bool _initializedWithSolution;

        /// <summary>
        /// Prepares window frame events
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            Package.SolutionOpened += OnInternalSolutionOpened;
            Package.SolutionClosed += OnInternalSolutionClosed;

            if (_initializedWithSolution) return;

            var vm = SpectNetPackage.Default.MachineViewModel;
            if (vm != null)
            {
                vm.VmStateChanged += VmOnVmStateChanged;
                ChangeCaption(vm.VmState);
            }
            _initializedWithSolution = true;
        }

        /// <summary>
        /// Responds to the solution opened event
        /// </summary>
        private void OnInternalSolutionOpened(object sender, EventArgs e)
        {
            var vm = SpectNetPackage.Default.MachineViewModel;
            if (vm != null)
            {
                vm.VmStateChanged += VmOnVmStateChanged;
                ChangeCaption(vm.VmState);
            }
            _initializedWithSolution = true;
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
        private void OnInternalSolutionClosed(object sender, EventArgs e)
        {
            var vm = SpectNetPackage.Default.MachineViewModel;
            if (vm != null)
            {
                vm.VmStateChanged -= VmOnVmStateChanged;
            }
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
        private void VmOnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            ChangeCaption(args.NewState);
        }

        /// <summary>
        /// Changes the caption according to machine state
        /// </summary>
        /// <param name="state"></param>
        protected void ChangeCaption(VmState state)
        {
            var additional = string.Empty;
            switch (state)
            {
                case VmState.None:
                case VmState.BuildingMachine:
                    additional = " (Not Started)";
                    break;
                case VmState.Stopped:
                    additional = " (Stopped)";
                    break;
                case VmState.Paused:
                    additional = " (Paused)";
                    break;
            }
            Caption = BaseCaption + additional;
        }

        /// <summary>
        /// Unregisters messages
        /// </summary>
        protected override void OnClose()
        {
            Package.SolutionOpened += OnInternalSolutionOpened;
            Package.SolutionClosed += OnInternalSolutionClosed;
            base.OnClose();
        }
    }
}