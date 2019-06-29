using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class implements the Spectrum emulator tool window.
    /// </summary>
    [Guid("de41a21a-714d-495e-9b3f-830965f9332b")]
    [Caption("ZX Spectrum Emulator")]
    [ToolWindowToolbar(0x1010)]
    public class SpectrumEmulatorToolWindow : 
        ToolWindowPaneBase<SpectrumEmulatorToolWindowControl, SpectrumEmulatorToolWindowViewModel>
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
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Tool Window Commands

        /// <summary>
        /// Starts the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1081)]
        public class StartVmCommand : SpectNetCommandBase
        {
            protected override void OnExecute()
            {
                VsxDialogs.Show("START");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Stops the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1082)]
        public class StopVmCommand : SpectNetCommandBase
        {
            protected override void OnExecute()
            {
                VsxDialogs.Show("STOP");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Pauses the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1083)]
        public class PauseVmCommand : SpectNetCommandBase
        {
            protected override void OnExecute()
            {
                VsxDialogs.Show("PAUSE");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Resets the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1084)]
        public class ResetVmCommand : SpectNetCommandBase
        {
            protected override void OnExecute()
            {
                VsxDialogs.Show("RESET");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        [CommandId(0x1085)]
        public class StartDebugVmCommand : SpectNetCommandBase
        {
            protected override void OnExecute()
            {
                VsxDialogs.Show("DEBUG");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1086)]
        public class StepIntoCommand : SpectNetCommandBase
        {
            protected override void OnExecute()
            {
                VsxDialogs.Show("STEP INTO");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1087)]
        public class StepOverCommand : SpectNetCommandBase
        {
            protected override void OnExecute()
            {
                VsxDialogs.Show("STEP OVER");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Steps out from the current Z80 subroutine call in debug mode
        /// </summary>
        [CommandId(0x1091)]
        public class StepOutCommand : SpectNetCommandBase
        {
            protected override void OnExecute()
            {
                VsxDialogs.Show("STEP OUT");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Saves the state of the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1088)]
        public class SaveVmStateCommand : SpectNetCommandBase
        {
            protected override void OnExecute()
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
            /// <summary>
            /// This flags indicates that the command UI should be 
            /// updated when the command has been completed --
            /// with failure or success
            /// </summary>
            //public override bool UpdateUiWhenComplete => true;

            protected override void OnExecute()
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
            protected override void OnExecute()
            {
                VsxDialogs.Show("ADD VM STATE");
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        #endregion
    }
}