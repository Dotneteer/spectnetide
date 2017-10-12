using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
using Spect.Net.Wpf.Mvvm.Messages;

namespace Spect.Net.VsPackage.ToolWindows.StackTool
{
    /// <summary>
    /// This class represents the view model of the Z80 CPU Stack tool window
    /// </summary>
    public class StackToolWindowViewModel: SpectrumGenericToolWindowViewModel
    {
        private bool _stackContentViewVisible;
        private ObservableCollection<StackPointerManipulationViewModel> _spManipulations;

        /// <summary>
        /// Indicates if the stack content view is visible
        /// </summary>
        public bool StackContentViewVisible
        {
            get => _stackContentViewVisible;
            set => Set(ref _stackContentViewVisible, value);
        }

        /// <summary>
        /// Stack pointer manipulation events
        /// </summary>
        public ObservableCollection<StackPointerManipulationViewModel> SpManipulations
        {
            get => _spManipulations;
            set => Set(ref _spManipulations, value);
        }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public StackToolWindowViewModel()
        {
            SpManipulations = new ObservableCollection<StackPointerManipulationViewModel>();

            if (IsInDesignMode)
            {
                SpManipulations.Add(new StackPointerManipulationViewModel(
                    new StackPointerManipulationEvent(0x1234, "ld sp,#4567", 0x2345, 0x4567, 123456789)));
                SpManipulations.Add(new StackPointerManipulationViewModel(
                    new StackPointerManipulationEvent(0x1234, "ld sp,hl", 0x2345, 0x4567, 123456789)));
            }
        }

        /// <summary>
        /// Set the machine status when the screen has been refreshed
        /// </summary>
        protected override void OnScreenRefreshed(MachineScreenRefreshedMessage msg)
        {
            base.OnScreenRefreshed(msg);
            if (ScreenRefreshCount % 10 == 0)
            {
                if (!(MachineViewModel?.StackDebugSupport is IStackEventData stackDebugSupport))
                {
                    return;
                }

                SpManipulations.Clear();
                foreach (var item in stackDebugSupport.StackPointerEvents)
                {
                    SpManipulations.Add(new StackPointerManipulationViewModel(item));
                }
            }
        }
    }
}