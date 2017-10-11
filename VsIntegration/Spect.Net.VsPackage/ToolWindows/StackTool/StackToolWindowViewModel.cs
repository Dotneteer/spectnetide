namespace Spect.Net.VsPackage.ToolWindows.StackTool
{
    /// <summary>
    /// This class represents the view model of the Z80 CPU Stack tool window
    /// </summary>
    public class StackToolWindowViewModel: SpectrumGenericToolWindowViewModel
    {
        private bool _stackContentViewVisible;

        /// <summary>
        /// Indicates if the stack content view is visible
        /// </summary>
        public bool StackContentViewVisible
        {
            get => _stackContentViewVisible;
            set => Set(ref _stackContentViewVisible, value);
        }
    }
}