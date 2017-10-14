using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.StackTool
{
    /// <summary>
    /// This class provides information about a stack pointer manipulation
    /// event
    /// </summary>
    public class StackContentManipulationViewModel: EnhancedViewModelBase
    {
        private string _operationAddress;
        private string _operation;
        private string _spValue;
        private string _content;
        private long _tacts;
        private bool _hasEventInfo;

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
        public string SpValue
        {
            get => _spValue;
            set => Set(ref _spValue, value);
        }

        /// <summary>
        /// New SP value
        /// </summary>
        public string Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        /// <summary>
        /// CPU tacts after the operation
        /// </summary>
        public long Tacts
        {
            get => _tacts;
            set => Set(ref _tacts, value);
        }

        /// <summary>
        /// Signs if this item is based on event info
        /// </summary>
        public bool HasEventInfo
        {
            get => _hasEventInfo;
            set => Set(ref _hasEventInfo, value);
        }

        public StackContentManipulationViewModel()
        {
            if (IsInDesignMode)
            {
                OperationAddress = "12AB";
                Operation = "call #1234";
                SpValue = "2345";
                Content = "1234";
                Tacts = 123456789L;
                HasEventInfo = true;
            }
        }

        public StackContentManipulationViewModel(StackContentManipulationEvent ev)
        {
            OperationAddress = $"{ev.OperationAddress:X4}";
            Operation = ev.Operation;
            SpValue = $"{ev.SpValue:X4}";
            Content = ev.Content == null ? null : $"{ev.Content.Value:X4}";
            Tacts = ev.Tacts;
            HasEventInfo = true;
        }
    }
}