using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.TestParser.Compiler;
using Spect.Net.TestParser.Plan;
using Spect.Net.TestParser.SyntaxTree.Expressions;
using Spect.Net.VsPackage.Vsx.Output;
using Spect.Net.VsPackage.Z80Programs;
using ErrorTask = Microsoft.VisualStudio.Shell.ErrorTask;
using TaskCategory = Microsoft.VisualStudio.Shell.TaskCategory;
using TaskErrorCategory = Microsoft.VisualStudio.Shell.TaskErrorCategory;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// This class is responsible for managing Z80 unit test files
    /// </summary>
    public class Z80TestManager : IMachineContext
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

        #region Test compilation

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

        #endregion

        #region UI management

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

        #endregion

        #region Tests execution

        /// <summary>
        /// Executes all tests that start with the specified node
        /// </summary>
        /// <param name="vm">Test explorer view model</param>
        /// <param name="node">Root node of the subtree to run the tests for</param>
        /// <param name="token">Token to stop tests</param>
        public Task RunTestsFromNode(TestExplorerToolWindowViewModel vm, TestItemBase node, CancellationToken token)
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
                ? ExecuteTestTree(vm, rootToRun, token) 
                : Task.FromResult(0);
        }

        /// <summary>
        /// Execute all test held by the specified root node
        /// </summary>
        /// <param name="vm">Test explorer view model</param>
        /// <param name="rootToRun">Root node instance</param>
        /// <param name="token">Token to cancel tests</param>
        private async Task ExecuteTestTree(TestExplorerToolWindowViewModel vm, TestRootItem rootToRun, CancellationToken token)
        {
            vm.TestRoot.SubTreeForEach(item => item.LogItems.Clear());
            vm.TestRoot.Log("Test execution started");
            var watch = new Stopwatch();
            watch.Start();
            rootToRun.State = TestState.Running;
            try
            {
                foreach (var fileToRun in rootToRun.TestFilesToRun)
                {
                    fileToRun.State = TestState.Running;
                    SetTestRootState(rootToRun);
                    await ExecuteFileTests(vm, fileToRun, token);
                    SetTestRootState(rootToRun);
                    vm.UpdateCounters();
                }
            }
            catch (Exception ex)
            {
                HandleException(rootToRun, ex, token, true);
            }
            finally
            {
                // --- Mark inconclusive nodes
                rootToRun.TestFilesToRun.ForEach(i =>
                {
                    if (i.State == TestState.NotRun) SetSubTreeState(i, TestState.Inconclusive);
                });
                SetTestRootState(rootToRun);
                vm.UpdateCounters();
                watch.Stop();
                vm.TestRoot.Log($"Test execution completed in {watch.Elapsed.TotalSeconds:####0.####} seconds");
                if (token.IsCancellationRequested)
                {
                    vm.TestRoot.Log("Test run has been cancelled by the user.", LogEntryType.Fail);
                }
                if (vm.Counters.Success == 1)
                {
                    vm.TestRoot.Log("1 test successfully ran.", LogEntryType.Success);
                }
                else if (vm.Counters.Success > 1)
                {
                    vm.TestRoot.Log($"{vm.Counters.Success} tests successfully ran.", LogEntryType.Success);
                }
                if (vm.Counters.Failed == 1)
                {
                    vm.TestRoot.Log("1 test failed.", LogEntryType.Fail);
                }
                else if (vm.Counters.Failed > 1)
                {
                    vm.TestRoot.Log($"{vm.Counters.Failed} tests failed.", LogEntryType.Fail);
                }

                if (vm.Counters.Aborted > 0 || vm.Counters.Inconclusive > 0)
                {
                    vm.TestRoot.Log("The test result is inconclusive.", LogEntryType.Fail);
                }
            }
        }

        /// <summary>
        /// Execute the tests within the specified test file
        /// </summary>
        /// <param name="vm">Test explorer view model</param>
        /// <param name="fileToRun">Test file to run</param>
        /// <param name="token">Token to cancel tests</param>
        private async Task ExecuteFileTests(TestExplorerToolWindowViewModel vm, TestFileItem fileToRun, CancellationToken token)
        {
            fileToRun.Log("Test file execution started");
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                foreach (var setToRun in fileToRun.TestSetsToRun)
                {
                    if (token.IsCancellationRequested) break;
                    setToRun.State = TestState.Running;
                    SetTestFileState(fileToRun);
                    await ExecuteSetTests(vm, setToRun, token);
                    SetTestFileState(fileToRun);
                    vm.UpdateCounters();
                }
            }
            catch (Exception ex)
            {
                HandleException(fileToRun, ex, token);
            }
            finally
            {
                // --- Mark inconclusive nodes
                fileToRun.TestSetsToRun.ForEach(i =>
                {
                    if (i.State == TestState.NotRun) SetSubTreeState(i, TestState.Inconclusive);
                });
                SetTestFileState(fileToRun);
                vm.UpdateCounters();
                watch.Stop();
                fileToRun.Log($"Test file execution completed in {watch.Elapsed.TotalSeconds:####0.####} seconds");
            }
        }

        /// <summary>
        /// Execute the tests within the specified test set
        /// </summary>
        /// <param name="vm">Test explorer view model</param>
        /// <param name="setToRun">Test set to run</param>
        /// <param name="token">Token to cancel tests</param>
        private async Task ExecuteSetTests(TestExplorerToolWindowViewModel vm, TestSetItem setToRun, CancellationToken token)
        {
            setToRun.Log("Test set execution started" 
                + (setToRun.Plan.TimeoutValue == 0 ? "" : $" with {setToRun.Plan.TimeoutValue}ms timeout" ));
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                // --- Set the test set machine context
                setToRun.Plan.MachineContext = this;

                // --- Set the startup state of the Spectrum VM
                await Package.StateFileManager.SetProjectMachineStartupState();

                // --- Inject the source code into the vm
                var plan = setToRun.Plan;
                Package.CodeManager.InjectCodeIntoVm(plan.CodeOutput);

                // --- Set up registers with default values
                ExecuteAssignment(plan.InitAssignments);

                // --- Set interrupt mode
                var cpu = Package.MachineViewModel.SpectrumVm.Cpu as IZ80CpuTestSupport;
                cpu?.SetIffValues(!setToRun.Plan.DisableInterrupt);

                // --- Execute setup code
                var success = await InvokeCode(plan.Setup, plan.TimeoutValue, token);
                if (!success)
                {
                    setToRun.Log("Test set setup code invocation failed.");
                    setToRun.State = TestState.Aborted;
                    return;
                }

                foreach (var testToRun in setToRun.TestsToRun)
                {
                    testToRun.State = TestState.Running;
                    SetTestSetState(setToRun);
                    await ExecuteTests(vm, testToRun, token);
                    SetTestSetState(setToRun);
                    vm.UpdateCounters();
                }

                // --- Execute cleanup code
                success = await InvokeCode(plan.Cleanup, plan.TimeoutValue, token);
                if (!success)
                {
                    setToRun.Log("Test set cleanup code invocation failed.");
                    setToRun.State = TestState.Aborted;
                    return;
                }

                // --- Stop the Spectrum VM
                var stopped = await Package.CodeManager.StopSpectrumVm(false);
                if (!stopped)
                {
                    setToRun.Log("Stopping the Spectrum virtual machine failed.");
                    setToRun.State = TestState.Aborted;
                }
            }
            catch (Exception ex)
            {
                HandleException(setToRun, ex, token);
            }
            finally
            {
                // --- Mark inconclusive tests
                setToRun.TestsToRun.ForEach(i =>
                {
                    if (i.State == TestState.NotRun) SetSubTreeState(i, TestState.Inconclusive);
                });
                SetTestSetState(setToRun);
                vm.UpdateCounters();
                setToRun.Log($"Test set execution completed in {watch.Elapsed.TotalSeconds:####0.####} seconds");
            }
        }

        /// <summary>
        /// Executes the test within a test set
        /// </summary>
        /// <param name="vm">Test explorer view model</param>
        /// <param name="testToRun">The test to run</param>
        /// <param name="token">Token to cancel tests</param>
        private async Task ExecuteTests(TestExplorerToolWindowViewModel vm, TestItem testToRun, CancellationToken token)
        {
            var timeout = testToRun.Plan.TimeoutValue ?? testToRun.Plan.TestSet.TimeoutValue;
            testToRun.Log("Test set execution started" + (timeout == 0 ? "" : $" with {timeout}ms timeout"));
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                // --- Set the test machine context
                testToRun.Plan.MachineContext = this;

                // --- Check for a single default test case
                if (testToRun.TestCasesToRun.Count == 0)
                {
                    // --- Execute arrange
                    ExecuteArrange(testToRun.Plan, testToRun.Plan.ArrangeAssignments);

                    // --- Set interrupt mode
                    var cpu = Package.MachineViewModel.SpectrumVm.Cpu as IZ80CpuTestSupport;
                    cpu?.SetIffValues(!testToRun.Plan.DisableInterrupt);

                    // --- Execute the test code
                    await InvokeCode(testToRun.Plan.Act, timeout, token);

                    // --- Execute assertions
                    if (ExecuteAssert(testToRun.Plan, testToRun.Plan.Assertions, out var stopIndex))
                    {
                        testToRun.State = TestState.Success;
                    }
                    else
                    {
                        testToRun.State = TestState.Failed;
                        testToRun.Log($"Assertion #{stopIndex} failed.", LogEntryType.Fail);
                    }
                }
                else
                {
                    // --- Iterate through test cases
                    testToRun.Plan.CurrentTestCaseIndex = -1;
                    foreach (var caseToRun in testToRun.TestCasesToRun)
                    {
                        caseToRun.State = TestState.Running;
                        testToRun.Plan.CurrentTestCaseIndex++;
                        await ExecuteCase(vm, testToRun, caseToRun, token);
                        vm.UpdateCounters();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(testToRun, ex, token);
            }
            finally
            {
                // --- Mark inconclusive tests
                testToRun.TestCasesToRun.ForEach(i =>
                {
                    if (i.State == TestState.NotRun) SetSubTreeState(i, TestState.Inconclusive);
                });
                SetTestState(testToRun);
                vm.UpdateCounters();
                testToRun.Log($"Test execution completed in {watch.Elapsed.TotalSeconds:####0.####} seconds");
                if (testToRun.TestCasesToRun.Count == 0)
                {
                    ReportTestResult(testToRun);
                }
            }
        }

        /// <summary>
        /// Executes the specified test case
        /// </summary>
        /// <param name="vm">Test explorer view model</param>
        /// <param name="testToRun">Test that hosts the test case</param>
        /// <param name="caseToRun">Test case to run</param>
        /// <param name="token">Token to cancel tests</param>
        private async Task ExecuteCase(TestExplorerToolWindowViewModel vm, TestItem testToRun, TestCaseItem caseToRun, CancellationToken token)
        {
            var timeout = testToRun.Plan.TimeoutValue ?? testToRun.Plan.TestSet.TimeoutValue;
            caseToRun.Log("Test set execution started" + (timeout == 0 ? "" : $" with {timeout}ms timeout"));
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                // --- Set the test case machine context
                caseToRun.Plan.MachineContext = this;

                // --- Execute arrange
                ExecuteArrange(caseToRun.Plan, testToRun.Plan.ArrangeAssignments);

                // --- Execute the test code
                await InvokeCode(testToRun.Plan.Act, timeout, token);

                // --- Execute assertions
                if (ExecuteAssert(caseToRun.Plan, testToRun.Plan.Assertions, out var stopIndex))
                {
                    caseToRun.State = TestState.Success;
                }
                else
                {
                    caseToRun.State = TestState.Failed;
                    caseToRun.Log($"Assertion #{stopIndex} failed.", LogEntryType.Fail);
                }
            }
            catch (Exception ex)
            {
                HandleException(caseToRun, ex, token);
            }
            finally
            {
                vm.UpdateCounters();
                caseToRun.Log($"Test execution completed in {watch.Elapsed.TotalSeconds:####0.####} seconds");
                ReportTestResult(caseToRun);
            }
        }

        /// <summary>
        /// Executes the assignments
        /// </summary>
        /// <param name="asgns">Assignments</param>
        private void ExecuteAssignment(IReadOnlyCollection<AssignmentPlanBase> asgns)
        {
            if (asgns == null) return;
            foreach (var asgn in asgns)
            {
                switch (asgn)
                {
                    case RegisterAssignmentPlan regAsgn:
                        AssignRegisterValue(regAsgn.RegisterName, regAsgn.Value);
                        break;

                    case FlagAssignmentPlan flagAsgn:
                        AssignFlagValue(flagAsgn.FlagName);
                        break;

                    case MemoryAssignmentPlan memAsgn:
                        var runSupport = Package.MachineViewModel.SpectrumVm as ISpectrumVmRunCodeSupport;
                        runSupport?.InjectCodeToMemory(memAsgn.Address, memAsgn.Value);
                        break;
                }
            }
        }

        /// <summary>
        /// Evaluates arrange assignments
        /// </summary>
        /// <param name="context">Context to use when evaluating the expression</param>
        /// <param name="asgns">Assignments</param>
        private void ExecuteArrange(IExpressionEvaluationContext context, IReadOnlyCollection<RunTimeAssignmentPlanBase> asgns)
        {
            if (asgns == null) return;
            foreach (var asgn in asgns)
            {
                switch (asgn)
                {
                    case RunTimeRegisterAssignmentPlan regAsgn:
                        var value = EvaluateExpression(regAsgn.Value, context).AsWord();
                        AssignRegisterValue(regAsgn.RegisterName, value);
                        break;

                    case RunTimeFlagAssignmentPlan flagAsgn:
                        AssignFlagValue(flagAsgn.FlagName);
                        break;

                    case RunTimeMemoryAssignmentPlan memAsgn:
                        break;
                }
            }
        }

        /// <summary>
        /// Checks all assertions
        /// </summary>
        /// <param name="context">Evaluation context</param>
        /// <param name="asserts">Assertions to check</param>
        /// <param name="stopIndex">The assertion index at which the evaluation stopped</param>
        private bool ExecuteAssert(IExpressionEvaluationContext context, List<ExpressionNode> asserts, out int stopIndex)
        {
            stopIndex = 0;
            if (asserts == null) return true;
            foreach (var assert in asserts)
            {
                stopIndex++;
                var meets = assert.Evaluate(context).AsBool();
                if (!meets)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Evaluates the specified expression
        /// </summary>
        /// <param name="expr">Expression to evaluate</param>
        /// <param name="context">Evaluation context</param>
        /// <returns></returns>
        private ExpressionValue EvaluateExpression(ExpressionNode expr, IExpressionEvaluationContext context)
        {
            var value = expr.Evaluate(context);
            if (value == ExpressionValue.NonEvaluated)
            {
                throw new TestExecutionException("Expression cannot be evaluated.");
            }

            if (value == ExpressionValue.Error)
            {
                throw new TestExecutionException($"Expression evaluated with error: {expr.EvaluationError}");
            }
            return value;
        }

        /// <summary>
        /// Assigns a value to a named register
        /// </summary>
        /// <param name="regName">Register name</param>
        /// <param name="value">Value to assign</param>
        private void AssignRegisterValue(string regName, ushort value)
        {
            var regs = Package.MachineViewModel.SpectrumVm.Cpu.Registers;
            switch (regName.ToUpper())
            {
                case "A": regs.A = (byte)value; break;
                case "B": regs.B = (byte)value; break;
                case "C": regs.C = (byte)value; break;
                case "D": regs.D = (byte)value; break;
                case "E": regs.E = (byte)value; break;
                case "H": regs.H = (byte)value; break;
                case "L": regs.L = (byte)value; break;
                case "XL":
                case "IXL": regs.XL = (byte)value; break;
                case "XH":
                case "IXH": regs.XH = (byte)value; break;
                case "YL":
                case "IYL": regs.YL = (byte)value; break;
                case "YH":
                case "IYH": regs.YH = (byte)value; break;
                case "I": regs.I = (byte)value; break;
                case "R": regs.R = (byte)value; break;
                case "BC": regs.BC = value; break;
                case "DE": regs.DE = value; break;
                case "HL": regs.HL = value; break;
                case "SP": regs.SP = value; break;
                case "IX": regs.IX = value; break;
                case "IY": regs.IY = value; break;
                case "AF'": regs._AF_ = value; break;
                case "BC'": regs._BC_ = value; break;
                case "DE'": regs._DE_ = value; break;
                case "HL'": regs._HL_ = value; break;
                default:
                    throw new TestExecutionException($"Invalid register name: {regName}");
            }
        }

        /// <summary>
        /// Assigns a value to a flag
        /// </summary>
        /// <param name="flagName">Flag name (naming includes the value, too)</param>
        private void AssignFlagValue(string flagName)
        {
            var regs = Package.MachineViewModel.SpectrumVm.Cpu.Registers;
            switch (flagName.ToUpper())
            {
                case "NZ": regs.F &= FlagsResetMask.Z; break;
                case "Z": regs.F |= FlagsSetMask.Z; break;
                case "NC": regs.F &= FlagsResetMask.C; break;
                case "C": regs.F |= FlagsSetMask.C; break;
                case "PE": regs.F &= FlagsResetMask.PV; break;
                case "PO": regs.F |= FlagsSetMask.PV; break;
                case "P": regs.F &= FlagsResetMask.S; break;
                case "M": regs.F |= FlagsSetMask.S; break;
                case "NH": regs.F &= FlagsResetMask.H; break;
                case "H": regs.F |= FlagsSetMask.H; break;
                case "A": regs.F &= FlagsResetMask.N; break;
                case "N": regs.F |= FlagsSetMask.N; break;
                case "N3": regs.F &= FlagsResetMask.R3; break;
                case "3": regs.F |= FlagsSetMask.R3; break;
                case "N5": regs.F &= FlagsResetMask.R5; break;
                case "5": regs.F |= FlagsSetMask.R5; break;
                default:
                    throw new TestExecutionException($"Invalid flag name: {flagName}");
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
            bool removeFromHalt = false;
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
                    removeFromHalt = true;
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
            if (removeFromHalt)
            {
                var cpu = Package.MachineViewModel.SpectrumVm.Cpu as IZ80CpuTestSupport;
                cpu?.RemoveFromHaltedState();
            }
            return success;
        }

        #endregion

        #region Exception management

        /// <summary>
        /// Handle exceptions during testing
        /// </summary>
        /// <param name="item">Test item that raised the exception</param>
        /// <param name="ex">Exception raised</param>
        /// <param name="token">Token to cancel tests</param>
        /// <param name="ignore">If true, ignore the exception; otherwise, rethrow</param>
        private static void HandleException(TestItemBase item, Exception ex, CancellationToken token, bool ignore = false)
        {
            if (!(ex is HandledTestExecutionException))
            {
                item.State = TestState.Aborted;
                string message;
                switch (ex)
                {
                    case TaskCanceledException _:
                        message = token.IsCancellationRequested
                            ? "The test has been cancelled by the user."
                            : "Timeout expired.";
                        break;
                    case TestExecutionException testEx:
                        message = testEx.Message;
                        break;
                    default:
                        message = $"The test engined detected an internal exception: {ex.Message}.";
                        break;
                }

                if (message.Length > 0)
                {
                    message += " ";
                }
                message += "Test aborted.";
                item.Log(message, LogEntryType.Fail);
            }
            if (!ignore) throw new HandledTestExecutionException();
        }

        #endregion

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

        /// <summary>
        /// Reports the result of a test or test case
        /// </summary>
        /// <param name="item"></param>
        private void ReportTestResult(TestItemBase item)
        {
            switch (item.State)
            {
                case TestState.Inconclusive:
                    item.Log("Test is inconclusive", LogEntryType.Fail);
                    break;
                case TestState.Failed:
                    item.Log("Test failed", LogEntryType.Fail);
                    break;
                case TestState.Success:
                    item.Log("Test succeded", LogEntryType.Success);
                    break;
            }
        }

        #endregion

        #region IMachineContext implementation

        /// <summary>
        /// Signs if this is a compile time context
        /// </summary>
        public bool IsCompileTimeContext => false;

        /// <summary>
        /// Gets the value of the specified Z80 register
        /// </summary>
        /// <param name="regName">Register name</param>
        /// <returns>
        /// The register's current value
        /// </returns>
        public ushort GetRegisterValue(string regName)
        {
            var regs = Package.MachineViewModel.SpectrumVm.Cpu.Registers;
            switch (regName.ToUpper())
            {
                case "A": return regs.A;
                case "B": return regs.B;
                case "C": return regs.C;
                case "D": return regs.D;
                case "E": return regs.E;
                case "H": return regs.H;
                case "L": return regs.L;
                case "XL":
                case "IXL": return regs.XL;
                case "XH":
                case "IXH": return regs.XH;
                case "YL":
                case "IYL": return regs.YL;
                case "YH":
                case "IYH": return regs.YH;
                case "I": return regs.I;
                case "R": return regs.R;
                case "BC": return regs.BC;
                case "DE": return regs.DE;
                case "HL": return regs.HL;
                case "SP": return regs.SP;
                case "IX": return regs.IX;
                case "IY": return regs.IY;
                case "AF'": return regs._AF_;
                case "BC'": return regs._BC_;
                case "DE'": return regs._DE_;
                case "HL'": return regs._HL_;
                default:
                    throw new TestExecutionException($"Invalid register name: {regName}");
            }
        }

        /// <summary>
        /// Gets the value of the specified Z80 flag
        /// </summary>
        /// <param name="flagName">Register name</param>
        /// <returns>
        /// The flags's current value
        /// </returns>
        public bool GetFlagValue(string flagName)
        {
            var f = Package.MachineViewModel.SpectrumVm.Cpu.Registers.F;
            switch (flagName.ToUpper())
            {
                case "NZ": return (f & FlagsSetMask.Z) == 0;
                case "Z": return (f & FlagsSetMask.Z) != 0;
                case "NC": return (f & FlagsSetMask.C) == 0;
                case "C": return (f & FlagsSetMask.C) != 0;
                case "PE": return (f & FlagsSetMask.PV) == 0;
                case "PO": return (f & FlagsSetMask.PV) != 0;
                case "P": return (f & FlagsSetMask.S) == 0;
                case "M": return (f & FlagsSetMask.S) != 0;
                case "NH": return (f & FlagsSetMask.H) == 0;
                case "H": return (f & FlagsSetMask.H) != 0;
                case "A": return (f & FlagsSetMask.N) == 0;
                case "N": return (f & FlagsSetMask.N) != 0;
                case "N3": return (f & FlagsSetMask.R3) == 0;
                case "3": return (f & FlagsSetMask.R3) != 0;
                case "N5": return (f & FlagsSetMask.R5) == 0;
                case "5": return (f & FlagsSetMask.R5) != 0;
                default:
                    throw new TestExecutionException($"Invalid flag name: {flagName}");
            }
        }

        /// <summary>
        /// Gets the range of the machines memory from start to end
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        public byte[] GetMemorySection(ushort start, ushort end)
        {
            var memory = Package.MachineViewModel.SpectrumVm.MemoryDevice.CloneMemory();
            if (start > end)
            {
                var tmp = start;
                start = end;
                end = tmp;
            }
            var length = end - start + 1;
            var result = new byte[length];
            for (var i = 0; i < length; i++) result[i] = memory[start + i];
            return result;
        }

        /// <summary>
        /// Gets the range of memory reach values
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        public byte[] GetReachSection(ushort start, ushort end)
        {
            return new byte[0];
        }

        #endregion
    }
}