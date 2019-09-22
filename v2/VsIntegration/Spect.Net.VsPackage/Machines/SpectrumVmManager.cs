using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Output;
using System.Threading.Tasks;
using System.Windows;

namespace Spect.Net.VsPackage.Machines
{
    /// <summary>
    /// This class provides helper methods to manage the 
    /// </summary>
    public static class SpectrumVmManager
    {
        /// <summary>
        /// Stops the Spectrum VM, displays confirmation, if required
        /// </summary>
        /// <returns>Tru, if start confirmed; otherwise, false</returns>
        public static async Task<bool> StopSpectrumVmAsync(bool needConfirm)
        {
            var vm = SpectNetPackage.Default.EmulatorViewModel;
            var machineState = vm.MachineState;
            if (machineState == VmState.Running || machineState == VmState.Paused)
            {
                if (needConfirm)
                {
                    var answer = VsxDialogs.Show("Are you sure, you want to restart " +
                                                 "the ZX Spectrum virtual machine?",
                        "The ZX Spectrum virtual machine is running",
                        MessageBoxButton.YesNo, VsxMessageBoxIcon.Question, 1);
                    if (answer == VsxDialogResult.No)
                    {
                        return false;
                    }
                }

                // --- Stop the machine and allow 50ms to stop.
                await vm.Machine.Stop();
                if (vm.MachineState == VmState.Stopped) return true;

                const string MESSAGE = "The ZX Spectrum virtual machine did not stop.";
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                await pane.WriteLineAsync(MESSAGE);
                VsxDialogs.Show(MESSAGE, "Unexpected issue",
                    MessageBoxButton.OK, VsxMessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}
