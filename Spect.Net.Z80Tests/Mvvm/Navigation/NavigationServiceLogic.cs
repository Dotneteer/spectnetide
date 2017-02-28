using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.Z80Tests.Mvvm.Messages;

namespace Spect.Net.Z80Tests.Mvvm.Navigation
{
    /// <summary>
    /// This class implements the navigation service that allows to navigate to a different view
    /// </summary>
    public class NavigationServiceLogic : INavigationService
    {
        private NavigableViewModelBase _defaultViewModel;
        private Stack<NavigableViewModelBase> _navigationStack = new Stack<NavigableViewModelBase>();

        /// <summary>
        /// Sets the view model instance to navigate to when there's no other
        /// view model to navigate to.
        /// </summary>
        /// <param name="viewModel">Default view model</param>
        public void SetDefaultViewModelInstance(NavigableViewModelBase viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }
            _defaultViewModel = viewModel;
        }

        /// <summary>
        /// Gets the current view model that is displayed on the UI
        /// </summary>
        /// <returns>The current view model</returns>
        public NavigableViewModelBase GetCurrentViewModel()
        {
            return _navigationStack.Count > 0 
                ? _navigationStack.Peek() 
                : null;
        }

        /// <summary>
        /// Navigates to the specified view model instance
        /// </summary>
        /// <param name="viewModel">View model to navigate to</param>
        public void NavigateTo(NavigableViewModelBase viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            // --- Check the stack for the specified view model
            var reorganized = GetReorganizedStack(viewModel);
            if (reorganized.Count < _navigationStack.Count)
            {
                // --- "reorganized" contains the original stack with "viewModel" removed.
                _navigationStack = reorganized;
            }

            // --- This will be the new viewModel
            DoNavigation(viewModel);
        }

        /// <summary>
        /// Navigates to a new instance of the specified view model type
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="tag">Optional tag</param>
        public void NavigateTo(Type viewModelType, object tag = null)
        {
            if (viewModelType == null)
            {
                throw new ArgumentNullException(nameof(viewModelType));
            }

            if (!viewModelType.GetTypeInfo().IsSubclassOf(typeof (NavigableViewModelBase)))
            {
                throw new ArgumentException($@"The argument must be a subtype of {nameof(NavigableViewModelBase)}", 
                    nameof(viewModelType));
            }

            // --- Check for singleton
            NavigableViewModelBase viewModel;
            var reorganized = GetReorganizedStack(viewModelType, out viewModel);

            if (viewModel != null && viewModel.IsSingleton)
            {
                _navigationStack = reorganized;
            }
            else
            {
                // --- Create the new view model instance
                viewModel = (NavigableViewModelBase)Activator.CreateInstance(viewModelType);

                // --- Pass input data
                viewModel.SetInputTag(tag);

                // --- Manage async init
                if (viewModel.AsyncInit)
                {
                    viewModel.StartAsyncInit();
                }
                else
                {
                    // --- If no async init, sign initialization completion
                    viewModel.InitState = InitState.Initialized;
                    viewModel.OnInitViewModelCompleted();
                }
            }
            DoNavigation(viewModel);
        }

        /// <summary>
        /// Returns the active view models (in navigation stack order)
        /// </summary>
        /// <returns>Active view models</returns>
        public IEnumerable<NavigableViewModelBase> GetActiveViewModels()
        {
            return _navigationStack;
        }

        /// <summary>
        /// Closes the specified view model, and removes it from the navigation stack
        /// </summary>
        /// <param name="viewModel">View model to close</param>
        /// <remarks>After navigation navigates to the current view</remarks>
        public void CloseViewModel(NavigableViewModelBase viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var reorganized = GetReorganizedStack(viewModel);
            if (reorganized.Count < _navigationStack.Count)
            {
                // --- "reorganized" contains the original stack with "viewModel" removed.
                _navigationStack = reorganized;
            }

            viewModel.OnViewModelClosed();

            // --- Go back to the previous or default view model
            DoNavigation(GetCurrentViewModel() ?? _defaultViewModel, true);
        }

        /// <summary>
        /// Check if the current view can be navigated away from, and
        /// then do the navigation
        /// </summary>
        /// <param name="viewModel">View model to navigate to</param>
        /// <param name="previousClosed">Signes that the previous view model has been closed</param>
        protected void DoNavigation(NavigableViewModelBase viewModel, bool previousClosed = false)
        {
            // --- Notify the current view that it has been left (navigated away)
            if (!previousClosed)
            {
                GetCurrentViewModel()?.OnViewModelLeft();

                // --- Check if this view allows history tracking
                // --- We never put the default view model to the navigation stack
                if (!viewModel.ExcludeFromNavigationHistory && viewModel != _defaultViewModel)
                {
                    _navigationStack.Push(viewModel);
                }
            }

            // --- Now, it is time to create the view that represents the view model
            // ReSharper disable once SuspiciousTypeConversion.Global
            var multipleViewSupport = viewModel as ISupportsMultipleView;
            if (multipleViewSupport == null)
            {
                // --- This view model supports only a single view
                viewModel.OnViewModelNavigatedTo();
                NavigateToView(viewModel);
                Messenger.Default.Send(new NavigatedToViewModelMessage(viewModel));
            }
            else
            {
                // --- This view model supports several views, let's navigate to the current 
                // --- one with this action
                Action completion = () =>
                {
                    var childViewModel = multipleViewSupport.GetCurrentViewModel();
                    if (childViewModel != null)
                    {
                        childViewModel.OnViewModelNavigatedTo();
                        NavigateToView(childViewModel);
                    }
                    Messenger.Default.Send(new NavigatedToViewModelMessage(viewModel));
                };

                if (multipleViewSupport.CurrentViewModelAvailableAfterCtor)
                {
                    // --- When the view model is instantiated, its initial view model is
                    // --- immediately available
                    completion();
                }
                else
                {
                    // --- We need to wait for the completion of view model initialization
                    // --- to be able to get the current view
                    if (viewModel.InitTask == null)
                    {
                        // --- No async init started
                        completion();
                    }
                    else
                    {
                        // --- Async init already started
                        viewModel.InitTask.GetAwaiter().OnCompleted(() =>
                        {
                            completion();
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Override this method to carry out the real navigation step.
        /// </summary>
        /// <param name="viewModel">View model to create the view for</param>
        protected virtual void NavigateToView(NavigableViewModelBase viewModel) { }

        /// <summary>
        /// Creates a reorganized stack with the specified view model instance removed
        /// </summary>
        /// <param name="viewModel">View model instance to search for</param>
        /// <returns>The reorganized stack</returns>
        private Stack<NavigableViewModelBase> GetReorganizedStack(NavigableViewModelBase viewModel)
        {
            var reorganized = new Stack<NavigableViewModelBase>();
            foreach (var vm in _navigationStack.Reverse().Where(vm => vm != viewModel))
            {
                reorganized.Push(vm);
            }
            return reorganized;
        }

        /// <summary>
        /// Creates a reorganized stack with the specified view model type removed
        /// </summary>
        /// <param name="viewModelType">View model type to search for</param>
        /// <param name="instance">View model instance found in the stack</param>
        /// <returns>The reorganized stack</returns>
        private Stack<NavigableViewModelBase> GetReorganizedStack(Type viewModelType, out NavigableViewModelBase instance)
        {
            instance = null;
            var reorganized = new Stack<NavigableViewModelBase>();
            foreach (var vm in _navigationStack.Reverse())
            {
                if (vm.GetType() == viewModelType)
                {
                    instance = vm;
                }
                else
                {
                    reorganized.Push(vm);
                }
            }
            return reorganized;
        }
    }
}