using System;
using System.Windows.Controls;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
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
                Messenger.Default.Register<VmStateChangedMessage>(this, OnVmStateChanged);
                Messenger.Default.Register<ScreenRefreshedMessage>(this, OnVmScreenRefreshed);
            };
            Unloaded += (s, e) =>
            {
                Messenger.Default.Unregister<VmStateChangedMessage>(this);
                Messenger.Default.Unregister<ScreenRefreshedMessage>(this);
            };
        }

        /// <summary>
        /// Override this message so that the control can handle virtual machine state changes
        /// </summary>
        /// <param name="msg">Message received</param>
        protected virtual void OnVmStateChanged(VmStateChangedMessage msg)
        {
        }

        /// <summary>
        /// Override this message to respond to screen refresh events
        /// </summary>
        /// <param name="msg">Message received</param>
        protected virtual void OnVmScreenRefreshed(ScreenRefreshedMessage msg)
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
            Dispatcher.InvokeAsync(action, priority);
        }
    }
}