using System.Windows;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// Interaction logic for TestTreeViewTestItemControl.xaml
    /// </summary>
    public partial class TestTreeViewTestCaseItemControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(TestTreeViewTestCaseItemControl), new PropertyMetadata(default(string)));

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TestStateProperty = DependencyProperty.Register(
            "TestState", typeof(TestState), typeof(TestTreeViewTestCaseItemControl), new PropertyMetadata(default(TestState)));

        public TestState TestState
        {
            get => (TestState) GetValue(TestStateProperty);
            set => SetValue(TestStateProperty, value);
        }

        public TestTreeViewTestCaseItemControl()
        {
            InitializeComponent();
        }
    }
}