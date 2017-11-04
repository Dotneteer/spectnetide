using System;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.WpfClient.Machine
{
    /// <summary>
    /// This class implements the controller that can be used in this client
    /// </summary>
    public class MachineController: SpectrumVmControllerBase
    {
        private TaskScheduler _context;

        public override void SaveMainContext()
        {
            _context = TaskScheduler.FromCurrentSynchronizationContext();
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
        public override Task ExecuteOnMainThread(Action action)
        {
            return Task.Factory.StartNew(() => { })
                .ContinueWith((t, o) => { action(); }, null, _context);
        }
    }
}