using System.Windows;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// Interaction logic for TestTreeViewFileItemControl.xaml
    /// </summary>
    public partial class TestTreeViewItemControl
    {
        public TestTreeViewItemControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(args.NewValue is TestItemBase item)) return;
            var resource = TryFindResource(item.NodeType);
            if (resource == null) return;
            TypeContent.Content = resource;
            var styleName = "Z80Text";
            switch (item.NodeType)
            {
                case "Set":
                    styleName = "Z80StatusText";
                    break;
                case "Test":
                case "Case":
                    styleName = "Z80HilitedText";
                    break;
            }

            if (!(TryFindResource(styleName) is Style style)) return;
            TitleText.Style = style;
        }
    }
}