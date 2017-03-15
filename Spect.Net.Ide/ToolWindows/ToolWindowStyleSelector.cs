using System.Windows;
using System.Windows.Controls;
using Spect.Net.Ide.ViewModels;

namespace Spect.Net.Ide.ToolWindows
{
    public class ToolWindowStyleSelector: StyleSelector
    {
        public Style ToolWindowStyle { get; set; }

        #region Overrides of StyleSelector

        /// <summary>When overridden in a derived class, returns a <see cref="T:System.Windows.Style" /> based on custom logic.</summary>
        /// <returns>Returns an application-specific style to apply; otherwise, null.</returns>
        /// <param name="item">The content.</param>
        /// <param name="container">The element to which the style will be applied.</param>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ToolWindowViewModel) return ToolWindowStyle;
            return base.SelectStyle(item, container);
        }

        #endregion
    }
}