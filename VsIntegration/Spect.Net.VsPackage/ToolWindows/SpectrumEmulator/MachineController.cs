using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx.Output;
using Spect.Net.VsPackage.Z80Programs;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class implements the controller that can be used in this client
    /// </summary>
    public class MachineController: SpectrumVmControllerBase
    {
        private readonly JoinableTaskCollection _joinableTaskCollection;

        public MachineController()
        {
            _joinableTaskCollection = 
                ThreadHelper.JoinableTaskContext.CreateCollection();
        }

        /// <summary>
        /// We provide that this machine is always controlled from
        /// the main thread.
        /// </summary>
        public override bool IsOnMainThread() => true;

        /// <summary>
        /// Override this method to provide the way that the controller can 
        /// switch back to the main (UI) thread.
        /// </summary>
        public override async Task ExecuteOnMainThread(Action action)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            action?.Invoke();
        }

        /// <summary>
        /// Override this method to handle the exception of the virtual machine
        /// </summary>
        /// <param name="exDuringRun">Exception that caused the vm to stop</param>
        protected override void OnVmStoppedWithException(Exception exDuringRun)
        {
            var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
            pane.WriteLine("The Spectrum virtual machine has stopped because of an exception.");
            pane.WriteLine("Exception details:");
            pane.WriteLine(exDuringRun);
        }

        /// <summary>
        /// Overrid this method to handle vm state changes within the controller
        /// </summary>
        /// <param name="oldState">Old VM state</param>
        /// <param name="newState">New VM state</param>
        protected override void OnVmStateChanged(VmState oldState, VmState newState)
        {
            var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
            pane.WriteLine($"Machine state changed: {oldState} --> {newState}.");
        }

        /// <summary>
        /// Does the lion's share of disposing the asycn command objects carefully
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;

            try
            {
                // --- Block Dispose until all async work has completed.
                ThreadHelper.JoinableTaskFactory.Run(_joinableTaskCollection.JoinTillEmptyAsync);
            }
            catch (OperationCanceledException)
            {
                // --- This exception is expected because we signaled the cancellation token
            }
            catch (AggregateException ex)
            {
                // --- ignore AggregateException containing only OperationCanceledExceptionI
                ex.Handle(inner => inner is OperationCanceledException);
            }
        }
    }
}