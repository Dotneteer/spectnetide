using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// Represents the view model of the Test Explorer tool window
    /// </summary>
    public class TestExplorerToolWindowViewModel : SpectrumGenericToolWindowViewModel
    {
        private ObservableCollection<TestItemBase> _testTreeItems;
        private bool _compiledWithError;
        private bool _hasAnyTestFileChanged;
        private TestItemBase _selectedItem;
        private bool _isTestInProgress;
        private TestCounters _counters;

        /// <summary>
        /// Test counters
        /// </summary>
        public TestCounters Counters
        {
            get => _counters;
            set => Set(ref _counters, value);
        }

        /// <summary>
        /// The test tree items of the Unit Test Explorer
        /// </summary>
        public ObservableCollection<TestItemBase> TestTreeItems
        {
            get => _testTreeItems;
            set => Set(ref _testTreeItems, value);
        }

        /// <summary>
        /// Indicates if any of the test file has changed since the last compilation
        /// </summary>
        public bool HasAnyTestFileChanged
        {
            get => _hasAnyTestFileChanged;
            set => Set(ref _hasAnyTestFileChanged, value);
        }

        /// <summary>
        /// Indicates if compilation failed
        /// </summary>
        public bool CompiledWithError
        {
            get => _compiledWithError;
            set => Set(ref _compiledWithError, value);
        }

        /// <summary>
        /// Signs that after compile the tests should be automatically expanded
        /// </summary>
        public bool AutoExpandAfterCompile { get; set; } = true;

        /// <summary>
        /// Signs that after compile the tests should be automatically collapsed
        /// </summary>
        public bool AutoCollapseAfterCompile { get; set; }

        /// <summary>
        /// The item selected in the test tree;
        /// </summary>
        public TestItemBase SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (!Set(ref _selectedItem, value)) return;
                TestRoot.SubTreeForEach(item => item.IsSelected = false, _selectedItem);
                _selectedItem.IsSelected = true;
                SelectedItemChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// This event signs that the view model's SelectedItem property has been changed
        /// </summary>
        public event EventHandler SelectedItemChanged;

        /// <summary>
        /// The root item of the test tree
        /// </summary>
        public TestRootItem TestRoot { get; private set; }

        /// <summary>
        /// Gets the cancellation token source that can cancel tests
        /// </summary>
        public CancellationTokenSource CancellationSource { get; private set; }

        /// <summary>
        /// Indicates if test is in progress
        /// </summary>
        public bool IsTestInProgress
        {
            get => _isTestInProgress;
            set => Set(ref _isTestInProgress, value);
        }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public TestExplorerToolWindowViewModel()
        {
            if (IsInDesignMode)
            {
                Counters = new TestCounters
                {
                    Total = 123,
                    Success = 99,
                    Failed = 12,
                    Aborted = 3,
                    Inconclusive = 16,
                    NotRun = 100
                };
                return;
            }
            _testTreeItems = new ObservableCollection<TestItemBase>();
            Counters = new TestCounters();
            Package.SolutionClosed += OnSolutionClosed;
            HasAnyTestFileChanged = true;
        }

        /// <summary>
        /// Cleans up when closing the splution
        /// </summary>
        private void OnSolutionClosed(object sender, EventArgs eventArgs)
        {
            _testTreeItems.Clear();
            HasAnyTestFileChanged = true;
            CompiledWithError = false;
            TestRoot = null;
            SelectedItem = null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            CancellationSource?.Dispose();
            Package.SolutionClosed -= OnSolutionClosed;
            base.Dispose();
        }

        /// <summary>
        /// Gets a new cancellation token that can be used to abort tests
        /// </summary>
        /// <returns>The new cancellation token</returns>
        public CancellationToken GetNewCancellationToken()
        {
            CancellationSource?.Dispose();
            CancellationSource = new CancellationTokenSource();
            return CancellationSource.Token;
        }

        /// <summary>
        /// Compiles all test file in the current project
        /// </summary>
        public void CompileAllTestFiles()
        {
            if (!HasAnyTestFileChanged)
            {
                return;
            }

            TestTreeItems.Clear();
            Package.ApplicationObject.ExecuteCommand("File.SaveAll");
            HasAnyTestFileChanged = false;
            var testFiles = Package.TestManager.CompileAllFiles();
            CompiledWithError = testFiles.ErrorCount != 0;
            if (CompiledWithError)
            {
                Package.TestManager.DisplayTestCompilationErrors(testFiles);
                return;
            }

            // --- Exit if no test files
            if (testFiles.TestFilePlans.Count == 0) return;

            // --- Compilation successfull, create tree view items
            var projectFolder = Path.GetDirectoryName(Package.CodeDiscoverySolution.CurrentProject.Root.FileName);
            var testTreeItems = new ObservableCollection<TestItemBase>();
            TestRoot = new TestRootItem(this, null)
            {
                State = TestState.NotRun,
                Title = "Z80 Unit Tests",
                FileName = null
            };
            testTreeItems.Add(TestRoot);
            Counters = new TestCounters();
            foreach (var filePlan in testFiles.TestFilePlans)
            {
                // --- Create test file items
                var newTestFileItem = new TestFileItem(this, TestRoot, filePlan)
                {
                    State = TestState.NotRun,
                    Title = projectFolder != null && filePlan.Filename.StartsWith(projectFolder)
                        ? "." + filePlan.Filename.Substring(projectFolder.Length)
                        : filePlan.Filename,
                    FileName = filePlan.Filename,
                    LineNo = 0,
                    ColumnNo = 1
                };
                TestRoot.ChildItems.Add(newTestFileItem);

                foreach (var testSetPlan in filePlan.TestSetPlans)
                {
                    // --- Create test sets
                    var newTestSetItem = new TestSetItem(this, newTestFileItem, testSetPlan)
                    {
                        State = TestState.NotRun,
                        Title = testSetPlan.Id,
                        FileName = filePlan.Filename,
                        LineNo = testSetPlan.Span.StartLine - 1,
                        ColumnNo = testSetPlan.Span.StartColumn
                    };
                    newTestFileItem.ChildItems.Add(newTestSetItem);

                    foreach (var testBlockPlan in testSetPlan.TestBlocks)
                    {
                        // --- Create test blocks
                        var newTestBlockItem = new TestItem(this, newTestSetItem, testBlockPlan)
                        {
                            State = TestState.NotRun,
                            Title = testBlockPlan.Id,
                            FileName = filePlan.Filename,
                            LineNo = testBlockPlan.Span.StartLine - 1,
                            ColumnNo = testBlockPlan.Span.StartColumn
                        };
                        newTestSetItem.ChildItems.Add(newTestBlockItem);
                        if (testBlockPlan.TestCases.Count == 0)
                        {
                            Counters.Total++;
                        }

                        foreach (var testCasePlan in testBlockPlan.TestCases)
                        {
                            // --- Create test cases
                            var newTestCase = new TestCaseItem(this, newTestBlockItem, testCasePlan)
                            {
                                State = TestState.NotRun,
                                Title = testCasePlan.Title,
                                FileName = filePlan.Filename,
                                LineNo = testCasePlan.Span.StartLine - 1,
                                ColumnNo = testCasePlan.Span.StartColumn
                            };
                            newTestBlockItem.ChildItems.Add(newTestCase);
                            Counters.Total++;
                        }
                    }
                }
            }
            TestTreeItems = testTreeItems;
            SelectedItem = null;
            Counters.NotRun = Counters.Total;
        }

        /// <summary>
        /// Updates the test counters according to the current state of the test tree items
        /// </summary>
        public void UpdateCounters()
        {
            if (TestRoot == null) return;
            var counters = new TestCounters();
            foreach (var file in TestRoot.TestFilesToRun)
            {
                foreach (var set in file.TestSetsToRun)
                {
                    foreach (var test in set.TestsToRun)
                    {
                        if (test.TestCasesToRun.Count == 0)
                        {
                            UpdateCounters(test);
                        }
                        else
                        {
                            foreach (var testCase in test.TestCasesToRun)
                            {
                                UpdateCounters(testCase);
                            }
                        }
                    }
                }
            }
            Counters = counters;

            void UpdateCounters(TestItemBase item)
            {
                counters.Total++;
                switch (item.State)
                {
                    case TestState.NotRun:
                        counters.NotRun++;
                        break;
                    case TestState.Inconclusive:
                        counters.Inconclusive++;
                        break;
                    case TestState.Aborted:
                        counters.Aborted++;
                        break;
                    case TestState.Failed:
                        counters.Failed++;
                        break;
                    case TestState.Success:
                        counters.Success++;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Respresents the tes counters
    /// </summary>
    public class TestCounters: EnhancedViewModelBase
    {
        private int _total;
        private int _success;
        private int _failed;
        private int _inconclusive;
        private int _aborted;
        private int _notRun;

        /// <summary>
        /// Total number of tests
        /// </summary>
        public int Total
        {
            get => _total;
            set => Set(ref _total, value);
        }

        /// <summary>
        /// #of successful tests
        /// </summary>
        public int Success
        {
            get => _success;
            set => Set(ref _success, value);
        }

        /// <summary>
        /// #of of failed tests
        /// </summary>
        public int Failed
        {
            get => _failed;
            set => Set(ref _failed, value);
        }

        /// <summary>
        /// #of of inconclusive tests
        /// </summary>
        public int Inconclusive
        {
            get => _inconclusive;
            set => Set(ref _inconclusive, value);
        }

        /// <summary>
        /// #of of aborted tests
        /// </summary>
        public int Aborted
        {
            get => _aborted;
            set => Set(ref _aborted, value);
        }

        /// <summary>
        /// #of tests not run
        /// </summary>
        public int NotRun
        {
            get => _notRun;
            set => Set(ref _notRun, value);
        }
    }
}