using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// Interaction logic for TestExplorerToolWindowControl.xaml
    /// </summary>
    public partial class TestExplorerToolWindowControl : ISupportsMvvm<TestExplorerToolWindowViewModel>
    {
        public SpectNetPackage Package => SpectNetPackage.Default;

        /// <summary>
        /// Gets the view model instance
        /// </summary>
        public TestExplorerToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        public void SetVm(TestExplorerToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public TestExplorerToolWindowControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Navigates to the source code of the selected test item
        /// </summary>
        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TestTree.SelectedItem is TestItemBase selected)
            {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                Package.ApplicationObject.Documents.Open(selected.FileName);
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
                Package.ApplicationObject.ExecuteCommand($"Edit.GoTo {selected.LineNo + 1}");
            }
            e.Handled = true;
        }

        /// <summary>
        /// Expands all tree nodes
        /// </summary>
        public void ExpandAll()
        {
            for (var i = 0; i < TestTree.Items.Count; i++)
            {
                var node = TestTree.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                node?.ExpandSubtree();
            }
        }

        /// <summary>
        /// Collapses all tree nodes
        /// </summary>
        public void CollapseAll()
        {
            for (var i = 0; i < TestTree.Items.Count; i++)
            {
                if (TestTree.ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem node)
                {
                    CollapseAll(node);
                }
            }

            void CollapseAll(TreeViewItem tvItem)
            {
                if (tvItem == null) return;
                var generator = tvItem.ItemContainerGenerator;
                for (var i = 0; i < tvItem.Items.Count; i++)
                {
                    var node = generator.ContainerFromIndex(i) as TreeViewItem;
                    CollapseAll(node);
                }
                tvItem.IsExpanded = false;
            }
        }
    }
}
