using System.Windows.Input;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// Interaction logic for Single128KeyControl.xaml
    /// </summary>
    public partial class Enter128KeyControl
    {
        /// <summary>
        /// Responds to the event when the main key is clicked
        /// </summary>
        public event MouseButtonEventHandler MainKeyClicked;

        public Enter128KeyControl()
        {
            InitializeComponent();
        }
    }
}
