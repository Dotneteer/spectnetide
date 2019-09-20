using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using System;
using System.Runtime.InteropServices;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.ToolWindows.StackTool
{
    [Guid("C5B4C2DE-5F72-4030-ABB0-4F2EA86380B8")]
    [Caption("Z80 CPU Stack")]
    [ToolWindowToolbar(0x1510)]
    public class StackToolWindow : SpectrumToolWindowPane<StackToolWindowControl, StackToolWindowViewModel>
    {
        /// <summary>
        /// Invoke this method from the main thread to initialize toolbar commands.
        /// </summary>
        public static void InitializeToolbarCommands()
        {
            new StackPointerCommand();
            new StackViewCommand();
        }

        protected override StackToolWindowViewModel GetVmInstance()
        {
            return SpectNetPackage.Default.StackViewModel;
        }

        /// <summary>
        /// Displays the open TZX file dialog
        /// </summary>
        [CommandId(0x1581)]
        public class StackPointerCommand : SpectNetCommandBase
        {
            /// <summary>
            /// Override this method to define the status query action
            /// </summary>
            /// <param name="mc"></param>
            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var tw = SpectNetPackage.Default.GetToolWindow<StackToolWindow>();
                if (tw == null)
                {
                    return;
                }
                mc.Enabled = true;
                mc.Checked = !tw.Vm.StackContentViewVisible;
            }

            protected override Task ExecuteAsync()
            {
                var tw = SpectNetPackage.Default.GetToolWindow<StackToolWindow>();
                if (tw != null)
                {
                    tw.Vm.StackContentViewVisible = false;
                }
                return Task.FromResult(0);
            }
        }

        /// <summary>
        /// Displays the open TZX file dialog
        /// </summary>
        [CommandId(0x1582)]
        public class StackViewCommand : SpectNetCommandBase
        {
            /// <summary>
            /// Override this method to define the status query action
            /// </summary>
            /// <param name="mc"></param>
            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var tw = SpectNetPackage.Default.GetToolWindow<StackToolWindow>();
                if (tw == null)
                {
                    return;
                }
                mc.Enabled = true;
                mc.Checked = tw.Vm.StackContentViewVisible;
            }

            protected override Task ExecuteAsync()
            {
                var tw = SpectNetPackage.Default.GetToolWindow<StackToolWindow>();
                if (tw != null)
                {
                    tw.Vm.StackContentViewVisible = true;
                }
                return Task.FromResult(0);
            }
        }
    }
}
