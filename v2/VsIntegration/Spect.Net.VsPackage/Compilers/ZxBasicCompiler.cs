using Spect.Net.Assembler.Assembler;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private class ZxBasicNamespacePreprocessor
        {
            private class NamespaceSegment
            {
                public class ParsedLabel
                {
                    string matchPattern;

                    public string OriginalName { get; private set; }
                    public string NameOnNamespace { get; private set; }

                    public ParsedLabel(string OriginalName, string NameOnNamespace)
                    {
                        this.OriginalName = OriginalName;
                        this.NameOnNamespace = NameOnNamespace;
                        matchPattern = $"(^|[\\s,\\(])({OriginalName})([\\s,:\\)]|$)";
                    }

                    public string ParseSentence(string Sentence)
                    {
                        return Regex.Replace(Sentence, matchPattern, $"$1{NameOnNamespace}$3");
                    }
                }

                List<ParsedLabel> globalLabels = new List<ParsedLabel>();
                Regex labelReg = new Regex("^\\s*?([-_a-zA-Z0-9][-_\\.a-zA-Z0-9]*?)\\s?(:|EQU)", RegexOptions.IgnoreCase);

                public string SegmentName { get; private set; }
                public string SegmentPath { get; private set; }
                public string ParentPath { get; private set; }

                public NamespaceSegment(string ParentPath, string SegmentName)
                {
                    this.SegmentName = SegmentName;
                    this.ParentPath = ParentPath;

                    SegmentPath = (string.IsNullOrWhiteSpace(ParentPath) ? "." : ParentPath) + SegmentName + ".";
                }
                public void ScanSentence(string Sentence, IEnumerable<AssemblerProcedure> ProcStac)
                {
                    var m = labelReg.Match(Sentence);

                    if (m != null && m.Success && !string.IsNullOrWhiteSpace(m.Groups[1].Value))
                    {
                        string labelValue = m.Groups[1].Value;

                        if (ProcStac != null && ProcStac.Any(p => p.IsLocal(labelValue)))
                            return;

                        string fullName = $"{SegmentPath}{m.Groups[1].Value}";
                        if (!globalLabels.Any(l => l.NameOnNamespace == fullName))
                        {
                            ParsedLabel label = new ParsedLabel(m.Groups[1].Value, fullName);
                            globalLabels.Add(label);
                        }

                    }
                }
                public string ParseSentence(string Sentence, IEnumerable<AssemblerProcedure> ProcStac)
                {

                    string parsedSentence = Sentence;

                    foreach (var label in globalLabels)
                    {
                        if (ProcStac == null || !ProcStac.Any(p => p.IsLocal(label.OriginalName)))
                            parsedSentence = label.ParseSentence(parsedSentence);
                    }

                    return parsedSentence;

                }
                public void AddLabel(string Label)
                {
                    ParsedLabel label = new ParsedLabel(Label, $"{SegmentPath}{Label}");
                    globalLabels.Add(label);
                }
            }
            private class AssemblerProcedure
            {
                Regex localLabelReg = new Regex("^\\s*?LOCAL\\s", RegexOptions.IgnoreCase);

                List<string> localLabels = new List<string>();

                public int ProcLineNumber { get; private set; }

                public AssemblerProcedure(int LineNumber)
                {
                    ProcLineNumber = LineNumber;
                }

                public void ScanSentence(string Sentence)
                {
                    Match m = localLabelReg.Match(Sentence);

                    if (m != null && m.Success)
                    {
                        var parts = Sentence.Split(", \r\n\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(1);
                        localLabels.AddRange(parts);
                    }
                }

                public bool IsLocal(string Label)
                {
                    return localLabels.IndexOf(Label) != -1;
                }
            }

            Regex pushReg = new Regex("^\\s*?push namespace ([\\._a-zA-Z0-9]*)", RegexOptions.IgnoreCase);
            Regex popReg = new Regex("^\\s*?pop namespace", RegexOptions.IgnoreCase);
            Regex procReg = new Regex("^\\s*?PROC\\s*$", RegexOptions.IgnoreCase);
            Regex endProcReg = new Regex("^\\s*?ENDP\\s*$", RegexOptions.IgnoreCase);
            Regex forcedNamespacelabelReg = new Regex("^\\s*?(\\.[-_\\.a-zA-Z0-9]*?)\\s?(:|EQU)", RegexOptions.IgnoreCase);

            NamespaceSegment currentSegment;
            Stack<NamespaceSegment> segments = new Stack<NamespaceSegment>();

            AssemblerProcedure currentProc;
            Stack<AssemblerProcedure> procs = new Stack<AssemblerProcedure>();

            List<NamespaceSegment> storedSegments = new List<NamespaceSegment>();
            List<AssemblerProcedure> storedProcs = new List<AssemblerProcedure>();

            string content;

            public ZxBasicNamespacePreprocessor(string FileContent)
            {
                content = FileContent;
            }

            public string ProcessContent()
            {
                storedSegments.Clear();
                storedProcs.Clear();
                segments.Clear();
                procs.Clear();
                currentProc = null;
                currentSegment = null;

                Match m;
                string line;

                StringBuilder sb = new StringBuilder();

                using (StringReader sr = new StringReader(content))
                {
                    int cnt = 0;

                    while ((line = sr.ReadLine()) != null)
                    {
                        cnt++;

                        m = pushReg.Match(line);

                        if (m != null && m.Success && !string.IsNullOrWhiteSpace(m.Groups[1].Value))
                        {
                            PushNamespaceSegment(m.Groups[1].Value);
                            continue;
                        }

                        if (popReg.IsMatch(line))
                        {
                            PopNamespaceSegment();
                            continue;
                        }

                        if (procReg.IsMatch(line))
                        {
                            PushStoreProc(cnt);
                            continue;
                        }

                        if (endProcReg.IsMatch(line))
                        {
                            PopProc();
                            continue;
                        }

                        m = forcedNamespacelabelReg.Match(line);

                        if (m != null && m.Success && !string.IsNullOrWhiteSpace(m.Groups[1].Value))
                        {
                            var pathParts = m.Groups[1].Value.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                            string lastNamespace = pathParts[0];
                            string label = pathParts.Last();
                            string path = "." + string.Join(".", pathParts.Skip(1).Take(pathParts.Length - 2)) + ".";

                            if (path == "..")
                                path = "";

                            var namespaceSegment = storedSegments.Where(ss => ss.SegmentName == lastNamespace && ss.ParentPath == path).FirstOrDefault();

                            if (namespaceSegment == null)
                            {
                                namespaceSegment = new NamespaceSegment(path, lastNamespace);
                                storedSegments.Add(namespaceSegment);
                            }

                            namespaceSegment.AddLabel(label);

                        }

                        if (currentProc != null)
                            currentProc.ScanSentence(line);

                        if (currentSegment != null)
                            currentSegment.ScanSentence(line, procs);

                        if (line.Contains("__PRINT_INIT"))
                            line = line;

                    }
                }

                segments.Clear();
                procs.Clear();

                using (StringReader sr = new StringReader(content))
                {
                    int cnt = 0;

                    while ((line = sr.ReadLine()) != null)
                    {
                        cnt++;

                        m = pushReg.Match(line);

                        if (m != null && m.Success && !string.IsNullOrWhiteSpace(m.Groups[1].Value))
                        {
                            PushNamespaceSegment(m.Groups[1].Value);
                            continue;
                        }

                        if (popReg.IsMatch(line))
                        {
                            PopNamespaceSegment();
                            continue;
                        }

                        if (procReg.IsMatch(line))
                        {
                            PushExistingProc(cnt);
                            sb.AppendLine(line);
                            continue;
                        }

                        if (endProcReg.IsMatch(line))
                        {
                            PopProc();
                            sb.AppendLine(line);
                            continue;
                        }

                        if (currentSegment != null)
                            line = currentSegment.ParseSentence(line, procs);

                        sb.AppendLine(line);

                    }
                }

                return sb.ToString();
            }

            private void PopProc()
            {
                procs.Pop();

                if (procs.Count > 0)
                    currentProc = procs.Peek();
                else
                    currentProc = null;
            }

            private void PushExistingProc(int cnt)
            {
                currentProc = storedProcs.Where(p => p.ProcLineNumber == cnt).FirstOrDefault();
                procs.Push(currentProc);
            }

            private void PushStoreProc(int cnt)
            {
                currentProc = new AssemblerProcedure(cnt);
                procs.Push(currentProc);
                storedProcs.Add(currentProc);
            }

            private void PushNamespaceSegment(string NamespaceSegment)
            {

                currentSegment = storedSegments.Where(s => s.SegmentName == NamespaceSegment && s.ParentPath == (currentSegment?.SegmentPath ?? "")).FirstOrDefault();

                if (currentSegment == null)
                {
                    currentSegment = new NamespaceSegment(currentSegment?.SegmentPath ?? "", NamespaceSegment);
                    storedSegments.Add(currentSegment);
                }

                segments.Push(currentSegment);

            }

            private void PopNamespaceSegment()
            {
                var segment = segments.Pop();

                if (segments.Count > 0)
                    currentSegment = segments.Peek();
                else
                    currentSegment = null;
            }

        }

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

            ZxBasicNamespacePreprocessor preProc = new ZxBasicNamespacePreprocessor(asmContents);

            asmContents = "\t.zxbasic\r\n" + preProc.ProcessContent();
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
