using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.StackTool
{
    [Guid("C5B4C2DE-5F72-4030-ABB0-4F2EA86380B8")]
    [Caption("Z80 CPU Stack")]
    [ToolWindowToolbar(typeof(SpectNetCommandSet), 0x1510)]
    public class StackToolWindow : SpectrumToolWindowPane<StackToolWindowControl, StackToolWindowViewModel>
    {
        /// <summary>
        /// Displays the open TZX file dialog
        /// </summary>
        [CommandId(0x1581)]
        public class StackPointerCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            /// <summary>
            /// Override this method to define the status query action
            /// </summary>
            /// <param name="mc"></param>
            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var tw = Package.GetToolWindow<StackToolWindow>();
                if (tw == null)
                {
                    return;
                }
                mc.Enabled = true;
                mc.Checked = !tw.Vm.StackContentViewVisible;
            }

            /// <summary>
            /// Override this method to execute the command
            /// </summary>
            protected override void OnExecute()
            {
                var tw = Package.GetToolWindow<StackToolWindow>();
                if (tw == null)
                {
                    return;
                }
                tw.Vm.StackContentViewVisible = false;
            }
        }

        /// <summary>
        /// Displays the open TZX file dialog
        /// </summary>
        [CommandId(0x1582)]
        public class StackViewCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            /// <summary>
            /// Override this method to define the status query action
            /// </summary>
            /// <param name="mc"></param>
            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var tw = Package.GetToolWindow<StackToolWindow>();
                if (tw == null)
                {
                    return;
                }
                mc.Enabled = true;
                mc.Checked = tw.Vm.StackContentViewVisible;
            }

            /// <summary>
            /// Override this method to execute the command
            /// </summary>
            protected override void OnExecute()
            {
                var tw = Package.GetToolWindow<StackToolWindow>();
                if (tw == null)
                {
                    return;
                }
                tw.Vm.StackContentViewVisible = true;
            }
        }
    }
}