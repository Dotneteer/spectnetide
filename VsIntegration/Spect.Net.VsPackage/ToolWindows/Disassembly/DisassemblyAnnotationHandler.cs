using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Commands;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// This class is responsible for managing disassembly annotations
    /// </summary>
    public class DisassemblyAnnotationHandler: IDisposable
    {
        /// <summary>
        /// The parent view model
        /// </summary>
        public DisassemblyToolWindowViewModel Parent { get; }

        /// <summary>
        /// Annotations for ROM pages
        /// </summary>
        public List<DisassemblyAnnotation> RomPageAnnotations { get; }

        /// <summary>
        /// Annotations for RAM banks
        /// </summary>
        public Dictionary<int, DisassemblyAnnotation> RamBankAnnotations { get; private set; }

        /// <summary>
        /// The default annotation file to load and save RAM annotations
        /// </summary>
        public AnnotationProjectItem RamAnnotationFile { get; private set; }



        /// <summary>
        /// The annotations that belong to the ROM
        /// </summary>
        public DisassemblyAnnotation RomAnnotations { get; set; }

        /// <summary>
        /// The annotations that belong to the project annotation file
        /// </summary>
        public DisassemblyAnnotation ProjectAnnotations { get; set; }

        /// <summary>
        /// The ROM and project annotations merged
        /// </summary>
        public DisassemblyAnnotation MergedAnnotations { get; set; }

        /// <summary>
        /// Gets the dictionary of labels
        /// </summary>
        public IReadOnlyDictionary<ushort, string> Labels => 
            MergedAnnotations.Labels;

        /// <summary>
        /// Gets the dictionary of comments
        /// </summary>
        public IReadOnlyDictionary<ushort, string> Comments => 
            MergedAnnotations.Comments;

        /// <summary>
        /// Gets the dictionary of prefix comments
        /// </summary>
        public IReadOnlyDictionary<ushort, string> PrefixComments =>
            MergedAnnotations.PrefixComments;

        /// <summary>
        /// Gets the dictionary of literals
        /// </summary>
        public IReadOnlyDictionary<ushort, List<string>> Literals =>
            MergedAnnotations.Literals;

        /// <summary>
        /// Gets the dictionary of literal replacements
        /// </summary>
        public IReadOnlyDictionary<ushort, string> LiteralReplacements =>
            MergedAnnotations.LiteralReplacements;

        /// <summary>
        /// Gets the name of the file that contains ROM annotations
        /// </summary>
        public string RomAnnotationFile { get; }

        /// <summary>
        /// Gets the name of the file that contains custom annotations
        /// </summary>
        /// <remarks>User annotations are always saved to this file.</remarks>
        public string ProjectAnnotationFile { get; }

        /// <summary>
        /// Initializes a new instance of the this class.
        /// </summary>
        /// <param name="parent">Parent view model</param>
        public DisassemblyAnnotationHandler(DisassemblyToolWindowViewModel parent)
        {
            Parent = parent;

            // --- Read ROM annotations
            var spectrumVm = Parent.MachineViewModel.SpectrumVm;
            RomPageAnnotations = new List<DisassemblyAnnotation>();
            var romConfig = spectrumVm.RomConfiguration;
            var roms = romConfig.NumberOfRoms;
            for (var i = 0; i < roms; i++)
            {
                var annData = spectrumVm.RomProvider.LoadRomAnnotations(romConfig.RomName,
                    roms == 1 ? -1 : roms);
                RomPageAnnotations.Add(DisassemblyAnnotation.Deserialize(annData));
            }

            // --- Read the initial RAM annotations
            RamBankAnnotations = new Dictionary<int, DisassemblyAnnotation>();
            Messenger.Default.Register<DefaultAnnotationFileChangedMessage>(this, OnAnnotationFileChanged);
            OnAnnotationFileChanged();
        }

        /// <summary>
        /// Updates the RAM annotation file according to changes
        /// </summary>
        /// <param name="msg"></param>
        private void OnAnnotationFileChanged(DefaultAnnotationFileChangedMessage msg = null)
        {
            var project = VsxPackage.GetPackage<SpectNetPackage>().CodeDiscoverySolution?.CurrentProject;
            RamAnnotationFile = project?.DefaultAnnotationItem
                ?? project?.AnnotationProjectItems?.FirstOrDefault();
            RamBankAnnotations.Clear();
            if (RamAnnotationFile == null) return;

            var disAnn = File.ReadAllText(RamAnnotationFile.Filename);
            RamBankAnnotations = DisassemblyAnnotation.DeserializeBankAnnotations(disAnn);
        }

        /// <summary>
        /// Initializes a new instance of the this class.
        /// </summary>
        /// <param name="parent">Parent view model</param>
        /// <param name="romAnnotationFile">The ROM annotation file</param>
        /// <param name="projectAnnotationFile">The project annotation file</param>
        public DisassemblyAnnotationHandler(DisassemblyToolWindowViewModel parent, string romAnnotationFile, 
            string projectAnnotationFile)
        {
            Parent = parent;
            RomAnnotationFile = romAnnotationFile;
            ProjectAnnotationFile = projectAnnotationFile;
        }

        /// <summary>
        /// Restores the annotations from the ROM annotation and current project
        /// annotation files.
        /// </summary>
        public void RestoreAnnotations()
        {
            if (RomAnnotationFile != null)
            {
                var romSerialized = File.ReadAllText(RomAnnotationFile);
                RomAnnotations = DisassemblyAnnotation.Deserialize(romSerialized);
            }
            if (ProjectAnnotationFile != null)
            {
                var projectSerialized = File.ReadAllText(ProjectAnnotationFile);
                ProjectAnnotations = DisassemblyAnnotation.Deserialize(projectSerialized);
            }
            Remerge();
        }

        /// <summary>
        /// Saves the contents of annotations
        /// </summary>
        public void SaveAnnotations(DisassemblyAnnotation annotation, string filename)
        {
            if (annotation == null || filename == null) return;

            var annotationData = annotation.Serialize();
            File.WriteAllText(filename, annotationData);
        }

        /// <summary>
        /// Stores the label in the annotations
        /// </summary>
        /// <param name="address">Label address</param>
        /// <param name="label">Label text</param>
        /// <param name="validationMessage">Validation message to display</param>
        public void SetLabel(ushort address, string label, out string validationMessage)
        {
            validationMessage = null;
            var target = SelectTarget(address);
            var result = target.Annotation.SetLabel(address, label);
            if (result)
            {
                SaveAnnotations(target.Annotation, target.Filename);
                MergedAnnotations.SetLabel(address, label);
                if (string.IsNullOrWhiteSpace(label))
                {
                    Remerge();
                }
                return;
            }
            validationMessage = "Label name is invalid/duplicated";
        }

        /// <summary>
        /// Stores a comment in annotations
        /// </summary>
        /// <param name="address">Comment address</param>
        /// <param name="comment">Comment text</param>
        public void SetComment(ushort address, string comment)
        {
            var target = SelectTarget(address);
            target.Annotation.SetComment(address, comment);
            SaveAnnotations(target.Annotation, target.Filename);
            MergedAnnotations.SetComment(address, comment);
            if (string.IsNullOrWhiteSpace(comment))
            {
                Remerge();
            }
        }

        /// <summary>
        /// Stores a prefix name in this collection
        /// </summary>
        /// <param name="address">Comment address</param>
        /// <param name="comment">Comment text</param>
        public void SetPrefixComment(ushort address, string comment)
        {
            var target = SelectTarget(address);
            target.Annotation.SetPrefixComment(address, comment);
            SaveAnnotations(target.Annotation, target.Filename);
            MergedAnnotations.SetPrefixComment(address, comment);
            if (string.IsNullOrWhiteSpace(comment))
            {
                Remerge();
            }
        }

        /// <summary>
        /// Stores a section in this collection
        /// </summary>
        /// <param name="startAddress">Start address</param>
        /// <param name="endAddress">End address</param>
        /// <param name="type">Memory section type</param>
        public void AddSection(ushort startAddress, ushort endAddress, MemorySectionType type)
        {
            var target = SelectTarget(startAddress);
            target.Annotation.MemoryMap.Add(new MemorySection(startAddress, endAddress, type));
            target.Annotation.MemoryMap.Normalize();
            SaveAnnotations(target.Annotation, target.Filename);
            MergedAnnotations.MemoryMap.Add(new MemorySection(startAddress, endAddress, type));
            MergedAnnotations.MemoryMap.Normalize();
            Remerge();
        }

        /// <summary>
        /// Replaces a literal in the disassembly item for the specified address. If
        /// the named literal does not exists, creates one for the symbol.
        /// </summary>
        /// <param name="disassAddress">Disassembly item address</param>
        /// <param name="literalName">Literal name</param>
        /// <returns>Null, if operation id ok, otherwise, error message</returns>
        /// <remarks>If the literal already exists, it must have the symbol's value.</remarks>
        public string ApplyLiteral(ushort disassAddress, string literalName)
        {
            if (!Parent.LineIndexes.TryGetValue(disassAddress, out int lineIndex))
            {
                return $"No disassembly line is associated with address #{disassAddress:X4}";
            }

            var disassItem = Parent.DisassemblyItems[lineIndex];
            if (!disassItem.Item.HasSymbol)
            {
                return $"Disassembly line #{disassAddress:X4} does not have an associated value to replace";
            }

            var symbolValue = disassItem.Item.SymbolValue;
            if (disassItem.Item.HasLabelSymbol)
            {
                return
                    $"%L {symbolValue:X4} {literalName}%Disassembly line #{disassAddress:X4} refers to a label. Use the 'L {symbolValue:X4}' command to define a label.";
            }

            var target = SelectTarget(disassAddress);
            var message = target.Annotation.ApplyLiteral(disassAddress, symbolValue, literalName);
            if (message != null) return message;

            SaveAnnotations(target.Annotation, target.Filename);
            MergedAnnotations.ApplyLiteral(disassAddress, symbolValue, literalName);
            if (string.IsNullOrWhiteSpace(literalName))
            {
                Remerge();
            }
            return null;
        }

        /// <summary>
        /// Remerges annotations
        /// </summary>
        private void Remerge()
        {
            MergedAnnotations = new DisassemblyAnnotation();
            if (RomAnnotationFile != null)
            {
                MergedAnnotations.Merge(RomAnnotations);
            }
            if (ProjectAnnotations != null)
            {
                MergedAnnotations.Merge(ProjectAnnotations);
            }
        }

        /// <summary>
        /// Retrieves the target of annotation according to the target address
        /// </summary>
        /// <param name="address">Address affected by the modification</param>
        /// <returns></returns>
        private (DisassemblyAnnotation Annotation, string Filename) SelectTarget(ushort address)
        {
            return address < 0x4000
                ? (RomAnnotations, RomAnnotationFile)
                : (ProjectAnnotations, ProjectAnnotationFile);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Messenger.Default.Unregister<DefaultAnnotationFileChangedMessage>(this);
            Parent?.Dispose();
        }
    }
}