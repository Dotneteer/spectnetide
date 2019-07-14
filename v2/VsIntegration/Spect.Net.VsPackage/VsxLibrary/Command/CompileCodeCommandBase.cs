using System;
using System.IO;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu;
using Spect.Net.VsPackage.Compilers;
using Task = System.Threading.Tasks.Task;
using VsTask = Microsoft.VisualStudio.Shell.Task;
// ReSharper disable SuspiciousTypeConversion.Global

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class represents a command that needs compilation as a part of execution.
    /// </summary>
    public abstract class CompileCodeCommandBase : ExecutableSpectrumProgramCommandBase
    {
        /// <summary>
        /// The output of the compilation
        /// </summary>
        protected AssemblerOutput Output { get; set; }

        /// <summary>
        /// Disables the command if compliation is in progress
        /// </summary>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;

            mc.Enabled = !SpectNetPackage.Default.CompilationInProgress;
        }

        /// <summary>
        /// Override this method to define how to prepare the command on the
        /// main thread of Visual Studio
        /// </summary>
        protected override void ExecuteOnMainThread()
        {
            base.ExecuteOnMainThread();
            if (IsCancelled) return;

            // --- Get the item
            GetItem(out var hierarchy, out _);
            if (hierarchy == null)
            {
                IsCancelled = true;
                return;
            }


            // --- Sign that the compilation is in progress, and there
            // --- in no compiled output yet
            var package = SpectNetPackage.Default;
            package.ErrorList.Clear();
            SpectNetPackage.Default.CompilationInProgress = true;
            package.DebugInfoProvider.CompiledOutput = null;
            package.ApplicationObject.ExecuteCommand("File.SaveAll");
        }

        /// <summary>
        /// Override this method to prepare assembler options
        /// </summary>
        /// <returns>Options to use with the assembler</returns>
        protected virtual AssemblerOptions PrepareAssemblerOptions()
        {
            var modelName = SpectNetPackage.Default.ActiveProject?.ModelName;
            var currentModel = SpectrumModels.GetModelTypeFromName(modelName);

            var options = new AssemblerOptions
            {
               CurrentModel = currentModel,
               // Use it only for ZX BASIC
               // ProcExplicitLocalsOnly = true
            };
            var runOptions = SpectNetPackage.Default.Options.RunSymbols;
            if (runOptions != null)
            {
                var symbols = runOptions.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
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
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            await Task.Run(() =>
            {
                GetCodeItem(out var hierarchy, out var itemId);
                if (hierarchy == null) return false;

                if (!(hierarchy is IVsProject project)) return false;
                project.GetMkDocument(itemId, out var itemFullPath);
                var extension = Path.GetExtension(itemFullPath);

                ICompilerService compiler = null;
                switch (extension?.ToLower())
                {
                    case ".z80asm":
                        compiler = new Z80AssemblyCompiler();
                        break;
                    case ".zxbas":
                        // TODO: Add ZX BASIC compiler here
                        break;
                }

                if (compiler == null) return false;
                var compiled = compiler.CompileDocument(itemFullPath, PrepareAssemblerOptions(), out var output);
                if (compiled)
                {
                    Output = output;
                }
                return compiled;
            });
        }

        /// <summary>
        /// Displays error and task information when compilation is done
        /// </summary>
        protected override Task CompleteOnMainThreadAsync()
        {
            DisplayAssemblyErrors();
            HandleAssemblyTasks();
            return Task.FromResult(0);
        }

        /// <summary>
        /// Takes care that compilation is allowed again
        /// </summary>
        protected override Task FinallyOnMainThreadAsync()
        {
            SpectNetPackage.Default.CompilationInProgress = false;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Collect errors
        /// </summary>
        protected void DisplayAssemblyErrors()
        {
            var package = SpectNetPackage.Default;
            package.ErrorList.Clear();
            if (Output.ErrorCount == 0) return;

            GetCodeItem(out var hierarchy, out _);
            foreach (var error in Output.Errors)
            {
                var errorTask = new ErrorTask
                {
                    Category = TaskCategory.User,
                    ErrorCategory = TaskErrorCategory.Error,
                    HierarchyItem = hierarchy,
                    Document = error.Filename ?? ItemPath,
                    Line = error.Line,
                    Column = error.Column,
                    Text = error.ErrorCode == null
                        ? error.Message
                        : $"{error.ErrorCode}: {error.Message}",
                    CanDelete = true
                };
                errorTask.Navigate += ErrorTaskOnNavigate;
                package.ErrorList.AddErrorTask(errorTask);
            }

            package.ApplicationObject.ExecuteCommand("View.ErrorList");
        }

        /// <summary>
        /// Collect tasks from comments
        /// </summary>
        protected void HandleAssemblyTasks()
        {
            var package = SpectNetPackage.Default;
            package.TaskList.Clear();
            foreach (var todo in Output.Tasks)
            {
                var task = new VsTask()
                {
                    Category = TaskCategory.Comments,
                    CanDelete = false,
                    Document = todo.Filename,
                    Line = todo.Line - 1,
                    Column = 1,
                    Text = todo.Description,
                };
                task.Navigate += TodoTaskOnNavigate;
                package.TaskList.AddTask(task);
            }
        }

        /// <summary>
        /// Navigate to the item from the task list
        /// </summary>
        private void TodoTaskOnNavigate(object sender, EventArgs eventArgs)
        {
            if (sender is VsTask task)
            {
                SpectNetPackage.Default.TaskList.Navigate(task);
            }
        }

        /// <summary>
        /// Navigate to the sender task.
        /// </summary>
        private void ErrorTaskOnNavigate(object sender, EventArgs eventArgs)
        {
            if (sender is ErrorTask task)
            {
                SpectNetPackage.Default.ErrorList.Navigate(task);
            }
        }
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread