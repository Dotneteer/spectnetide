using System.Collections.Generic;

namespace Spect.Net.Z80DisAsm
{
    /// <summary>
    /// This class is is responsible for disassembling Z80 binary code
    /// </summary>
    public class Z80Disassembler
    {
        private ushort _offset;
        private IList<byte> _currentOpCodes;

        /// <summary>
        /// The project to disassemble
        /// </summary>
        public Z80DisAsmProject Project { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Z80Disassembler(Z80DisAsmProject project)
        {
            Project = project;
        }

        /// <summary>
        /// Executes the disassembly process
        /// </summary>
        /// <param name="startFrom">The index to start disassembly from</param>
        /// <param name="length">The length of code to disassemble</param>
        /// <returns></returns>
        public Z80DisAsmOutput Disassemble(ushort startFrom = 0, ushort length = 0)
        {
            var output = new Z80DisAsmOutput();
            _offset = startFrom;
            var codeLength = Project.Z80Binary.Length;
            if (length > 0 && length < Project.Z80Binary.Length)
            {
                codeLength = length;
            }

            while (_offset < codeLength)
            {
                var item = DisassembleOperation();
                if (item != null)
                {
                    output.OutputItems.Add(item);
                }
            }
            return output;
        }

        /// <summary>
        /// Disassembles a single instruction
        /// </summary>
        private DisassemblyItem DisassembleOperation()
        {
            var address = (ushort)(_offset + Project.StartOffset);
            var opCode = Fetch();
            _currentOpCodes = new List<byte> { opCode };
            if (opCode == 0xED)
            {
                DisassembleExtendedOperation();
            }
            else if (opCode == 0xCB)
            {
                DisassembleBitOperation();
            }
            else if (opCode == 0xDD)
            {
                DisassembleIndexedOperation(0);
            }
            else if (opCode == 0xFD)
            {
                DisassembleIndexedOperation(0);
            }
            var item = new DisassemblyItem(address, _currentOpCodes, null, "<none>");
            return item;
        }

        private void DisassembleExtendedOperation()
        {
            var opCode = Fetch();
            _currentOpCodes.Add(opCode);
        }

        private void DisassembleBitOperation()
        {
            var opCode = Fetch();
            _currentOpCodes.Add(opCode);
        }

        private void DisassembleIndexedOperation(int i)
        {
            var opCode = Fetch();
            _currentOpCodes.Add(opCode);
            if (opCode == 0xCB)
            {
                DisassembleIndexedBitOperation(i);
            }
        }

        private void DisassembleIndexedBitOperation(int i)
        {
            var displacement = Fetch();
            _currentOpCodes.Add(displacement);
            var opCode = Fetch();
            _currentOpCodes.Add(opCode);
        }

        private byte Fetch()
        {
            return Project.Z80Binary[_offset++];
        }
    }
}