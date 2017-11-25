using System;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.Wpf.Mvvm.Messages;

namespace Spect.Net.VsPackage.ToolWindows
{
    public abstract class SpectrumToolWindowPane<TControl, TVm>: VsxToolWindowPane<SpectNetPackage, TControl, TVm>
        where TControl : ContentControl, ISupportsMvvm<TVm>, new()
        where TVm : ViewModelBase, new()

    {
        /// <summary>
        /// Signs if this tool window follows VM state changes
        /// </summary>
        public bool FollowVmState { get; set; }

        /// <summary>
        /// Prepares window frame events
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            Package.SolutionOpened += OnInternalSolutionOpened;
            Package.SolutionClosed += OnInternalSolutionClosed;
            Messenger.Default.Register<VmStateChangedMessage>(this, OnVmStateChanged);

            var vm = VsxPackage.GetPackage<SpectNetPackage>().MachineViewModel;
            if (vm != null)
            {
                ChangeCaption(vm.VmState);
            }
        }

        /// <summary>
        /// Responds to the solution opened event
        /// </summary>
        private void OnInternalSolutionOpened(object sender, EventArgs e)
        {
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
        /// <param name="msg"></param>
        protected virtual void OnVmStateChanged(VmStateChangedMessage msg)
        {
            ChangeCaption(msg.NewState);
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
            Messenger.Default.Unregister<VmStateChangedMessage>(this);
            Package.SolutionOpened += OnInternalSolutionOpened;
            Package.SolutionClosed += OnInternalSolutionClosed;
            base.OnClose();
        }
    }
}