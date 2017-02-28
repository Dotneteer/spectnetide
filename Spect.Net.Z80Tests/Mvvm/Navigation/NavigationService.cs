using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Spect.Net.Z80Tests.Mvvm.Navigation
{
    /// <summary>
    /// This class implements the navigation service that allows to navigate to a different view
    /// </summary>
    public class NavigationService : NavigationServiceLogic
    {
        /// <summary>
        /// The target element to show the view in the content
        /// </summary>
        public ContentControl TargetElement { get; set; }

        /// <summary>
        /// The namespace to look up the view class in
        /// </summary>
        public List<string> ViewNamespaces { get; set; }

        /// <summary>
        /// The assembly to look up the view classes
        /// </summary>
        public Assembly ViewAssembly { get; set; }

        /// <summary>
        /// Overrid this method to carry out the real navigation step.
        /// </summary>
        /// <param name="viewModel">View model to navigate to</param>
        protected override void NavigateToView(NavigableViewModelBase viewModel)
        {
            try
            {
                // --- Create the view related to the view model
                var className = viewModel.GetType().Name;
                if (!className.EndsWith("ViewModel"))
                {
                    throw new ArgumentException("ViewModel class name should end with 'ViewModel' for CreateView to work");
                }

                className = className.Remove(className.Length - "Model".Length);
                Type viewType = null;
                foreach (var viewNs in ViewNamespaces)
                {
                    string fullName = $"{viewNs}.{className}";
                    viewType = ViewAssembly.GetType(fullName);
                    if (viewType != null) break;
                }
                if (viewType == null)
                {
                    throw new ArgumentException($"View type {className} cannot be found in any registered namespaces.");
                }

                var view = (FrameworkElement)(Activator.CreateInstance(viewType));
                view.DataContext = TargetElement.DataContext = viewModel;
                view.VerticalAlignment = VerticalAlignment.Stretch;
                view.HorizontalAlignment = HorizontalAlignment.Stretch;
                TargetElement.Content = view;
                TargetElement.Loaded += OnContentLoaded;
            }
            catch (Exception ex)
            {
                var message = new TextBlock
                {
                    Text = ex.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 24
                };
                TargetElement.Content = message;
            }
        }

        /// <summary>
        /// Handle the event when the view model's content has been loaded
        /// </summary>
        private void OnContentLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            TargetElement.Loaded -= OnContentLoaded;

            var viewModel = TargetElement.DataContext as NavigableViewModelBase;
            viewModel?.OnViewLoaded();
        }
    }
}