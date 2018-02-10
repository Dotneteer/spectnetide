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

        /// <summary>
        /// Initializes the tree node with the specified parent
        /// </summary>
        /// <param name="parent"></param>
        protected TestItemBase(TestItemBase parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// The optional parent node of this item
        /// </summary>
        public TestItemBase Parent { get; }

        /// <summary>
        /// Test state of the current node
        /// </summary>
        public TestState State
        {
            get => _state;
            set => Set(ref _state, value);
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

        public TestRootItem(TestItemBase parent) : base(parent)
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

        public TestFileItem(TestItemBase parent, TestFilePlan plan) : base(parent)
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
            throw new System.NotImplementedException();
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

        public TestSetItem(TestItemBase parent, TestSetPlan plan) : base(parent)
        {
            Plan = plan;
            NodeType = "Set";
        }

        /// <summary>
        /// Collect all child items to run
        /// </summary>
        public void CollectAllToRun()
        {
            throw new System.NotImplementedException();
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

        public TestItem(TestItemBase parent, TestBlockPlan plan) : base(parent)
        {
            Plan = plan;
            NodeType = "Test";
        }

        /// <summary>
        /// Collect all child items to run
        /// </summary>
        public void CollectAllToRun()
        {
            throw new System.NotImplementedException();
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

        public TestCaseItem(TestItemBase parent, TestCasePlan plan) : base(parent)
        {
            Plan = plan;
            NodeType = "Case";
        }
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