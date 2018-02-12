using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.SpectrumEmu.Machine;
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
        /// The call stub is created at this address
        /// </summary>
        public const ushort CALL_STUB_ADDRESS = 0x5BA0;

        /// <summary>
        /// The package that host the project
        /// </summary>
        public SpectNetPackage Package => SpectNetPackage.Default;

        /// <summary>
        /// Signs that compilation is in progress
        /// </summary>
        public bool CompilatioInProgress { get; set; }

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
                    if (token.IsCancellationRequested) break;
                    setToRun.State = TestState.Running;
                    SetTestFileState(fileToRun);
                    await ExecuteSetTests(setToRun, token);
                    SetTestFileState(fileToRun);
                }
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
        /// Execute the tests within the specified test set
        /// </summary>
        /// <param name="setToRun">Test set to run</param>
        /// <param name="token">Token to cancel tests</param>
        /// <returns>True, if test ran; false, if aborted</returns>
        private async Task ExecuteSetTests(TestSetItem setToRun, CancellationToken token)
        {
            try
            {
                // --- Set the startup state of the Spectrum VM
                var machineSet = await Package.StateFileManager.SetProjectMachineStartupState();
                if (!machineSet)
                {
                    setToRun.State = TestState.Aborted;
                    return;
                }

                // --- Inject the source code into the vm
                var plan = setToRun.Plan;
                Package.CodeManager.InjectCodeIntoVm(plan.CodeOutput);

                // --- Set up registers with default values
                ExecuteAssignment(plan.InitAssignments);

                // --- Execute setup code
                var success = await InvokeCode(plan.Setup, plan.TimeoutValue, token);
                if (!success)
                {
                    setToRun.State = TestState.Aborted;
                    return;
                }

                foreach (var testToRun in setToRun.TestsToRun)
                {
                    testToRun.State = TestState.Running;
                    SetTestSetState(setToRun);
                    await ExecuteTests(testToRun, token);
                    SetTestSetState(setToRun);
                }

                // --- Execute cleanup code
                success = await InvokeCode(plan.Cleanup, plan.TimeoutValue, token);
                if (!success)
                {
                    setToRun.State = TestState.Aborted;
                    return;
                }

                // --- Stop the Spectrum VM
                var stopped = await Package.CodeManager.StopSpectrumVm(false);
                if (!stopped)
                {
                    setToRun.State = TestState.Aborted;
                }
            }
            catch (TaskCanceledException)
            {
                setToRun.State = TestState.Aborted;
            }
            catch (Exception)
            {
                setToRun.State = TestState.Aborted;
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

                    // --- Execute the test code
                    await InvokeCode(testToRun.Plan.Act,
                        testToRun.Plan.TimeoutValue ?? testToRun.Plan.TestSet.TimeoutValue, token);
                    await Task.Delay(300, token);
                    // TODO: Execute assert
                    testToRun.State = TestState.Success;
                }
                else
                {
                    foreach (var caseToRun in testToRun.TestCasesToRun)
                    {
                        caseToRun.State = TestState.Running;
                        await ExecuteCase(testToRun, caseToRun, token);
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

        private async Task ExecuteCase(TestItem testToRun, TestCaseItem caseToRun, CancellationToken token)
        {
            try
            {
                // TODO: Execute arrange

                // --- Execute the test code
                await InvokeCode(testToRun.Plan.Act,
                    testToRun.Plan.TimeoutValue ?? testToRun.Plan.TestSet.TimeoutValue, token);
                await Task.Delay(300, token);
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
        /// Executes the assignments
        /// </summary>
        /// <param name="asgns"></param>
        private void ExecuteAssignment(IReadOnlyCollection<AssignmentPlanBase> asgns)
        {
            if (asgns == null) return;
            var regs = Package.MachineViewModel.SpectrumVm.Cpu.Registers;
            foreach (var asgn in asgns)
            {
                switch (asgn)
                {
                    case RegisterAssignmentPlan regAsgn:
                        switch (regAsgn.RegisterName.ToUpper())
                        {
                            case "A": regs.A = (byte) regAsgn.Value; break;
                            case "B": regs.B = (byte)regAsgn.Value; break;
                            case "C": regs.C = (byte)regAsgn.Value; break;
                            case "D": regs.D = (byte)regAsgn.Value; break;
                            case "E": regs.E = (byte)regAsgn.Value; break;
                            case "H": regs.H = (byte)regAsgn.Value; break;
                            case "L": regs.L = (byte)regAsgn.Value; break;
                            case "XL":
                            case "IXL": regs.XL = (byte) regAsgn.Value; break;
                            case "XH":
                            case "IXH": regs.XH = (byte)regAsgn.Value; break;
                            case "YL":
                            case "IYL": regs.YL = (byte)regAsgn.Value; break;
                            case "YH":
                            case "IYH": regs.YH = (byte)regAsgn.Value; break;
                            case "I": regs.I = (byte)regAsgn.Value; break;
                            case "R": regs.R = (byte)regAsgn.Value; break;
                            case "BC": regs.BC = (byte)regAsgn.Value; break;
                            case "DE": regs.DE = (byte)regAsgn.Value; break;
                            case "HL": regs.HL = (byte)regAsgn.Value; break;
                            case "SP": regs.SP = (byte)regAsgn.Value; break;
                            case "IX": regs.IX = (byte)regAsgn.Value; break;
                            case "IY": regs.IY = (byte)regAsgn.Value; break;
                            case "AF'": regs._AF_ = (byte)regAsgn.Value; break;
                            case "BC'": regs._BC_ = (byte)regAsgn.Value; break;
                            case "DE'": regs._DE_ = (byte)regAsgn.Value; break;
                            case "HL'": regs._HL_ = (byte)regAsgn.Value; break;
                        }
                        break;

                    case FlagAssignmentPlan flagAsgn:
                        switch (flagAsgn.FlagName.ToUpper())
                        {
                            case "NZ": ResetFlag(FlagsResetMask.Z); break;
                            case "Z": SetFlag(FlagsSetMask.Z); break;
                            case "NC": ResetFlag(FlagsResetMask.C); break;
                            case "C": SetFlag(FlagsSetMask.C); break;
                            case "PE": ResetFlag(FlagsResetMask.PV); break;
                            case "PO": SetFlag(FlagsSetMask.PV); break;
                            case "P": ResetFlag(FlagsResetMask.S); break;
                            case "M": SetFlag(FlagsSetMask.S); break;
                            case "NH": ResetFlag(FlagsResetMask.H); break;
                            case "H": SetFlag(FlagsSetMask.H); break;
                            case "A": ResetFlag(FlagsResetMask.N); break;
                            case "N": SetFlag(FlagsSetMask.N); break;
                            case "N3": ResetFlag(FlagsResetMask.R3); break;
                            case "3": SetFlag(FlagsSetMask.R3); break;
                            case "N5": ResetFlag(FlagsResetMask.R5); break;
                            case "5": SetFlag(FlagsSetMask.R5); break;
                        }
                        break;

                    case MemoryAssignmentPlan memAsgn:
                        var runSupport = Package.MachineViewModel.SpectrumVm as ISpectrumVmRunCodeSupport;
                        runSupport?.InjectCodeToMemory(memAsgn.Address, memAsgn.Value);
                        break;
                }
            }

            void SetFlag(byte mask)
            {
                regs.F |= mask;
            }

            void ResetFlag(byte mask)
            {
                regs.F &= mask;
            }
        }

        /// <summary>
        /// Invokes the code and waits for its completion within the specified
        /// timeout limits.
        /// </summary>
        /// <param name="invokePlan">Invokation plan</param>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <param name="token">Token to cancel test run</param>
        /// <returns>True, if code completed; otherwise, false</returns>
        private async Task<bool> InvokeCode(InvokePlanBase invokePlan, int timeout, CancellationToken token)
        {
            if (invokePlan == null) return true;
            if (!(Package.MachineViewModel.SpectrumVm is ISpectrumVmRunCodeSupport runCodeSupport)) return false;

            ExecuteCycleOptions runOptions;
            if (invokePlan is CallPlan callPlan)
            {
                // --- Create CALL stub in #5BA0
                Package.MachineViewModel.SpectrumVm.Cpu.Registers.PC = CALL_STUB_ADDRESS;
                runCodeSupport.InjectCodeToMemory(CALL_STUB_ADDRESS, new byte[] { 0xCD, (byte)callPlan.Address, (byte)(callPlan.Address >> 8) });
                runOptions = new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint, 
                    terminationPoint: CALL_STUB_ADDRESS + 3, 
                    hiddenMode: true,
                    timeoutMs: timeout);
            }
            else if (invokePlan is StartPlan startPlan)
            {
                Package.MachineViewModel.SpectrumVm.Cpu.Registers.PC = startPlan.Address;
                if (startPlan.StopAddress == null)
                {
                    // --- Start and run until halt
                    runOptions = new ExecuteCycleOptions(EmulationMode.UntilHalt, hiddenMode: true, timeoutMs: timeout);
                }
                else
                {
                    // --- Start and run until the stop address is reached
                    runOptions = new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                        terminationPoint: startPlan.StopAddress.Value,
                        hiddenMode: true,
                        timeoutMs: timeout);
                }
            }
            else
            {
                return false;
            }

            // --- Start the code and wait while it stops
            var controller = Package.MachineViewModel.MachineController;
            controller.StartVm(runOptions);
            await controller.StarterTask;
            while (controller.CompletionTask == null)
            {
                await Task.Delay(1, token);
            }
            await controller.CompletionTask;
            var success = !controller.CompletionTask.IsFaulted && !controller.CompletionTask.IsCanceled;
            controller.PauseVm();
            await controller.CompletionTask;
            return success;
        }

        #region Helper methods

        private static void SetTestItemState(TestItemBase root, IEnumerable<TestItemBase> children)
        {
            if (root.State == TestState.Aborted) return;
            var childList = children.ToList();
            if (childList.Any(i => i.State == TestState.Aborted || i.State == TestState.Inconclusive))
            {
                root.State = TestState.Inconclusive;
            }
            else if (childList.Any(i => i.State == TestState.Running))
            {
                root.State = TestState.Running;
            }
            else
            {
                root.State = childList.Any(i => i.State == TestState.Failed)
                    ? TestState.Failed
                    : TestState.Success;
            }
        }

        /// <summary>
        /// Set the state of the test root node according to its files' state
        /// </summary>
        /// <param name="rootToRun">Root node</param>
        private static void SetTestRootState(TestRootItem rootToRun)
        {
            SetTestItemState(rootToRun, rootToRun.TestFilesToRun);
        }

        /// <summary>
        /// Set the state of the test file node according to its test sets' state
        /// </summary>
        /// <param name="fileToRun">Test file node</param>
        private static void SetTestFileState(TestFileItem fileToRun)
        {
            SetTestItemState(fileToRun, fileToRun.TestSetsToRun);
        }

        /// <summary>
        /// Set the state of the test set node according to its tests' state
        /// </summary>
        /// <param name="setToRun">Test set node</param>
        private static void SetTestSetState(TestSetItem setToRun)
        {
            SetTestItemState(setToRun, setToRun.ChildItems);
        }

        /// <summary>
        /// Set the state of the test set node according to its tests' state
        /// </summary>
        /// <param name="testToRun">Test node</param>
        private static void SetTestState(TestItem testToRun)
        {
            if (testToRun.TestCasesToRun.Count == 0) return;
            SetTestItemState(testToRun, testToRun.TestCasesToRun);
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

        #endregion
    }
}