using GalaSoft.MvvmLight.Messaging;
using Spect.Net.Z80Tests.Views;

namespace Spect.Net.Z80Tests
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            AppViewModel.Reset();
            AppViewModel.Init();
        }

        private void Application_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            Messenger.Default.Send(new AppClosesMessage());
        }
    }
}
