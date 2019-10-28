using Spect.Net.Assembler.SyntaxTree.Pragmas;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents COMPAREBIN information to check
    /// </summary>
    public class BinaryComparisonInfo
    {
        public BinaryComparisonInfo(CompareBinPragma comparePragma, BinarySegment segment, int segmentLength)
        {
            ComparePragma = comparePragma;
            Segment = segment;
            SegmentLength = segmentLength;
        }

        /// <summary>
        /// The code segment to compare with the binary
        /// </summary>
        public BinarySegment Segment { get; }

        /// <summary>
        /// The length of segment to check
        /// </summary>
        public int SegmentLength { get; }

        /// <summary>
        /// The COMPAREBIN pragma information
        /// </summary>
        public CompareBinPragma ComparePragma { get; }
    }
}