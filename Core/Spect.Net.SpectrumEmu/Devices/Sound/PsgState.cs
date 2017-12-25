// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable ConvertToAutoProperty

using System;

namespace Spect.Net.SpectrumEmu.Devices.Sound
{
    /// <summary>
    /// This class represents the state of the PSG (Programable Sound Gnerator)
    /// </summary>
    public class PsgState
    {
        private ushort _chATone;
        private ushort _chBTone;
        private ushort _chCTone;
        private byte _noise;
        private byte _mixer;
        private byte _volumeA;
        private byte _volumeB;
        private byte _volumeC;
        private ushort _envelopePeriod;
        private byte _envelopeShape;

        /// <summary>
        /// Channel A Fine Tune Register
        /// </summary>
        public byte Register0
        {
            get => (byte)_chATone;
            set => _chATone = (ushort) ((_chATone & 0x0F00) | value);
        }

        /// <summary>
        /// Channel A Coarse Tune Register
        /// </summary>
        public byte Register1
        {
            get => (byte)((_chATone >> 8) & 0x0F);
            set => _chATone = (ushort)((_chATone & 0xFF) | ((value & 0x0F) << 8));
        }

        /// <summary>
        /// Channel A Value
        /// </summary>
        public ushort ChannelA => _chATone;

        /// <summary>
        /// Channel B Fine Tune Register
        /// </summary>
        public byte Register2
        {
            get => (byte)_chBTone;
            set => _chBTone = (ushort)((_chBTone & 0x0F00) | value);
        }

        /// <summary>
        /// Channel B Coarse Tune Register
        /// </summary>
        public byte Register3
        {
            get => (byte)((_chBTone >> 8) & 0x0F);
            set => _chBTone = (ushort)((_chBTone & 0xFF) | ((value & 0x0F) << 8));
        }

        /// <summary>
        /// Channel B Value
        /// </summary>
        public ushort ChannelB => _chBTone;

        /// <summary>
        /// Channel C Fine Tune Register
        /// </summary>
        public byte Register4
        {
            get => (byte)_chCTone;
            set => _chCTone = (ushort)((_chCTone & 0x0F00) | value);
        }

        /// <summary>
        /// Channel C Coarse Tune Register
        /// </summary>
        public byte Register5
        {
            get => (byte)((_chCTone >> 8) & 0x0F);
            set => _chCTone = (ushort)((_chCTone & 0xFF) | ((value & 0x0F) << 8));
        }

        /// <summary>
        /// Channel C Value
        /// </summary>
        public ushort ChannelC => _chCTone;

        /// <summary>
        /// Noise Period Register
        /// </summary>
        public byte Register6
        {
            get => (byte)(_noise & 0x03F);
            set => _noise = (byte)(value & 0x3F);
        }

        /// <summary>
        /// Mixer Control-I/O Enable Register
        /// </summary>
        public byte Register7
        {
            get => (byte)(_mixer & 0x7F);
            set => _mixer = (byte)(value & 0x7F);
        }

        /// <summary>
        /// Input is enabled in Register 7
        /// </summary>
        public bool InputEnabled => (_mixer & 0b0100_0000) == 0;

        /// <summary>
        /// Tone A is enabled in Register 7
        /// </summary>
        public bool ToneAEnabled => (_mixer & 0b0000_0001) == 0;

        /// <summary>
        /// Tone B is enabled in Register 7
        /// </summary>
        public bool ToneBEnabled => (_mixer & 0b0000_0010) == 0;

        /// <summary>
        /// Tone C is enabled in Register 7
        /// </summary>
        public bool ToneCEnabled => (_mixer & 0b0000_0100) == 0;

        /// <summary>
        /// Noise A is enabled in Register 7
        /// </summary>
        public bool NoiseAEnabled => (_mixer & 0b0000_1000) == 0;

        /// <summary>
        /// Noise B is enabled in Register 7
        /// </summary>
        public bool NoiseBEnabled => (_mixer & 0b0001_0000) == 0;

        /// <summary>
        /// Noise C is enabled in Register 7
        /// </summary>
        public bool NoiseCEnabled => (_mixer & 0b0010_0000) == 0;

        /// <summary>
        /// Amplitude Control A Register
        /// </summary>
        public byte Register8
        {
            get => (byte) (_volumeA & 0x1F);
            set => _volumeA = (byte) (value & 0x1F);
        }

        /// <summary>
        /// Gets the amplitude level of Channel A
        /// </summary>
        public byte AmplitudeA => (byte) (_volumeA & 0x0F);

        /// <summary>
        /// Indicates if enveéope mode should be used for Channel A
        /// </summary>
        public bool UseEnvelopeA => (_volumeA & 0b0001_0000) != 0;

        /// <summary>
        /// Amplitude Control B Register
        /// </summary>
        public byte Register9
        {
            get => (byte)(_volumeB & 0x1F);
            set => _volumeB = (byte)(value & 0x1F);
        }

        /// <summary>
        /// Gets the amplitude level of Channel B
        /// </summary>
        public byte AmplitudeB => (byte)(_volumeB & 0x0F);

        /// <summary>
        /// Indicates if envelope mode should be used for Channel B
        /// </summary>
        public bool UseEnvelopeB => (_volumeB & 0b0001_0000) != 0;

        /// <summary>
        /// Amplitude Control C Register
        /// </summary>
        public byte Register10
        {
            get => (byte)(_volumeC & 0x1F);
            set => _volumeC = (byte)(value & 0x1F);
        }

        /// <summary>
        /// Gets the amplitude level of Channel C
        /// </summary>
        public byte AmplitudeC => (byte)(_volumeC & 0x0F);

        /// <summary>
        /// Indicates if envelope mode should be used for Channel C
        /// </summary>
        public bool UseEnvelopeC => (_volumeC & 0b0001_0000) != 0;

        /// <summary>
        /// Envelope Period LSB Register
        /// </summary>
        public byte Register11
        {
            get => (byte)_envelopePeriod;
            set => _envelopePeriod = (ushort) ((_envelopePeriod & 0xFF00) | value);
        }

        /// <summary>
        /// Envelope Period MSB Register
        /// </summary>
        public byte Register12
        {
            get => (byte) (_envelopePeriod >> 8);
            set => _envelopePeriod = (ushort) ((_envelopePeriod & 0xFF) | (value << 8));
        }

        /// <summary>
        /// Envelope period value
        /// </summary>
        public ushort EnvelopePeriod => _envelopePeriod;

        /// <summary>
        /// Envelope shape register
        /// </summary>
        public byte Register13
        {
            get => (byte) (_envelopeShape & 0x0F);
            set => _envelopeShape = (byte) (value & 0x0F);
        }

        /// <summary>
        /// Hold flag of the envelope 
        /// </summary>
        public bool HoldFlag => (_envelopeShape & 0x01) != 0;

        /// <summary>
        /// Alternate flag of the envelope 
        /// </summary>
        public bool AlternateFlag => (_envelopeShape & 0x02) != 0;

        /// <summary>
        /// Attack flag of the envelope 
        /// </summary>
        public bool AttackFlag => (_envelopeShape & 0x04) != 0;

        /// <summary>
        /// Continue flag of the envelope 
        /// </summary>
        public bool ContinueFlag => (_envelopeShape & 0x08) != 0;

        /// <summary>
        /// I/O Port register
        /// </summary>
        public byte Register14 { get; set; }

        /// <summary>
        /// Gets a register by its index
        /// </summary>
        /// <param name="index">
        /// Register index (should be between 0 and 14)
        /// </param>
        /// <returns>Register value</returns>
        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Register0;
                    case 1: return Register1;
                    case 2: return Register2;
                    case 3: return Register3;
                    case 4: return Register4;
                    case 5: return Register5;
                    case 6: return Register6;
                    case 7: return Register7;
                    case 8: return Register8;
                    case 9: return Register9;
                    case 10: return Register10;
                    case 11: return Register11;
                    case 12: return Register12;
                    case 13: return Register13;
                    case 14: return Register14;
                    default: throw new InvalidOperationException("Index must be between 0 and 14");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: Register0 = value; break;
                    case 1: Register1 = value; break;
                    case 2: Register2 = value; break;
                    case 3: Register3 = value; break;
                    case 4: Register4 = value; break;
                    case 5: Register5 = value; break;
                    case 6: Register6 = value; break;
                    case 7: Register7 = value; break;
                    case 8: Register8 = value; break;
                    case 9: Register9 = value; break;
                    case 10: Register10 = value; break;
                    case 11: Register11 = value; break;
                    case 12: Register12 = value; break;
                    case 13: Register13 = value; break;
                    case 14: Register14 = value; break;
                    default: throw new InvalidOperationException("Index must be between 0 and 14");
                }
            }
        }
    }
}