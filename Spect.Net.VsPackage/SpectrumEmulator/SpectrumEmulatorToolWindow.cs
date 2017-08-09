using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.Wpf.SpectrumControl;

// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.VsPackage.SpectrumEmulator
{
    /// <summary>
    /// This class implements the Spectrum emulator tool window.
    /// </summary>
    [Guid("de41a21a-714d-495e-9b3f-830965f9332b")]
    [Caption("ZX Spectrum Emulator")]
    [ToolWindowToolbar(typeof(SpectNetCommandSet), 0x1010)]
    public class SpectrumEmulatorToolWindow : VsxToolWindowPane<SpectNetPackage, SpectrumEmulatorToolWindowControl>
    {
        /// <summary>Called when the active IVsWindowFrame changes.</summary>
        /// <param name="oldFrame">The old active frame.</param>
        /// <param name="newFrame">The new active frame.</param>
        public override void OnActiveFrameChanged(IVsWindowFrame oldFrame, IVsWindowFrame newFrame)
        {
            Content.SpectrumControl.AllowKeyboardScan = newFrame == Frame;
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1081)]
        public class StartVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute() 
                => Package.SpectrumVmViewModel.StartVmCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc) 
                => mc.Enabled = Package.SpectrumVmViewModel.VmState != SpectrumVmState.Running;
        }

        /// <summary>
        /// Stops the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1082)]
        public class StopVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute() => 
                Package.SpectrumVmViewModel.StopVmCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var state = Package.SpectrumVmViewModel.VmState;
                mc.Enabled = state == SpectrumVmState.Running
                             || state == SpectrumVmState.Paused;
            }
        }

        /// <summary>
        /// Pauses the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1083)]
        public class PauseVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.SpectrumVmViewModel.PauseVmCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.SpectrumVmViewModel.VmState == SpectrumVmState.Running;
        }

        /// <summary>
        /// Resets the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1084)]
        public class ResetVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.SpectrumVmViewModel.ResetVmCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.SpectrumVmViewModel.VmState == SpectrumVmState.Running;
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        [CommandId(0x1085)]
        public class StartDebugVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.SpectrumVmViewModel.StartDebugVmCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.SpectrumVmViewModel.VmState != SpectrumVmState.Running;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1086)]
        public class StepIntoCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.SpectrumVmViewModel.StepIntoCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.SpectrumVmViewModel.VmState == SpectrumVmState.Paused;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1087)]
        public class StepOverCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.SpectrumVmViewModel.StepOverCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.SpectrumVmViewModel.VmState == SpectrumVmState.Paused;
        }
    }
}
