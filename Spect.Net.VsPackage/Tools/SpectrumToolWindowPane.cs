using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Tools
{
    public abstract class SpectrumToolWindowPane<TPackage, TControl>: VsxToolWindowPane<TPackage, TControl>
        where TPackage : VsxPackage
        where TControl : ContentControl, new()
    {
        /// <summary>
        /// Prepares window frame events
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            Messenger.Default.Register<SolutionClosedMessage>(this, OnSolutionClosed);
        }

        /// <summary>
        /// Closes this window whenever the current solution closes
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void OnSolutionClosed(SolutionClosedMessage obj)
        {
            ClosePane();
        }

        /// <summary>
        /// Unregisters messages
        /// </summary>
        protected override void OnClose()
        {
            Messenger.Default.Unregister<SolutionClosedMessage>(this);
            base.OnClose();
        }
    }
}