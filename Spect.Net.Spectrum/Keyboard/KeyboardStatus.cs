namespace Spect.Net.Spectrum.Keyboard
{
    public class KeyboardStatus
    {
        private readonly byte[] _lineStatus = new byte[8];

        /// <summary>
        /// Sets the status of the specified Spectrum keyboard key
        /// </summary>
        /// <param name="key">Key code</param>
        /// <param name="isDown">True, if the key is down; otherwise, false</param>
        public void SetStatus(SpectrumKeyCode key, bool isDown)
        {
            var lineIndex = (byte)key / 5;
            var lineMask = 1 << (byte) key%5;
            _lineStatus[lineIndex] = isDown
                ? (byte)(_lineStatus[lineIndex] | lineMask)
                : (byte)(_lineStatus[lineIndex] & ~lineMask);

        }

        /// <summary>
        /// Gets the status of the specified Spectrum keyboard key.
        /// </summary>
        /// <param name="key">Key code</param>
        /// <returns>True, if the key is down; otherwise, false</returns>
        public bool GetStatus(SpectrumKeyCode key)
        {
            var lineIndex = (byte)key / 5;
            var lineMask = 1 << (byte)key % 5;
            return (_lineStatus[lineIndex] & lineMask) != 0;
        }

        /// <summary>
        /// Gets the byte we would get when querying the I/O address with the
        /// specified byte as the highest 8 bits of the address line
        /// </summary>
        /// <param name="lines">The highest 8 bits of the address line</param>
        /// <returns>
        /// The status value to be received when querying the I/O
        /// </returns>
        public byte GetLineStatus(byte lines)
        {
            byte status = 0;
            lines = (byte)~lines;

            var lineIndex = 0;
            while (lines > 0)
            {
                if ((lines & 0x01) != 0)
                {
                    status |= _lineStatus[lineIndex];
                }
                lineIndex++;
                lines >>= 1;
            }
            return (byte)~status;
        }
    }
}