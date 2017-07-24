using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.WpfClient;

namespace Spect.Net.Wpf.Test.Client
{
    [TestClass]
    public class AppViewModelTests
    {
        [TestMethod]
        public void SingletonInstanceIsInitialized()
        {
            // --- Assert
            AppViewModel.Default.ShouldNotBeNull();
            AppViewModel.Default.SpectrumVmViewModel.ShouldNotBeNull();
        }

        [TestMethod]
        public void ResetReinitializesSingletonInstance()
        {
            // --- Arrange
            var before = AppViewModel.Default;

            // --- Act
            AppViewModel.Reset();

            // --- Assert
            AppViewModel.Default.ShouldNotBeSameAs(before);
            AppViewModel.Default.ShouldNotBeNull();
            AppViewModel.Default.SpectrumVmViewModel.ShouldNotBeNull();
        }
    }
}
