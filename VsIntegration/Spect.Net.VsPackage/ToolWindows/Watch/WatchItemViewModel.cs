using System.Windows;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.Watch
{
    /// <summary>
    /// This class represents a single watch entry
    /// </summary>
    public class WatchItemViewModel: EnhancedViewModelBase
    {
        private string _expression;
        private string _value;
        private bool _hasError;
        private WatchToolWindowViewModel _parent;
        private int _seqNo;

        public int SeqNo
        {
            get => _seqNo;
            set => Set(ref _seqNo, value);
        }

        /// <summary>
        /// Watch expression to evaluate
        /// </summary>
        public string Expression
        {
            get => _expression;
            set => Set(ref _expression, value);
        }

        /// <summary>
        /// Expression value
        /// </summary>
        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        /// <summary>
        /// Indicates if the expression evaluation has an error
        /// </summary>
        public bool HasError
        {
            get => _hasError;
            set => Set(ref _hasError, value);
        }

        /// <summary>
        /// Parent view model
        /// </summary>
        public WatchToolWindowViewModel Parent
        {
            get => _parent;
            set => Set(ref _parent, value);
        }

        public WatchItemViewModel()
        {
            //if (IsInDesignMode)
            //{
                Expression = "(34AC)";
                Value = "003457683";
                HasError = false;
            //}
        }

        public void StartSizing(Point startPosition)
        {
            Parent?.StartSizing(startPosition);
        }
    }
}