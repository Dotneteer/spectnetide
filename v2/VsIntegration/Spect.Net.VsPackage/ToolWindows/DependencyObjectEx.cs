using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// ReSharper disable TryCastAlwaysSucceeds

namespace Spect.Net.VsPackage.ToolWindows
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
        /// Gets the parent of the dependency object.
        /// </summary>
        /// <typeparam name="TParent">Parent element type</typeparam>
        /// <param name="element"></param>
        /// <returns>Parent object, if found; otherwise, null</returns>
        public static TParent GetParent<TParent>(this DependencyObject element)
            where TParent: DependencyObject
        {
            var parent = element;
            while (parent != null)
            {
                if (parent.GetType() == typeof(TParent)) return (TParent)parent;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(this DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                if (!(child is T))
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    // If the child's name is set for search
                    if (!(child is FrameworkElement frameworkElement) || frameworkElement.Name != childName) continue;
                    // if the child's name is of the request name

                    foundChild = (T)child;
                    break;
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}