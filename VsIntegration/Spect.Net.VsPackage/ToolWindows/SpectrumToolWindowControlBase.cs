using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class is intended to be the base user control for all Spectrum-related
    /// tool windows controls.
    /// </summary>
    public abstract class SpectrumToolWindowControlBase: UserControl
    {
        /// <summary>
        /// Signs whether the control is loaded
        /// </summary>
        public bool IsControlLoaded { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Controls.UserControl" /> class.</summary>
        protected SpectrumToolWindowControlBase()
        {
            IsControlLoaded = false;
            Loaded += (s, e) =>
            {
                SpectNetPackage.Default.MachineViewModel.VmStateChanged += OnInternalVmStateChanged;
                IsControlLoaded = true;
            };
            Unloaded += (s, e) =>
            {
                IsControlLoaded = false;
                SpectNetPackage.Default.MachineViewModel.VmStateChanged -= OnInternalVmStateChanged;
            };
        }

        /// <summary>
        /// Dispatch the vm state changed message on the UI thread
        /// </summary>
        private void OnInternalVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            DispatchOnUiThread(() => OnVmStateChanged(args.OldState, args.NewState));
        }

        /// <summary>
        /// Override this message to respond to vm state changes events
        /// </summary>
        protected virtual void OnVmStateChanged(VmState oldState, VmState newState)
        {
        }

        /// <summary>
        /// This method dispatches the specified action asynchronously on the UI thread with the provided
        /// priority
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="priority">Dispatch priority. By default, it is the highest (Send)</param>
        /// <remarks>
        /// This implementation does not handle the result of the async dispatch
        /// </remarks>
        protected void DispatchOnUiThread(Action action, DispatcherPriority priority = DispatcherPriority.Send)
        {
            Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    action();
                }
                catch
                {
                    // --- We intentionally ignore this exception
                }
            }, priority);
        }
    }
}