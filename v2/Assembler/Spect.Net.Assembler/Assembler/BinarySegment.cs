using System.Collections.Generic;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a single segment of the code compilation
    /// </summary>
    public class BinarySegment
    {
        /// <summary>
        /// The bank of this segment
        /// </summary>
        public ushort? Bank { get; set; }

        /// <summary>
        /// Start offset used for banks
        /// </summary>
        public ushort BankOffset { get; set; }

        /// <summary>
        /// Maximum code length
        /// </summary>
        public int MaxCodeLength { get; set; }

        /// <summary>
        /// Start address of the compiled block
        /// </summary>
        public ushort StartAddress { get; set; }

        /// <summary>
        /// Displacement of the this segment
        /// </summary>
        public ushort? Displacement { get; set; }

        /// <summary>
        /// Intel HEX start address of the this segment
        /// </summary>
        public ushort? XorgValue { get; set; }

        /// <summary>
        /// Emitted Z80 binary code
        /// </summary>
        public List<byte> EmittedCode { get; set; } = new List<byte>(1024);

        /// <summary>
        /// Signs if overflow has been detected
        /// </summary>
        public bool OverflowDetected { get; private set; }

        /// <summary>
        /// The current code generation offset
        /// </summary>
        public int CurrentOffset => EmittedCode.Count;

        /// <summary>
        /// Shows the offset of the instruction being compiled.
        /// </summary>
        public int CurrentInstructionOffset { get; set; }

        /// <summary>
        /// Emits a new data byte
        /// </summary>
        /// <param name="data"></param>
        public string EmitByte(byte data)
        {
            EmittedCode.Add(data);
            if (StartAddress + EmittedCode.Count > 0x10000 && !OverflowDetected)
            {
                OverflowDetected = true;
                return Errors.Z0304;
            }
            if (EmittedCode.Count > MaxCodeLength && !OverflowDetected)
            {
                OverflowDetected = true;
                return Errors.Z0309;
            }
            return null;
        }
    }
}