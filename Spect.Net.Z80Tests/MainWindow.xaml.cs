using System.Text;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.Z80Tests.Mvvm.Messages;
using Spect.Net.Z80Tests.Mvvm.Navigation;
using Spect.Net.Z80Tests.ViewModels.SpectrumEmu;

namespace Spect.Net.Z80Tests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly NavigationService _navigationService;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = AppViewModel.Default;
            if (ViewModelBase.IsInDesignModeStatic) return;

            // --- Bootstrap navigation
            _navigationService = (NavigationService)SimpleIoc.Default.GetInstance<INavigationService>();
            _navigationService.TargetElement = MainView;

            // --- Handle navigation messages
            Messenger.Default.Register<MenuActionInvokedMessage>(this, OnMenuActionInvoked);
            Messenger.Default.Register<NavigatedToViewModelMessage>(this, OnNavigatedToViewModel);
        }

        /// <summary>
        /// Handles the selected manu item
        /// </summary>
        /// <param name="msg"></param>
        private void OnMenuActionInvoked(MenuActionInvokedMessage msg)
        {
            if (msg.ImmediateAction != null)
            {
                // --- An immediate menu action with no UI
                var action = msg.ImmediateAction;
                action(msg.Tag);
            }
            else
            {
                // --- Navigate to the view model
                _navigationService.NavigateTo(msg.ViewModelType, msg.Tag);
            }
        }

        private void OnNavigatedToViewModel(NavigatedToViewModelMessage msg)
        {
            var mainViewElement = MainView.Content as UIElement;
            if (mainViewElement == null) return;

            if (mainViewElement.Focusable)
            {
                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
        }

        private void DisplayClicked(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo(typeof(SpectrumDebugViewModel));
        }
    }
}
