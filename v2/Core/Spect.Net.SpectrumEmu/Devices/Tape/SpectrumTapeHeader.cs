using System;
using System.Text;

namespace Spect.Net.SpectrumEmu.Devices.Tape
{
    /// <summary>
    /// This class represents a spectrum tape header
    /// </summary>
    public class SpectrumTapeHeader
    {
        private const int HEADER_LEN = 19;
        private const int TYPE_OFFS = 1;
        private const int NAME_OFFS = 2;
        private const int NAME_LEN = 10;
        private const int DATA_LEN_OFFS = 12;
        private const int PAR1_OFFS = 14;
        private const int PAR2_OFFS = 16;
        private const int CHK_OFFS = 18;

        /// <summary>
        /// The bytes of the header
        /// </summary>
        public byte[] HeaderBytes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SpectrumTapeHeader()
        {
            HeaderBytes = new byte[HEADER_LEN];
            for (var i = 0; i < HEADER_LEN; i++) HeaderBytes[i] = 0x00;
            CalcChecksum();
        }

        /// <summary>
        /// Initializes a new instance with the specified header data.
        /// </summary>
        /// <param name="header">Header data</param>
        public SpectrumTapeHeader(byte[] header)
        {
            if (header == null) throw  new ArgumentNullException(nameof(header));
            if (header.Length != HEADER_LEN)
            {
                throw new ArgumentException($"Header must be exactly {HEADER_LEN} bytes long");
            }
            HeaderBytes = new byte[HEADER_LEN];
            header.CopyTo(HeaderBytes, 0);
            CalcChecksum();
        }

        /// <summary>
        /// Gets or sets the type of the header
        /// </summary>
        public byte Type
        {
            get => HeaderBytes[TYPE_OFFS];
            set
            {
                HeaderBytes[TYPE_OFFS] = (byte)(value & 0x03);
                CalcChecksum();
            }
        }

        /// <summary>
        /// Gets or sets the program name
        /// </summary>
        public string Name
        {
            get
            {
                var name = new StringBuilder(NAME_LEN + 4);
                for (var i = NAME_OFFS; i < NAME_OFFS + NAME_LEN; i++)
                {
                    name.Append((char) HeaderBytes[i]);
                }
                return name.ToString().TrimEnd();
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Length > NAME_LEN) value = value.Substring(0, NAME_LEN);
                else if (value.Length < NAME_LEN) value = value.PadRight(NAME_LEN, ' ');
                for (var i = NAME_OFFS; i < NAME_OFFS + NAME_LEN; i++)
                {
                    HeaderBytes[i] = (byte)value[i-NAME_OFFS];
                }
                CalcChecksum();
            }
        }

        /// <summary>
        /// Gets or sets the Data Length
        /// </summary>
        public ushort DataLength
        {
            get => GetWord(DATA_LEN_OFFS);
            set => SetWord(DATA_LEN_OFFS, value);
        }

        /// <summary>
        /// Gets or sets Parameter1
        /// </summary>
        public ushort Parameter1
        {
            get => GetWord(PAR1_OFFS);
            set => SetWord(PAR1_OFFS, value);
        }

        /// <summary>
        /// Gets or sets Parameter1
        /// </summary>
        public ushort Parameter2
        {
            get => GetWord(PAR2_OFFS);
            set => SetWord(PAR2_OFFS, value);
        }

        /// <summary>
        /// Gets the value of checksum
        /// </summary>
        public byte Checksum => HeaderBytes[CHK_OFFS];

        /// <summary>
        /// Calculate the checksum
        /// </summary>
        private void CalcChecksum()
        {
            var chk = 0x00;
            for (var i = 0; i < HEADER_LEN - 1; i++) chk ^= HeaderBytes[i];
            HeaderBytes[CHK_OFFS] = (byte)chk;
        }

        /// <summary>
        /// Gets the word value from the specified offset
        /// </summary>
        private ushort GetWord(int offset) => 
            (ushort)(HeaderBytes[offset] + 256 * HeaderBytes[offset + 1]);

        /// <summary>
        /// Sets the word value at the specified offset
        /// </summary>
        private void SetWord(int offset, ushort value)
        {
            HeaderBytes[offset] = (byte)(value & 0xff);
            HeaderBytes[offset + 1] = (byte)(value >> 8);
            CalcChecksum();
        }
    }
}