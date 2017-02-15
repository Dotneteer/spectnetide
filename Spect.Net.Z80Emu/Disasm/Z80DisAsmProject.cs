using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spect.Net.Z80Emu.Disasm
{
    /// <summary>
    /// This class describes a project that is used as an input to the 
    /// disassembly process
    /// </summary>
    public class Z80DisAsmProject
    {
        private readonly Dictionary<string, DisassemblyLabel> _labelsByName = new Dictionary<string, DisassemblyLabel>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<ushort, DisassemblyLabel> _labelsByAddr = new Dictionary<ushort, DisassemblyLabel>();
        private readonly Dictionary<ushort, DisassemblyLabel> _customLabels = new Dictionary<ushort, DisassemblyLabel>();
        private readonly Dictionary<ushort, string> _comments = new Dictionary<ushort, string>();

        /// <summary>
        /// Gets the dictionary of labels within the collection
        /// </summary>
        public IReadOnlyDictionary<ushort, DisassemblyLabel> Labels { get; }

        /// <summary>
        /// Gets the dictionary of custom labels
        /// </summary>
        public IReadOnlyDictionary<ushort, DisassemblyLabel> CustomLabels { get; }

        /// <summary>
        /// Gets the dictionary of comments
        /// </summary>
        public IReadOnlyDictionary<ushort, string> Comments { get; }

        /// <summary>
        /// The Z80 binary to disassemble
        /// </summary>
        public byte[] Z80Binary { get; set; }
        public ushort StartOffset { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Z80DisAsmProject()
        {
            Z80Binary = new byte[0];
            StartOffset = 0;
            Labels = new ReadOnlyDictionary<ushort, DisassemblyLabel>(_labelsByAddr);
            CustomLabels = new ReadOnlyDictionary<ushort, DisassemblyLabel>(_customLabels);
            Comments = new ReadOnlyDictionary<ushort, string>(_comments);
        }

        /// <summary>
        /// Removes the symbols associated with this project
        /// </summary>
        public void ClearAnnotations()
        {
            _labelsByName.Clear();
            _labelsByAddr.Clear();
            _customLabels.Clear();
        }

        /// <summary>
        /// Gets a label by its name
        /// </summary>
        /// <param name="name">Label name</param>
        /// <returns>Label information if found; otherwise, null</returns>
        public DisassemblyLabel GetLabelByName(string name)
        {
            DisassemblyLabel disassemblyLabel;
            _labelsByName.TryGetValue(name, out disassemblyLabel);
            return disassemblyLabel;
        }

        /// <summary>
        /// Gets a label by its address
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <returns>Label information if found; otherwise, null</returns>
        public DisassemblyLabel GetLabelByAddress(ushort addr)
        {
            DisassemblyLabel disassemblyLabel;
            if (!_customLabels.TryGetValue(addr, out disassemblyLabel))
            {
                _labelsByAddr.TryGetValue(addr, out disassemblyLabel);
            }
            return disassemblyLabel;
        }

        /// <summary>
        /// Creates a new label according to its address and optional name
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <param name="name">Optional name</param>
        /// <returns>The newly created label</returns>
        public DisassemblyLabel CreateLabel(ushort addr, string name = null)
        {
            var label = new DisassemblyLabel(name ?? $"L{addr:X4}", addr);
            _labelsByAddr[addr] = label;
            _labelsByName[label.Name] = label;
            return label;
        }

        /// <summary>
        /// Updates the name of a specific label
        /// </summary>
        /// <param name="disassemblyLabel">Label to update </param>
        /// <param name="newName">New label name</param>
        public void UpdateLabelName(DisassemblyLabel disassemblyLabel, string newName)
        {
        }

        /// <summary>
        /// Sets the specified custom label
        /// </summary>
        /// <param name="addr">Address information</param>
        /// <param name="label">Label name</param>
        public void SetCustomLabel(ushort addr, string label)
        {
            if (string.IsNullOrEmpty(label)) return;
            _customLabels[addr] = new DisassemblyLabel(label.Length > 12 ? label.Substring(0, 12) : label, addr);
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
        public void SetCustomComment(ushort addr, string comment)
        {
            if (string.IsNullOrEmpty(comment)) return;
            _comments[addr] = comment;
        }
    }
}