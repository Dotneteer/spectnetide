using GalaSoft.MvvmLight;

namespace Spect.Net.Z80Tests.Mvvm.Navigation
{
    /// <summary>
    /// Defines a common base class for all menu items
    /// </summary>
    public abstract class MenuItemViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Icon or other resource that defines a pictogram for the menu item
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// Menu item Title
        /// </summary>
        public string Title { get; set; }

        protected MenuItemViewModelBase()
        {
        }

        protected MenuItemViewModelBase(string resource, string title)
        {
            Resource = resource;
            Title = title;
        }
    }
}