using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spect.Net.TestParser.Compiler;
using Spect.Net.TestParser.Plan;
using Spect.Net.VsPackage.ToolWindows.TestExplorer;
using Spect.Net.VsPackage.Vsx.Output;
using ErrorTask = Microsoft.VisualStudio.Shell.ErrorTask;
using TaskCategory = Microsoft.VisualStudio.Shell.TaskCategory;
using TaskErrorCategory = Microsoft.VisualStudio.Shell.TaskErrorCategory;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Spect.Net.VsPackage.Z80Programs
{
    /// <summary>
    /// This class is responsible for managing Z80 unit test files
    /// </summary>
    public class Z80TestManager
    {
        /// <summary>
        /// The package that host the project
        /// </summary>
        public SpectNetPackage Package => SpectNetPackage.Default;

        /// <summary>
        /// Signs that compilation is in progress
        /// </summary>
        public bool CompilatioInProgress { get; set; }

        /// <summary>
        /// Compiles the file with the specified file name
        /// </summary>
        /// <param name="filename">Test file to compile</param>
        /// <param name="createLog">Signs if build log should be created</param>
        /// <returns>Test plan</returns>
        public TestFilePlan CompileFile(string filename, bool createLog = true)
        {
            var start = DateTime.Now;
            var pane = OutputWindow.GetPane<Z80BuildOutputPane>();
            if (createLog)
            {
                pane.WriteLine("Z80 Test Compiler");
                pane.WriteLine($"Compiling {filename}");
            }
            var compiler = new Z80TestCompiler
            {
                DefaultSourceFolder = Path.GetDirectoryName(filename)
            };
            if (createLog)
            {
                var duration = (DateTime.Now - start).TotalMilliseconds;
                pane.WriteLine($"Compile time: {duration}ms");

            }
            return compiler.CompileFile(filename);
        }

        /// <summary>
        /// Compiles the code.
        /// </summary>
        /// <returns>True, if compilation successful; otherwise, false</returns>
        /// <param name="createLog">Signs if build log should be created</param>
        public TestProjectPlan CompileAllFiles(bool createLog = true)
        {
            Package.ErrorList.Clear();
            var result = new TestProjectPlan();
            var testFiles = Package.CodeDiscoverySolution.CurrentProject.Z80TestProjectItems;
            if (testFiles.Count == 0) return result;

            var testManager = Package.TestManager;
            var start = DateTime.Now;
            var pane = OutputWindow.GetPane<Z80BuildOutputPane>();
            if (createLog)
            {
                pane.WriteLine("Z80 Test Compiler");
            }
            foreach (var file in testFiles)
            {
                var filename = file.Filename;
                if (createLog)
                {
                    pane.WriteLine($"Compiling {filename}");
                }
                var testPlan = testManager.CompileFile(filename);
                result.Add(testPlan);
            }

            if (createLog)
            {
                var duration = (DateTime.Now - start).TotalMilliseconds;
                pane.WriteLine($"Compile time: {duration}ms");
            }
            return result;
        }

        /// <summary>
        /// Collect test compilation errors
        /// </summary>
        public void DisplayTestCompilationErrors(TestProjectPlan projectPlan)
        {
            Package.ErrorList.Clear();
            var errorFound = false;
            foreach (var plan in projectPlan.TestFilePlans)
            {
                foreach (var error in plan.Errors)
                {
                    errorFound = true;
                    var errorTask = new ErrorTask
                    {
                        Category = TaskCategory.User,
                        ErrorCategory = TaskErrorCategory.Error,
                        HierarchyItem = Package.CodeManager.CurrentHierarchy,
                        Document = error.Filename ?? plan.Filename,
                        Line = error.Line,
                        Column = error.Column,
                        Text = error.ErrorCode == null
                            ? error.Message
                            : $"{error.ErrorCode}: {error.Message}",
                        CanDelete = true
                    };
                    errorTask.Navigate += ErrorTaskOnNavigate;
                    Package.ErrorList.AddErrorTask(errorTask);
                }
            }

            if (errorFound)
            {
                Package.ApplicationObject.ExecuteCommand("View.ErrorList");
            }
        }

        /// <summary>
        /// Executes all tests that start with the specified node
        /// </summary>
        /// <param name="node">Root node of the subtree to run the tests for</param>
        /// <param name="token">Token to stop tests</param>
        public Task RunTestsFromNode(TestItemBase node, CancellationToken token)
        {
            TestRootItem rootToRun = null;
            switch (node)
            {
                case TestRootItem rootNode:
                    // --- Prepare all file nodes to run
                    rootNode.TestFilesToRun.Clear();
                    foreach (var child in rootNode.ChildItems)
                    {
                        if (!(child is TestFileItem fileItem)) continue;
                        rootNode.TestFilesToRun.Add(fileItem);
                        fileItem.CollectAllToRun();
                    }
                    rootToRun = rootNode;
                    break;
                case TestSetItem setNode:
                {
                    // --- Prepare this test set to run
                    setNode.TestsToRun.Clear();
                    setNode.CollectAllToRun();
                    var fileItem = setNode.Parent as TestFileItem;
                    var root = rootToRun = fileItem?.Parent as TestRootItem;
                    root?.TestFilesToRun.Add(fileItem);
                    fileItem?.TestSetsToRun.Add(setNode);
                    break;
                }
                case TestItem testNode:
                {
                    // --- Prepare this test to run
                    testNode.TestCasesToRun.Clear();
                    testNode.CollectAllToRun();
                    var setItem = testNode.Parent as TestSetItem;
                    var fileItem = setItem?.Parent as TestFileItem;
                    var root = rootToRun = fileItem?.Parent as TestRootItem;
                    root?.TestFilesToRun.Add(fileItem);
                    fileItem?.TestSetsToRun.Add(setItem);
                    setItem?.TestsToRun.Add(testNode);
                    break;
                }
                case TestCaseItem caseNode:
                {
                    // --- Prepare this test case to run
                    var testItem = caseNode.Parent as TestItem;
                    var setItem = testItem?.Parent as TestSetItem;
                    var fileItem = setItem?.Parent as TestFileItem;
                    var root = rootToRun = fileItem?.Parent as TestRootItem;
                    root?.TestFilesToRun.Add(fileItem);
                    fileItem?.TestSetsToRun.Add(setItem);
                    setItem?.TestsToRun.Add(testItem);
                    testItem?.TestCasesToRun.Add(caseNode);
                    break;
                }
            }

            return rootToRun != null 
                ? ExecuteTestTree(rootToRun, token) 
                : Task.FromResult(0);
        }

        /// <summary>
        /// Execute all test held by the specified root node
        /// </summary>
        /// <param name="rootToRun">Root node instance</param>
        /// <param name="token">Token to cancel tests</param>
        private async Task ExecuteTestTree(TestRootItem rootToRun, CancellationToken token)
        {
            rootToRun.State = TestState.Running;
            try
            {
                foreach (var fileToRun in rootToRun.TestFilesToRun)
                {
                    fileToRun.State = TestState.Running;
                    SetTestRootState(rootToRun);
                    await ExecuteFileTests(fileToRun, token);
                    SetTestRootState(rootToRun);
                }
            }
            catch (Exception)
            {
                // --- Intentionally ignored
            }
            finally
            {
                // --- Mark inconclusive nodes
                rootToRun.TestFilesToRun.ForEach(i =>
                {
                    if (i.State == TestState.NotRun) SetSubTreeState(i, TestState.Inconclusive);
                });
                SetTestRootState(rootToRun);
            }
        }

        /// <summary>
        /// Set the state of the test root node according to its files' state
        /// </summary>
        /// <param name="rootToRun">Root node</param>
        private static void SetTestRootState(TestRootItem rootToRun)
        {
            if (rootToRun.TestFilesToRun.Any(i => i.State == TestState.Aborted || i.State == TestState.Inconclusive))
            {
                rootToRun.State = TestState.Inconclusive;
            }
            else if (rootToRun.TestFilesToRun.Any(i => i.State == TestState.Running))
            {
                rootToRun.State = TestState.Running;
            }
            else
            {
                rootToRun.State = rootToRun.TestFilesToRun.Any(i => i.State == TestState.Failed)
                    ? TestState.Failed
                    : TestState.Success;
            }
        }

        /// <summary>
        /// Execute the tests within the specified test file
        /// </summary>
        /// <param name="fileToRun">Test file to run</param>
        /// <param name="token">Token to cancel tests</param>
        private async Task ExecuteFileTests(TestFileItem fileToRun, CancellationToken token)
        {
            try
            {
                foreach (var setToRun in fileToRun.TestSetsToRun)
                {
                    setToRun.State = TestState.Running;
                    SetTestFileState(fileToRun);
                    await ExecuteSetTests(setToRun, token);
                    SetTestFileState(fileToRun);
                }
            }
            catch (TaskCanceledException)
            {
                fileToRun.State = TestState.Aborted;
                throw;
            }
            catch (Exception)
            {
                fileToRun.State = TestState.Aborted;
                throw;
            }
            finally
            {
                // --- Mark inconclusive nodes
                fileToRun.TestSetsToRun.ForEach(i =>
                {
                    if (i.State == TestState.NotRun) SetSubTreeState(i, TestState.Inconclusive);
                });
                SetTestFileState(fileToRun);
            }
        }

        /// <summary>
        /// Set the state of the test file node according to its test sets' state
        /// </summary>
        /// <param name="fileToRun">Test file node</param>
        private static void SetTestFileState(TestFileItem fileToRun)
        {
            if (fileToRun.TestSetsToRun.Any(i => i.State == TestState.Aborted || i.State == TestState.Inconclusive))
            {
                fileToRun.State = TestState.Inconclusive;
            }
            else if (fileToRun.TestSetsToRun.Any(i => i.State == TestState.Running))
            {
                fileToRun.State = TestState.Running;
            }
            else
            {
                fileToRun.State = fileToRun.TestSetsToRun.Any(i => i.State == TestState.Failed)
                    ? TestState.Failed
                    : TestState.Success;
            }
        }

        /// <summary>
        /// Execute the tests within the specified test set
        /// </summary>
        /// <param name="setToRun">Test set to run</param>
        /// <param name="token">Token to cancel tests</param>
        /// <returns>True, if test ran; false, if aborted</returns>
        private async Task ExecuteSetTests(TestSetItem setToRun, CancellationToken token)
        {
            try
            {
                // TODO: Start Spectrum VM
                // TODO: Inject source code
                // TODO: Execute init setting
                // TODO: Execute setup code

                foreach (var testToRun in setToRun.TestsToRun)
                {
                    testToRun.State = TestState.Running;
                    SetTestSetState(setToRun);
                    await ExecuteTests(testToRun, token);
                    SetTestSetState(setToRun);
                }

                // TODO: Execute cleanup code
            }
            catch (TaskCanceledException)
            {
                setToRun.State = TestState.Aborted;
                throw;
            }
            catch (Exception)
            {
                setToRun.State = TestState.Aborted;
                throw;
            }
            finally
            {
                // --- Mark inconclusive tests
                setToRun.TestsToRun.ForEach(i =>
                {
                    if (i.State == TestState.NotRun) SetSubTreeState(i, TestState.Inconclusive);
                });
                SetTestSetState(setToRun);
            }
        }

        /// <summary>
        /// Set the state of the test set node according to its tests' state
        /// </summary>
        /// <param name="setToRun">Test set node</param>
        private static void SetTestSetState(TestSetItem setToRun)
        {
            if (setToRun.TestsToRun.Any(i => i.State == TestState.Aborted || i.State == TestState.Inconclusive))
            {
                setToRun.State = TestState.Inconclusive;
            }
            else if (setToRun.TestsToRun.Any(i => i.State == TestState.Running))
            {
                setToRun.State = TestState.Running;
            }
            else
            {
                setToRun.State = setToRun.TestsToRun.Any(i => i.State == TestState.Failed)
                    ? TestState.Failed
                    : TestState.Success;
            }
        }

        /// <summary>
        /// Executes the test within a test set
        /// </summary>
        /// <param name="testToRun">The test to run</param>
        /// <param name="token">Token to cancel tests</param>
        /// <returns>True, if test ran; false, if aborted</returns>
        private async Task ExecuteTests(TestItem testToRun, CancellationToken token)
        {
            try
            {
                if (testToRun.TestCasesToRun.Count == 0)
                {
                    // --- No test cases
                    // TODO: Execute arrange
                    // TODO: Execute act
                    await Task.Delay(3000, token);
                    // TODO: Execute assert
                    testToRun.State = TestState.Success;
                }
                else
                {
                    foreach (var caseToRun in testToRun.TestCasesToRun)
                    {
                        caseToRun.State = TestState.Running;
                        await ExecuteCase(caseToRun, token);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                testToRun.State = TestState.Aborted;
                throw;
            }
            catch (Exception)
            {
                testToRun.State = TestState.Aborted;
                throw;
            }
            finally
            {
                // --- Mark inconclusive tests
                testToRun.TestCasesToRun.ForEach(i =>
                {
                    if (i.State == TestState.NotRun) SetSubTreeState(i, TestState.Inconclusive);
                });
                SetTestState(testToRun);
            }
        }

        private async Task ExecuteCase(TestCaseItem caseToRun, CancellationToken token)
        {
            try
            {
                // TODO: Execute arrange
                // TODO: Execute act
                await Task.Delay(3000, token);
                // TODO: Execute assert
                caseToRun.State = TestState.Success;
            }
            catch (TaskCanceledException)
            {
                caseToRun.State = TestState.Aborted;
                throw;
            }
            catch (Exception)
            {
                caseToRun.State = TestState.Aborted;
                throw;
            }
        }

        /// <summary>
        /// Set the state of the test set node according to its tests' state
        /// </summary>
        /// <param name="testToRun"></param>
        private static void SetTestState(TestItem testToRun)
        {
            if (testToRun.TestCasesToRun.Count == 0) return;
            if (testToRun.TestCasesToRun.Any(i => i.State == TestState.Aborted || i.State == TestState.Inconclusive))
            {
                testToRun.State = TestState.Inconclusive;
            }
            else if (testToRun.TestCasesToRun.Any(i => i.State == TestState.Running))
            {
                testToRun.State = TestState.Running;
            }
            else
            {
                testToRun.State = testToRun.TestCasesToRun.Any(i => i.State == TestState.Failed)
                    ? TestState.Failed
                    : TestState.Success;
            }
        }

        /// <summary>
        /// Set the state of the specified sub tree
        /// </summary>
        /// <param name="node">Subtree root node</param>
        /// <param name="state">State to set</param>
        /// <param name="except">Optional node to ignore</param>
        public void SetSubTreeState(TestItemBase node, TestState state, TestItemBase except = null)
        {
            if (node == except) return;
            node.State = state;
            foreach (var child in node.ChildItems)
            {
                SetSubTreeState(child, state, except);
            }
        }

        /// <summary>
        /// Navigate to the sender task.
        /// </summary>
        private void ErrorTaskOnNavigate(object sender, EventArgs eventArgs)
        {
            if (sender is ErrorTask task)
            {
                Package.ErrorList.Navigate(task);
            }
        }
    }
}