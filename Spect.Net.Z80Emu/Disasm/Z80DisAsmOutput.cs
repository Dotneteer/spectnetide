﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Spect.Net.Z80Emu.Debug;

namespace Spect.Net.Z80Emu.Disasm
{
    /// <summary>
    /// This class represents the output of the disassembly project
    /// </summary>
    public class Z80DisAsmOutput
    {
        private readonly List<DisassemblyItem> _outputItems = new List<DisassemblyItem>();
        private readonly Dictionary<ushort, DisassemblyItem> _outputByAddress = 
            new Dictionary<ushort, DisassemblyItem>();

        /// <summary>
        /// Gets the list of output items
        /// </summary>
        public IReadOnlyList<DisassemblyItem> OutputItems { get; }

        /// <summary>
        /// Gets the provider that can add debug information to this disassembly item
        /// </summary>
        public IZ80DebugInfoProvider DebugInfoProvider { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Z80DisAsmOutput()
        {
            OutputItems = new ReadOnlyCollection<DisassemblyItem>(_outputItems);
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
                DisassemblyItem item;
                _outputByAddress.TryGetValue(addr, out item);
                return item;
            }
        }

        /// <summary>
        /// Sets a debug info provider for this output item collection
        /// </summary>
        /// <param name="provider"></param>
        public void SetDebugInfoProvider(IZ80DebugInfoProvider provider)
        {
            DebugInfoProvider = provider;
        }
    }
}