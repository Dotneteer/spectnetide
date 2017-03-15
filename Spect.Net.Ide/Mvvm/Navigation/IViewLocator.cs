using System;
using System.Windows;

namespace Spect.Net.Ide.Mvvm.Navigation
{
    /// <summary>
    /// This interface defines the functionality of a view locator
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Checks is the specified <paramref name="viewModelType"/> has
        /// an associated view.
        /// </summary>
        /// <param name="viewModelType">The type that represents the view model</param>
        /// <returns>The type of the view, if found; otherwise, null</returns>
        Type HasView(Type viewModelType);

        /// <summary>
        /// Creates an instance of the view specified with its <paramref name="viewModelType"/>.
        /// </summary>
        /// <param name="viewModelType">The type that represents the view model</param>
        /// <returns>The view model instance</returns>
        FrameworkElement CreateViewFor(Type viewModelType);
    }
}