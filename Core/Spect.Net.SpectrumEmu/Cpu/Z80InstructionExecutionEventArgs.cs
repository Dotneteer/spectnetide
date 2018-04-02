using System;
using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// Represents the event args of a Z80 operation
    /// </summary>
    public class Z80InstructionExecutionEventArgs: EventArgs
    {
        /// <summary>
        /// PC before the execution
        /// </summary>
        public ushort PcBefore { get; }

        /// <summary>
        /// Code prefixes
        /// </summary>
        public IList<byte> Instruction { get; }

        /// <summary>
        /// Operation code
        /// </summary>
        public byte OpCode { get; }

        /// <summary>
        /// PC after the operation
        /// </summary>
        public ushort? PcAfter { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.EventArgs" /> class.</summary>
        public Z80InstructionExecutionEventArgs(ushort pcBefore, IList<byte> instruction, byte opCode, ushort? pcAfter = null)
        {
            PcBefore = pcBefore;
            Instruction = new List<byte>(instruction);
            OpCode = opCode;
            PcAfter = pcAfter;
        }
    }
}