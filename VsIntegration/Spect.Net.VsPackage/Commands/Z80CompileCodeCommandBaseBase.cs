using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Assembler.Assembler;
using Spect.Net.VsPackage.Vsx.Output;
using Spect.Net.VsPackage.Z80Programs;

namespace Spect.Net.VsPackage.Commands
{
    public abstract class Z80CompileCodeCommandBaseBase : Z80ProgramCommandBase
    {
        /// <summary>
        /// The output of the compilation
        /// </summary>
        protected AssemblerOutput Output { get; set; }

        /// <summary>Override this method to define the status query action</summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            mc.Enabled = !Package.CodeManager.CompilatioInProgress;
        }

        /// <summary>
        /// Override this method to define how to prepare the command on the
        /// main thread of Visual Studio
        /// </summary>
        protected override void PrepareCommandOnMainThread(ref bool cancel)
        {
            // --- Get the item
            GetItem(out var hierarchy, out _);
            if (hierarchy == null)
            {
                cancel = true;
                return;
            }
            Package.CodeManager.CompilatioInProgress = true;
            Package.ApplicationObject.ExecuteCommand("File.SaveAll");
        }

        /// <summary>
        /// Compiles the code.
        /// </summary>
        /// <returns>True, if compilation successful; otherwise, false</returns>
        protected virtual bool CompileCode(IVsHierarchy hierarchy, uint itemId)
        {
            if (hierarchy == null) return false;

            var codeManager = Package.CodeManager;

            // --- Step #1: Compile
            var start = DateTime.Now;
            var pane = OutputWindow.GetPane<Z80OutputPane>();
            pane.WriteLine("Z80 Assembler");
            Output = codeManager.Compile(hierarchy, itemId);
            var duration = (DateTime.Now - start).TotalMilliseconds;
            pane.WriteLine($"Compile time: {duration}ms");

            if (Output.ErrorCount != 0)
            {
                // --- Compilation completed with errors
                return false;
            }
            return true;
        }

        /// <summary>
        /// Override this method to define the completion of successful
        /// command execution on the main thread of Visual Studio
        /// </summary>
        protected override void CompleteOnMainThread()
        {
            DisplayAssemblyErrors();
            HandleAssemblyTasks();
        }

        /// <summary>
        /// Collect tasks from comments
        /// </summary>
        protected void HandleAssemblyTasks()
        {
            Package.TaskList.Clear();
            foreach (var todo in Output.Tasks)
            {
                var task = new Task()
                {
                    Category = TaskCategory.Comments,
                    CanDelete = false,
                    Document = todo.Filename,
                    Line = todo.Line - 1,
                    Column = 1,
                    Text = todo.Description,
                };
                task.Navigate += TodoTaskOnNavigate;
                Package.TaskList.AddTask(task);
            }
        }

        /// <summary>
        /// Collect errors
        /// </summary>
        protected void DisplayAssemblyErrors()
        {
            Package.ErrorList.Clear();
            if (Output.ErrorCount == 0) return;

            foreach (var error in Output.Errors)
            {
                var errorTask = new ErrorTask
                {
                    Category = TaskCategory.User,
                    ErrorCategory = TaskErrorCategory.Error,
                    HierarchyItem = Package.CodeManager.CurrentHierarchy,
                    Document = error.Filename ?? ItemPath,
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

            Package.ApplicationObject.ExecuteCommand("View.ErrorList");
        }

        /// <summary>
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected override void FinallyOnMainThread()
        {
            Package.CodeManager.CompilatioInProgress = false;
        }

        /// <summary>
        /// Navigate to the item from the task list
        /// </summary>
        private void TodoTaskOnNavigate(object sender, EventArgs eventArgs)
        {
            if (sender is Task task)
            {
                Package.TaskList.Navigate(task);
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