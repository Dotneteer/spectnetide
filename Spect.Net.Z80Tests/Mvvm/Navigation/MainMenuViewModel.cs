using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Spect.Net.Z80Tests.Mvvm.Messages;

namespace Spect.Net.Z80Tests.Mvvm.Navigation
{
    /// <summary>
    /// This view model represents the main menu
    /// </summary>
    public class MainMenuViewModel : ViewModelBase
    {
        private bool _isMainMenuOpen;
        private MenuCategoryViewModel _selectedCategory;
        private bool _isCategoryMenuOpen;


        /// <summary>
        /// Indicates if the category menu is open or not
        /// </summary>
        public bool IsMainMenuOpen
        {
            get { return _isMainMenuOpen; }
            set { Set(ref _isMainMenuOpen, value); }
        }

        /// <summary>
        /// Gets or sets the selected menu category
        /// </summary>
        public MenuCategoryViewModel SelectedCategory
        {
            get { return _selectedCategory; }
            set { Set(ref _selectedCategory, value); }
        }

        /// <summary>
        /// Gets the menu categories associated with the main menu
        /// </summary>
        public List<MenuCategoryViewModel> MenuCategories { get; set; }

        /// <summary>
        /// Displays the menu items for the visible category
        /// </summary>
        public List<MenuItemViewModel> DisplayedCategoryItems { get; private set; }

        /// <summary>
        /// Displays the system menu items
        /// </summary>
        public List<MenuItemViewModel> SystemMenuItems { get; set; }

        /// <summary>
        /// Indicates if the menu pane is displayed or not
        /// </summary>
        public bool IsCategoryMenuOpen
        {
            get { return _isCategoryMenuOpen; }
            set { Set(ref _isCategoryMenuOpen, value); }
        }

        /// <summary>
        /// Command to toggle the category menu
        /// </summary>
        public RelayCommand ToggleMainMenu { get; private set; }

        /// <summary>
        /// This command is executed when a menu category is selected
        /// </summary>
        public RelayCommand<MenuCategoryViewModel> SelectCategory { get; private set; }

        /// <summary>
        /// This command is executed when a menu item is selected
        /// </summary>
        public RelayCommand<MenuItemViewModel> SelectMenuItem { get; private set; }

        public MainMenuViewModel()
        {
            IsMainMenuOpen = false;
            IsCategoryMenuOpen = false;
            MenuCategories = new List<MenuCategoryViewModel>();
            DisplayedCategoryItems = new List<MenuItemViewModel>();
            SystemMenuItems = new List<MenuItemViewModel>();
            ToggleMainMenu = new RelayCommand(OnMainMenuToggled);
            SelectCategory = new RelayCommand<MenuCategoryViewModel>(OnSelectCategory);
            SelectMenuItem = new RelayCommand<MenuItemViewModel>(ExecuteMenuItem);
            MessengerInstance.Register<MenuActionInvokedMessage>(this, OnMenuActionInvoked);
        }

        private void OnMainMenuToggled()
        {
            if (IsMainMenuOpen)
            {
                IsMainMenuOpen = false;
                IsCategoryMenuOpen = false;
            }
            else
            {
                IsMainMenuOpen = true;
                OnSelectCategory(SelectedCategory);
            }
        }

        /// <summary>
        /// Responds to the selection of a menu category
        /// </summary>
        /// <param name="selected">Selected menu category</param>
        private void OnSelectCategory(MenuCategoryViewModel selected)
        {
            SelectedCategory = selected;
            if (SelectedCategory == null) return;

            // --- If it is a single menu item, execute the command immediately
            DisplayedCategoryItems = SelectedCategory.MenuItems;
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged(nameof(DisplayedCategoryItems));
            if (SelectedCategory.ImmediateAction && SelectedCategory.MenuItems.Count > 0)
            {
                SelectedCategory.MenuItems[0].InvokeMenuAction.Execute(null);
            }
            else if (SelectedCategory.MenuItems.Count > 0)
            {
                // --- This category has menu items, show them
                IsMainMenuOpen = true;
                IsCategoryMenuOpen = true;
            }
        }

        /// <summary>
        /// Executes the specified menu item
        /// </summary>
        /// <param name="menuItem">Menu item to execute</param>
        private static void ExecuteMenuItem(MenuItemViewModel menuItem)
        {
            menuItem?.InvokeMenuAction.Execute(null);
        }

        private void OnMenuActionInvoked(MenuActionInvokedMessage msg)
        {
            IsMainMenuOpen = false;
            IsCategoryMenuOpen = false;
            SelectedCategory = null;
            DisplayedCategoryItems = new List<MenuItemViewModel>();
        }
    }
}