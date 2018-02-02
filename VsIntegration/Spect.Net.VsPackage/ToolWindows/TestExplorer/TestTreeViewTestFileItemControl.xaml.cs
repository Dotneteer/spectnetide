using System.Windows;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// Interaction logic for TestTreeViewFileItemControl.xaml
    /// </summary>
    public partial class TestTreeViewTestFileItemControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(TestTreeViewTestFileItemControl), new PropertyMetadata(default(string)));

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TestStateProperty = DependencyProperty.Register(
            "TestState", typeof(TestState), typeof(TestTreeViewTestFileItemControl), new PropertyMetadata(default(TestState)));

        public TestState TestState
        {
            get => (TestState) GetValue(TestStateProperty);
            set => SetValue(TestStateProperty, value);
        }

        public TestTreeViewTestFileItemControl()
        {
            InitializeComponent();
        }
    }
}