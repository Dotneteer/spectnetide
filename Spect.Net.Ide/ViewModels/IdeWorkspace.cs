using System.Collections.ObjectModel;
using System.Windows.Controls;
using Spect.Net.Ide.Mvvm.Navigation;
using Spect.Net.Ide.ToolWindows;

namespace Spect.Net.Ide.ViewModels
{
    /// <summary>
    /// The entire workspace of the IDE
    /// </summary>
    public class IdeWorkspace: ViewModelBaseWithDesignTimeFix
    {
        /// <summary>
        /// The tool windows within the IDE
        /// </summary>
        public ObservableCollection<ToolWindowViewModel> ToolWindows { get; set; }

        /// <summary>
        /// The selectro to choose the appropriate tool window template
        /// </summary>
        public DataTemplateSelector ToolWindowTemplateSelector { get; set; }

        public IdeWorkspace()
        {
            ToolWindows = new ObservableCollection<ToolWindowViewModel>();
            ToolWindowTemplateSelector = new ToolWindowTemplateSelector();

            // --- Sample tool window
            ToolWindows.Add(new RegistersViewModel());
        }
    }
}