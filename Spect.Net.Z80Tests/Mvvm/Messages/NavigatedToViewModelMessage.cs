using GalaSoft.MvvmLight.Messaging;
using Spect.Net.Z80Tests.Mvvm.Navigation;

namespace Spect.Net.Z80Tests.Mvvm.Messages
{
    /// <summary>
    /// This message is raised when the navigation service navigates to a 
    /// new view model
    /// </summary>
    public class NavigatedToViewModelMessage : MessageBase
    {
        /// <summary>
        /// The new view model
        /// </summary>
        public NavigableViewModelBase ViewModel { get; }

        /// <summary>
        /// Initializes the message
        /// </summary>
        /// <param name="viewModel">The new view model</param>
        public NavigatedToViewModelMessage(NavigableViewModelBase viewModel)
        {
            ViewModel = viewModel;
        }
    }
}