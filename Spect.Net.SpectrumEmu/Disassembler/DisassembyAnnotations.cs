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

        /// <summary>
        /// Regex to check label syntax
        /// </summary>
        public static readonly Regex LabelRegex = new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]{0,15}$");

        private readonly Dictionary<ushort, CustomLabel> _customLabels = new Dictionary<ushort, CustomLabel>();
        private readonly Dictionary<ushort, CustomComment> _customComments = new Dictionary<ushort, CustomComment>();

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
        /// Sets the specified custom label
        /// </summary>
        /// <param name="addr">Address information</param>
        /// <param name="label">Label name</param>
        /// <returns>
        /// True, if the label has been created, modified, or removed;
        /// otherwise; false.
        /// </returns>
        public bool CreateCustomLabel(ushort addr, string label)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return _customLabels.Remove(addr);
            }
            if (label.Length > MAX_LABEL_LENGTH)
            {
                label = label.Substring(0, MAX_LABEL_LENGTH);
            }
            if (!LabelRegex.IsMatch(label)) return false;
            if (Z80Disassembler.DisAsmKeywords.Contains(label.ToUpper())) return false;
            _customLabels[addr] = new CustomLabel(addr, label);
            return true;
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
        /// <returns>
        /// True, if the label has been created, modified, or removed;
        /// otherwise; false.
        /// </returns>
        public bool CreateCustomComment(ushort addr, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                // --- Remove the comment
                if (!_customComments.TryGetValue(addr, out CustomComment oldComment)) return false;

                if (oldComment.PrefixComment == null)
                {
                    // --- Remove the entire comment
                    _customComments.Remove(addr);
                }
                else
                {
                    // --- Remove only the comment part
                    _customComments[addr] = new CustomComment(addr, null, oldComment.PrefixComment);
                }
                return true;
            }

            // --- Set a comment
            if (_customComments.TryGetValue(addr, out CustomComment otherComment))
            {
                _customComments[addr] = new CustomComment(addr, comment, otherComment.PrefixComment);
            }
            else
            {
                _customComments[addr] = new CustomComment(addr, comment);
            }
            return true;
        }

        /// <summary>
        /// Sets the specified custom comment
        /// </summary>
        /// <param name="addr">Address information</param>
        /// <param name="comment">Disassembly comment</param>
        /// <returns>
        /// True, if the label has been created, modified, or removed;
        /// otherwise; false.
        /// </returns>
        public bool CreateCustomPrefixComment(ushort addr, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                // --- Remove the comment
                if (!_customComments.TryGetValue(addr, out CustomComment oldComment)) return false;

                if (oldComment.Comment == null)
                {
                    // --- Remove the entire comment
                    _customComments.Remove(addr);
                }
                else
                {
                    // --- Remove only the prefix comment part
                    _customComments[addr] = new CustomComment(addr, oldComment.Comment);
                }
                return true;
            }

            // --- Set a comment
            if (_customComments.TryGetValue(addr, out CustomComment otherComment))
            {
                _customComments[addr] = new CustomComment(addr, otherComment.Comment, comment);
            }
            else
            {
                _customComments[addr] = new CustomComment(addr, null, comment);
            }
            return true;
        }
    }
}