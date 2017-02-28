using System;
using System.Collections.Generic;
using System.Reflection;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Spect.Net.Z80Tests.Mvvm.Navigation;
using Spect.Net.Z80Tests.ViewModels;
using Spect.Net.Z80Tests.Views;
using INavigationService = Spect.Net.Z80Tests.Mvvm.Navigation.INavigationService;

namespace Spect.Net.Z80Tests
{
    /// <summary>
    /// This class represents the view model of the application's frame. It has two main
    /// purposes:
    /// - We use it in MainPage.xamle
    /// - It has a singleton instance, AppViewMode.Default, to access the app state
    ///   from anywhere within the app
    /// </summary>
    public class AppViewModel : ViewModelBase
    {
        /// <summary>
        /// The singleton instance of this class
        /// </summary>
        public static AppViewModel Default { get; private set; } = new AppViewModel();

        /// <summary>
        /// Don't instantiate this from code. Use AppViewModel.Default instead. 
        /// </summary>
        protected AppViewModel()
        {
        }

        /// <summary>
        /// Resets this object to its initial state
        /// </summary>
        public static void Reset()
        {
            // --- Create the default instance
            Default = new AppViewModel();
        }

        public static void Init()
        {
            // --- Register services we use while this client runs
            RegisterApplicationServices();
        }

        /// <summary>
        /// Set the namespace for view definitions
        /// </summary>
        /// <param name="namespaceType"></param>
        public static void SetViewNamespace(Type namespaceType)
        {
        }

        /// <summary>
        /// Reference to main navigation view model
        /// </summary>
        public MainMenuViewModel MainMenu { get; set; }

        /// <summary>
        /// Registers services that communicate with the WebApi backend
        /// </summary>
        private static void RegisterApplicationServices()
        {
            var ioc = SimpleIoc.Default;
            var navigationService = new NavigationService
            {
                ViewNamespaces = new List<string>
                {
                    typeof(DashboardView).Namespace
                },
                ViewAssembly = typeof(DashboardView).GetTypeInfo().Assembly
            };
            navigationService.SetDefaultViewModelInstance(new DashboardViewModel());
            ioc.Register<INavigationService>(() => navigationService);
        }
    }
}