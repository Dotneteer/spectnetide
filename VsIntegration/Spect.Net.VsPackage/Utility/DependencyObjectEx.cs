using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// ReSharper disable TryCastAlwaysSucceeds

namespace Spect.Net.VsPackage.Utility
{
    public static class DependencyObjectEx
    {
        /// <summary>
        /// Gets the ScrollViewer of the specified dependency object
        /// </summary>
        /// <param name="element">Dependency object</param>
        /// <returns>ScrollViewer if found; otherwise, null</returns>
        public static ScrollViewer GetScrollViewer(this DependencyObject element)
        {
            if (element is ScrollViewer) return element as ScrollViewer;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);

                var result = GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the VirtualizingStackPanel of the specified dependency object
        /// </summary>
        /// <param name="element">Dependency object</param>
        /// <returns>ScrollViewer if found; otherwise, null</returns>
        public static VirtualizingStackPanel GetInnerStackPanel(this DependencyObject element)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                if (child == null) continue;

                if (child is VirtualizingStackPanel)
                {
                    return child as VirtualizingStackPanel;
                }
                var panel = GetInnerStackPanel(child);
                if (panel != null)
                {
                    return panel;
                }
            }
            return null;
        }
    }
}