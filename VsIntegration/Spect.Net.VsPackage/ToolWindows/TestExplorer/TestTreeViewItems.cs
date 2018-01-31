using System.Collections.ObjectModel;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// This abstract class is the root of every tree view item types that can
    /// be used in a Test Tree View.
    /// </summary>
    public class TestTreeItem: EnhancedViewModelBase
    {
        private TestState _state;
        private TestNodeType _nodeType;
        private string _title;
        private ObservableCollection<TestTreeItem> _childItems;

        /// <summary>
        /// Test state of the current node
        /// </summary>
        public TestState State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        /// <summary>
        /// The visual type of the node
        /// </summary>
        public TestNodeType NodeType
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
        public ObservableCollection<TestTreeItem> ChildItems
        {
            get => _childItems;
            set => Set(ref _childItems, value);
        }
    }

    /// <summary>
    /// The visual type of a test tree view node
    /// </summary>
    public enum TestNodeType
    {
        TestFile,
        TestSet,
        Test,
        Case,
        Category
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