using System.Collections.Generic;

namespace Spect.Net.Z80Tests.Mvvm.Navigation
{
    /// <summary>
    /// This view model represents a menu category
    /// </summary>
    public class MenuCategoryViewModel : MenuItemViewModelBase
    {
        /// <summary>
        /// The items belonging to the specified category
        /// </summary>
        public List<MenuItemViewModel> MenuItems { get; set; }

        /// <summary>
        /// If this flag is true, the first menu item is actioned immediately
        /// </summary>
        public bool ImmediateAction { get; set; }

        public MenuCategoryViewModel(string resource, string title,
            IEnumerable<MenuItemViewModel> menuItems, bool immediateAction = false) : base(resource, title)
        {
            MenuItems = new List<MenuItemViewModel>(menuItems);
            ImmediateAction = immediateAction;
        }
    }
}