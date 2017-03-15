using System;
using System.Reflection;
using GalaSoft.MvvmLight.Command;
using Spect.Net.Ide.Mvvm.Attributes;
using Spect.Net.Ide.Mvvm.Messages;

namespace Spect.Net.Ide.Mvvm.Navigation
{
    /// <summary>
    /// Defines the view model of an executable menu item
    /// </summary>
    public class MenuItemViewModel : MenuItemViewModelBase
    {
        /// <summary>
        /// The view model type to navigate to when this menu item is activated
        /// </summary>
        public Type ViewModelType { get; set; }

        public Action<object> ActionMethod { get; }

        /// <summary>
        /// Optional tag belonging to the menu item
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Invokes the menu action
        /// </summary>
        public RelayCommand InvokeMenuAction { get; private set; }

        public MenuItemViewModel(Type viewModelType, string resource = null, string title = null, object tag = null)
        {
            ViewModelType = viewModelType;

            // --- Set field values from attributes
            var viewResourceAttr = viewModelType.GetTypeInfo().GetCustomAttribute<ViewResourceAttribute>();
            Resource = resource == null && viewResourceAttr != null
                ? viewResourceAttr.Resource : resource;

            var viewTitleAttr = viewModelType.GetTypeInfo().GetCustomAttribute<ViewTitleAttribute>();
            Title = title == null && viewTitleAttr != null
                ? viewTitleAttr.Title : title;

            Tag = tag;
            InvokeMenuAction = new RelayCommand(() =>
            {
                MessengerInstance.Send(new MenuActionInvokedMessage(ViewModelType, Tag));
            }
            );
        }

        public MenuItemViewModel(Action<object> action, string resource, string title, object tag = null) :
            base(resource, title)
        {
            ActionMethod = action;
            Tag = tag;
            InvokeMenuAction = new RelayCommand(() =>
            {
                MessengerInstance.Send(new MenuActionInvokedMessage(ActionMethod, Tag));
            }
            );
        }
    }
}