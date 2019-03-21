using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.StackTool
{
    /// <summary>
    /// This class provides information about a stack pointer manipulation
    /// event
    /// </summary>
    public class StackPointerManipulationViewModel: EnhancedViewModelBase
    {
        private string _operationAddress;
        private string _operation;
        private string _oldValue;
        private string _newValue;
        private long _tacts;

        /// <summary>
        /// Address of the operation that modified SP
        /// </summary>
        public string OperationAddress
        {
            get => _operationAddress;
            set => Set(ref _operationAddress, value);
        }

        /// <summary>
        /// The operation that modified SP
        /// </summary>
        public string Operation
        {
            get => _operation;
            set => Set(ref _operation, value);
        }

        /// <summary>
        /// Old SP value
        /// </summary>
        public string OldValue
        {
            get => _oldValue;
            set => Set(ref _oldValue, value);
        }

        /// <summary>
        /// New SP value
        /// </summary>
        public string NewValue
        {
            get => _newValue;
            set => Set(ref _newValue, value);
        }

        /// <summary>
        /// CPU tacts after the operation
        /// </summary>
        public long Tacts
        {
            get => _tacts;
            set => Set(ref _tacts, value);
        }

        public StackPointerManipulationViewModel()
        {
            if (IsInDesignMode)
            {
                OperationAddress = "12AB";
                Operation = "ld sp,#1234";
                OldValue = "2345";
                NewValue = "1234";
                Tacts = 123456789L;
            }
        }

        public StackPointerManipulationViewModel(StackPointerManipulationEvent ev)
        {
            OperationAddress = $"{ev.OperationAddress:X4}";
            Operation = ev.Operation;
            OldValue = $"{ev.OldValue:X4}";
            NewValue = $"{ev.NewValue:X4}";
            Tacts = ev.Tacts;
        }
    }
}