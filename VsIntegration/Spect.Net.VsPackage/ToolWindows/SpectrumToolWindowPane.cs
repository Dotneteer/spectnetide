using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.Wpf.Mvvm;
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
            Messenger.Default.Register<SolutionOpenedMessage>(this, OnSolutionOpened);
            Messenger.Default.Register<SolutionClosedMessage>(this, OnSolutionClosed);
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
        /// <param name="msg">Solution opened message</param>
        protected virtual void OnSolutionOpened(SolutionOpenedMessage msg)
        {
        }

        /// <summary>
        /// Closes this window whenever the current solution closes
        /// </summary>
        /// <param name="msg">Solution closed message</param>
        protected virtual void OnSolutionClosed(SolutionClosedMessage msg)
        {
            ClosePane();
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
            Messenger.Default.Unregister<SolutionClosedMessage>(this);
            Messenger.Default.Unregister<SolutionOpenedMessage>(this);
            base.OnClose();
        }
    }
}