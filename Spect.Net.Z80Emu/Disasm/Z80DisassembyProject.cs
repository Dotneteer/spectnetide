using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spect.Net.Z80Emu.Disasm
{
    /// <summary>
    /// This class describes a project that is used as an input to the 
    /// disassembly process
    /// </summary>
    public class Z80DisassembyProject
    {
        /// <summary>
        /// Maximum label length
        /// </summary>
        public const int MAX_LABEL_LENGTH = 16;

        private static readonly Regex s_LabelRegex = new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]{0,15}$");
        private readonly Dictionary<ushort, DisassemblyLabel> _labels = new Dictionary<ushort, DisassemblyLabel>();
        private readonly Dictionary<ushort, CustomLabel> _customLabels = new Dictionary<ushort, CustomLabel>();
        private readonly Dictionary<ushort, string> _comments = new Dictionary<ushort, string>();
        private readonly List<DisassemblyDataSection> _dataSections = new List<DisassemblyDataSection>();

        /// <summary>
        /// Gets the dictionary of labels within the collection
        /// </summary>
        public IReadOnlyDictionary<ushort, DisassemblyLabel> Labels { get; }

        /// <summary>
        /// Gets the dictionary of custom labels
        /// </summary>
        public IReadOnlyDictionary<ushort, CustomLabel> CustomLabels { get; }

        /// <summary>
        /// Gets the dictionary of comments
        /// </summary>
        public IReadOnlyDictionary<ushort, string> Comments { get; }

        /// <summary>
        /// Gets the list of data sections
        /// </summary>
        public IReadOnlyList<DisassemblyDataSection> DataSections { get; }
        
        /// <summary>
        /// The Z80 binary to disassemble
        /// </summary>
        public byte[] Z80Binary { get; private set; }

        /// <summary>
        /// The index of the first byte that should be disassembled from the
        /// Z80 binary
        /// </summary>
        public int FirstByteIndex { get; set; }

        /// <summary>
        /// The lenght of the section that should be disassembled from the Z80
        /// binary
        /// </summary>
        public int BinaryLength { get; set; }

        /// <summary>
        /// The start offset of the firs byte to disassemble (at index 0) of the Z80 binary
        /// </summary>
        public ushort StartOffset { get; set; }

        /// <summary>
        /// The output of the dissassembly process
        /// </summary>
        public Z80DisassemblyOutput Output { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Z80DisassembyProject()
        {
            Z80Binary = new byte[0];
            StartOffset = 0;
            FirstByteIndex = 0;
            BinaryLength = 0;
            Labels = new ReadOnlyDictionary<ushort, DisassemblyLabel>(_labels);
            CustomLabels = new ReadOnlyDictionary<ushort, CustomLabel>(_customLabels);
            Comments = new ReadOnlyDictionary<ushort, string>(_comments);
            DataSections = new ReadOnlyCollection<DisassemblyDataSection>(_dataSections);
            Output = new Z80DisassemblyOutput();
        }

        /// <summary>
        /// Sets the Z80 binary to be used within the project
        /// </summary>
        /// <param name="binary">Binary information</param>
        /// <param name="firstByteIndex">Index of the first byte; null means 0</param>
        /// <param name="binaryLenght">Binary length; null means till the end of the binary</param>
        public void SetZ80Binary(byte[] binary, int? firstByteIndex = null, int? binaryLenght = null)
        {
            if (binary == null)
            {
                throw new ArgumentNullException(nameof(binary));
            }
            if (firstByteIndex == null)
            {
                firstByteIndex = 0;
            }
            if (firstByteIndex < 0)
            {
                throw new IndexOutOfRangeException("FirstByteIndex cannot be less than zero");
            }
            if (binaryLenght == null)
            {
                binaryLenght = binary.Length - firstByteIndex.Value;
            }
            if (firstByteIndex + binaryLenght > binary.Length)
            {
                throw new IndexOutOfRangeException("FirstByteIndex + BinaryLenght cannot exceed the lenght og the binary");
            }

            Z80Binary = binary;
            FirstByteIndex = firstByteIndex.Value;
            BinaryLength = binaryLenght.Value;
        }

        /// <summary>
        /// Carries out the disassembling process
        /// </summary>
        public void Disassemble()
        {
            Output.Clear();
            var disassembler = new Z80Disassembler(this);
            disassembler.Disassemble(Output);
        }

        /// <summary>
        /// Removes the symbols associated with this project
        /// </summary>
        public void ClearAnnotations()
        {
            _labels.Clear();
            _customLabels.Clear();
            _comments.Clear();
            _dataSections.Clear();
        }

        /// <summary>
        /// Gets a label by its address
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <returns>Label information if found; otherwise, null</returns>
        public string GetLabelNameByAddress(ushort addr)
        {
            CustomLabel disassemblyLabel;
            return _customLabels.TryGetValue(addr, out disassemblyLabel) 
                ? disassemblyLabel.Name 
                : $"L{addr:X4}";
        }

        /// <summary>
        /// Creates a new label according to its address and optional name
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <param name="referringOpAddr">
        /// The address of operation referring to the label
        /// </param>
        /// <returns>The newly created label</returns>
        public string CollectLabel(ushort addr, ushort? referringOpAddr)
        {
            DisassemblyLabel label;
            if (!_labels.TryGetValue(addr, out label))
            {
                label = new DisassemblyLabel(addr);
                _labels.Add(label.Address, label);
            }
            if (referringOpAddr.HasValue)
            {
                label.References.Add(referringOpAddr.Value);
            }
            return GetLabelNameByAddress(addr);
        }

        /// <summary>
        /// Sets the specified custom label
        /// </summary>
        /// <param name="addr">Address information</param>
        /// <param name="label">Label name</param>
        public void SetCustomLabel(ushort addr, string label)
        {
            if (label == null) return;
            if (label.Length > MAX_LABEL_LENGTH)
            {
                label = label.Substring(0, MAX_LABEL_LENGTH);
            }
            if (!s_LabelRegex.IsMatch(label)) return;
            if (Z80Disassembler.DisAsmKeywords.Contains(label.ToUpper())) return;

            CollectLabel(addr, null);
            _customLabels[addr] = new CustomLabel(addr, label); 
        }

        /// <summary>
        /// Gets the comment associated with the specified address
        /// </summary>
        /// <param name="addr">disassembly instruction address</param>
        /// <returns>Comment, if found; otherwise, null</returns>
        public string GetCommentByAddress(ushort addr)
        {
            string comment;
            _comments.TryGetValue(addr, out comment);
            return comment;
        }

        /// <summary>
        /// Sets the specified custom comment
        /// </summary>
        /// <param name="addr">Address information</param>
        /// <param name="comment">Disassembly comment</param>
        public void SetComment(ushort addr, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return;
            _comments[addr] = comment;
        }

        /// <summary>
        /// Removes the specified data section
        /// </summary>
        /// <param name="section">The section to remove</param>
        /// <returns>True, if the data section has been removed</returns>
        /// <remarks>
        /// After removing the data section, you may disassembly the project
        /// again to reflect the changes. 
        /// </remarks>
        public bool RemoveDataSection(DisassemblyDataSection section)
        {
            return _dataSections.Remove(section);
        }

        /// <summary>
        /// Adds a new data section to the list of existing one.
        /// </summary>
        /// <param name="section">New data section to add</param>
        /// <returns>
        /// True, if the new section has overlapped with another section; otherwise, false
        /// </returns>
        /// <remarks>
        /// If the new data section overlaps another section, the sections are split 
        /// accordingly. This method assumes that there are no overlapping data 
        /// sections yet.
        /// </remarks>
        public bool AddDataSection(DisassemblyDataSection section)
        {
            var overlappingSections = _dataSections.Where(section.Intersects).ToList();

            if (overlappingSections.Count == 0)
            {
                // --- Simple case: no other overlapping data section
                _dataSections.Add(section);
                return false;
            }

            // --- Let's go through each overlapping section
            foreach (var otherSection in overlappingSections)
            {
                // --- Because of the overlap, the other section should be removed.
                RemoveDataSection(otherSection);
                if (section.ContainsSection(otherSection)) 
                {
                    // --- The other section can be entirely removed
                    continue;
                }
                if (otherSection.FromAddr < section.FromAddr)
                {
                    // --- The other section has a fraction that preceeds the 
                    // --- new section
                    _dataSections.Add(new DisassemblyDataSection(
                        otherSection.FromAddr, 
                        (ushort)(section.FromAddr - 1), 
                        otherSection.SectionType));
                }
                if (otherSection.ToAddr > section.ToAddr)
                {
                    // --- The other section has a fraction that tails the
                    // --- new section
                    _dataSections.Add(new DisassemblyDataSection(
                        (ushort)(section.ToAddr + 1),
                        otherSection.ToAddr, 
                        otherSection.SectionType));
                }
            }

            _dataSections.Add(section);
            return true;
        }
    }
}