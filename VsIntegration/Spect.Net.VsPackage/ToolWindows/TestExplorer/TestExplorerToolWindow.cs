using System;
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
            Vm.SelectedItemChanged += OnSelectedItemChanged;
        }

        protected override void Dispose(bool disposing)
        {
            Package.TestFileChanged -= OnTestFileChanged;
            Vm.SelectedItemChanged -= OnSelectedItemChanged;
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
        /// Updates the command UI whenever the selected item has changed.
        /// </summary>
        private void OnSelectedItemChanged(object sender, EventArgs e)
        {
            SpectNetPackage.UpdateCommandUi();
        }

        /// <summary>
        /// Compiles unit tests
        /// </summary>
        /// <returns>True, if compilation is successful</returns>
        private static bool CompileTests()
        {
            var tw = SpectNetPackage.Default.GetToolWindow<TestExplorerToolWindow>();
            var vm = tw.Vm;
            vm.CompileAllTestFiles();

            // --- Indicate successful compilation in the caption
            if (!vm.CompiledWithError)
            {
                tw.Caption = tw.BaseCaption;
            }

            // --- Set the preset expand/collapse state
            if (vm.AutoExpandAfterCompile)
            {
                tw.Content.ExpandAll();
            }
            else if (vm.AutoCollapseAfterCompile)
            {
                tw.Content.CollapseAll();
            }

            return !vm.CompiledWithError;
        }

        /// <summary>
        /// Runs all unit tests
        /// </summary>
        [CommandId(0x1986)]
        public class CompileAllTestsCommand :
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
                CompileTests();
                return Task.FromResult(0);
            }

            /// <summary>
            /// Override this method to define the action to execute on the main
            /// thread of Visual Studio -- finally
            /// </summary>
            protected override async Task FinallyOnMainThread()
            {
                await Package.MachineViewModel.Stop();
                SpectNetPackage.UpdateCommandUi();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                if (ToolWindowInstance?.Vm == null) return;
                mc.Enabled = !ToolWindowInstance.Vm.IsTestInProgress;
            }
        }

        /// <summary>
        /// This class is the base class of commands that can run test commands
        /// </summary>
        public abstract class RunTestsCommandBase :
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
            protected override async Task ExecuteAsync()
            {
                var tw = ToolWindowInstance;
                var vm = tw.Vm;
                if (!CompileTests()) return;

                // --- Run the test
                vm.IsTestInProgress = true;
                //await SwitchToMainThreadAsync();
                //SpectNetPackage.UpdateCommandUi();
                //await SwitchToBackgroundThreadAsync();
                try
                {
                    var stopped = await Package.CodeManager.StopSpectrumVm(
                        Package.Options.ConfirmTestMachineRestart);
                    if (!stopped) return;

                    Package.TestManager.SetSubTreeState(vm.TestRoot, TestState.NotRun);
                    await Package.TestManager.RunTestsFromNode(vm, GetTestRootToRun(), vm.GetNewCancellationToken());
                }
                finally
                {
                    vm.IsTestInProgress = false;
                }
            }

            /// <summary>
            /// Stop the virtual machine and update the UI
            /// </summary>
            protected override async Task FinallyOnMainThread()
            {
                await Package.MachineViewModel.Stop();
                SpectNetPackage.UpdateCommandUi();
            }

            /// <summary>
            /// Override this method to obtain the root node of tests to run
            /// </summary>
            /// <returns></returns>
            protected abstract TestItemBase GetTestRootToRun();
        }

        /// <summary>
        /// Runs all unit tests
        /// </summary>
        [CommandId(0x1980)]
        public class RunAllTestsCommand : RunTestsCommandBase
        {
            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                if (ToolWindowInstance?.Vm == null) return;
                mc.Enabled = !ToolWindowInstance.Vm.IsTestInProgress;
            }

            /// <summary>
            /// Override this method to obtain the root node of tests to run
            /// </summary>
            /// <returns>The root of the test tree</returns>
            protected override TestItemBase GetTestRootToRun() 
                => ToolWindowInstance.Vm.TestRoot;
        }

        /// <summary>
        /// Runs selected unit tests
        /// </summary>
        [CommandId(0x1981)]
        public class RunTestCommand : RunTestsCommandBase
        {
            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var vm = ToolWindowInstance?.Vm;
                if (vm == null) return;
                mc.Enabled = !vm.HasAnyTestFileChanged
                             && !vm.CompiledWithError
                             && vm.SelectedItem != null
                             && !vm.IsTestInProgress;
            }

            /// <summary>
            /// Override this method to obtain the root node of tests to run
            /// </summary>
            /// <returns></returns>
            protected override TestItemBase GetTestRootToRun() 
                => ToolWindowInstance.Vm.SelectedItem;
        }

        /// <summary>
        /// Debug selected unit tests
        /// </summary>
        [CommandId(0x1982)]
        public class DebugTestCommand :
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
            {
                mc.Enabled = false;
                //var vm = ToolWindowInstance?.Vm;
                //if (vm == null) return;
                //mc.Enabled = !vm.HasAnyTestFileChanged
                //             && !vm.CompiledWithError
                //             && vm.SelectedItem != null
                //             && !vm.IsTestInProgress;
            }
        }

        /// <summary>
        /// Stops testing
        /// </summary>
        [CommandId(0x1983)]
        public class StopTestingCommand :
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
            protected override async Task ExecuteAsync()
            {
                ToolWindowInstance.Vm.CancellationSource.Cancel();
                await Package.MachineViewModel.Machine.Stop();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                if (ToolWindowInstance?.Vm == null) return;
                mc.Enabled = ToolWindowInstance.Vm.IsTestInProgress;
            }
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