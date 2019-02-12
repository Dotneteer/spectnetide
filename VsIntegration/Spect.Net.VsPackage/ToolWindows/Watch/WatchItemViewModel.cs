using System.Windows;
using Spect.Net.EvalParser.SyntaxTree;
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
        private string _format;

        public int SeqNo
        {
            get => _seqNo;
            set => Set(ref _seqNo, value);
        }

        /// <summary>
        /// The format of the expression
        /// </summary>
        public string Format
        {
            get => _format;
            set => Set(ref _format, value);
        }

        /// <summary>
        /// The expression node to evaluate
        /// </summary>
        public ExpressionNode ExpressionNode { get; set; }

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

        /// <summary>
        /// Tests if this item can move up
        /// </summary>
        public bool CanMoveUp => SeqNo > 1;

        /// <summary>
        /// Tests if this item can move down
        /// </summary>
        public bool CanMoveDown => SeqNo < Parent.WatchItems.Count;

        /// <summary>
        /// Notifies the parent to modify this item
        /// </summary>
        public RelayCommand UpdateCommand { get; set; }

        /// <summary>
        /// Notifies the parent to remove this item
        /// </summary>
        public RelayCommand RemoveCommand { get; set; }

        /// <summary>
        /// Notifies the parent that this item should be moved up
        /// </summary>
        public RelayCommand MoveUpCommand { get; set; }

        /// <summary>
        /// Notifies the parent that this item should be moved down
        /// </summary>
        public RelayCommand MoveDownCommand { get; set; }

        /// <summary>
        /// Initializes the view model
        /// </summary>
        public WatchItemViewModel()
        {
            if (IsInDesignMode)
            {
                Expression = "(34AC)";
                Format = "byte";
                Value = "003457683";
                HasError = false;
            }
            UpdateCommand = new RelayCommand(() =>
            {
                Parent.UpdateWatchItem(this);
            });
            RemoveCommand = new RelayCommand(() =>
            {
                Parent.RemoveWatchItem(this);
            });
            MoveUpCommand = new RelayCommand(() =>
            {
                Parent.MoveUpWatchItem(this);
            });
            MoveDownCommand = new RelayCommand(() =>
            {
                Parent.MoveDownWatchItem(this);
            });
        }

        /// <summary>
        /// Notifies the parent that sizing is started.
        /// </summary>
        /// <param name="startPosition"></param>
        public void StartSizing(Point startPosition)
        {
            Parent?.StartSizing(startPosition);
        }

        /// <summary>
        /// Refreshes the statues of commands
        /// </summary>
        public void RefreshCommandStatus()
        {
            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanged("CanMoveDown");
            RaisePropertyChanged("CanMoveUp");
            // ReSharper restore ExplicitCallerInfoArgument
        }
    }
}