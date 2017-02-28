namespace Spect.Net.Z80Tests.Mvvm.Navigation
{
    /// <summary>
    /// View models can implement this interface to sign that they support
    /// multiple views.
    /// </summary>
    public interface ISupportsMultipleView
    {
        /// <summary>
        /// Returns the view model that contains the information for the current view to display.
        /// View resolution is carried out by this view model.
        /// </summary>
        /// <returns>
        /// The current view model to display with its corresponding view
        /// </returns>
        NavigableViewModelBase GetCurrentViewModel();

        /// <summary>
        /// This flag indicates whether the GetCurrentViewModel method can be invoked
        /// right after the view model has been instantiated
        /// </summary>
        bool CurrentViewModelAvailableAfterCtor { get; }
    }
}