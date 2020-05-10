using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using System;

namespace Spect.Net.VsPackage.ToolWindows.BasicList
{
    /// <summary>
    /// Interaction logic for BasicListToolWindowControl.xaml
    /// </summary>
    public partial class BasicListToolWindowControl : ISupportsMvvm<BasicListToolWindowViewModel>
    {
        private BasicListViewModel _lastModel = new BasicListViewModel();
        public BasicListToolWindowViewModel Vm { get; private set; }


        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<BasicListToolWindowViewModel>.SetVm(BasicListToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public BasicListToolWindowControl()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                Vm.EmulatorViewModel.VmStateChanged += OnVmStateChanged;
                Vm.EmulatorViewModel.RenderFrameCompleted += OnRenderFrameCompleted;
            };
            Unloaded += (s, e) =>
            {
                Vm.EmulatorViewModel.VmStateChanged -= OnVmStateChanged;
                Vm.EmulatorViewModel.RenderFrameCompleted -= OnRenderFrameCompleted;
            };
        }

        /// <summary>
        /// Refresh the view periodically
        /// </summary>
        private void OnRenderFrameCompleted(object sender, RenderFrameEventArgs e)
        {
            if (Vm.ScreenRefreshCount % 100 == 0)
            {
#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
                Dispatcher.Invoke(() =>
                {
                    var newModel = Vm.CreateBasicListViewModel();
                    newModel.MimicZxBasic = true;
                    newModel.DecodeBasicProgram();
                    if (!BasicListViewModel.Compare(_lastModel, newModel))
                    {
                        Vm.RefreshBasicList();
                        _lastModel = newModel;
                    }
                });
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
            }
        }

        /// <summary>
        /// Manage the LoadCompleted event
        /// </summary>
        private void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            if (args.NewState == VmState.Running)
            {
                RefreshBasicList();
                Vm.SpectrumVm.TapeLoadDevice.LoadCompleted += OnLoadCompleted;
            }
            else if (args.NewState == VmState.Stopped)
            {
                Vm.SpectrumVm.TapeLoadDevice.LoadCompleted -= OnLoadCompleted;
            }
            RefreshBasicList();
        }

        private void OnLoadCompleted(object sender, EventArgs e)
        {
            RefreshBasicList();
        }

        private void RefreshBasicList()
        {
#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
            Dispatcher.InvokeAsync(() => {
                Vm.RefreshBasicList();
                _lastModel = Vm.List;
            });
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
        }
    }
}
