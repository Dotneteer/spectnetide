using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.Vsx;
using Task = System.Threading.Tasks.Task;

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
        public TestExplorerToolWindow()
        {
            Package.TestFileChanged += OnTestFileChanged;
        }

        protected override void Dispose(bool disposing)
        {
            Package.TestFileChanged -= OnTestFileChanged;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Respond to changes in any test file's contents
        /// </summary>
        private void OnTestFileChanged(object sender, FileChangedEventArgs e)
        {
            Caption = BaseCaption + "*";
            Vm.HasAnyTestFileChanged = true;
            SpectNetPackage.UpdateCommandUi();
        }

        /// <summary>
        /// Runs all unit tests
        /// </summary>
        [CommandId(0x1980)]
        public class RunAllTestsCommand :
            VsxAsyncCommand<SpectNetPackage, SpectNetCommandSet>
        {
            /// <summary>
            /// This tool window instance
            /// </summary>
            public TestExplorerToolWindow ToolWindowInstance => Package.GetToolWindow<TestExplorerToolWindow>();

            /// <summary>
            /// Override this method to define the async command body te execute on the
            /// background thread
            /// </summary>
            protected override Task ExecuteAsync()
            {
                var tw = ToolWindowInstance;
                tw.Vm.CompileAllTestFiles();

                // --- Indicate successful compilation in the caption
                if (!tw.Vm.CompiledWithError)
                {
                    tw.Caption = tw.BaseCaption;
                }

                // --- Set the preset expand/collapse state
                if (tw.Vm.AutoExpandAfterCompile)
                {
                    tw.Content.ExpandAll();
                }
                else if (tw.Vm.AutoExpandAfterCompile)
                {
                    tw.Content.CollapseAll();
                }

                // --- Sign that all test nodes are in "Not Run" state
                Package.TestManager.ResetTestNodes(tw.Vm);
                return Task.FromResult(0);
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Runs selected unit tests
        /// </summary>
        [CommandId(0x1981)]
        public class RunTestCommand :
            VsxAsyncCommand<SpectNetPackage, SpectNetCommandSet>
        {
            /// <summary>
            /// This tool window instance
            /// </summary>
            public TestExplorerToolWindow ToolWindowInstance => Package.GetToolWindow<TestExplorerToolWindow>();

            /// <summary>
            /// Override this method to define the async command body te execute on the
            /// background thread
            /// </summary>
            protected override Task ExecuteAsync()
            {
                return Task.FromResult(0);
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = !ToolWindowInstance.Vm.HasAnyTestFileChanged
                                && !ToolWindowInstance.Vm.CompiledWithError
                                && ToolWindowInstance.Vm.TestTreeItems.Count > 0;
        }

        /// <summary>
        /// Debug selected unit tests
        /// </summary>
        [CommandId(0x1982)]
        public class DebugTestCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            /// <summary>
            /// This tool window instance
            /// </summary>
            public TestExplorerToolWindow ToolWindowInstance => Package.GetToolWindow<TestExplorerToolWindow>();
            protected override void OnExecute()
            {
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = !ToolWindowInstance.Vm.HasAnyTestFileChanged
                                && !ToolWindowInstance.Vm.CompiledWithError
                                && ToolWindowInstance.Vm.TestTreeItems.Count > 0;
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