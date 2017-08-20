using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Machine;

// ReSharper disable AssignNullToNotNullAttribute

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class provides information about the current workspace
    /// in which a Spectrum Code Discovery project runs
    /// </summary>
    public class WorkspaceInfo
    {
        /// <summary>
        /// The current Spectrum Code Discovery project to run
        /// </summary>
        public DiscoveryProject CurrentProject { get; set; }

        /// <summary>
        /// The Rom to use when running the Spectrum virtual machine
        /// </summary>
        public RomProjectItem RomItem { get; set; }

        /// <summary>
        /// The Tzx file to play when loading
        /// </summary>
        public TzxProjectItem TzxItem { get; set; }

        /// <summary>
        /// The annotation to use with the current project
        /// </summary>
        public AnnotationProjectItem AnnotationItem { get; set; }

        /// <summary>
        /// Indicates if the virtual machine should start with a specific
        /// .vmstate file (VmState property)
        /// </summary>
        public bool StartWithVmState { get; set; }

        /// <summary>
        /// The virtual machine state file to use when starting the machine
        /// </summary>
        public VmStateProjectItem VmState { get; set; }

        /// <summary>
        /// The current ROm to be used within this project
        /// </summary>
        public RomInfo RomInfo { get; set; }

        /// <summary>
        /// Creates workspace info from the specified solution
        /// </summary>
        /// <param name="solution">Solution to build the workspace from</param>
        /// <returns>Workspace info</returns>
        public static WorkspaceInfo CreateFromSolution(SolutionStructure solution)
        {
            var result = new WorkspaceInfo();
            var currentProject = solution.Projects.FirstOrDefault();
            if ((result.CurrentProject = currentProject) == null)
            {
                return null;
            }

            // --- Set project items
            result.RomItem = currentProject.RomProjectItems.FirstOrDefault();
            result.TzxItem = currentProject.TzxProjectItems.FirstOrDefault();
            result.AnnotationItem = currentProject.AnnotationProjectItems
                .FirstOrDefault(i => Path.GetFileName(i.Filename)?.ToLower() == "annotations.disann");
            result.VmState = currentProject.VmStateProjectItems.FirstOrDefault();
            result.StartWithVmState = false;

            LoadRom(result);
            return result;
        }

        private static void LoadRom(WorkspaceInfo workspace)
        {
            var rom = workspace.RomItem;
            if (rom == null) return;

            var romAnnotationFile =
                Path.Combine(
                    Path.GetDirectoryName(rom.Filename),
                    Path.GetFileNameWithoutExtension(rom.Filename)) + ".disann";
            if (!File.Exists(romAnnotationFile)) return;

            // --- Get the contents of the annotation file
            var serialized = File.ReadAllText(romAnnotationFile);
            var annotations = DisassemblyAnnotation.Deserialize(serialized);
            workspace.RomInfo = new RomInfo
            {
                MemorySections = new List<MemorySection>(annotations.MemoryMap),
                LoadBytesInvalidHeaderAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$LoadBytesInvalidHeaderAddress")).Key,
                LoadBytesResumeAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$LoadBytesResumeAddress")).Key,
                LoadBytesRoutineAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$LoadBytesRoutineAddress")).Key,
                SaveBytesRoutineAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$SaveBytesRoutineAddress")).Key,
                SaveBytesResumeAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$SaveBytesResumeAddress")).Key,
                TokenTableAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$TokenTableAddress")).Key,
                TokenCount = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$TokenCount")).Key,
                TokenOffset = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$TokenOffset")).Key,
                TokenTable = new List<string>()
            };

            // --- Get the content of the ROM file
            using (var stream = new StreamReader(rom.Filename).BaseStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                workspace.RomInfo.RomBytes = bytes;
            }

            // --- Read the list of tokens from the ROM
            ReadTokenTable(workspace.RomInfo);
        }

        /// <summary>
        /// Reads the table of tokens from the ROM
        /// </summary>
        private static void ReadTokenTable(RomInfo info)
        {
            var tokenPtr = info.TokenTableAddress;
            tokenPtr++;
            var tokenCount = info.TokenCount;
            var token = "";
            while (tokenCount > 0)
            {
                var nextChar = info.RomBytes[tokenPtr++];
                if ((nextChar & 0x80) > 0)
                {
                    token += (char)(nextChar & 0xFF7F);
                    info.TokenTable.Add(token);
                    tokenCount--;
                    token = "";
                }
                else
                {
                    token += (char)nextChar;
                }
            }
        }
    }
}