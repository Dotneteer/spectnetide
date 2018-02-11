using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// Represents the view model of the Test Explorer tool window
    /// </summary>
    public class TestExplorerToolWindowViewModel : SpectNetPackageToolWindowViewModelBase
    {
        private ObservableCollection<TestItemBase> _testTreeItems;
        private bool _compiledWithError;
        private bool _hasAnyTestFileChanged;
        private TestItemBase _selectedItem;
        private bool _isTestInProgress;

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
            }
        }

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
            _testTreeItems = new ObservableCollection<TestItemBase>();
            Package.SolutionClosed += OnSolutionClosed;
            HasAnyTestFileChanged = true;
        }

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
                        }
                    }
                }
            }
            TestTreeItems = testTreeItems;
            SelectedItem = null;
        }
    }
}