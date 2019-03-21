using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.TestParser.Plan;
using Spect.Net.VsPackage.Vsx;
using Task = System.Threading.Tasks.Task;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Spect.Net.VsPackage.Z80Programs.Commands
{
    /// <summary>
    /// This class represents a command that can be associated with a .z80asm file.
    /// </summary>
    public abstract class Z80TestCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// The output of the compilation
        /// </summary>
        protected TestProjectPlan Output { get; } = new TestProjectPlan();

        /// <summary>
        /// This command accepts only Z80 code files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[] {".z80test", ".z80cdproj"};

        /// <summary>Override this method to define the status query action</summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            mc.Enabled = !Package.TestManager.CompilatioInProgress;
        }

        /// <summary>
        /// Compiles the Z80 code file
        /// </summary>
        protected override Task ExecuteAsync()
        {
            CompileCode();
            return Task.FromResult(0);
        }

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
            Package.TestManager.CompilatioInProgress = true;
            Package.ApplicationObject.ExecuteCommand("File.SaveAll");
            Output.Clear();
        }

        /// <summary>
        /// Compiles the code.
        /// </summary>
        /// <returns>True, if compilation successful; otherwise, false</returns>
        protected virtual bool CompileCode()
        {
            GetItem(out var hierarchy, out var itemId);
            if (!(hierarchy is IVsProject project)) return false;
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            project.GetMkDocument(itemId, out var itemFullPath);
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread

            var testPlan = Package.TestManager.CompileFile(itemFullPath);
            Output.Add(testPlan);
            return testPlan.Errors.Count == 0;
        }

        /// <summary>
        /// Override this method to define the completion of successful
        /// command execution on the main thread of Visual Studio
        /// </summary>
        protected override void CompleteOnMainThread()
        {
            Package.TestManager.DisplayTestCompilationErrors(Output);
        }

        /// <summary>
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected override Task FinallyOnMainThreadAsync()
        {
            Package.TestManager.CompilatioInProgress = false;
            if (Package.Options.ConfirmTestCompile && Output.ErrorCount == 0)
            {
                VsxDialogs.Show("The unit test code has been successfully compiled.");
            }
            return Task.FromResult(0);
        }
    }
}