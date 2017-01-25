using System;

namespace Spect.Net.Z80Emu.Core
{
    public partial class Z80
    {
        /// <summary>
        /// Indexed bit (0xDDCB or 0xFDCB-prefixed) operations jump table
        /// </summary>
        private Action<byte>[] _indexedBitOperations;

        private void InitializeIndexedBitOpsExecutionTable()
        {
            _indexedOperations = new Action<byte>[]
            {
            };
        }
    }
}