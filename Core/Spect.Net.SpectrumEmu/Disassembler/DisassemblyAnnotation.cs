using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class describes those labels, comments, and literals that are used to decorate
    /// the raw disassembly
    /// </summary>
    public class DisassemblyAnnotation
    {
        /// <summary>
        /// Maximum label length
        /// </summary>
        public const int MAX_LABEL_LENGTH = 16;

        /// <summary>
        /// Regex to check label syntax
        /// </summary>
        public static readonly Regex LabelRegex = new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]{0,15}$");

        private Dictionary<ushort, string> _labels = new Dictionary<ushort, string>();
        private Dictionary<ushort, string> _comments = new Dictionary<ushort, string>();
        private Dictionary<ushort, string> _prefixComments = new Dictionary<ushort, string>();
        private Dictionary<ushort, List<string>> _literals = new Dictionary<ushort, List<string>>();
        private readonly Dictionary<string, ushort> _literalValues = new Dictionary<string, ushort>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<ushort, string> _literalReplacements = new Dictionary<ushort, string>();

        /// <summary>
        /// Gets the dictionary of labels
        /// </summary>
        public IReadOnlyDictionary<ushort, string> Labels { get; private set; }

        /// <summary>
        /// Gets the dictionary of comments
        /// </summary>
        public IReadOnlyDictionary<ushort, string> Comments { get; private set; }

        /// <summary>
        /// Gets the dictionary of prefix comments
        /// </summary>
        public IReadOnlyDictionary<ushort, string> PrefixComments { get; private set; }

        /// <summary>
        /// Gets the dictionary of literals
        /// </summary>
        public IReadOnlyDictionary<ushort, List<string>> Literals { get; private set; }

        /// <summary>
        /// Gets the dictionary of literal replacements
        /// </summary>
        public IReadOnlyDictionary<ushort, string> LiteralReplacements { get; private set; }

        /// <summary>
        /// The memory map structure
        /// </summary>
        public MemoryMap MemoryMap { get; }

        public DisassemblyAnnotation()
        {
            InitReadOnlyProps();
            MemoryMap = new MemoryMap();
        }

        private void InitReadOnlyProps()
        {
            Labels = new ReadOnlyDictionary<ushort, string>(_labels);
            Comments = new ReadOnlyDictionary<ushort, string>(_comments);
            PrefixComments = new ReadOnlyDictionary<ushort, string>(_prefixComments);
            Literals = new ReadOnlyDictionary<ushort, List<string>>(_literals);
            LiteralReplacements = new ReadOnlyDictionary<ushort, string>(_literalReplacements);
        }

        /// <summary>
        /// Stores a label in annotations
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

            var existinglabel = _labels.FirstOrDefault(kv =>
                string.Compare(kv.Value, label, StringComparison.OrdinalIgnoreCase) == 0);
            if (existinglabel.Value != null && existinglabel.Key != address)
            {
                return false;
            }

            _labels[address] = label;
            return true;
        }

        /// <summary>
        /// Stores a comment in annotations
        /// </summary>
        /// <param name="address">Comment address</param>
        /// <param name="comment">Comment text</param>
        /// <returns>
        /// True, if any modification has been done; otherwise, false
        /// </returns>
        /// <remarks>
        /// If the name text is null, empty, or contains only whitespaces, the name
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
        /// Stores a prefix name in this collection
        /// </summary>
        /// <param name="address">Comment address</param>
        /// <param name="comment">Comment text</param>
        /// <returns>
        /// True, if any modification has been done; otherwise, false
        /// </returns>
        /// <remarks>
        /// If the name text is null, empty, or contains only whitespaces, the name
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
        /// Adds a literal to this collection
        /// </summary>
        /// <param name="key">Literal key</param>
        /// <param name="name">Literal name</param>
        /// <returns>
        /// True, if any modification has been done; otherwise, false
        /// </returns>
        public bool AddLiteral(ushort key, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (!_literals.TryGetValue(key, out List<string> names))
            {
                names = new List<string>();
            }

            // --- Check if the same name is assigned to any other key
            if (_literalValues.TryGetValue(name, out ushort addr) && addr != key)
            {
                return false;
            }

            if (names.Contains(name))
            {
                return false;
            }
            names.Add(name);
            _literalValues.Add(name, key);
            _literals[key] = names;
            return true;
        }

        /// <summary>
        /// Removes a literal from this collection
        /// </summary>
        /// <param name="key">Literal key</param>
        /// <param name="name">Literal name</param>
        /// <returns>
        /// True, if any modification has been done; otherwise, false
        /// </returns>
        /// <remarks>
        /// If the name text is null, empty, or contains only whitespaces, the name
        /// gets removed.
        /// </remarks>
        public bool RemoveLiteral(ushort key, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (!_literals.TryGetValue(key, out List<string> names))
            {
                names = new List<string>();
            }
            if (!names.Contains(name))
            {
                return false;
            }

            names.Remove(name);
            if (names.Count > 0)
            {
                _literals[key] = names;
            }
            else
            {
                _literals.Remove(key);
            }
            return true;
        }

        /// <summary>
        /// Stores a literal replacement in this collection
        /// </summary>
        /// <param name="address">Literal replacement address</param>
        /// <param name="literalName">Literal name to replace a value</param>
        /// <returns>
        /// True, if any modification has been done; otherwise, false
        /// </returns>
        /// <remarks>
        /// If the name text is null, empty, or contains only whitespaces, the name
        /// gets removed.
        /// </remarks>
        public bool SetLiteralReplacement(ushort address, string literalName)
        {
            if (string.IsNullOrWhiteSpace(literalName))
            {
                return _literalReplacements.Remove(address);
            }
            _literalReplacements[address] = literalName;
            return true;
        }

        /// <summary>
        /// Replaces a literal in the disassembly item for the specified address. If
        /// the named literal does not exists, creates one for the symbol.
        /// </summary>
        /// <param name="address">Disassembly item address</param>
        /// <param name="symbol">Symbol value for the literal</param>
        /// <param name="literalName">Literal name</param>
        /// <returns>Null, if operation id ok, otherwise, error message</returns>
        /// <remarks>If the literal already exists, it must have the symbol's value.</remarks>
        public string ApplyLiteral(ushort address, ushort symbol, string literalName)
        {
            if (string.IsNullOrWhiteSpace(literalName))
            {
                _literalReplacements.Remove(address);
                return null;
            }

            ushort? literal;
            if (literalName == "#")
            {
                // --- Apply the first literal that is available for the specified symbol
                if (!_literals.TryGetValue(symbol, out var values))
                {
                    // --- Fall back to labels
                    if (!_labels.TryGetValue(symbol, out var labelName))
                    {
                        // --- No symbol, no label
                        return $"There is no symbol associated with #{symbol:X4} yet.";
                    }

                    // --- We found a matching label
                    literalName = labelName;
                    literal = symbol;
                }
                else
                {
                    // --- We found a matching symbol with one or more names
                    // --- We deliberately choose the first name
                    literalName = values.OrderBy(v => v).First();
                    literal = GetLiteralValue(literalName);
                }
            }
            else
            {
                // --- Let's check if we have a value to the specified literal
                literal = GetLiteralValue(literalName);
            }

            if (literal.HasValue)
            {
                if (literal.Value != symbol)
                {
                    return
                        $"'{literalName}' cannot be assigned to #{symbol:X4}, as it already has a value of #{literal.Value:X4}";
                }
            }
            else
            {
                AddLiteral(symbol, literalName);
            }
            _literalReplacements[address] = literalName;
            return null;
        }

        /// <summary>
        /// Checks if the specified liter is defined.
        /// </summary>
        /// <param name="literalName">Literal to check</param>
        /// <returns>True, if the specified literal has already been defined; otherwise, false</returns>ű
        /// <remarks>Literal names are case sensitive</remarks>
        public ushort? GetLiteralValue(string literalName) =>
            _literalValues.TryGetValue(literalName, out ushort addr)
                ? addr : (ushort?)null;

        /// <summary>
        /// Merges this decoration with another one
        /// </summary>
        /// <param name="other">Other disassembly decoration</param>
        /// <remarks>
        /// Definitions in the other decoration override the ones defeined here
        /// </remarks>
        public void Merge(DisassemblyAnnotation other)
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
            foreach (var prefixComment in other.PrefixComments)
            {
                _prefixComments[prefixComment.Key] = prefixComment.Value;
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
            foreach (var replacement in other.LiteralReplacements)
            {
                _literalReplacements[replacement.Key] = replacement.Value;
            }
            foreach (var section in other.MemoryMap)
            {
                MemoryMap.Add(section);
            }
        }

        /// <summary>
        /// Seriazlizes the contents of this instance into a JSON string
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(new DisassemblyDecorationData
            {
                Labels = _labels,
                Comments = _comments,
                PrefixComments = _prefixComments,
                Literals = _literals,
                LiteralReplacements = _literalReplacements,
                MemorySections = new List<MemorySection>(MemoryMap)
            }, 
            Formatting.Indented);
        }

        /// <summary>
        /// Deserializes the specified JSON string into a DisassemblyAnnotation
        /// instance
        /// </summary>
        /// <param name="json">JSON representation</param>
        /// <returns>The deserialized object</returns>
        public static DisassemblyAnnotation Deserialize(string json)
        {
            DisassemblyDecorationData data;
            try
            {
                data = JsonConvert.DeserializeObject<DisassemblyDecorationData>(json);
            }
            catch
            {
                return new DisassemblyAnnotation();
            }
            var result = new DisassemblyAnnotation();

            if (data != null) 
            {
                result = new DisassemblyAnnotation
                {
                    _labels = data.Labels,
                    _comments = data.Comments,
                    _prefixComments = data.PrefixComments,
                    _literals = data.Literals,
                    _literalReplacements = data.LiteralReplacements
                };
                foreach (var section in data.MemorySections)
                {
                    result.MemoryMap.Add(section);
                }
                foreach (var literal in data.Literals)
                {
                    foreach (var item in literal.Value)
                    {
                        result._literalValues[item] = literal.Key;
                    }
                }
            }
            result.InitReadOnlyProps();
            return result;
        }

        /// <summary>
        /// Deserializes the specified JSON string into a DisassemblyAnnotation
        /// instance
        /// </summary>
        /// <param name="json">JSON representation</param>
        /// <returns>The deserialized object</returns>
        public static Dictionary<int, DisassemblyAnnotation> DeserializeBankAnnotations(string json)
        {
            Dictionary<int, DisassemblyDecorationData> dataList;
            try
            {
                dataList = JsonConvert.DeserializeObject<Dictionary<int, DisassemblyDecorationData>>(json);
            }
            catch
            {
                return new Dictionary<int, DisassemblyAnnotation>();
            }
            var result = new Dictionary<int, DisassemblyAnnotation>();
            foreach (var disAnn in dataList)
            {
                var data = disAnn.Value;
                var ann = new DisassemblyAnnotation
                {
                    _labels = data.Labels,
                    _comments = data.Comments,
                    _prefixComments = data.PrefixComments,
                    _literals = data.Literals,
                    _literalReplacements = data.LiteralReplacements
                };
                foreach (var section in data.MemorySections)
                {
                    ann.MemoryMap.Add(section);
                }
                foreach (var literal in data.Literals)
                {
                    foreach (var item in literal.Value)
                    {
                        ann._literalValues[item] = literal.Key;
                    }
                }
                ann.InitReadOnlyProps();
                result.Add(disAnn.Key, ann);
            }
            return result;
        }

        /// <summary>
        /// Helper class for JSON serizalization
        /// </summary>
        public class DisassemblyDecorationData
        {
            public Dictionary<ushort, string> Labels { get; set; }
            public Dictionary<ushort, string> Comments { get; set; }
            public Dictionary<ushort, string> PrefixComments { get; set; }
            public Dictionary<ushort, List<string>> Literals { get; set; }
            public Dictionary<ushort, string> LiteralReplacements { get; set; }
            public List<MemorySection> MemorySections { get; set; }

            public DisassemblyDecorationData()
            {
                Labels = new Dictionary<ushort, string>();
                Comments = new Dictionary<ushort, string>();
                PrefixComments = new Dictionary<ushort, string>();
                Literals = new Dictionary<ushort, List<string>>();
                LiteralReplacements = new Dictionary<ushort, string>();
                MemorySections = new List<MemorySection>();
            }
        }
    }
}