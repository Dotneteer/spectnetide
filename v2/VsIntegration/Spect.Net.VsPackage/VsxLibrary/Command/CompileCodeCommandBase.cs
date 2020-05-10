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
using System.Threading.Tasks;
using System.Windows;
using Spect.Net.VsPackage.VsxLibrary.Output;
using Task = System.Threading.Tasks.Task;
using VsTask = Microsoft.VisualStudio.Shell.Task;
using OutputWindow = Spect.Net.VsPackage.VsxLibrary.Output.OutputWindow;
using Spect.Net.VsPackage.Dialogs.Export;
using Microsoft.VisualStudio.Shell.TableManager;
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

        // --- Compiler state variables
        private ICompilerService _compiler;
        private IVsHierarchy _hierarchy;
        private uint _itemId;

        /// <summary>
        /// The full path of item compiled
        /// </summary>
        protected string ItemFullPath { get; set; }

        /// <summary>
        /// The output of the compilation
        /// </summary>
        protected AssemblerOutput Output { get; set; }

        /// <summary>
        /// Error message of the pre-build event
        /// </summary>
        protected string PrebuildError { get; set; }

        /// <summary>
        /// Error message of the post-build event
        /// </summary>
        protected string PostbuildError { get; set; }

        /// <summary>
        /// Error message of the cleanup event
        /// </summary>
        protected string CleanupError { get; set; }

        /// <summary>
        /// Error message of the pre-export event
        /// </summary>
        protected string PreexportError { get; set; }

        /// <summary>
        /// Error message of the post-export event
        /// </summary>
        protected string PostexportError { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool CompileSuccess { get; set; }

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
        /// Disables the command if compilation is in progress
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
        protected override async Task ExecuteOnMainThreadAsync()
        {
            // --- Get the item
            GetAffectedItem(out _hierarchy, out _itemId);
            if (_hierarchy == null || !(_hierarchy is IVsProject project))
            {
                IsCancelled = true;
                return;
            }

            project.GetMkDocument(_itemId, out var itemFullPath);
            ItemFullPath = itemFullPath;
            var extension = Path.GetExtension(ItemFullPath);

            _compiler = null;
            switch (extension?.ToLower())
            {
                case ".z80asm":
                    _compiler = new Z80AssemblyCompiler();
                    break;
                case ".zxbas":
                    _compiler = new ZxBasicCompiler();
                    break;
            }

            // --- Insist to have a compiler
            if (_compiler == null)
            {
                IsCancelled = true;
                return;
            }

            // --- Check that the compiler has been installed
            if (!await _compiler.IsAvailable())
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
            if (ItemExtension.ToLower() == ".zxbas")
            {
                currentModel = SpectrumModelType.Spectrum48;
            }

            var options = new AssemblerOptions
            {
                CurrentModel = currentModel
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
        /// Gets the document that this command should use
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="itemId"></param>
        protected virtual void GetAffectedItem(out IVsHierarchy hierarchy, out uint itemId)
        {
            GetItem(out hierarchy, out itemId);
        }

        /// <summary>
        /// Compiles the program code
        /// </summary>
        /// <returns>True, if compilation successful; otherwise, false</returns>
        public async Task<bool> CompileCode()
        {
            // --- Execute pre-build event
            var codeManager = SpectNetPackage.Default.CodeManager;
            PrebuildError = null;
            PostbuildError = null;
            CleanupError = null;
            PreexportError = null;
            PostexportError = null;
            var eventOutput = await codeManager.RunPreBuildEvent(ItemFullPath);
            if (eventOutput != null)
            {
                PrebuildError = eventOutput;
                CleanupError = await codeManager.RunBuildErrorEvent(ItemFullPath);
                DisplayBuildErrors();
                return false;
            }

            var start = DateTime.Now;
            var pane = OutputWindow.GetPane<Z80AssemblerOutputPane>();
            await pane.WriteLineAsync(_compiler.ServiceName);
            var output = await _compiler.CompileDocument(ItemFullPath, PrepareAssemblerOptions());
            var duration = (DateTime.Now - start).TotalMilliseconds;
            await pane.WriteLineAsync($"Compile time: {duration}ms");
            var compiled = output != null;
            if (compiled)
            {
                // --- Sign that compilation was successful
                HostPackage.DebugInfoProvider.CompiledOutput = Output = output;
                CreateCompilationListFile(_hierarchy, _itemId);
            }
            HostPackage.CodeManager.RaiseCompilationCompleted(output);

            // --- Execute post-build event
            if (compiled && output.Errors.Count == 0)
            {
                // --- Export if required
                if (!string.IsNullOrEmpty(SpectNetPackage.Default.Options.ExportOnCompile))
                {
                    var parts = SpectNetPackage.Default.Options.ExportOnCompile.Split(';');
                    var vm = new ExportZ80ProgramViewModel
                    {
                        Name = Path.GetFileNameWithoutExtension(ItemPath) ?? "MyCode"
                    };
                    if (parts.Length > 0)
                    {
                        // --- Validate format
                        var format = parts[0].Trim().ToLower();
                        if (format == "tap")
                        {
                            vm.Format = ExportFormat.Tap;
                        }
                        else if (format == "tzx")
                        {
                            vm.Format = ExportFormat.Tzx;
                        }
                        else if (format == "hex")
                        {
                            vm.Format = ExportFormat.IntelHex;
                        }
                        else
                        {
                            PostbuildError = $"Invalid export format: {parts[0]}";
                        }
                    }

                    if (PostbuildError == null && parts.Length > 1)
                    {
                        var flag = parts[1].Trim();
                        if (flag == "" || flag == "1")
                        {
                            vm.AutoStart = true;
                        }
                        else if (flag == "0")
                        {
                            vm.AutoStart = false;
                        }
                        else
                        {
                            PostbuildError = $"Invalid export Auto-Start flag (use 0 or 1): {parts[1]}";
                        }
                    }

                    if (PostbuildError == null && parts.Length > 2)
                    {
                        var flag = parts[2].Trim();
                        if (flag == "" || flag == "1")
                        {
                            vm.ApplyClear = true;
                        }
                        else if (flag == "0")
                        {
                            vm.ApplyClear = false;
                        }
                        else
                        {
                            PostbuildError = $"Invalid export Apply CLEAR flag (use 0 or 1): {parts[2]}";
                        }
                    }

                    vm.SingleBlock = true;
                    vm.AddToProject = false;

                    if (PostbuildError == null && parts.Length > 3)
                    {
                        var flag = parts[3].Trim();
                        if (flag == "" || flag == "0")
                        {
                            vm.AddPause0 = false;
                        }
                        else if (flag == "1")
                        {
                            vm.AddPause0 = true;
                        }
                        else
                        {
                            PostbuildError = $"Invalid export Add PAUSE flag (use 0 or 1): {parts[3]}";
                        }
                    }

                    if (PostbuildError == null && parts.Length > 4 && parts[4].Trim() != "")
                    {
                        if (ushort.TryParse(parts[4], out var startAddr))
                        {
                            vm.StartAddress = startAddr.ToString();
                        }
                        else
                        {
                            PostbuildError = $"Invalid Start Address (use 0-65535): {parts[4]}";
                        }
                    }

                    if (PostbuildError == null && parts.Length > 5 && parts[5].Trim() != "")
                    {
                        if (ushort.TryParse(parts[5], out var border))
                        {
                            if (border >= 0 && border <= 7)
                            {
                                vm.Border = border.ToString();
                            }
                            else
                            {
                                PostbuildError = $"Invalid Border value (use 0-7): {parts[5]}";
                            }
                        }
                        else
                        {
                            PostbuildError = $"Invalid Border value (use 0-7): {parts[5]}";
                        }
                    }

                    if (PostbuildError == null && parts.Length > 6)
                    {
                        vm.ScreenFile = parts[6];
                    }

                    if (PostbuildError == null)
                    {
                        ExportProgramCommand.ExportCompiledCode(Output, vm);
                    }
                }

                // --- Run post-build tasks
                eventOutput = await codeManager.RunPostBuildEvent(ItemFullPath);
                if (eventOutput != null)
                {
                    PostbuildError = eventOutput;
                    CleanupError = await codeManager.RunBuildErrorEvent(ItemFullPath);
                    DisplayBuildErrors();
                    return false;
                }
            }
            else
            {
                CleanupError = await codeManager.RunBuildErrorEvent(ItemFullPath);
                DisplayBuildErrors();
            }
            return compiled;
        }

        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            IVsStatusbar statusBar = (IVsStatusbar)await SpectNetPackage.Default.GetServiceAsync(typeof(SVsStatusbar));
            statusBar.IsFrozen(out int frozen);
            if (frozen != 0)
            {
                statusBar.FreezeOutput(0);
            }
            statusBar.SetText("Building ZX Spectrum code");
            statusBar.FreezeOutput(1);
            object icon = (short)Constants.SBAI_Build;
            statusBar.Animation(1, ref icon);
            CompileSuccess = await Task.Run(() => CompileCode());
            statusBar.FreezeOutput(0);
            statusBar.Animation(0, ref icon);
            statusBar.Clear();
        }

        /// <summary>
        /// Displays error and task information when compilation is done
        /// </summary>
        protected override Task CompleteOnMainThreadAsync()
        {
            DisplayBuildErrors();
            HandleAssemblyTasks();
            return Task.FromResult(0);
        }

        /// <summary>
        /// Takes care that compilation is allowed again
        /// </summary>
        protected override Task FinallyOnMainThreadAsync()
        {
            HostPackage.CompilationInProgress = false;
            if (Output.Errors.Count > 0 ||
                PrebuildError != null
                || PostbuildError != null
                || CleanupError != null)
            {
                HostPackage.ApplicationObject.ExecuteCommand("View.ErrorList");
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Collect errors
        /// </summary>
        protected void DisplayBuildErrors()
        {
            HostPackage.ErrorList.Clear();

            GetCodeItem(out var hierarchy, out _);
            if (PrebuildError != null)
            {
                var prebuildTask = new ErrorTask
                {
                    Category = TaskCategory.User,
                    ErrorCategory = TaskErrorCategory.Error,
                    HierarchyItem = hierarchy,
                    Text = $"Pre-build: {PrebuildError}",
                    CanDelete = true
                };
                HostPackage.ErrorList.AddErrorTask(prebuildTask);
            }

            if (Output != null)
            {
                foreach (var error in Output.Errors)
                {
                    var errorTask = new ErrorTask
                    {
                        Category = TaskCategory.User,
                        ErrorCategory = error.IsWarning ? TaskErrorCategory.Warning : TaskErrorCategory.Error,
                        HierarchyItem = hierarchy,
                        Document = error.Filename ?? ItemPath,
                        Line = error.Line - 1,
                        Column = error.Column,
                        Text = error.ErrorCode == null
                            ? error.Message
                            : $"{error.ErrorCode}: {error.Message}",
                        CanDelete = true
                    };
                    errorTask.Navigate += ErrorTaskOnNavigate;
                    HostPackage.ErrorList.AddErrorTask(errorTask);
                }
            }

            if (PostbuildError != null)
            {
                var postbuildTask = new ErrorTask
                {
                    Category = TaskCategory.User,
                    ErrorCategory = TaskErrorCategory.Error,
                    HierarchyItem = hierarchy,
                    Text = $"Post-build: {PostbuildError}",
                    CanDelete = true
                };
                HostPackage.ErrorList.AddErrorTask(postbuildTask);
            }

            if (PreexportError != null)
            {
                var cleanupTask = new ErrorTask
                {
                    Category = TaskCategory.User,
                    ErrorCategory = TaskErrorCategory.Error,
                    HierarchyItem = hierarchy,
                    Text = $"Pre-export: {PreexportError}",
                    CanDelete = true
                };
                HostPackage.ErrorList.AddErrorTask(cleanupTask);
            }

            if (PostexportError != null)
            {
                var cleanupTask = new ErrorTask
                {
                    Category = TaskCategory.User,
                    ErrorCategory = TaskErrorCategory.Error,
                    HierarchyItem = hierarchy,
                    Text = $"Post-export: {PostexportError}",
                    CanDelete = true
                };
                HostPackage.ErrorList.AddErrorTask(cleanupTask);
            }

            if (CleanupError != null)
            {
                var cleanupTask = new ErrorTask
                {
                    Category = TaskCategory.User,
                    ErrorCategory = TaskErrorCategory.Error,
                    HierarchyItem = hierarchy,
                    Text = $"Cleanup: {CleanupError}",
                    CanDelete = true
                };
                HostPackage.ErrorList.AddErrorTask(cleanupTask);
            }
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
                var navTask = new ErrorTask
                {
                    Category = task.Category,
                    ErrorCategory = task.ErrorCategory,
                    HierarchyItem = task.HierarchyItem,
                    Document = task.Document,
                    Line = task.Line + 1,
                    Column = task.Column,
                    Text = task.Text,
                    CanDelete = task.CanDelete
                };
                HostPackage.ErrorList.Navigate(navTask);
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