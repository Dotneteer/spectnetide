using System.Collections.Generic;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a single segment of the code compilation
    /// </summary>
    public class BinarySegment
    {
        /// <summary>
        /// Start address of the compiled block
        /// </summary>
        public ushort StartAddress { get; set; }

        /// <summary>
        /// Displacement of the this segment
        /// </summary>
        public ushort? Displacement { get; set; }

        /// <summary>
        /// Indicates if this segment is displaced
        /// </summary>
        public bool IsDisplaced => Displacement.HasValue;

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
        /// Emits a new data byte
        /// </summary>
        /// <param name="data"></param>
        public bool EmitByte(byte data)
        {
            EmittedCode.Add(data);
            if (StartAddress + EmittedCode.Count > 0x10000 && !OverflowDetected)
            {
                OverflowDetected = true;
                return true;
            }
            return false;
        }
    }
}