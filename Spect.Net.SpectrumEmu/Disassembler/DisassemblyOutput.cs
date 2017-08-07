using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class represents the output of the disassembly project
    /// </summary>
    public class DisassemblyOutput
    {
        private readonly List<DisassemblyItem> _outputItems = new List<DisassemblyItem>();
        private readonly Dictionary<ushort, DisassemblyItem> _outputByAddress = 
            new Dictionary<ushort, DisassemblyItem>();

        /// <summary>
        /// Gets the list of output items
        /// </summary>
        public IReadOnlyList<DisassemblyItem> OutputItems { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DisassemblyOutput()
        {
            OutputItems = new ReadOnlyCollection<DisassemblyItem>(_outputItems);
        }

        /// <summary>
        /// Clears the entire output
        /// </summary>
        public void Clear()
        {
            _outputItems.Clear();
            _outputByAddress.Clear();
        }

        /// <summary>
        /// Adds a new item to the output
        /// </summary>
        /// <param name="item">Disassembly item to add</param>
        public void AddItem(DisassemblyItem item)
        {
            _outputItems.Add(item);
            _outputByAddress.Add(item.Address, item);
        }

        /// <summary>
        /// Gets a disassembly item by its address
        /// </summary>
        /// <param name="addr">Item address</param>
        /// <returns>The speicifid item, if found; otherwise, null</returns>
        public DisassemblyItem this[ushort addr]
        {
            get
            {
                _outputByAddress.TryGetValue(addr, out DisassemblyItem item);
                return item;
            }
        }
    }
}