using System.Collections.ObjectModel;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// This abstract class is the root of every tree view item types that can
    /// be used in a Test Tree View.
    /// </summary>
    public abstract class TestTreeItemBase: EnhancedViewModelBase
    {
        private TestState _state;
        private string _title;
        private ObservableCollection<TestTreeItemBase> _childItems = new ObservableCollection<TestTreeItemBase>();
        private string _nodeType;

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
        public ObservableCollection<TestTreeItemBase> ChildItems
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
    public class TestTreeRootItem : TestTreeItemBase
    {
        public TestTreeRootItem()
        {
            NodeType = "Root";
        }
    }

    /// <summary>
    /// Represents a file item in the test tree
    /// </summary>
    public class TestTreeFileItem : TestTreeItemBase
    {
        public TestTreeFileItem()
        {
            NodeType = "File";
            if (!IsInDesignMode) return;
            State = TestState.Success;
        }
    }

    /// <summary>
    /// Represents a test set item in the test tree
    /// </summary>
    public class TestTreeTestSetItem : TestTreeItemBase
    {
        public TestTreeTestSetItem()
        {
            NodeType = "Set";
        }
    }

    /// <summary>
    /// Represents a test item in the test tree
    /// </summary>
    public class TestTreeTestItem : TestTreeItemBase
    {
        public TestTreeTestItem()
        {
            NodeType = "Test";
        }
    }

    /// <summary>
    /// Represents a test case item in the test tree
    /// </summary>
    public class TestTreeTestCaseItem : TestTreeItemBase
    {
        public TestTreeTestCaseItem()
        {
            NodeType = "Case";
        }
    }

    /// <summary>
    /// Represents the possible states/outcomes of a test tree view node
    /// </summary>
    public enum TestState
    {
        NotRun,
        Running,
        Inconclusive,
        Aborted,
        Failed,
        Success
    }
}