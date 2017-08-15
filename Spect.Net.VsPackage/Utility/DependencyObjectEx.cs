using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        /// <summary>
        /// This method handles the list view scrolling keys for the specified dependency object.
        /// </summary>
        /// <param name="element">Dependency object</param>
        /// <param name="e">Key event arguments</param>
        /// <remarks>
        /// Handled keys: Up, Down, (Ctrl+)PageUp, (Ctrl+)PageDown, Home, End
        /// </remarks>
        public static void HandleListViewKeyEvents(this DependencyObject element, KeyEventArgs e)
        {
            var stack = element.GetInnerStackPanel();
            if (stack == null) return;

            var sw = element.GetScrollViewer();
            if (sw == null) return;

            var ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            var multiplier = ctrlPressed ? 10 : 1;
            switch (e.Key)
            {
                case Key.Up:
                    sw.ScrollToVerticalOffset(stack.VerticalOffset - 1.0);
                    e.Handled = true;
                    break;

                case Key.Down:
                    sw.ScrollToVerticalOffset(stack.VerticalOffset + 1.0);
                    e.Handled = true;
                    break;

                case Key.PageUp:
                    sw.ScrollToVerticalOffset(stack.VerticalOffset - stack.ViewportHeight * multiplier);
                    e.Handled = true;
                    break;

                case Key.PageDown:
                    sw.ScrollToVerticalOffset(stack.VerticalOffset + stack.ViewportHeight * multiplier);
                    e.Handled = true;
                    break;

                case Key.Home:
                    if (ctrlPressed)
                    {
                        sw.ScrollToTop();
                        e.Handled = true;
                    }
                    break;

                case Key.End:
                    if (ctrlPressed)
                    {
                        sw.ScrollToBottom();
                        e.Handled = true;
                    }
                    break;
            }
        }
    }
}