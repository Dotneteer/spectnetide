using System;
using System.Collections.ObjectModel;
using System.Windows;
using Antlr4.Runtime;
using Spect.Net.EvalParser;
using Spect.Net.EvalParser.Generated;
using Spect.Net.EvalParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Machine;

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

        public bool ItemsVisible => WatchItems.Count > 0;

        public SpectrumEvaluationContext EvalContext { get; }
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
            EvalContext = new SpectrumEvaluationContext(MachineViewModel.SpectrumVm);
            WatchItems.CollectionChanged += ItemsCollectionChanged;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            WatchItems.CollectionChanged -= ItemsCollectionChanged;
            EvalContext.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// This event is raised when the command line has been modified
        /// </summary>
        public event EventHandler<string> CommandLineModified;

        /// <summary>
        /// Updates the watch item
        /// </summary>
        /// <param name="item">Watch item to update</param>
        public void UpdateWatchItem(WatchItemViewModel item)
        {
            var format = string.IsNullOrEmpty(item.Format) ? "" : $" :{item.Format}";
            var commandLine = $"* {item.SeqNo} {item.ExpressionNode}{format}";
            CommandLineModified?.Invoke(this, commandLine);
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
        /// Moves the specified item down
        /// </summary>
        /// <param name="item">Item to move down</param>
        public void MoveDownWatchItem(WatchItemViewModel item)
        {
            var itemIndex = WatchItems.IndexOf(item);
            if (itemIndex < 0 || itemIndex > WatchItems.Count - 2) return;
            var tmp = WatchItems[itemIndex + 1];
            WatchItems[itemIndex + 1] = WatchItems[itemIndex];
            WatchItems[itemIndex] = tmp;
            RefreshWatchItems();
        }

        /// <summary>
        /// Moves the specified item up
        /// </summary>
        /// <param name="item">Item to move up</param>
        public void MoveUpWatchItem(WatchItemViewModel item)
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
                    // --- Parse the given expression
                    if (!PrepareWatchItem(parser.Arg1, ref validationMessage, out var newItem))
                    {
                        return false;
                    }
                    WatchItems.Add(newItem);
                    RefreshWatchItems();
                    if (MachineViewModel.MachineState == VmState.Paused
                        || MachineViewModel.MachineState == VmState.Running)
                    {
                        EvaluateWatchItems();
                    }
                    break;

                case WatchCommandType.RemoveWatch:
                    var index = parser.Address - 1;
                    if (index >= 0 && index < WatchItems.Count)
                    {
                        WatchItems.RemoveAt(index);
                        RefreshWatchItems();
                    }
                    break;

                case WatchCommandType.UpdateWatch:
                    if (!PrepareWatchItem(parser.Arg1, ref validationMessage, out var updatedItem))
                    {
                        return false;
                    }
                    index = parser.Address - 1;
                    if (index < 0 || index >= WatchItems.Count)
                    {
                        validationMessage = $"The index value must be between 1 and {WatchItems.Count}.";
                        return false;
                    }
                    WatchItems[index] = updatedItem;
                    RefreshWatchItems();
                    if (MachineViewModel.MachineState == VmState.Paused
                        || MachineViewModel.MachineState == VmState.Running)
                    {
                        EvaluateWatchItems();
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
        /// Prepares the WatchItemViewModel instance from the command line
        /// </summary>
        /// <param name="expression">Expression to parse</param>
        /// <param name="validationMessage">Validation message to pass</param>
        /// <param name="newItem">The new item</param>
        /// <returns></returns>
        private bool PrepareWatchItem(string expression, ref string validationMessage,
            out WatchItemViewModel newItem)
        {
            newItem = null;
            var inputStream = new AntlrInputStream(expression);
            var lexer = new Z80EvalLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var evalParser = new Z80EvalParser(tokenStream);
            var context = evalParser.compileUnit();
            var visitor = new Z80EvalVisitor();
            var z80Expr = (Z80ExpressionNode) visitor.Visit(context);
            if (evalParser.SyntaxErrors.Count > 0)
            {
                validationMessage = "Syntax error in the specified watch expression";
                return false;
            }

            // --- Create the watch item
            var formatStr = z80Expr.FormatSpecifier?.Format == null
                ? null
                : $"{z80Expr.FormatSpecifier.Format}";
            newItem = new WatchItemViewModel
            {
                SeqNo = WatchItems.Count + 1,
                ExpressionNode = z80Expr.Expression,
                Format = formatStr,
                Expression = z80Expr.Expression.ToString(),
                Value = "(not evaluated yet)",
                Parent = this,
                HasError = false
            };
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
        /// Override to handle the paused state of the virtual machine.
        /// </summary>
        /// <remarks>This method is called for the first pause, too</remarks>
        protected override void OnPaused()
        {
            base.OnPaused();
            EvaluateWatchItems();
        }

        /// <summary>
        /// Set the machine status when the screen has been refreshed
        /// </summary>
        protected override void OnScreenRefreshed()
        {
            base.OnScreenRefreshed();
            if (ScreenRefreshCount % 10 != 0) return;
            EvaluateWatchItems();
        }

        /// <summary>
        /// Signs the changes of the WatchItems collection
        /// </summary>
        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged("ItemsVisible");
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

        /// <summary>
        /// Evaluates the watch items
        /// </summary>
        private void EvaluateWatchItems()
        {
            foreach (var item in WatchItems)
            {
                try
                {
                    if (item.ExpressionNode == null) continue;
                    var value = item.ExpressionNode.Evaluate(EvalContext);
                    item.HasError = value == ExpressionValue.Error;
                    item.Value = item.HasError 
                        ? item.ExpressionNode.EvaluationError 
                        : FormatWatchExpression(item.ExpressionNode, value, item.Format);
                }
                catch
                {
                    // --- This exception is intentionally ignored.
                }
            }
        }

        /// <summary>
        /// Formats a watch expression
        /// </summary>
        /// <param name="expression">Expression node</param>
        /// <param name="exprValue">Evaluated expression value</param>
        /// <param name="formatSpecifier">Format specifier</param>
        /// <returns></returns>
        public static string FormatWatchExpression(ExpressionNode expression, ExpressionValue exprValue,
            string formatSpecifier)
        {
            // --- We do not accept error
            if (exprValue == ExpressionValue.Error)
            {
                return expression.EvaluationError;
            }

            // --- No format provided, use default based on expression type
            if (string.IsNullOrEmpty(formatSpecifier))
            {
                switch (expression.ValueType)
                {
                    case ExpressionValueType.Bool:
                        formatSpecifier = "F";
                        break;
                    case ExpressionValueType.Byte:
                        formatSpecifier = "B";
                        break;
                    case ExpressionValueType.DWord:
                        formatSpecifier = "DW";
                        break;
                    default:
                        formatSpecifier = "W";
                        break;
                }
            }

            // --- Convert value to the specified output
            var v = exprValue.Value;
            switch (formatSpecifier)
            {
                case "F":
                    return v == 0 ? "FALSE" : "TRUE";
                case "B":
                    var b = (byte) v;
                    return $"#{b:X2} ({b})";
                case "-B":
                    var sb = (sbyte)v;
                    return $"#{sb:X2} ({sb})";
                case "C":
                    var c = (char) (byte) v;
                    return (byte)v >= 32 && (byte)v <= 126 ? $"'{c}'" : $"'\\0x{(byte)v:X2}'";
                case "H4":
                    var b0 = (byte)v;
                    var b1 = (byte)(v >> 8);
                    return $"#{b0:X2} #{b1:X2}";
                case "H8":
                    b0 = (byte)v;
                    b1 = (byte)(v >> 8);
                    var b2 = (byte)(v >> 16);
                    var b3 = (byte)(v >> 24);
                    return $"#{b0:X2} #{b1:X2} #{b2:X2} #{b3:X2}";
                case "W":
                    var w = (ushort)v;
                    return $"#{w:X4} ({w})";
                case "-W":
                    var sw = (short)v;
                    return $"#{sw:X4} ({sw})";
                case "DW":
                    return $"#{v:X8} ({v})";
                case "-DW":
                    var sdw = (int) v;
                    return $"#{sdw:X8} ({sdw})";
                case "%8":
                    var s0 = Convert.ToString((byte) v, 2).PadLeft(8, '0');
                    return $"{s0}";
                case "%16":
                    s0 = Convert.ToString((byte)v, 2).PadLeft(8, '0');
                    var s1 = Convert.ToString((byte)(v >> 8), 2).PadLeft(8, '0');
                    return $"{s1} {s0}";
                case "%32":
                    s0 = Convert.ToString((byte)v, 2).PadLeft(8, '0');
                    s1 = Convert.ToString((byte)(v >> 8), 2).PadLeft(8, '0');
                    var s2 = Convert.ToString((byte)(v >> 16), 2).PadLeft(8, '0');
                    var s3 = Convert.ToString((byte)(v >> 24), 2).PadLeft(8, '0');
                    return $"{s3} {s2} {s1} {s0}";
                default:
                    return v.ToString();
            }
        }
    }
}