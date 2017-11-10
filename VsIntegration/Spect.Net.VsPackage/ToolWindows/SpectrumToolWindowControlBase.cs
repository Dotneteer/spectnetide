using System;
using System.Windows.Controls;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Mvvm.Messages;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class is intended to be the base user control for all Spectrum-related
    /// tool windows controls.
    /// </summary>
    public abstract class SpectrumToolWindowControlBase: UserControl
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Controls.UserControl" /> class.</summary>
        protected SpectrumToolWindowControlBase()
        {
            Loaded += (s, e) =>
            {
                Messenger.Default.Register<VmStateChangedMessage>(this, OnInternalVmStateChanged);
            };
            Unloaded += (s, e) =>
            {
                Messenger.Default.Unregister<VmStateChangedMessage>(this);
            };
        }

        /// <summary>
        /// Dispatch the vm state changed message on the UI thread
        /// </summary>
        /// <param name="msg">Message received</param>
        private void OnInternalVmStateChanged(VmStateChangedMessage msg)
        {
            DispatchOnUiThread(() => OnVmStateChanged(msg.OldState, msg.NewState));
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