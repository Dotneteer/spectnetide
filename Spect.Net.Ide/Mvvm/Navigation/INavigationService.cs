using System;
using System.Collections.Generic;

namespace Spect.Net.Ide.Mvvm.Navigation
{
    /// <summary>
    /// This interface defines the navigation service that allows to navigate to a different view
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Sets the view model instance to navigate to when there's no other
        /// view model to navigate to.
        /// </summary>
        /// <param name="viewModel">Default view model</param>
        void SetDefaultViewModelInstance(NavigableViewModelBase viewModel);

        /// <summary>
        /// Gets the current view model that is displayed on the UI
        /// </summary>
        /// <returns>The current view model</returns>
        NavigableViewModelBase GetCurrentViewModel();

        /// <summary>
        /// Navigates to the specified view model instance
        /// </summary>
        /// <param name="viewModel">View model to navigate to</param>
        void NavigateTo(NavigableViewModelBase viewModel);

        /// <summary>
        /// Navigates to a new instance of the specified view model type
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="tag">Optional tag</param>
        void NavigateTo(Type viewModelType, object tag = null);

        /// <summary>
        /// Returns the active view models (in navigation stack order)
        /// </summary>
        /// <returns>Active view models</returns>
        IEnumerable<NavigableViewModelBase> GetActiveViewModels();

        /// <summary>
        /// Closes the specified view model, and removes it from the navigation stack
        /// </summary>
        /// <param name="viewModel">View model to close</param>
        void CloseViewModel(NavigableViewModelBase viewModel);
    }
}