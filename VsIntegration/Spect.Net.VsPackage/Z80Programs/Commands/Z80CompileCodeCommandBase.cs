using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Assembler.Assembler;
using Spect.Net.VsPackage.Vsx.Output;

namespace Spect.Net.VsPackage.Z80Programs.Commands
{
    public abstract class Z80CompileCodeCommandBase : Z80ProgramCommandBase
    {
        /// <summary>
        /// The output of the compilation
        /// </summary>
        protected AssemblerOutput Output { get; set; }

        /// <summary>
        /// Gets the start address to use when running code
        /// </summary>
        public int StartAddress => 
            Output == null 
                ? -1
                : Output.EntryAddress 
                    ?? Output.Segments[0].StartAddress;

        /// <summary>
        /// Gets the start address to use when exporting code
        /// </summary>
        public int ExportStartAddress =>
            Output == null
                ? -1
                : Output.ExportEntryAddress
                  ?? Output.EntryAddress
                  ?? Output.Segments[0].StartAddress;

        /// <summary>Override this method to define the status query action</summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            mc.Enabled = !Package.CodeManager.CompilatioInProgress;
        }

        /// <summary>
        /// Get the path of the item to compile
        /// </summary>
        protected virtual string CompiledItemPath => ItemPath;

        /// <summary>
        /// Override this method to define how to prepare the command on the
        /// main thread of Visual Studio
        /// </summary>
        protected override void PrepareCommandOnMainThread(ref bool cancel)
        {
            base.PrepareCommandOnMainThread(ref cancel);
            if (cancel) return;

            // --- Get the item
            GetItem(out var hierarchy, out _);
            if (hierarchy == null)
            {
                cancel = true;
                return;
            }

            // --- Clear the error list
            Package.ErrorList.Clear();

            // --- Sign that the compilation is in progress, and there
            // --- in no compiled output yet
            Package.CodeManager.CompilatioInProgress = true;
            Package.DebugInfoProvider.CompiledOutput = null;
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
            var pane = OutputWindow.GetPane<Z80BuildOutputPane>();
            pane.WriteLine("Z80 Assembler");
            Output = codeManager.Compile(hierarchy, itemId, PrepareOptions());
            var duration = (DateTime.Now - start).TotalMilliseconds;
            pane.WriteLine($"Compile time: {duration}ms");

            if (Output.ErrorCount != 0)
            {
                // --- Compilation completed with errors
                return false;
            }

            // --- Sign the compilation was successful
            Package.DebugInfoProvider.CompiledOutput = Output;
            return true;
        }

        /// <summary>
        /// Override this method to prepare assembler options
        /// </summary>
        /// <returns>Options to use with the assembler</returns>
        protected virtual AssemblerOptions PrepareOptions()
        {
            var options = new AssemblerOptions
            {
                CurrentModel = SpectNetPackage.GetCurrentSpectrumModelType()
            };
            var runOptions = SpectNetPackage.Default.Options.RunSymbols;
            if (runOptions != null)
            {
                var symbols = runOptions.Split(new [] {';'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var symbol in symbols)
                {
                    if (!options.PredefinedSymbols.Contains(symbol))
                    {
                        options.PredefinedSymbols.Add(symbol);
                    }
                }
            }
            return options;
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