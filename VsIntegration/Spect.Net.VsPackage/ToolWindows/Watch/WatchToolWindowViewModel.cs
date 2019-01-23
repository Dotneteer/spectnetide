using System.Collections.ObjectModel;
using System.Windows;

namespace Spect.Net.VsPackage.ToolWindows.Watch
{
    /// <summary>
    /// This view model represents the watch view's data and behavior
    /// </summary>
    public class WatchToolWindowViewModel : SpectrumGenericToolWindowViewModel
    {
        private int _labelWidth;
        private bool _sizing;
        private double _startPosition;
        private double _startSize;

        /// <summary>
        /// Items of the Watch tool window
        /// </summary>
        public ObservableCollection<WatchItemViewModel> WatchItems { get; } =
            new ObservableCollection<WatchItemViewModel>();

        /// <summary>
        /// The width of the label
        /// </summary>
        public int LabelWidth
        {
            get => _labelWidth;
            set => Set(ref _labelWidth, value);
        }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public WatchToolWindowViewModel()
        {
            //if (IsInDesignMode)
            //{
                WatchItems.Add(new WatchItemViewModel());
                WatchItems.Add(new WatchItemViewModel());
                WatchItems.Add(new WatchItemViewModel());
                foreach (var item in WatchItems)
                {
                    item.Parent = this;
                }
            //}

            LabelWidth = 100;
        }

        public bool ProcessCommandline(string commandText, out string validationMessage)
        {
            validationMessage = null;
            var parser = new WatchCommandParser(commandText);
            switch (parser.Command)
            {
                case WatchCommandType.Invalid:
                    validationMessage = "Invalid command syntax";
                    return false;

                case WatchCommandType.AddWatch:
                    var newItem = new WatchItemViewModel
                    {
                        SeqNo = WatchItems.Count + 1,
                        Expression = parser.Arg1,
                        Value = "<not evaluated yet>",
                        Parent = this,
                        HasError = false
                    };
                    WatchItems.Add(newItem);
                    break;

                case WatchCommandType.RemoveWatch:
                    var index = parser.Address - 1;
                    if (index >= 0 && index < WatchItems.Count)
                    {
                        WatchItems.RemoveAt(index);
                        RenumberWatchItems();
                    }
                    break;

                case WatchCommandType.EraseAll:
                    WatchItems.Clear();
                    break;

                case WatchCommandType.MoveItem:
                    break;

                case WatchCommandType.ChangeLabelWidth:
                    LabelWidth = parser.Address;
                    break;

                default:
                    return false;
            }

            return true;
        }

        public void StartSizing(Point startPosition)
        {
            _sizing = true;
            _startSize = LabelWidth;
            _startPosition = startPosition.X;
        }

        public void HandleSizing(Point startPosition)
        {
            if (!_sizing) return;
            var newSize = _startSize - _startPosition + startPosition.X;
            LabelWidth = (int)newSize;
        }

        public void EndSizing()
        {
            _sizing = false;
        }

        private void RenumberWatchItems()
        {
            for (var i = 0; i < WatchItems.Count; i++)
            {
                WatchItems[i].SeqNo = i + 1;
            }
        }
    }
}