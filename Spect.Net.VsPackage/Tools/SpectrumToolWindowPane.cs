using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Mvvm;
using Spect.Net.SpectrumEmu.Mvvm.Messages;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Tools
{
    public abstract class SpectrumToolWindowPane<TControl>: VsxToolWindowPane<SpectNetPackage, TControl>
        where TControl : ContentControl, new()
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
            Messenger.Default.Register<SolutionClosedMessage>(this, OnSolutionClosed);
            Messenger.Default.Register<VmStateChangedMessage>(this, OnVmStateChanged);

            var vm = VsxPackage.GetPackage<SpectNetPackage>().MachineViewModel;
            if (vm != null)
            {
                ChangeCaption(vm.VmState);
            }
        }

        /// <summary>
        /// Closes this window whenever the current solution closes
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void OnSolutionClosed(SolutionClosedMessage obj)
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
            base.OnClose();
        }
    }
}