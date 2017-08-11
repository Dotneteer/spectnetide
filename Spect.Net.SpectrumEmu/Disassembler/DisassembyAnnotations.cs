using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class describes a project that is used as an input to the 
    /// disassembly process
    /// </summary>
    public class DisassembyAnnotations
    {
        /// <summary>
        /// Maximum label length
        /// </summary>
        public const int MAX_LABEL_LENGTH = 16;

        private static readonly Regex s_LabelRegex = new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]{0,15}$");
        private Dictionary<ushort, CustomLabel> _customLabels = new Dictionary<ushort, CustomLabel>();
        private Dictionary<ushort, CustomComment> _customComments = new Dictionary<ushort, CustomComment>();

        /// <summary>
        /// Gets the dictionary of custom labels
        /// </summary>
        public IReadOnlyDictionary<ushort, CustomLabel> CustomLabels { get; }

        /// <summary>
        /// Gets the dictionary of comments
        /// </summary>
        public IReadOnlyDictionary<ushort, CustomComment> CustomComments { get; }

        /// <summary>
        /// Gets the sections of memory
        /// </summary>
        public IReadOnlyList<MemorySection> MemorySections { get; }

        public DisassembyAnnotations()
        {
            CustomLabels = new ReadOnlyDictionary<ushort, CustomLabel>(_customLabels);
            CustomComments = new ReadOnlyDictionary<ushort, CustomComment>(_customComments);
            MemorySections = new[]
            {
                new MemorySection(0x0000, 0x3CFF),
                new MemorySection(0x3D00, 0x3FFF, MemorySectionType.ByteArray),
                new MemorySection(0x4000, 0x5AFF, MemorySectionType.Skip),
                new MemorySection(0x5B00, 0x5BFF, MemorySectionType.Skip),
                new MemorySection(0x5C00, 0x5CB5, MemorySectionType.WordArray),
                new MemorySection(0x5CB6, 0xFFFF)
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public DisassembyAnnotations(MemoryMap memoryMap)
        {
            MemorySections = new ReadOnlyCollection<MemorySection>(memoryMap);
            CustomLabels = new ReadOnlyDictionary<ushort, CustomLabel>(_customLabels);
            CustomComments = new ReadOnlyDictionary<ushort, CustomComment>(_customComments);
        }

        /// <summary>
        /// Sets the lookup table for custom labels
        /// </summary>
        /// <param name="customLabels">Custom labels</param>
        public void SetCustomLabels(Dictionary<ushort, CustomLabel> customLabels)
        {
            _customLabels = customLabels ?? throw new ArgumentNullException(nameof(customLabels));
        }

        /// <summary>
        /// Sets the lookup table for custom comments
        /// </summary>
        /// <param name="customComments">Custom comments</param>
        public void SetCustomComments(Dictionary<ushort, CustomComment> customComments)
        {
            _customComments = customComments ?? throw new ArgumentNullException(nameof(customComments));
        }
        /// <summary>
        /// Sets the specified custom label
        /// </summary>
        /// <param name="addr">Address information</param>
        /// <param name="label">Label name</param>
        public void CreateCustomLabel(ushort addr, string label)
        {
            if (label == null) return;
            if (label.Length > MAX_LABEL_LENGTH)
            {
                label = label.Substring(0, MAX_LABEL_LENGTH);
            }
            if (!s_LabelRegex.IsMatch(label)) return;
            if (Z80Disassembler.DisAsmKeywords.Contains(label.ToUpper())) return;
            _customLabels[addr] = new CustomLabel(addr, label); 
        }

        /// <summary>
        /// Gets the comment associated with the specified address
        /// </summary>
        /// <param name="addr">disassembly instruction address</param>
        /// <returns>Comment, if found; otherwise, null</returns>
        public CustomComment GetCommentByAddress(ushort addr)
        {
            _customComments.TryGetValue(addr, out CustomComment comment);
            return comment;
        }

        /// <summary>
        /// Sets the specified custom comment
        /// </summary>
        /// <param name="addr">Address information</param>
        /// <param name="comment">Disassembly comment</param>
        /// <param name="prefixComment">Optional prefix comment</param>
        public void SetComment(ushort addr, string comment, string prefixComment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return;
            _customComments[addr] = new CustomComment(addr, comment, prefixComment);
        }
    }
}