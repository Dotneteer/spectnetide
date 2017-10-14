using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.Wpf.Mvvm;
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
        private ObservableCollection<StackContentManipulationViewModel> _contentManipulations;

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
        /// Stack content manipulation events
        /// </summary>
        public ObservableCollection<StackContentManipulationViewModel> ContentManipulations
        {
            get => _contentManipulations;
            set => Set(ref _contentManipulations, value);
        }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public StackToolWindowViewModel()
        {
            SpManipulations = new ObservableCollection<StackPointerManipulationViewModel>();
            ContentManipulations = new ObservableCollection<StackContentManipulationViewModel>();
            StackContentViewVisible = true;

            if (IsInDesignMode)
            {
                SpManipulations.Add(new StackPointerManipulationViewModel(
                    new StackPointerManipulationEvent(0x1234, "ld sp,#4567", 0x2345, 0x4567, 123456789)));
                SpManipulations.Add(new StackPointerManipulationViewModel(
                    new StackPointerManipulationEvent(0x1234, "ld sp,hl", 0x2345, 0x4567, 123456789)));
                ContentManipulations.Add(new StackContentManipulationViewModel(
                    new StackContentManipulationEvent(0x1234, "push hl", 0xFFF0, 0x2345, 123456789)));
                ContentManipulations.Add(new StackContentManipulationViewModel(
                    new StackContentManipulationEvent(0x2345, "rst #10", 0xFFEF, 0xABCD, 234567890)));
                StackContentViewVisible = true;
            }
        }

        /// <summary>
        /// Clear the stack view whenever the virtual machine starts.
        /// </summary>
        protected override void OnVmStateChanged(MachineStateChangedMessage msg)
        {
            if (msg.OldState == VmState.None || msg.OldState == VmState.Stopped)
            {
                SpManipulations.Clear();
                ContentManipulations.Clear();
            }
        }

        /// <summary>
        /// Refreshes the stack view
        /// </summary>
        public void Refresh()
        {
            if (!(MachineViewModel?.StackDebugSupport is IStackEventData stackDebugSupport))
            {
                return;
            }

            // --- Display stack pointer events
            var oldCount = SpManipulations.Count;
            var index = 0;
            foreach (var item in stackDebugSupport.StackPointerEvents.Reverse())
            {
                // --- Rewrite the displayed list to avoid flickering
                var vmItem = new StackPointerManipulationViewModel(item);
                if (index < oldCount)
                {
                    SpManipulations[index] = vmItem;
                }
                else
                {
                    SpManipulations.Add(vmItem);
                }
                index++;
            }

            // --- Do not display content events while SP is not set
            if (oldCount == 0)
            {
                return;
            }

            // --- Display stack content events
            var ramTopVar = SystemVariables.Get("RAMTOP")?.Address;
            if (ramTopVar == null)
            {
                return;
            }

            var spectrumVm = MachineViewModel?.SpectrumVm;
            var memory = spectrumVm?.MemoryDevice?.GetMemoryBuffer();
            var spValue = spectrumVm?.Cpu?.Registers?.SP;
            if (memory == null || spValue == null)
            {
                return;
            }

            // --- Obtain the top of the memory
            var ramTop = (ushort)(memory[(ushort)ramTopVar] + memory[(ushort)(ramTopVar + 1)] * 0x100);
            var maxItems = VsxPackage.GetPackage<SpectNetPackage>().Options.StackManipulationEvents;
            oldCount = ContentManipulations.Count;
            index = 0;
            for (var addr = (ushort)spValue; addr < ramTop; addr += 2)
            {
                if (addr > memory.Length) break;

                var spContent = (ushort) (memory[addr] + memory[addr + 1] * 0x100);
                StackContentManipulationViewModel spVm;
                if (stackDebugSupport.StackContentEvents.TryGetValue(addr, out var contentEvent))
                {
                    // --- There is an event for this SP address
                    spVm = new StackContentManipulationViewModel(contentEvent);
                }
                else
                {
                    // --- There is no event for this SP address
                    spVm = new StackContentManipulationViewModel
                    {
                        SpValue = $"{addr:X4}",
                        Content = $"{spContent:X4}",
                    };
                }

                // --- Rewrite the displayed list to avoid flickering
                if (index < oldCount)
                {
                    ContentManipulations[index] = spVm;
                }
                else
                {
                    ContentManipulations.Add(spVm);
                }

                index++;
                if (index > maxItems) break;
            }

            // --- Remove old items
            while (oldCount > index)
            {
                ContentManipulations.RemoveAt(ContentManipulations.Count - 1);
                oldCount--;
            }
        }
    }
}