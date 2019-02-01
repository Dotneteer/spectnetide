namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class represents the view model of the Spectrum Emulator tool window
    /// </summary>
    public class SpectrumEmulatorToolWindowViewModel : SpectrumGenericToolWindowViewModel
    {
        /// <summary>
        /// Set the machine status when the screen has been refreshed
        /// </summary>
        protected override void OnScreenRefreshed()
        {
            base.OnScreenRefreshed();
            if (ScreenRefreshCount % 20 == 0)
            {
                MachineViewModel?.SpectrumVm?.DebugInfoProvider.PrepareBreakpoints();
            }
        }
    }
}