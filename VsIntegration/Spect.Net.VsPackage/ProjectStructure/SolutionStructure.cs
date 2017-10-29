using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Utility;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents the structure of the solution
    /// </summary>
    public class SolutionStructure: Z80HierarchyBase<Solution, DiscoveryProject>
    {
        private readonly IVsSolution _solutionService;

        /// <summary>
        /// Spectrum code discovery projects within the solution
        /// </summary>
        public IReadOnlyList<DiscoveryProject> Projects { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SolutionStructure()
            : base(VsxPackage.GetPackage<SpectNetPackage>().ApplicationObject.DTE.Solution,
                  Package.GetGlobalService(typeof(SVsSolution)) as IVsHierarchy)
        {
            _solutionService = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            Projects = new ReadOnlyCollection<DiscoveryProject>(HierarchyItems);
        }

        /// <summary>
        /// The current Z80 project
        /// </summary>
        public DiscoveryProject CurrentProject => Projects.FirstOrDefault();

        /// <summary>
        /// The project item that represents the current ROM
        /// </summary>
        public RomProjectItem CurrentRomItem => 
            CurrentProject?.RomProjectItems.FirstOrDefault();

        /// <summary>
        /// The project item that represents the current TZX file
        /// </summary>
        public TzxProjectItem CurrentTzxItem =>
            CurrentProject?.TzxProjectItems.FirstOrDefault();

        /// <summary>
        /// The project item that represents the current TAP file
        /// </summary>
        public TapProjectItem CurrentTapItem =>
            CurrentProject?.TapProjectItems.FirstOrDefault();

        /// <summary>
        /// The project item that represents the current annotation file
        /// </summary>
        public AnnotationProjectItem CurrentAnnotationItem =>
            CurrentProject?.AnnotationProjectItems.FirstOrDefault();

        /// <summary>
        /// The ROM information of the current project
        /// </summary>
        public RomInfo RomInfo { get; private set; }

        /// <summary>
        /// Scans the solution for Spectrum Code Discovery projects
        /// </summary>
        public void CollectProjects()
        {
            HierarchyItems.Clear();
            foreach (Project project in Root.Projects)
            {
                if (project.Kind == VsHierarchyTypes.SolutionFolder)
                {
                    Traverse(project);
                }
                else if (project.Kind == VsHierarchyTypes.CodeDiscoveryProject)
                {
                    _solutionService.GetProjectOfUniqueName(project.UniqueName, out var projectHierarchy);
                    HierarchyItems.Add(new DiscoveryProject(project, projectHierarchy));
                }
            }

        }

        /// <summary>
        /// Loads the ROM information for the specified workspace
        /// </summary>
        public void LoadRom()
        {
            var rom = CurrentRomItem;
            if (rom == null) return;

            // ReSharper disable AssignNullToNotNullAttribute
            var romAnnotationFile =
                Path.Combine(
                    Path.GetDirectoryName(rom.Filename),
                    Path.GetFileNameWithoutExtension(rom.Filename)) + ".disann";
            // ReSharper restore AssignNullToNotNullAttribute
            if (!File.Exists(romAnnotationFile)) return;

            // --- Get the contents of the annotation file
            var serialized = File.ReadAllText(romAnnotationFile);
            var annotations = DisassemblyAnnotation.Deserialize(serialized);
            RomInfo = new RomInfo
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
                RomInfo.RomBytes = bytes;
            }

            // --- Read the list of tokens from the ROM
            var tokenPtr = RomInfo.TokenTableAddress;
            tokenPtr++;
            var tokenCount = RomInfo.TokenCount;
            var token = "";
            while (tokenCount > 0)
            {
                var nextChar = RomInfo.RomBytes[tokenPtr++];
                if ((nextChar & 0x80) > 0)
                {
                    token += (char)(nextChar & 0xFF7F);
                    RomInfo.TokenTable.Add(token);
                    tokenCount--;
                    token = "";
                }
                else
                {
                    token += (char)nextChar;
                }
            }
        }

        /// <summary>
        /// Traverses the specified project for ZX Spectrum projects
        /// </summary>
        /// <param name="projectToTraverse"></param>
        private void Traverse(Project projectToTraverse)
        {
            foreach (ProjectItem item in projectToTraverse.ProjectItems)
            {
                var project = item.SubProject;
                if (project == null) continue;

                if (project.Kind == VsHierarchyTypes.SolutionFolder)
                {
                    Traverse(project);
                }
                else if (project.Kind == VsHierarchyTypes.CodeDiscoveryProject)
                {
                    _solutionService.GetProjectOfUniqueName(project.UniqueName, out var projectHierarchy);
                    HierarchyItems.Add(new DiscoveryProject(project, projectHierarchy));
                }
            }
        }
    }
}