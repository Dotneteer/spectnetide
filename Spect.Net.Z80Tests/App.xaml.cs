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
    }
}
