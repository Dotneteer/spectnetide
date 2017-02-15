using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spect.Net.Z80Emu.Disasm
{
    /// <summary>
    /// This class stores the labels corresponding to a particular
    /// disassembly project
    /// </summary>
    public class LabelStore
    {
        private readonly Dictionary<string, LabelInfo> _labelsByName = new Dictionary<string, LabelInfo>();
        private readonly Dictionary<ushort, LabelInfo> _labelsByAddr = new Dictionary<ushort, LabelInfo>();
        private readonly Dictionary<ushort, LabelInfo> _customLabels = new Dictionary<ushort, LabelInfo>();

        /// <summary>
        /// Gets the dictionary of labels within the collection
        /// </summary>
        public IReadOnlyDictionary<ushort, LabelInfo> Labels { get; }

        /// <summary>
        /// Gets the dictionary of custom labels
        /// </summary>
        public IReadOnlyDictionary<ushort, LabelInfo> CustomLabels { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public LabelStore()
        {
            Labels = new ReadOnlyDictionary<ushort, LabelInfo>(_labelsByAddr);
            CustomLabels = new ReadOnlyDictionary<ushort, LabelInfo>(_customLabels);
        }

        /// <summary>
        /// Clears the label store
        /// </summary>
        public void Clear()
        {
            _labelsByName.Clear();
            _labelsByAddr.Clear();
        }

        /// <summary>
        /// Gets a label by its address
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <returns>Label information if found; otherwise, null</returns>
        public LabelInfo this[ushort addr]
        {
            get
            {
                LabelInfo label;
                if (!_customLabels.TryGetValue(addr, out label))
                {
                    _labelsByAddr.TryGetValue(addr, out label);
                }
                return label;
            }
        }

        /// <summary>
        /// Gets a label by its name
        /// </summary>
        /// <param name="name">Label name</param>
        /// <returns>Label information if found; otherwise, null</returns>
        public LabelInfo this[string name]
        {
            get
            {
                LabelInfo label;
                _labelsByName.TryGetValue(name, out label);
                return label;
            }
        }

        /// <summary>
        /// Creates a new label according to its address and optional name
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <param name="name">Optional name</param>
        /// <returns>The newly created label</returns>
        public LabelInfo CreateLabel(ushort addr, string name = null)
        {
            var label = new LabelInfo(name ?? $"L{addr:X4}", addr);
            _labelsByAddr[addr] = label;
            _labelsByName[label.Name] = label;
            return label;
        }

        /// <summary>
        /// Updates the name of a specific label
        /// </summary>
        /// <param name="label"></param>
        /// <param name="newName"></param>
        public void UpdateName(LabelInfo label, string newName)
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
            _customLabels[addr] = new LabelInfo(label.Length > 12 ? label.Substring(0, 12) : label, addr);
        }
    }
}