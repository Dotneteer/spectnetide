using System;
using System.Collections.Generic;

namespace Spect.Net.Z80Tests.Mvvm.Navigation
{
    /// <summary>
    /// This naigations service is to be used during design time
    /// </summary>
    public class DesignTimeNavigationService: INavigationService
    {
        private const string MESSAGE = 
            "This is a design time navigation service. "
            + "Register your own live navigation service for production use.";
        public void SetDefaultViewModelInstance(NavigableViewModelBase viewModel)
        {
            throw new NotImplementedException(MESSAGE);
        }

        public NavigableViewModelBase GetCurrentViewModel()
        {
            throw new NotImplementedException(MESSAGE);
        }

        public void NavigateTo(NavigableViewModelBase viewModel)
        {
            throw new NotImplementedException(MESSAGE);
        }

        public void NavigateTo(Type viewModelType, object tag = null)
        {
            throw new NotImplementedException(MESSAGE);
        }

        public IEnumerable<NavigableViewModelBase> GetActiveViewModels()
        {
            throw new NotImplementedException(MESSAGE);
        }

        public void CloseViewModel(NavigableViewModelBase viewModel)
        {
            throw new NotImplementedException(MESSAGE);
        }
    }
}