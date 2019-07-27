using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.ToolWindows.Keyboard;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class implements the Spectrum emulator tool window.
    /// </summary>
    [Guid("de41a21a-714d-495e-9b3f-830965f9332b")]
    [Caption("ZX Spectrum Emulator")]
    [ToolWindowToolbar(0x1010)]
    public class SpectrumEmulatorToolWindow :
        SpectrumToolWindowPane<SpectrumEmulatorToolWindowControl, SpectrumEmulatorToolWindowViewModel>
    {
        /// <summary>
        /// Invoke this method from the main thread to initialize toolbar commands.
        /// </summary>
        public static void InitializeToolbarCommands()
        {
            // ReSharper disable ObjectCreationAsStatement
            new StartVmCommand();
            new StopVmCommand();
            new PauseVmCommand();
            new ResetVmCommand();
            new StartDebugVmCommand();
            new StepIntoCommand();
            new StepOverCommand();
            new StepOutCommand();
            new SaveVmStateCommand();
            new LoadVmStateCommand();
            new AddVmStateCommand();
            new ShowKeyboardCommand();
            new ToggleShadowScreenCommand();
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Tool Window Commands

        /// <summary>
        /// This class is intended to be the base class of all ZX Spectrum emulator-
        /// specific commands
        /// </summary>
        public abstract class SpectrumVmCommand : SpectNetCommandBase
        {
            /// <summary>
            /// Gets the current ZX Spectrum machine
            /// </summary>
            public SpectrumMachine Machine => SpectNetPackage.Default.EmulatorViewModel.Machine;

            /// <summary>
            /// Gets the current ZX Spectrum machine state
            /// </summary>
            public VmState MachineState => SpectNetPackage.Default.EmulatorViewModel.MachineState;
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1081)]
        public class StartVmCommand : SpectrumVmCommand
        {
            protected override void ExecuteOnMainThread()
            {
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                Machine.Start();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState != VmState.Running;
        }

        /// <summary>
        /// Stops the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1082)]
        public class StopVmCommand : SpectrumVmCommand
        {
            /// <summary>
            /// Override this method to define the async command body te execute on the
            /// background thread
            /// </summary>
            protected override async Task ExecuteAsync()
            {
                await Machine.Stop();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                mc.Enabled = MachineState == VmState.Running 
                    || MachineState == VmState.Paused;
            }
        }

        /// <summary>
        /// Pauses the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1083)]
        public class PauseVmCommand : SpectrumVmCommand
        {
            /// <summary>
            /// Override this method to define the async command body te execute on the
            /// background thread
            /// </summary>
            protected override async Task ExecuteAsync()
            {
                await Machine.Pause();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState == VmState.Running;
        }

        /// <summary>
        /// Resets the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1084)]
        public class ResetVmCommand : SpectrumVmCommand
        {
            /// <summary>
            /// Override this method to define the async command body te execute on the
            /// background thread
            /// </summary>
            protected override async Task ExecuteAsync()
            {
                await Machine.Stop();
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                Machine.Start();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState == VmState.Running;
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        [CommandId(0x1085)]
        public class StartDebugVmCommand : SpectrumVmCommand
        {
            protected override void ExecuteOnMainThread()
            {
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                Machine.StartDebug();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState != VmState.Running;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1086)]
        public class StepIntoCommand : SpectrumVmCommand
        {
            protected override async Task ExecuteAsync()
            {
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                await Machine.StepInto();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState == VmState.Paused;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1087)]
        public class StepOverCommand : SpectrumVmCommand
        {
            protected override async Task ExecuteAsync()
            {
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                await Machine.StepOver();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState == VmState.Paused;
        }

        /// <summary>
        /// Steps out from the current Z80 subroutine call in debug mode
        /// </summary>
        [CommandId(0x1091)]
        public class StepOutCommand : SpectrumVmCommand
        {
            protected override async Task ExecuteAsync()
            {
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                await Machine.StepOut();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState == VmState.Paused;
        }

        /// <summary>
        /// Saves the state of the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1088)]
        public class SaveVmStateCommand : SpectNetCommandBase
        {
            protected override void ExecuteOnMainThread()
            {
                VsxDialogs.Show("SAVE STATE");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Loads the state of the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1089)]
        public class LoadVmStateCommand : SpectNetCommandBase
        {
            public override bool UpdateUiWhenComplete => true;

            protected override void ExecuteOnMainThread()
            {
                VsxDialogs.Show("LOAD VM STATE");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Loads the state of the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1090)]
        public class AddVmStateCommand : SpectNetCommandBase
        {
            protected override void ExecuteOnMainThread()
            {
                VsxDialogs.Show("ADD VM STATE");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Displays the ZX Spectrum Emulator Keyboard
        /// </summary>
        [CommandId(0x1092)]
        [ToolWindow(typeof(KeyboardToolWindow))]
        public class ShowKeyboardCommand : ShowToolWindowCommandBase
        {
            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Enables/Disables the shadow screen
        /// </summary>
        [CommandId(0x1093)]
        public class ToggleShadowScreenCommand : SpectNetCommandBase
        {
            protected EmulatorViewModel Evm => SpectNetPackage.Default.EmulatorViewModel;
            protected override void ExecuteOnMainThread()
            {
                Evm.ShadowScreenEnabled = !Evm.ShadowScreenEnabled;
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Checked = Evm.ShadowScreenEnabled;
        }

        #endregion
    }
}