namespace Spect.Net.SpectrumEmu.Disassembler
{
    public partial class Z80Disassembler
    {
        private SpectrumSpecificMode _spectMode;

        /// <summary>
        /// Checks if the disassembler should enter into Spectrum-specific mode after
        /// the specified disassembly item.
        /// </summary>
        /// <param name="item">Item used to check the Spectrum-specific mode</param>
        /// <returns>
        /// True, to move to the Spectrum-specific mode; otherwise, false
        /// </returns>
        private bool ShouldEnterSpectrumSpecificMode(DisassemblyItem item)
        {
            // --- Check for RST #08
            if (item?.OpCodes.Trim() == "CF")
            {
                _spectMode = SpectrumSpecificMode.Rst08;
                item.HardComment = "(Report error)";
                return true;
            }
            return false;
        }

        /// <summary>
        /// Disassembles the subsequent operation as Spectrum-specific
        /// </summary>
        /// <param name="carryOn">
        /// True, if the disassembler still remains in Spectrum-specific mode;
        /// otherwise, false
        /// </param>
        /// <returns>Disassembled operation</returns>
        private DisassemblyItem DisassembleSpectrumSpecificOperation(out bool carryOn)
        {
            if (_spectMode == SpectrumSpecificMode.None)
            {
                carryOn = false;
                return null;
            }

            if (_spectMode == SpectrumSpecificMode.Rst08)
            {
                // --- The next byte is the operation code
                var address = (ushort)_offset;
                var errorCode = Fetch();
                carryOn = false;
                _spectMode = SpectrumSpecificMode.None;
                return new DisassemblyItem(address)
                {
                    OpCodes = _currentOpCodes.ToString(),
                    Instruction = $".defb #{errorCode:X2}",
                    HardComment = $"(error code: #{errorCode:X2})",
                    LastAddress = (ushort)(_offset - 1)
                };
            }
            carryOn = false;
            return null;
        }

        /// <summary>
        /// Spectrum-specific disassembly mode
        /// </summary>
        private enum SpectrumSpecificMode
        {
            None = 0,
            Rst08,
            Rst28,
        }
    }
}