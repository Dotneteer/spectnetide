using System.Collections.ObjectModel;
using System.Windows;

namespace Spect.Net.VsPackage.ToolWindows.Watch
{
    /// <summary>
    /// This view model represents the watch view's data and behavior
    /// </summary>
    public class WatchToolWindowViewModel : SpectrumGenericToolWindowViewModel
    {
        private const int MIN_LABEL_WIDTH = 40;

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
            if (IsInDesignMode)
            {
                WatchItems.Add(new WatchItemViewModel());
                WatchItems.Add(new WatchItemViewModel());
                WatchItems.Add(new WatchItemViewModel());
                foreach (var item in WatchItems)
                {
                    item.Parent = this;
                }
            }

            LabelWidth = 100;
        }

        /// <summary>
        /// Removes the specified item from the collection
        /// </summary>
        /// <param name="item">Item to remove</param>
        public void RemoveWatchItem(WatchItemViewModel item)
        {
            WatchItems.Remove(item);
            RefreshWatchItems();
        }

        /// <summary>
        /// Moves the specified item up
        /// </summary>
        /// <param name="item">Item to move up</param>
        public void MoveUpWatchItem(WatchItemViewModel item)
        {
            var itemIndex = WatchItems.IndexOf(item);
            if (itemIndex < 0 || itemIndex > WatchItems.Count - 2) return;
            var tmp = WatchItems[itemIndex + 1];
            WatchItems[itemIndex + 1] = WatchItems[itemIndex];
            WatchItems[itemIndex] = tmp;
            RefreshWatchItems();
        }

        /// <summary>
        /// Moves the specified item down
        /// </summary>
        /// <param name="item">Item to move down</param>
        public void MoveDownWatchItem(WatchItemViewModel item)
        {
            var itemIndex = WatchItems.IndexOf(item);
            if (itemIndex < 1 || itemIndex > WatchItems.Count - 1) return;
            var tmp = WatchItems[itemIndex - 1];
            WatchItems[itemIndex - 1] = WatchItems[itemIndex];
            WatchItems[itemIndex] = tmp;
            RefreshWatchItems();
        }

        /// <summary>
        /// Processes the command line instruction
        /// </summary>
        /// <param name="commandText">Command text to process</param>
        /// <param name="validationMessage">Validation message in case of error</param>
        /// <returns>True, if command processed</returns>
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
                    RefreshWatchItems();
                    break;

                case WatchCommandType.RemoveWatch:
                    var index = parser.Address - 1;
                    if (index >= 0 && index < WatchItems.Count)
                    {
                        WatchItems.RemoveAt(index);
                        RefreshWatchItems();
                    }
                    break;

                case WatchCommandType.EraseAll:
                    WatchItems.Clear();
                    break;

                case WatchCommandType.MoveItem:
                    var index1 = parser.Address;
                    var index2 = parser.Address2;
                    if (index1 < 1 || index1 > WatchItems.Count || index2 < 1 || index2 > WatchItems.Count)
                    {
                        validationMessage = $"Index values must be between 1 and {WatchItems.Count}.";
                        return false;
                    }

                    index1--;
                    index2--;
                    var tmp = WatchItems[index1];
                    WatchItems[index1] = WatchItems[index2];
                    WatchItems[index2] = tmp;
                    RefreshWatchItems();
                    break;

                case WatchCommandType.ChangeLabelWidth:
                    LabelWidth = parser.Address < MIN_LABEL_WIDTH ? MIN_LABEL_WIDTH : parser.Address;
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Starts sizing the label width
        /// </summary>
        /// <param name="startPosition">Mouse position</param>
        public void StartSizing(Point startPosition)
        {
            _sizing = true;
            _startSize = LabelWidth;
            _startPosition = startPosition.X;
        }

        /// <summary>
        /// Handles a sizing when the mouse moves
        /// </summary>
        /// <param name="startPosition">Mouse position</param>
        public void HandleSizing(Point startPosition)
        {
            if (!_sizing) return;
            var newSize = _startSize - _startPosition + startPosition.X;
            LabelWidth = newSize < MIN_LABEL_WIDTH ? MIN_LABEL_WIDTH : (int)newSize;
        }

        /// <summary>
        /// Ends sizing
        /// </summary>
        public void EndSizing()
        {
            _sizing = false;
        }

        /// <summary>
        /// Override to handle the start of the virtual machine.
        /// </summary>
        /// <remarks>This method is called for the first start, too</remarks>
        protected override void OnStart()
        {
            base.OnStart();
            foreach (var item in WatchItems)
            {
                item.Value = "(not evaluated yet)";
            }
        }

        /// <summary>
        /// Override to handle the paused state of the virtual machine.
        /// </summary>
        /// <remarks>This method is called for the first pause, too</remarks>
        protected override void OnPaused()
        {
            base.OnPaused();
            foreach (var item in WatchItems)
            {
                item.Value = "(paused)";
            }
        }

        /// <summary>
        /// Set the machine status when the screen has been refreshed
        /// </summary>
        protected override void OnScreenRefreshed()
        {
            base.OnScreenRefreshed();
            if (ScreenRefreshCount % 10 != 0) return;
            foreach (var item in WatchItems)
            {
                item.Value = ScreenRefreshCount.ToString();
            }
        }

        /// <summary>
        /// Renumbers the watch items
        /// </summary>
        private void RefreshWatchItems()
        {
            for (var i = 0; i < WatchItems.Count; i++)
            {
                WatchItems[i].SeqNo = i + 1;
                WatchItems[i].RefreshCommandStatus();
            }
        }
    }
}