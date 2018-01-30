using System;
using System.IO;
using Microsoft.VisualStudio.Shell;
using Spect.Net.TestParser.Compiler;
using Spect.Net.TestParser.Plan;

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
        public SpectNetPackage Package { get; }

        /// <summary>
        /// Signs that compilation is in progress
        /// </summary>
        public bool CompilatioInProgress { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Z80TestManager()
        {
            Package = SpectNetPackage.Default;
        }

        /// <summary>
        /// Compiles the file with the specified file name
        /// </summary>
        /// <param name="filename">Test file to compile</param>
        /// <returns>Test plan</returns>
        public TestFilePlan CompileFile(string filename)
        {
            var compiler = new Z80TestCompiler
            {
                DefaultSourceFolder = Path.GetDirectoryName(filename)
            };
            return compiler.CompileFile(filename);
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