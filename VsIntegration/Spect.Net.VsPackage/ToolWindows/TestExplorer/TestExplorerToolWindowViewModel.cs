using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using Spect.Net.VsPackage.ProjectStructure;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// Represents the view model of the Test Explorer tool window
    /// </summary>
    public class TestExplorerToolWindowViewModel : SpectNetPackageToolWindowBase
    {
        private ObservableCollection<TestTreeItemBase> _testTreeItems;
        private bool _compiledWithError;
        private bool _hasAnyTestFileChanged;

        /// <summary>
        /// The test tree items of the Unit Test Explorer
        /// </summary>
        public ObservableCollection<TestTreeItemBase> TestTreeItems
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
        /// Command that compiles all files
        /// </summary>
        public RelayCommand CompileAllCommand { get; set; }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public TestExplorerToolWindowViewModel()
        {
            _testTreeItems = new ObservableCollection<TestTreeItemBase>();
            CompileAllCommand = new RelayCommand(CompileAllTestFiles);
            HasAnyTestFileChanged = true;
            Package.TestFileChanged += OnTestFileChanged;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Package.TestFileChanged -= OnTestFileChanged;
            base.Dispose();
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
            var testTreeItems = new ObservableCollection<TestTreeItemBase>();
            var testRootItem = new TestTreeRootItem
            {
                State = TestState.NotRun,
                Title = "Z80 Unit Tests",
                FileName = null
            };
            testTreeItems.Add(testRootItem);
            foreach (var fileItem in testFiles.TestFilePlans)
            {
                // --- Create test file items
                var newTestFileItem = new TestTreeFileItem()
                {
                    State = TestState.NotRun,
                    Title = fileItem.Filename,
                    FileName = fileItem.Filename,
                    LineNo = 0,
                    ColumnNo = 1
                };
                testRootItem.ChildItems.Add(newTestFileItem);

                foreach (var testSetItem in fileItem.TestSetPlans)
                {
                    // --- Create test sets
                    var newTestSetItem = new TestTreeTestSetItem()
                    {
                        State = TestState.NotRun,
                        Title = testSetItem.Id,
                        FileName = fileItem.Filename,
                        LineNo = testSetItem.Span.StartLine - 1,
                        ColumnNo = testSetItem.Span.StartColumn
                    };
                    newTestFileItem.ChildItems.Add(newTestSetItem);

                    foreach (var testItem in testSetItem.TestBlocks)
                    {
                        // --- Create test blocks
                        var newTestBlockItem = new TestTreeTestItem()
                        {
                            State = TestState.NotRun,
                            Title = testItem.Id,
                            FileName = fileItem.Filename,
                            LineNo = testItem.Span.StartLine - 1,
                            ColumnNo = testItem.Span.StartColumn
                        };
                        newTestSetItem.ChildItems.Add(newTestBlockItem);

                        foreach (var testCase in testItem.TestCases)
                        {
                            // --- Create test cases
                            var newTestCase = new TestTreeTestCaseItem()
                            {
                                State = TestState.NotRun,
                                Title = testCase.Title,
                                FileName = fileItem.Filename,
                                LineNo = testCase.Span.StartLine - 1,
                                ColumnNo = testCase.Span.StartColumn
                            };
                            newTestBlockItem.ChildItems.Add(newTestCase);
                        }
                    }
                }
            }
            TestTreeItems = testTreeItems;
        }

        #region Helpers

        private void OnTestFileChanged(object sender, FileChangedEventArgs fileChangedEventArgs)
        {
            HasAnyTestFileChanged = true;
        }

        #endregion
    }
}