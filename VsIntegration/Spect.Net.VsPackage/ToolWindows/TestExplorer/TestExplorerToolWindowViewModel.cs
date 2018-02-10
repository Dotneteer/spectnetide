using System.Collections.ObjectModel;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// Represents the view model of the Test Explorer tool window
    /// </summary>
    public class TestExplorerToolWindowViewModel : SpectNetPackageToolWindowBase
    {
        private ObservableCollection<TestItemBase> _testTreeItems;
        private bool _compiledWithError;
        private bool _hasAnyTestFileChanged;
        private TestItemBase _selectedItem;

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
        public bool AutoExpandAfterCompile { get; set; }

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
            set => Set(ref _selectedItem, value);
        }

        /// <summary>
        /// The root item of the test tree
        /// </summary>
        public TestRootItem TestRoot { get; private set; }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public TestExplorerToolWindowViewModel()
        {
            _testTreeItems = new ObservableCollection<TestItemBase>();
            HasAnyTestFileChanged = true;
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

            // --- Compilation successfull, create tree view items
            var testTreeItems = new ObservableCollection<TestItemBase>();
            TestRoot = new TestRootItem(null)
            {
                State = TestState.NotRun,
                Title = "Z80 Unit Tests",
                FileName = null
            };
            testTreeItems.Add(TestRoot);
            foreach (var filePlan in testFiles.TestFilePlans)
            {
                // --- Create test file items
                var newTestFileItem = new TestFileItem(TestRoot, filePlan)
                {
                    State = TestState.NotRun,
                    Title = filePlan.Filename,
                    FileName = filePlan.Filename,
                    LineNo = 0,
                    ColumnNo = 1
                };
                TestRoot.ChildItems.Add(newTestFileItem);

                foreach (var testSetPlan in filePlan.TestSetPlans)
                {
                    // --- Create test sets
                    var newTestSetItem = new TestSetItem(newTestFileItem, testSetPlan)
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
                        var newTestBlockItem = new TestItem(newTestSetItem, testBlockPlan)
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
                            var newTestCase = new TestCaseItem(newTestBlockItem, testCasePlan)
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
        }
    }
}