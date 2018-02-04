using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// This class implements the Z80 Registers tool window.
    /// </summary>
    [Guid("D6E4C776-5EFB-48CE-A491-A00A56D89BCA")]
    [Caption("Z80 Unit Test Explorer")]
    [ToolWindowToolbar(typeof(SpectNetCommandSet), 0x1910)]
    public class TestExplorerToolWindow :
        VsxToolWindowPane<TestExplorerToolWindowControl, TestExplorerToolWindowViewModel>
    {
        /// <summary>
        /// Runs all unit tests
        /// </summary>
        [CommandId(0x1980)]
        public class RunAllTestsCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
                var tw = Package.GetToolWindow<TestExplorerToolWindow>();
                tw.Vm.CompileAllCommand.Execute(null);
                if (tw.Vm.AutoExpandAfterCompile)
                {
                    tw.Content.ExpandAll();
                }
                else if (tw.Vm.AutoExpandAfterCompile)
                {
                    tw.Content.CollapseAll();
                }
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Runs selected unit tests
        /// </summary>
        [CommandId(0x1981)]
        public class RunTestCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Debug selected unit tests
        /// </summary>
        [CommandId(0x1982)]
        public class DebugTestCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Stops testing
        /// </summary>
        [CommandId(0x1983)]
        public class StopTestingCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Runs all unit tests
        /// </summary>
        [CommandId(0x1984)]
        public class ExpandAllCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
                var tw = Package.GetToolWindow<TestExplorerToolWindow>();
                tw.Content.ExpandAll();
                tw.Vm.AutoExpandAfterCompile = true;
                tw.Vm.AutoCollapseAfterCompile = false;
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Runs all unit tests
        /// </summary>
        [CommandId(0x1985)]
        public class CollapseAllCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
                var tw = Package.GetToolWindow<TestExplorerToolWindow>();
                tw.Content.CollapseAll();
                tw.Vm.AutoCollapseAfterCompile = true;
                tw.Vm.AutoExpandAfterCompile = false;
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }
    }
}