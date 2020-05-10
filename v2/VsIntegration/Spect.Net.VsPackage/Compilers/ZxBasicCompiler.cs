using Spect.Net.Assembler.Assembler;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Output;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using OutputWindow = Spect.Net.VsPackage.VsxLibrary.Output.OutputWindow;

namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This class represents the ZX BASIC compiler
    /// </summary>
    public class ZxBasicCompiler : ICompilerService
    {
        private const string ZXB_NOT_FOUND_MESSAGE =
            "SpectNetIDE cannot run ZXB.EXE ({0}). Please check that you specified the " +
            "correct path in the Spect.Net IDE options page (ZXB utility path) or added it " +
            "to the PATH evnironment variable.\nFor more details, check this article: " +
            SETUP_URL +
            "\n\nWhen you click OK, SpectNetIDE opens this link for you.";

        private const string SETUP_URL = "https://dotneteer.github.io/spectnetide/getting-started/setup-zx-basic";

        private const string ZXBASIC_TEMP_FOLDER = "ZxBasic";

        /// <summary>
        /// Tests if the compiler is available.
        /// </summary>
        /// <returns>True, if the compiler is installed, and so available.</returns>
        public async Task<bool> IsAvailable()
        {
            var runner = new ZxbRunner(SpectNetPackage.Default.Options.ZxbPath);
            try
            {
                await runner.RunAsync(new ZxbOptions());
            }
            catch (Exception ex)
            {
                VsxDialogs.Show(string.Format(ZXB_NOT_FOUND_MESSAGE, ex.Message),
                    "Error when running ZXB", MessageBoxButton.OK, VsxMessageBoxIcon.Exclamation);
                System.Diagnostics.Process.Start(SETUP_URL);
                return false;
            }
            return true;
        }

        /// <summary>
        /// The name of the service
        /// </summary>
        public string ServiceName => "ZX BASIC Compiler";

        private EventHandler<AssemblerMessageArgs> _traceMessageHandler;

        /// <summary>
        /// Gets the handler that displays trace messages
        /// </summary>
        /// <returns>Trace message handler</returns>
        public EventHandler<AssemblerMessageArgs> GetTraceMessageHandler()
        {
            return _traceMessageHandler;
        }

        /// <summary>
        /// Sets the handler that displays trace messages
        /// </summary>
        /// <param name="messageHandler">Message handler to use</param>
        public void SetTraceMessageHandler(EventHandler<AssemblerMessageArgs> messageHandler)
        {
            _traceMessageHandler = messageHandler;
        }

        /// <summary>
        /// Compiles the specified Visua Studio document.
        /// </summary>
        /// <param name="itemPath">VS document item path</param>
        /// <param name="options">Assembler options to use</param>
        /// <returns>True, if compilation is successful; otherwise, false</returns>
        public async Task<AssemblerOutput> CompileDocument(string itemPath,
            AssemblerOptions options)
        {
            var addToProject = SpectNetPackage.Default.Options.StoreGeneratedZ80Files;
            var zxbOptions = PrepareZxbOptions(itemPath, addToProject);
            var output = new AssemblerOutput(new SourceFileItem(itemPath), options?.UseCaseSensitiveSymbols ?? false);
            var runner = new ZxbRunner(SpectNetPackage.Default.Options.ZxbPath);
            ZxbResult result;
            try
            {
                result = await runner.RunAsync(zxbOptions, true);
            }
            catch (Exception ex)
            {
                output.Errors.Clear();
                output.Errors.Add(new AssemblerErrorInfo("ZXB", "", 1, 0, ex.Message));
                return output;
            }
            if (result.ExitCode != 0)
            {
                // --- Compile error - stop here
                output.Errors.Clear();
                output.Errors.AddRange(result.Errors);
                return output;
            }

            // --- Add the generated file to the project
            if (addToProject)
            {
                var zxBasItem =
                    SpectNetPackage.Default.ActiveProject.ZxBasicProjectItems.FirstOrDefault(pi =>
                        pi.Filename == itemPath)?.DteProjectItem;
                if (SpectNetPackage.Default.Options.NestGeneratedZ80Files && zxBasItem != null)
                {
                    var newItem = zxBasItem.ProjectItems.AddFromFile(zxbOptions.OutputFilename);
                    newItem.Properties.Item("DependentUpon").Value = zxBasItem.Name;
                }
                else
                {
                    SpectNetPackage.Default.ActiveRoot.ProjectItems.AddFromFile(zxbOptions.OutputFilename);
                }
            }

            var asmContents = File.ReadAllText(zxbOptions.OutputFilename);
            asmContents = "\t.zxbasic\r\n" + asmContents;
            var hasHeapSizeLabel = Regex.Match(asmContents, "ZXBASIC_HEAP_SIZE\\s+EQU");
            if (!hasHeapSizeLabel.Success)
            {
                // --- HACK: Take care that "ZXBASIC_HEAP_SIZE EQU" is added to the assembly file
                asmContents = Regex.Replace(asmContents, "ZXBASIC_USER_DATA_END\\s+EQU\\s+ZXBASIC_MEM_HEAP",
                    "ZXBASIC_USER_DATA_END EQU ZXBASIC_USER_DATA");
            }
            File.WriteAllText(zxbOptions.OutputFilename, asmContents);

            // --- Second pass, compile the assembly file
            var compiler = new Z80Assembler();
            if (_traceMessageHandler != null)
            {
                compiler.AssemblerMessageCreated += _traceMessageHandler;
            }
            compiler.AssemblerMessageCreated += OnAssemblerMessage;
            try
            {
                output = compiler.CompileFile(zxbOptions.OutputFilename, options);
                output.ModelType = SpectrumModelType.Spectrum48;
            }
            finally
            {
                if (_traceMessageHandler != null)
                {
                    compiler.AssemblerMessageCreated -= _traceMessageHandler;
                }
                compiler.AssemblerMessageCreated -= OnAssemblerMessage;
            }
            return output;
        }

        /// <summary>
        /// Responds to the event when the Z80 assembler releases a message
        /// </summary>
        private void OnAssemblerMessage(object sender, AssemblerMessageArgs e)
        {
            var pane = OutputWindow.GetPane<Z80AssemblerOutputPane>();
            pane.WriteLine(e.Message);
        }

        /// <summary>
        /// Prepares the ZXB options to run
        /// </summary>
        /// <returns>Options to use when compiling ZX BASIC project</returns>
        private ZxbOptions PrepareZxbOptions(string documentPath, bool addToProject)
        {
            // --- Try to find options declaration in the source file
            var contents = File.ReadAllText(documentPath);
            var commentRegExp = new Regex("\\s*(rem|REM)\\s*(@options|@OPTIONS)\\s*(.*)");
            var match = commentRegExp.Match(contents);
            var rawArgs = match.Success ? match.Groups[3].Value : null;

            var outputBase = addToProject
                ? documentPath
                : Path.Combine(SpectNetPackage.Default.Solution.SolutionDir,
                    SolutionStructure.PRIVATE_FOLDER,
                    ZXBASIC_TEMP_FOLDER,
                    Path.GetFileName(documentPath));
            var outDir = Path.GetDirectoryName(outputBase);
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            var outputFile = Path.ChangeExtension(outputBase, ".zxbas.z80asm");

            var packageOptions = SpectNetPackage.Default.Options;
            var options = new ZxbOptions
            {
                RawArgs = rawArgs,
                ProgramFilename = documentPath,
                OutputFilename = outputFile,
                Optimize = packageOptions.Optimize,
                OrgValue = packageOptions.OrgValue,
                ArrayBaseOne = packageOptions.ArrayBaseOne,
                StringBaseOne = packageOptions.StringBaseOne,
                HeapSize = packageOptions.HeapSize,
                DebugMemory = packageOptions.DebugMemory,
                DebugArray = packageOptions.DebugArray,
                StrictBool = packageOptions.StrictBool,
                EnableBreak = packageOptions.EnableBreak,
                ExplicitDim = packageOptions.ExplicitDim,
                StrictTypes = packageOptions.StrictTypes
            };
            return options;
        }
    }
}
