using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TestTree.SelectedItem is TestItemBase selected)
            {
                Package.ApplicationObject.Documents.Open(selected.FileName);
                Package.ApplicationObject.ExecuteCommand($"Edit.GoTo {selected.LineNo + 1}");
            }
            e.Handled = true;
        }

        public void ExpandAll()
        {
            for (var i = 0; i < TestTree.Items.Count; i++)
            {
                var node = TestTree.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                node?.ExpandSubtree();
            }
        }

        public void CollapseAll()
        {
            for (var i = 0; i < TestTree.Items.Count; i++)
            {
                var node = TestTree.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                if (node != null)
                {
                    CollapseAll(node);
                }
            }
        }

        public void CollapseAll(TreeViewItem tvItem)
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

        private void OnExpandedOrCollapsed(object sender, RoutedEventArgs e)
        {
            Vm.AutoExpandAfterCompile = false;
            Vm.AutoCollapseAfterCompile = false;
        }
    }
}
