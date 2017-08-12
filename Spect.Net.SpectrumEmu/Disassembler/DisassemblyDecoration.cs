using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class describes those labels, comments, and literals that are used to decorate
    /// the raw disassembly
    /// </summary>
    public class DisassemblyDecoration
    {
        /// <summary>
        /// Maximum label length
        /// </summary>
        public const int MAX_LABEL_LENGTH = 16;

        /// <summary>
        /// Regex to check label syntax
        /// </summary>
        public static readonly Regex LabelRegex = new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]{0,15}$");

        private readonly Dictionary<ushort, string> _labels = new Dictionary<ushort, string>();
        private readonly Dictionary<ushort, string> _comments = new Dictionary<ushort, string>();
        private readonly Dictionary<ushort, string> _prefixComments = new Dictionary<ushort, string>();
        private readonly Dictionary<ushort, List<string>> _literals = new Dictionary<ushort, List<string>>();

        /// <summary>
        /// Gets the dictionary of labels
        /// </summary>
        public IReadOnlyDictionary<ushort, string> Labels { get; }

        /// <summary>
        /// Gets the dictionary of comments
        /// </summary>
        public IReadOnlyDictionary<ushort, string> Comments { get; }

        /// <summary>
        /// Gets the dictionary of prefix comments
        /// </summary>
        public IReadOnlyDictionary<ushort, string> PrefixComments { get; }

        /// <summary>
        /// Gets the dictionary of literals
        /// </summary>
        public IReadOnlyDictionary<ushort, List<string>> Literals { get; }


        public DisassemblyDecoration()
        {
            Labels = new ReadOnlyDictionary<ushort, string>(_labels);
            Comments = new ReadOnlyDictionary<ushort, string>(_comments);
            PrefixComments = new ReadOnlyDictionary<ushort, string>(_prefixComments);
            Literals = new ReadOnlyDictionary<ushort, List<string>>(_literals);
        }

        /// <summary>
        /// Stores a label in this collection
        /// </summary>
        /// <param name="address">Label address</param>
        /// <param name="label">Label text</param>
        /// <returns>
        /// True, if any modification has been done; otherwise, false
        /// </returns>
        /// <remarks>
        /// If the label text is null, empty, or contains only whitespaces, the label
        /// gets removed.
        /// </remarks>
        public bool SetLabel(ushort address, string label)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return _labels.Remove(address);
            }

            if (label.Length > MAX_LABEL_LENGTH)
            {
                label = label.Substring(0, MAX_LABEL_LENGTH);
            }
            if (!LabelRegex.IsMatch(label)) return false;
            if (Z80Disassembler.DisAsmKeywords.Contains(label.ToUpper()))
            {
                return false;
            }

            _labels[address] = label;
            return true;
        }

        /// <summary>
        /// Stores a comment in this collection
        /// </summary>
        /// <param name="address">Comment address</param>
        /// <param name="comment">Comment text</param>
        /// <returns>
        /// True, if any modification has been done; otherwise, false
        /// </returns>
        /// <remarks>
        /// If the comment text is null, empty, or contains only whitespaces, the comment
        /// gets removed.
        /// </remarks>
        public bool SetComment(ushort address, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return _comments.Remove(address);
            }
            _comments[address] = comment;
            return true;
        }

        /// <summary>
        /// Stores a prefix comment in this collection
        /// </summary>
        /// <param name="address">Comment address</param>
        /// <param name="comment">Comment text</param>
        /// <returns>
        /// True, if any modification has been done; otherwise, false
        /// </returns>
        /// <remarks>
        /// If the comment text is null, empty, or contains only whitespaces, the comment
        /// gets removed.
        /// </remarks>
        public bool SetPrefixComment(ushort address, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return _prefixComments.Remove(address);
            }
            _prefixComments[address] = comment;
            return true;
        }

        /// <summary>
        /// Merges this decoration with another one
        /// </summary>
        /// <param name="other">Other disassembly decoration</param>
        /// <remarks>
        /// Definitions in the other decoration override the ones defeined here
        /// </remarks>
        public void Merge(DisassemblyDecoration other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (other == this) return;

            foreach (var label in other.Labels)
            {
                _labels[label.Key] = label.Value;
            }
            foreach (var comment in other.Comments)
            {
                _comments[comment.Key] = comment.Value;
            }
            foreach (var prefixComent in other.PrefixComments)
            {
                _prefixComments[prefixComent.Key] = prefixComent.Value;
            }
            foreach (var literal in other.Literals)
            {
                if (_literals.ContainsKey(literal.Key))
                {
                    _literals[literal.Key] = literal.Value.Union(_literals[literal.Key]).Distinct().ToList();
                }
                else
                {
                    _literals[literal.Key] = literal.Value;
                }
            }
        }
    }
}