using System.IO;

namespace Spect.Net.SpectrumEmu.Tzx
{
    /// <summary>
    /// This class represents a segment of the TZX file
    /// </summary>
    public abstract class TzxSegmentBase
    {
        /// <summary>
        /// Writes this segment to the specified <paramref name="writer" />
        /// </summary>
        /// <param name="writer">Segment output</param>
        public abstract void Write(BinaryWriter writer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public abstract void Read(BinaryReader reader);
    }
}