namespace Spect.Net.Ide.ViewModels
{
    /// <summary>
    /// The view model of a tool window
    /// </summary>
    public class ToolWindowViewModel : PaneViewModel
    {
        private bool _isVisible;

        /// <summary>
        /// Is the tool window visible?
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { Set(ref _isVisible, value); }
        }
    }
}