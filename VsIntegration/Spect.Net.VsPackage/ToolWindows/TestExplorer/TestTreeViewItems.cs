using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Spect.Net.TestParser.Plan;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// This abstract class is the root of every tree view item types that can
    /// be used in a Test Tree View.
    /// </summary>
    public abstract class TestItemBase: EnhancedViewModelBase
    {
        private TestState _state;
        private string _title;
        private ObservableCollection<TestItemBase> _childItems = new ObservableCollection<TestItemBase>();
        private string _nodeType;
        private bool _isSelected;
        private ObservableCollection<LogEntry> _logItems = new ObservableCollection<LogEntry>();

        protected TestItemBase()
        {
        }

        /// <summary>
        /// Initializes the tree node with the specified parent
        /// </summary>
        /// <param name="vm">Parent view model</param>
        /// <param name="parent">Parent node</param>
        protected TestItemBase(TestExplorerToolWindowViewModel vm, TestItemBase parent)
        {
            Parent = parent;
            Vm = vm ?? throw new ArgumentNullException(nameof(vm));
        }

        /// <summary>
        /// The optional parent node of this item
        /// </summary>
        public TestItemBase Parent { get; }

        /// <summary>
        /// The view model that owns this node item
        /// </summary>
        public TestExplorerToolWindowViewModel Vm { get; }

        /// <summary>
        /// Test state of the current node
        /// </summary>
        public TestState State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        /// <summary>
        /// Indicates if the item is selected in the tree view
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (!Set(ref _isSelected, value)) return;
                if (value)
                {
                    Vm.SelectedItem = this;
                }
            }
        }

        /// <summary>
        /// Node type
        /// </summary>
        public string NodeType
        {
            get => _nodeType;
            set => Set(ref _nodeType, value);
        }

        /// <summary>
        /// The title of the node
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        /// <summary>
        /// The collection of child tree nodes
        /// </summary>
        public ObservableCollection<TestItemBase> ChildItems
        {
            get => _childItems;
            set => Set(ref _childItems, value);
        }

        /// <summary>
        /// File behind the tree node
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Line number that represents the node in the file
        /// </summary>
        public int LineNo { get; set; }

        /// <summary>
        /// Column number that represents the node
        /// </summary>
        public int ColumnNo { get; set; }

        /// <summary>
        /// The log items belonging to the entry
        /// </summary>
        public ObservableCollection<LogEntry> LogItems
        {
            get => _logItems;
            set => Set(ref _logItems, value);
        }

        /// <summary>
        /// Executes the specified action on the subtree starting with this node
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="except">Do not execute the action for this item</param>
        /// <param name="includeThis">
        /// True, if the root should be included; otherwise, false
        /// </param>
        public void SubTreeForEach(Action<TestItemBase> action, TestItemBase except = null, bool includeThis = true)
        {
            if (this == except) return;
            if (includeThis) action(this);
            foreach (var child in ChildItems)
            {
                child.SubTreeForEach(action, except);
            }
        }

        /// <summary>
        /// Logs a new message for this item
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="type">Message type</param>
        public void Log(string message, LogEntryType type = LogEntryType.Info)
        {
            LogItems.Add(new LogEntry(message, type));
        }
    }

    /// <summary>
    /// Represents a file item in the test tree
    /// </summary>
    public class TestRootItem : TestItemBase
    {
        /// <summary>
        /// The list of test file items to run
        /// </summary>
        public List<TestFileItem> TestFilesToRun { get; } = new List<TestFileItem>();

        public TestRootItem(TestExplorerToolWindowViewModel vm, TestItemBase parent) : base(vm, parent)
        {
            NodeType = "Root";
        }
    }

    /// <summary>
    /// Represents a file item in the test tree
    /// </summary>
    public class TestFileItem : TestItemBase
    {
        /// <summary>
        /// The plan represented by this node
        /// </summary>
        public TestFilePlan Plan { get; }

        /// <summary>
        /// List of test sets to run
        /// </summary>
        public List<TestSetItem> TestSetsToRun { get; } = new List<TestSetItem>();

        public TestFileItem()
        {
            if (!IsInDesignMode) return;
            Plan = null;
            NodeType = "File";
            State = TestState.Success;
            Title = "Title";
        }

        public TestFileItem(TestExplorerToolWindowViewModel vm, TestItemBase parent, TestFilePlan plan) : base(vm, parent)
        {
            Plan = plan;
            NodeType = "File";
            if (!IsInDesignMode) return;
            State = TestState.Success;
        }

        /// <summary>
        /// Collect all child items to run
        /// </summary>
        public void CollectAllToRun()
        {
            TestSetsToRun.Clear();
            foreach (var item in ChildItems)
            {
                if (!(item is TestSetItem setItem)) continue;
                TestSetsToRun.Add(setItem);
                setItem.CollectAllToRun();
            }
        }
    }

    /// <summary>
    /// Represents a test set item in the test tree
    /// </summary>
    public class TestSetItem : TestItemBase
    {
        /// <summary>
        /// The plan represented by this node
        /// </summary>
        public TestSetPlan Plan { get; }

        /// <summary>
        /// List of tests to run
        /// </summary>
        public List<TestItem> TestsToRun { get; } = new List<TestItem>();

        public TestSetItem(TestExplorerToolWindowViewModel vm, TestItemBase parent, TestSetPlan plan) : base(vm, parent)
        {
            Plan = plan;
            NodeType = "Set";
        }

        /// <summary>
        /// Collect all child items to run
        /// </summary>
        public void CollectAllToRun()
        {
            TestsToRun.Clear();
            foreach (var item in ChildItems)
            {
                if (!(item is TestItem testItem)) continue;
                TestsToRun.Add(testItem);
                testItem.CollectAllToRun();
            }
        }
    }

    /// <summary>
    /// Represents a test item in the test tree
    /// </summary>
    public class TestItem : TestItemBase
    {
        /// <summary>
        /// The plan represented by this node
        /// </summary>
        public TestBlockPlan Plan { get; }

        /// <summary>
        /// List of test cases to run
        /// </summary>
        public List<TestCaseItem> TestCasesToRun { get; } = new List<TestCaseItem>();

        public TestItem(TestExplorerToolWindowViewModel vm, TestItemBase parent, TestBlockPlan plan) : base(vm, parent)
        {
            Plan = plan;
            NodeType = "Test";
        }

        /// <summary>
        /// Collect all child items to run
        /// </summary>
        public void CollectAllToRun()
        {
            TestCasesToRun.Clear();
            foreach (var item in ChildItems)
            {
                if (!(item is TestCaseItem caseItem)) continue;
                TestCasesToRun.Add(caseItem);
            }
        }
    }

    /// <summary>
    /// Represents a test case item in the test tree
    /// </summary>
    public class TestCaseItem : TestItemBase
    {
        /// <summary>
        /// The plan represented by this node
        /// </summary>
        public TestCasePlan Plan { get; }

        public TestCaseItem(TestExplorerToolWindowViewModel vm, TestItemBase parent, TestCasePlan plan) : base(vm, parent)
        {
            Plan = plan;
            NodeType = "Case";
        }
    }

    /// <summary>
    /// An entry of a test item
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Log message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Type of the entry
        /// </summary>
        public LogEntryType Type { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public LogEntry(string message, LogEntryType type)
        {
            Message = message;
            Type = type;
        }
    }

    /// <summary>
    /// Types of log entries
    /// </summary>
    public enum LogEntryType
    {
        Info,
        Success,
        Fail
    }

    /// <summary>
    /// Represents the possible states/outcomes of a test tree view node
    /// </summary>
    /// <remarks>
    /// Keep the order of the enumerated values, the business logic makes
    /// assumptions on this order
    /// </remarks>
    public enum TestState
    {
        NotRun = 0,
        Running = 1,
        Inconclusive = 2,
        Aborted = 3,
        Failed = 4,
        Success = 5
    }
}