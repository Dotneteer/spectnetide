using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu;
using Spect.Net.VsPackage.Commands;
using Spect.Net.VsPackage.Compilers;
using Spect.Net.VsPackage.SolutionItems;
using System;
using System.IO;
using System.Text;
using System.Windows;
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
        public const string LIST_TMP_FOLDER = ".SpectNetIde/Lists";
        private const string FILE_EXISTS_MESSAGE =
            "The list file exists in the project. Would you like to override it?";

        private const string INVALID_FOLDER_MESSAGE =
            "The list file folder specified in the Options dialog " +
            "contains invalid characters or an absolute path. Go to the Options dialog and " +
            "fix the issue so that you can add the list file to the project.";

        /// <summary>
        /// The output of the compilation
        /// </summary>
        protected AssemblerOutput Output { get; set; }

        /// <summary>
        /// The start address of the memory view
        /// </summary>
        public int MemoryStartAddress
        {
            get
            {
                if (Output == null) return -1;
                if (Output.EntryAddress != null) return Output.EntryAddress.Value;
                return Output.ExportEntryAddress
                    ?? Output.Segments[0].StartAddress;
            }
        }

        /// <summary>
        /// The start address of the disassembly view
        /// </summary>
        public int DisassemblyStartAddress
        {
            get
            {
                if (Output == null) return -1;
                if (Output.EntryAddress != null) return Output.EntryAddress.Value;
                return Output.ExportEntryAddress
                       ?? Output.Segments[0].StartAddress;
            }
        }

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

        /// <summary>
        /// Disables the command if compliation is in progress
        /// </summary>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;

            mc.Enabled = !HostPackage.CompilationInProgress;
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
            HostPackage.ErrorList.Clear();
            HostPackage.CompilationInProgress = true;
            HostPackage.DebugInfoProvider.CompiledOutput = null;
            HostPackage.ApplicationObject.ExecuteCommand("File.SaveAll");
        }

        /// <summary>
        /// Override this method to prepare assembler options
        /// </summary>
        /// <returns>Options to use with the assembler</returns>
        protected virtual AssemblerOptions PrepareAssemblerOptions()
        {
            var modelName = HostPackage.ActiveProject?.ModelName;
            var currentModel = SpectrumModels.GetModelTypeFromName(modelName);

            var options = new AssemblerOptions
            {
                CurrentModel = currentModel,
                // Use it only for ZX BASIC
                // ProcExplicitLocalsOnly = true
            };
            var runOptions = HostPackage.Options.RunSymbols;
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
        /// Compiles the program code
        /// </summary>
        /// <returns>True, if compilation successful; otherwise, false</returns>
        public bool CompileCode()
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
            var start = DateTime.Now;
            SpectNetPackage.Log(compiler.ServiceName);
            var compiled = compiler.CompileDocument(itemFullPath, PrepareAssemblerOptions(), out var output);
            var duration = (DateTime.Now - start).TotalMilliseconds;
            SpectNetPackage.Log($"Compile time: {duration}ms");
            if (compiled)
            {
                // --- Sign that compilation was successful
                HostPackage.DebugInfoProvider.CompiledOutput = Output = output;
                CreateCompilationListFile(hierarchy, itemId);
            }
            HostPackage.CodeManager.RaiseCompilationCompleted(output);
            return compiled;
        }

        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            await Task.Run(() => CompileCode());
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
            HostPackage.CompilationInProgress = false;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Collect errors
        /// </summary>
        protected void DisplayAssemblyErrors()
        {
            HostPackage.ErrorList.Clear();
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
                HostPackage.ErrorList.AddErrorTask(errorTask);
            }

            HostPackage.ApplicationObject.ExecuteCommand("View.ErrorList");
        }

        /// <summary>
        /// Collect tasks from comments
        /// </summary>
        protected void HandleAssemblyTasks()
        {
            HostPackage.TaskList.Clear();
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
                HostPackage.TaskList.AddTask(task);
            }
        }

        /// <summary>
        /// Navigate to the item from the task list
        /// </summary>
        private void TodoTaskOnNavigate(object sender, EventArgs eventArgs)
        {
            if (sender is VsTask task)
            {
                HostPackage.TaskList.Navigate(task);
            }
        }

        /// <summary>
        /// Navigate to the sender task.
        /// </summary>
        private void ErrorTaskOnNavigate(object sender, EventArgs eventArgs)
        {
            if (sender is ErrorTask task)
            {
                HostPackage.ErrorList.Navigate(task);
            }
        }

        /// <summary>
        /// Creates the compilation list file as set up in the package options
        /// </summary>
        /// <param name="hierarchy">Hierarchy object</param>
        /// <param name="itemId">Identifier of item to compile</param>
        private void CreateCompilationListFile(IVsHierarchy hierarchy, uint itemId)
        {
            var options = HostPackage.Options;
            if (!options.GenerateCompilationList
                || options.GenerateForCompileOnly && !(this is CompileCodeCommand))
            {
                return;
            }

            // --- Create list file name
            if (!(hierarchy is IVsProject project)) return;
            project.GetMkDocument(itemId, out var itemFullPath);
            var codeFile = Path.GetFileNameWithoutExtension(itemFullPath);
            var suffix = string.IsNullOrWhiteSpace(options.CompilationFileSuffix)
                ? string.Empty
                : DateTime.Now.ToString(options.CompilationFileSuffix);
            var listFilename = $"{codeFile}{suffix}{options.CompilationFileExtension ?? ".list"}";
            var listFolder = string.IsNullOrWhiteSpace(options.CompilationFileFolder)
                ? LIST_TMP_FOLDER
                : options.CompilationFileFolder;

            // -- Make sure list folder exists
            if (!Directory.Exists(listFolder))
            {
                Directory.CreateDirectory(listFolder);
            }

            // --- Save the list file
            var listContents = CreateListFileContents();
            var fullListFileName = Path.Combine(listFolder, listFilename);
            try
            {
                File.WriteAllText(fullListFileName, listContents);
            }
            catch
            {
                VsxDialogs.Show($"Error when writing list file {fullListFileName}. "
                    + "The file name may contain invalid characters, or you miss file permissions.",
                    "Error when creating list file.", MessageBoxButton.OK, VsxMessageBoxIcon.Error);
                return;
            }

            if (options.AddCompilationToProject)
            {
                SpectrumProject.AddFileToProject(HostPackage.Options.CompilationProjectFolder, 
                    fullListFileName, INVALID_FOLDER_MESSAGE, FILE_EXISTS_MESSAGE, false);
            }
        }

        /// <summary>
        /// Creates the contents of the list file from the output information
        /// </summary>
        /// <returns>List file contents</returns>
        private string CreateListFileContents()
        {
            // --- Create a format string from the list template
            var options = HostPackage.Options;
            var template = options.CompilationLineTemplate;
            template = ReplaceFirst(template, "{F}", "{0}");
            template = ReplaceFirst(template, "{F2}", "{0,2}");
            template = ReplaceFirst(template, "{F3}", "{0,3}");
            template = ReplaceFirst(template, "{L}", "{1}");
            template = ReplaceFirst(template, "{L3}", "{1,3}");
            template = ReplaceFirst(template, "{L4}", "{1,4}");
            template = ReplaceFirst(template, "{L5}", "{1,5}");
            template = ReplaceFirst(template, "{A}", "{2:X4}");
            template = ReplaceFirst(template, "{C}", "{3}");
            template = ReplaceFirst(template, "{CX}", "{3,-11}");
            template = ReplaceFirst(template, "{S}", "{4}");
            template = template.Replace("\\t", "\t");
            template = template.Replace("{F}", "");
            template = template.Replace("{F2}", "");
            template = template.Replace("{F3}", "");
            template = template.Replace("{L}", "");
            template = template.Replace("{L3}", "");
            template = template.Replace("{L4}", "");
            template = template.Replace("{L5}", "");
            template = template.Replace("{A}", "");
            template = template.Replace("{C}", "");
            template = template.Replace("{CX}", "");
            template = template.Replace("{S}", "");
            template += Environment.NewLine;

            // --- Initialize the output loop
            var list = new StringBuilder(1024 * 1024);
            var currentFileIndex = -1;

            // --- Create file map
            if (options.ListFileOutputMode == ListFileOutputMode.FileMap)
            {
                list.AppendLine("; File Mapping:");
                for (var i = 0; i < Output.SourceFileList.Count; i++)
                {
                    list.AppendLine($"; File #{i}: {Output.SourceFileList[i].Filename}");
                }
                list.AppendLine();
            }

            // --- Create listing output
            foreach (var outItem in Output.ListFileItems)
            {
                // --- Check file header
                if (options.ListFileOutputMode == ListFileOutputMode.Header
                    && outItem.FileIndex != currentFileIndex)
                {
                    list.AppendLine();
                    list.AppendLine($"; File #{outItem.FileIndex}: {Output.SourceFileList[outItem.FileIndex].Filename}");
                    list.AppendLine();
                }
                currentFileIndex = outItem.FileIndex;

                // --- Emit non-code lines
                if (outItem.CodeLength == 0)
                {
                    list.AppendFormat(template,
                        outItem.FileIndex,
                        outItem.LineNumber,
                        outItem.Address,
                        string.Empty,
                        outItem.SourceText);
                    continue;
                }

                // --- Output separate lines for each 4 operation codes
                for (var i = 0; i < outItem.CodeLength; i += 4)
                {
                    var opCodes = new StringBuilder(20);
                    for (var j = i; j < i + 4 && j < outItem.CodeLength; j++)
                    {
                        opCodes.Append(
                            $"{Output.Segments[outItem.SegmentIndex].EmittedCode[outItem.CodeStartIndex + j]:X2} ");
                    }

                    // --- Display list line contents
                    list.AppendFormat(template,
                        outItem.FileIndex,
                        outItem.LineNumber,
                        outItem.Address + i,
                        opCodes.ToString().TrimEnd(),
                        i == 0 ? outItem.SourceText : string.Empty);
                }
            }

            // --- Done
            return list.ToString();

            string ReplaceFirst(string source, string oldString, string newString)
            {
                var pos = source.IndexOf(oldString, StringComparison.Ordinal);
                if (pos < 0) return source;

                return pos == 0
                    ? newString + source.Substring(oldString.Length)
                    : source.Substring(0, pos) + newString + source.Substring(pos + oldString.Length);
            }
        }
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread