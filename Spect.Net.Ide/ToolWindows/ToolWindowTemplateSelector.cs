using System.Windows;
using System.Windows.Controls;
using Spect.Net.Ide.ViewModels;

namespace Spect.Net.Ide.ToolWindows
{
    public class ToolWindowTemplateSelector: DataTemplateSelector
    {
        /// <summary>
        /// The template for the Registers tool window
        /// </summary>
        public DataTemplate RegistersToolWindowTemplate { get; set; }

        #region Overrides of DataTemplateSelector

        /// <summary>When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate" /> based on custom logic.</summary>
        /// <returns>Returns a <see cref="T:System.Windows.DataTemplate" /> or null. The default value is null.</returns>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //if (item is RegistersViewModel) return RegistersToolWindowTemplate;
            if (item is RegistersViewModel)
            {
                var dt = new DataTemplate();
                var ucFactory = new FrameworkElementFactory(typeof(RegistersToolWindow));
                dt.VisualTree = ucFactory;
                return dt;
            }
            return base.SelectTemplate(item, container);
        }

        #endregion
    }
}