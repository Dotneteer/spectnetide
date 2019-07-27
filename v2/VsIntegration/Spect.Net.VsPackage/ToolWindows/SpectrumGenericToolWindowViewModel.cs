using System;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.Wpf.Mvvm;

// ReSharper disable IdentifierTypo

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class is intended to be the base of all ZX Spectrum related tool window
    /// view models
    /// </summary>
    public class SpectrumGenericToolWindowViewModel: EnhancedViewModelBase, IDisposable
    {
        /// <summary>
        /// The aggregated ZX Spectrum view model
        /// </summary>
        public EmulatorViewModel EmulatorViewModel => SpectNetPackage.Default.EmulatorViewModel;

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public SpectrumGenericToolWindowViewModel()
        {
            if (IsInDesignMode) return;
            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();
        }

        /// <summary>
        /// Override this method to initialize this view model
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}