using System;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Assembler.Assembler;
using Spect.Net.VsPackage.Commands;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Vsx.Output;
using Task = System.Threading.Tasks.Task;
using VsTask = Microsoft.VisualStudio.Shell.Task;
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable StringLiteralTypo
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.Z80Programs.Commands
{
    public abstract class Z80CompileCodeCommandBase : Z80ProgramCommandBase
    {
        public const string LIST_TMP_FOLDER = ".SpectNetIde/Lists";
        private const string FILE_EXISTS_MESSAGE = "The list file exists in the project. " +
                                                   "Would you like to override it?";

        private const string INVALID_FOLDER_MESSAGE = "The list file folder specified in the Options dialog " +
                                                      "contains invalid characters or an absolute path. Go to the Options dialog and " +
                                                      "fix the issue so that you can add the list file to the project.";


        /// <summary>Override this method to define the status query action</summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;

            mc.Enabled = !Package.Z80CodeManager.CompilatioInProgress;
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
            Package.Z80CodeManager.CompilatioInProgress = true;
            Package.DebugInfoProvider.CompiledOutput = null;
            Package.ApplicationObject.ExecuteCommand("File.SaveAll");
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
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected override Task FinallyOnMainThreadAsync()
        {
            Package.Z80CodeManager.CompilatioInProgress = false;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get the path of the item to compile
        /// </summary>
        protected virtual string CompiledItemPath => ItemPath;

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
        /// Compiles the code.
        /// </summary>
        /// <param name="hierarchy">Hierarchy object</param>
        /// <param name="itemId">Identifier of item to compile</param>
        /// <returns>True, if compilation successful; otherwise, false</returns>
        protected virtual bool CompileCode(IVsHierarchy hierarchy, uint itemId)
        {
            if (hierarchy == null) return false;

            if (!(hierarchy is IVsProject project)) return false;
            project.GetMkDocument(itemId, out var itemFullPath);
            var extension = Path.GetExtension(itemFullPath);

            switch (extension?.ToLower())
            {
                case ".z80asm":
                    return CompileZ80Assembly(hierarchy, itemId);
                case ".zxbas":
                    return CompileZxBasic(hierarchy, itemId);
                default:
                    return false;
            }
        }

        #region Z80 Assembly Compilations

        /// <summary>
        /// Override this method to prepare assembler options
        /// </summary>
        /// <returns>Options to use with the assembler</returns>
        protected virtual AssemblerOptions PrepareAssemblerOptions()
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

        protected bool CompileZ80Assembly(IVsHierarchy hierarchy, uint itemId)
        {
            var codeManager = Package.Z80CodeManager;

            // --- Step #1: Compile
            var start = DateTime.Now;
            var pane = OutputWindow.GetPane<Z80BuildOutputPane>();
            pane.WriteLine("Z80 Assembler");
            Output = codeManager.Compile(hierarchy, itemId, PrepareAssemblerOptions());
            var duration = (DateTime.Now - start).TotalMilliseconds;
            pane.WriteLine($"Compile time: {duration}ms");

            if (Output.ErrorCount != 0)
            {
                // --- Compilation completed with errors
                return false;
            }

            // --- Sign the compilation was successful
            Package.DebugInfoProvider.CompiledOutput = Output;

            // --- Create the compilation list file
            CreateCompilationListFile(hierarchy, itemId);
            return true;
        }

        /// <summary>
        /// Collect tasks from comments
        /// </summary>
        protected void HandleAssemblyTasks()
        {
            Package.TaskList.Clear();
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
                    HierarchyItem = Package.Z80CodeManager.CurrentHierarchy,
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
        /// Navigate to the item from the task list
        /// </summary>
        private void TodoTaskOnNavigate(object sender, EventArgs eventArgs)
        {
            if (sender is VsTask task)
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

        /// <summary>
        /// Creates the compilation list file as set up in the package options
        /// </summary>
        /// <param name="hierarchy">Hierarchy object</param>
        /// <param name="itemId">Identifier of item to compile</param>
        private void CreateCompilationListFile(IVsHierarchy hierarchy, uint itemId)
        {
            var options = Package.Options;
            if (!options.GenerateCompilationList
                || options.GenerateForCompileOnly && !(this is CompileZ80CodeCommand))
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
                DiscoveryProject.AddFileToProject(Package.Options.CompilationProjectFolder, fullListFileName,
                    INVALID_FOLDER_MESSAGE, FILE_EXISTS_MESSAGE, false);
            }
        }

        /// <summary>
        /// Creates the contents of the list file from the output information
        /// </summary>
        /// <returns>List file contents</returns>
        private string CreateListFileContents()
        {
            // --- Create a format string from the list template
            var options = Package.Options;
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

        #endregion

        #region Boriel's BASIC Compilations

        protected bool CompileZxBasic(IVsHierarchy hierarchy, uint itemId)
        {
            Output = null;
            return false;
        }

        #endregion
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
