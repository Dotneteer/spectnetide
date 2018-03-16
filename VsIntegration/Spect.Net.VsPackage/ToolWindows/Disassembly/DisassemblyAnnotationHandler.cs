using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// This class is responsible for managing disassembly annotations
    /// </summary>
    public class DisassemblyAnnotationHandler: IDisposable
    {
        /// <summary>
        /// Maximum label length
        /// </summary>
        public const int MAX_LABEL_LENGTH = 16;

        /// <summary>
        /// Regex to check label syntax
        /// </summary>
        public static readonly Regex LabelRegex = new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]{0,15}$");

        /// <summary>
        /// The parent view model
        /// </summary>
        public DisassemblyToolWindowViewModel Parent { get; }

        /// <summary>
        /// Annotations for ROM pages
        /// </summary>
        public Dictionary<int, DisassemblyAnnotation> RomPageAnnotations { get; }

        /// <summary>
        /// ROM annotation files
        /// </summary>
        public Dictionary<int, string> RomAnnotationFiles { get; }

        /// <summary>
        /// Annotations for RAM banks
        /// </summary>
        public Dictionary<int, DisassemblyAnnotation> RamBankAnnotations { get; private set; }

        /// <summary>
        /// The file to save RAM annotations to
        /// </summary>
        public string RamBankAnnotationFile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the this class.
        /// </summary>
        /// <param name="parent">Parent view model</param>
        public DisassemblyAnnotationHandler(DisassemblyToolWindowViewModel parent)
        {
            Parent = parent;

            // --- Read ROM annotations
            var spectrumVm = parent.MachineViewModel.SpectrumVm;
            RomPageAnnotations = new Dictionary<int, DisassemblyAnnotation>();
            RomAnnotationFiles = new Dictionary<int, string>();
            var romConfig = spectrumVm.RomConfiguration;
            var roms = romConfig.NumberOfRoms;
            for (var i = 0; i < roms; i++)
            {
                var annFile = spectrumVm.RomProvider.GetAnnotationResourceName(romConfig.RomName,
                    roms == 1 ? -1 : i);
                var annData = spectrumVm.RomProvider.LoadRomAnnotations(romConfig.RomName,
                    roms == 1 ? -1 : i);

                DisassemblyAnnotation.Deserialize(annData, out var annotation);
                RomPageAnnotations.Add(i, annotation);
                RomAnnotationFiles.Add(i, annFile);
            }

            // --- Read the initial RAM annotations
            RamBankAnnotations = new Dictionary<int, DisassemblyAnnotation>();
            parent.Package.CodeManager.AnnotationFileChanged += OnAnnotationFileChanged;
            OnAnnotationFileChanged(null, EventArgs.Empty);

            // --- Register Disassembly providers to use
            if (RomPageAnnotations.TryGetValue(romConfig.Spectrum48RomIndex, out var spectrumRomAnn))
            {
                Z80Disassembler.SetProvider<ISpectrum48RomLabelProvider>(
                    new Spectrum48RomLabelProvider(spectrumRomAnn));
            }
        }

        /// <summary>
        /// Updates the RAM annotation file according to changes
        /// </summary>
        private void OnAnnotationFileChanged(object sender, EventArgs eventArgs)
        {
            var project = Parent.Package.CodeDiscoverySolution?.CurrentProject;
            var annFile = project?.DefaultAnnotationItem
                          ?? project?.AnnotationProjectItems?.FirstOrDefault();
            RamBankAnnotations.Clear();
            if (annFile == null) return;

            RamBankAnnotationFile = annFile.Filename;
            var disAnn = File.ReadAllText(annFile.Filename);
            DisassemblyAnnotation.DeserializeBankAnnotations(disAnn, out var annotations);
            RamBankAnnotations = annotations;
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
            if (!string.IsNullOrWhiteSpace(label))
            {
                if (LabelDefinedInOtherBank(label))
                {
                    validationMessage = "Label name is duplicated";
                    return;
                }
            }
            var annotation = Parent.GetAnnotationFor(address, out var annAddr);
            var result = annotation.SetLabel(annAddr, label);
            if (result)
            {
                SaveAnnotations(annotation, address);
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
            var annotation = Parent.GetAnnotationFor(address, out var annAddr);
            annotation.SetComment(annAddr, comment);
            SaveAnnotations(annotation, address);
        }

        /// <summary>
        /// Stores a prefix name in this collection
        /// </summary>
        /// <param name="address">Comment address</param>
        /// <param name="comment">Comment text</param>
        public void SetPrefixComment(ushort address, string comment)
        {
            var annotation = Parent.GetAnnotationFor(address, out var annAddr);
            annotation.SetPrefixComment(annAddr, comment);
            SaveAnnotations(annotation, address);
        }

        /// <summary>
        /// Stores a section in this collection
        /// </summary>
        /// <param name="startAddress">Start address</param>
        /// <param name="endAddress">End address</param>
        /// <param name="type">Memory section type</param>
        public void AddSection(ushort startAddress, ushort endAddress, MemorySectionType type)
        {
            var startAnn = Parent.GetAnnotationFor(startAddress, out var start);
            var endAnn = Parent.GetAnnotationFor(endAddress, out var end);
            if (startAnn == endAnn)
            {
                // --- The section is within one bank
                var tempSection = new MemorySection(start, end, type);
                startAnn.MemoryMap.Add(tempSection);
                startAnn.MemoryMap.Normalize();
                SaveAnnotations(startAnn, startAddress);
            }
            else
            {
                // --- The section overlaps multiple banks
                // --- We must be in FullViewMode to get here
                var origSection = new MemorySection(startAddress, endAddress, type);
                for (var bank = 0; bank <= 3; bank++)
                {
                    var bankSection = new MemorySection((ushort)(bank*0x4000), (ushort)(bank*0x4000 + 0x3FFF));
                    if (origSection.Overlaps(bankSection))
                    {
                        // --- There is a memory section for this bank
                        var cutSection = origSection.Intersect(bankSection);
                        var bankAnn = Parent.GetAnnotationFor(cutSection.StartAddress, out var cutStart);
                        Parent.GetAnnotationFor(cutSection.EndAddress, out var cutEnd);
                        bankAnn.MemoryMap.Add(new MemorySection(cutStart, cutEnd, type));
                        bankAnn.MemoryMap.Normalize();
                        SaveAnnotations(bankAnn, startAddress);
                    }
                }
            }
        }

        /// <summary>
        /// Replaces a literal in the disassembly item for the specified address. If
        /// the named literal does not exists, creates one for the symbol.
        /// </summary>
        /// <param name="address">Disassembly item address</param>
        /// <param name="literalName">Literal name</param>
        /// <returns>Null, if operation id ok, otherwise, error message</returns>
        /// <remarks>If the literal already exists, it must have the symbol's value.</remarks>
        public string ApplyLiteral(ushort address, string literalName)
        {
            if (!Parent.LineIndexes.TryGetValue(address, out int lineIndex))
            {
                return $"No disassembly line is associated with address #{address:X4}";
            }

            var disassItem = Parent.DisassemblyItems[lineIndex];
            if (!disassItem.Item.HasSymbol)
            {
                return $"Disassembly line #{address:X4} does not have an associated value to replace";
            }

            var symbolValue = disassItem.Item.SymbolValue;
            if (disassItem.Item.HasLabelSymbol)
            {
                return
                    $"%L {symbolValue:X4} {literalName}%Disassembly line #{address:X4} refers to a label. Use the 'L {symbolValue:X4}' command to define a label.";
            }

            var annotation = Parent.GetAnnotationFor(address, out _);
            var message = annotation.ApplyLiteral(address, symbolValue, literalName);
            if (message != null) return message;

            SaveAnnotations(annotation, address);
            return null;
        }

        /// <summary>
        /// Saves the annotation file for the specified address
        /// </summary>
        /// <param name="annotation">Annotation to save</param>
        /// <param name="address"></param>
        public void SaveAnnotations(DisassemblyAnnotation annotation, ushort address)
        {
            string filename;
            var isRom = false;
            var spectrumVm = Parent.MachineViewModel.SpectrumVm;
            if (Parent.FullViewMode)
            {
                var memDevice = spectrumVm.MemoryDevice;
                var locationInfo = memDevice.GetAddressLocation(address);
                if (locationInfo.IsInRom)
                {
                    filename = RomAnnotationFiles.TryGetValue(locationInfo.Index, out var romFile)
                        ? romFile : null;
                    isRom = true;
                }
                else
                {
                    filename = RamBankAnnotationFile;
                }
            }
            else if (Parent.RomViewMode)
            {
                filename = RomAnnotationFiles.TryGetValue(Parent.RomIndex, out var romFile)
                    ? romFile : null;
                isRom = true;
            }
            else
            {
                filename = RamBankAnnotationFile;
            }
            if (filename == null) return;

            var annotationData = isRom ? annotation.Serialize() : SerializeRamBankAnnotations();
            File.WriteAllText(filename, annotationData);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            SpectNetPackage.Default.CodeManager.AnnotationFileChanged -= OnAnnotationFileChanged;
            Parent?.Dispose();
        }

        #region Helper methods

        /// <summary>
        /// Serializes RAM bank annotations
        /// </summary>
        /// <returns></returns>
        private string SerializeRamBankAnnotations()
        {
            var annData = RamBankAnnotations.ToDictionary(k => k.Key, 
                v => v.Value.ToDisassemblyDecorationData());
            return JsonConvert.SerializeObject(annData, Formatting.Indented);
        }

        /// <summary>
        /// Checks if the specified label is already defined
        /// </summary>
        /// <param name="label">Label to check</param>
        /// <returns>True, if label is already defined; otherwise, false</returns>
        private bool LabelDefinedInOtherBank(string label)
        {
            var memoryDevice = Parent.MachineViewModel.SpectrumVm.MemoryDevice;
            if (Parent.RomViewMode)
            {
                // --- The label is allowed in another ROM, but not in the current one
                return false;
            }
            if (Parent.RamBankViewMode || Parent.FullViewMode)
            {
                var contains = RamBankAnnotations.Values.Any(ann => ann.Labels.Values.Any(
                    l => string.Compare(l, label, StringComparison.OrdinalIgnoreCase) == 0));
                if (contains) return true;
            }
            if (!Parent.FullViewMode) return false;
            return RomPageAnnotations.TryGetValue(memoryDevice.GetSelectedRomIndex(), out var romAnn) &&
                romAnn.Labels.Values.Any(l => string.Compare(l, label,
                    StringComparison.OrdinalIgnoreCase) == 0);
        }

        #endregion
    }
}