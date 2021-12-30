// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable ConvertToAutoProperty

using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Sound
{
    /// <summary>
    /// This class represents the state of the PSG (Programable Sound Gnerator)
    /// </summary>
    public class PsgState
    {
        // --- Amplitude table
        private static readonly float[] s_Amplitudes = 
        {
            0.0000f, 0.0100f, 0.0145f, 0.0211f,
            0.0307f, 0.0455f, 0.0645f, 0.1074f,
            0.1266f, 0.2050f, 0.2922f, 0.3728f,
            0.4925f, 0.6353f, 0.8056f, 1.0000f
        };

        private static readonly float[] s_WaveForm =
        {
            0.20f, 0.05f, 0.00f, 0.00f,
            0.00f, 0.00f, 0.05f, 0.20f,
            0.80f, 0.95f, 1.00f, 1.00f,
            1.00f, 1.00f, 0.95f, 0.80f
        };

        // --- Backing fields for registers
        private PsgRegister[] _regs = new PsgRegister[16];
        private int _noiseSeed;
        private ushort _lastNoiseIndex;

        /// <summary>
        /// The virtual machine that hosts the PSG
        /// </summary>
        public ISpectrumVm HostVm { get; }

        /// <summary>
        /// Initializes the PSG state 
        /// </summary>
        /// <param name="hostVm">Hosting VM</param>
        public PsgState(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
        }

        /// <summary>
        /// Channel A Fine Tune Register
        /// </summary>
        public byte Register0
        {
            get => _regs[0].Value;
            set
            {
                _regs[0].Value = value;
                _regs[0].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// Channel A Coarse Tune Register
        /// </summary>
        public byte Register1
        {
            get => _regs[1].Value;
            set
            {
                _regs[1].Value = (byte)(value & 0x0F);
                _regs[1].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// Channel A Value
        /// </summary>
        public ushort ChannelA => (ushort)(_regs[1].Value << 8 | _regs[0].Value);

        /// <summary>
        /// CPU tact when Channel A value was last time modified
        /// </summary>
        public long ChannelAModified => _regs[0].ModifiedTact > _regs[1].ModifiedTact
            ? _regs[0].ModifiedTact
            : _regs[1].ModifiedTact;

        /// <summary>
        /// Channel B Fine Tune Register
        /// </summary>
        public byte Register2
        {
            get => _regs[2].Value;
            set
            {
                _regs[2].Value = value;
                _regs[2].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// Channel B Coarse Tune Register
        /// </summary>
        public byte Register3
        {
            get => _regs[3].Value;
            set
            {
                _regs[3].Value = (byte)(value & 0x0F);
                _regs[3].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// Channel B Value
        /// </summary>
        public ushort ChannelB => (ushort)(_regs[3].Value << 8 | _regs[2].Value);

        /// <summary>
        /// CPU tact when Channel B value was last time modified
        /// </summary>
        public long ChannelBModified => _regs[2].ModifiedTact > _regs[3].ModifiedTact
            ? _regs[2].ModifiedTact
            : _regs[3].ModifiedTact;

        /// <summary>
        /// Channel C Fine Tune Register
        /// </summary>
        public byte Register4
        {
            get => _regs[4].Value;
            set
            {
                _regs[4].Value = value;
                _regs[4].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// Channel C Coarse Tune Register
        /// </summary>
        public byte Register5
        {
            get => _regs[5].Value;
            set
            {
                _regs[5].Value = (byte)(value & 0x0F);
                _regs[5].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// Channel C Value
        /// </summary>
        public ushort ChannelC => (ushort)(_regs[5].Value << 8 | _regs[4].Value);

        /// <summary>
        /// CPU tact when Channel C value was last time modified
        /// </summary>
        public long ChannelCModified => _regs[4].ModifiedTact > _regs[5].ModifiedTact
            ? _regs[4].ModifiedTact
            : _regs[5].ModifiedTact;

        /// <summary>
        /// Noise Period Register
        /// </summary>
        public byte Register6
        {
            get => _regs[6].Value;
            set
            {
                _regs[6].Value = (byte)(value & 0x3F);
                _regs[6].ModifiedTact = HostVm.Cpu.Tacts;
                _lastNoiseIndex = 0;
            }
        }

        /// <summary>
        /// CPU tact when Nosie Period value was last time modified
        /// </summary>
        public long NoisePeriodModified => _regs[6].ModifiedTact;

        /// <summary>
        /// Mixer Control-I/O Enable Register
        /// </summary>
        public byte Register7
        {
            get => _regs[7].Value;
            set
            {
                _regs[7].Value = (byte)(value & 0x7F);
                _regs[7].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// CPU tact when Mixer register value was last time modified
        /// </summary>
        public long MixerModified => _regs[7].ModifiedTact;

        /// <summary>
        /// Input is enabled in Register 7
        /// </summary>
        public bool InputEnabled => (_regs[7].Value & 0b0100_0000) == 0;

        /// <summary>
        /// Tone A is enabled in Register 7
        /// </summary>
        public bool ToneAEnabled => (_regs[7].Value & 0b0000_0001) == 0;

        /// <summary>
        /// Tone B is enabled in Register 7
        /// </summary>
        public bool ToneBEnabled => (_regs[7].Value & 0b0000_0010) == 0;

        /// <summary>
        /// Tone C is enabled in Register 7
        /// </summary>
        public bool ToneCEnabled => (_regs[7].Value & 0b0000_0100) == 0;

        /// <summary>
        /// Noise A is enabled in Register 7
        /// </summary>
        public bool NoiseAEnabled => (_regs[7].Value & 0b0000_1000) == 0;

        /// <summary>
        /// Noise B is enabled in Register 7
        /// </summary>
        public bool NoiseBEnabled => (_regs[7].Value & 0b0001_0000) == 0;

        /// <summary>
        /// Noise C is enabled in Register 7
        /// </summary>
        public bool NoiseCEnabled => (_regs[7].Value & 0b0010_0000) == 0;

        /// <summary>
        /// Amplitude Control A Register
        /// </summary>
        public byte Register8
        {
            get => _regs[8].Value;
            set
            {
                _regs[8].Value = (byte)(value & 0x1F);
                _regs[8].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// Gets the amplitude level of Channel A
        /// </summary>
        public byte AmplitudeA => (byte) (_regs[8].Value & 0x0F);

        /// <summary>
        /// CPU tact when Amplitude A register value was last time modified
        /// </summary>
        public long AmplitudeAModified => _regs[8].ModifiedTact;

        /// <summary>
        /// Indicates if enveéope mode should be used for Channel A
        /// </summary>
        public bool UseEnvelopeA => (_regs[8].Value & 0b0001_0000) != 0;

        /// <summary>
        /// Amplitude Control B Register
        /// </summary>
        public byte Register9
        {
            get => _regs[9].Value;
            set
            {
                _regs[9].Value = (byte)(value & 0x1F);
                _regs[9].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// Gets the amplitude level of Channel B
        /// </summary>
        public byte AmplitudeB => (byte)(_regs[9].Value & 0x0F);

        /// <summary>
        /// CPU tact when Amplitude B register value was last time modified
        /// </summary>
        public long AmplitudeBModified => _regs[9].ModifiedTact;

        /// <summary>
        /// Indicates if envelope mode should be used for Channel B
        /// </summary>
        public bool UseEnvelopeB => (_regs[9].Value & 0b0001_0000) != 0;

        /// <summary>
        /// Amplitude Control C Register
        /// </summary>
        public byte Register10
        {
            get => _regs[10].Value;
            set
            {
                _regs[10].Value = (byte)(value & 0x1F);
                _regs[10].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// Gets the amplitude level of Channel C
        /// </summary>
        public byte AmplitudeC => (byte)(_regs[10].Value & 0x0F);

        /// <summary>
        /// CPU tact when Amplitude C register value was last time modified
        /// </summary>
        public long AmplitudeCModified => _regs[10].ModifiedTact;

        /// <summary>
        /// Indicates if envelope mode should be used for Channel C
        /// </summary>
        public bool UseEnvelopeC => (_regs[10].Value & 0b0001_0000) != 0;

        /// <summary>
        /// Envelope Period LSB Register
        /// </summary>
        public byte Register11
        {
            get => _regs[11].Value;
            set
            {
                _regs[11].Value = value;
                _regs[11].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// Envelope Period MSB Register
        /// </summary>
        public byte Register12
        {
            get => _regs[12].Value;
            set
            {
                _regs[12].Value = (byte)(value & 0xFF);
                _regs[12].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// CPU tact when Envelope Period value was last time modified
        /// </summary>
        public long EnvelopePeriodModified => _regs[11].ModifiedTact > _regs[12].ModifiedTact
            ? _regs[11].ModifiedTact
            : _regs[12].ModifiedTact;

        /// <summary>
        /// Envelope period value
        /// </summary>
        public ushort EnvelopePeriod => (ushort)((_regs[12].Value << 8) | _regs[11].Value);

        /// <summary>
        /// Envelope shape register
        /// </summary>
        public byte Register13
        {
            get => _regs[13].Value;
            set
            {
                _regs[13].Value = (byte)(value & 0x0F);
                _regs[13].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// CPU tact when Envelope Shape register value was last time modified
        /// </summary>
        public long EnvelopeShapeModified => _regs[13].ModifiedTact;

        /// <summary>
        /// Hold flag of the envelope 
        /// </summary>
        public bool HoldFlag => (_regs[13].Value & 0x01) != 0;

        /// <summary>
        /// Alternate flag of the envelope 
        /// </summary>
        public bool AlternateFlag => (_regs[13].Value & 0x02) != 0;

        /// <summary>
        /// Attack flag of the envelope 
        /// </summary>
        public bool AttackFlag => (_regs[13].Value & 0x04) != 0;

        /// <summary>
        /// Continue flag of the envelope 
        /// </summary>
        public bool ContinueFlag => (_regs[13].Value & 0x08) != 0;

        /// <summary>
        /// I/O Port register A
        /// </summary>
        public byte Register14 {
            get => _regs[14].Value;
            set
            {
                _regs[14].Value = (byte)(value & 0xFF);
                _regs[14].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

        /// <summary>
        /// I/O Port register B
        /// </summary>
        public byte Register15 {
            get => _regs[15].Value;
            set
            {
                _regs[15].Value = (byte)(value & 0xFF);
                _regs[15].ModifiedTact = HostVm.Cpu.Tacts;
            }
        }

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
                switch (index & 0x0F)
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
                    case 15: return Register15;
                }
                // --- We cannot reach this state
                return 0;
            }

            set
            {
                switch (index & 0x0F)
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
                    case 15: Register15 = value; break;
                }
            }
        }

        /// <summary>
        /// Gets the effective value of Channel A
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for</param>
        /// <returns>Sample value</returns>
        public float GetChannelASample(long tact)
        {
            if (!ToneAEnabled || ChannelA == 0) return 0.0f;
            var phase = ((tact - ChannelAModified) / 32) % ChannelA * 16 / ChannelA;
            return s_WaveForm[(int)phase];
        }

        /// <summary>
        /// Gets the effective value of Channel B
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for</param>
        /// <returns>Sample value</returns>
        public float GetChannelBSample(long tact)
        {
            if (!ToneBEnabled || ChannelB == 0) return 0.0f;
            var phase = ((tact - ChannelBModified) / 32) % ChannelB * 16 / ChannelB;
            return s_WaveForm[(int)phase];
        }

        /// <summary>
        /// Gets the effective value of Channel C
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for</param>
        /// <returns>Sample value</returns>
        public float GetChannelCSample(long tact)
        {
            if (!ToneCEnabled || ChannelC == 0) return 0.0f;
            var phase = ((tact - ChannelCModified) / 32) % ChannelC * 16 / ChannelC;
            return s_WaveForm[(int)phase];
        }

        /// <summary>
        /// Gets the effective value of the Noise Generator
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for</param>
        /// <returns>Sample value</returns>
        public float GetNoiseSample(long tact)
        {
            if (Register6 == 0) return 0.0f;
            var noiseIndex = (ushort)((tact - NoisePeriodModified) / 32 / Register6);
            while (noiseIndex > _lastNoiseIndex)
            {
                _noiseSeed = (_noiseSeed * 2 + 1) ^ (((_noiseSeed >> 16) ^ (_noiseSeed >> 13)) & 1);
                _lastNoiseIndex++;
            }
            return ((_noiseSeed >> 16) & 1) == 0 ? 0.0f : 1.0f;
        }

        /// <summary>
        /// Gets the effective amplitude of Channel A
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for</param>
        /// <returns>Sample value</returns>
        public float GetAmplitudeA(long tact)
        {
            return UseEnvelopeA ? GetEnvelopeValue(tact) : s_Amplitudes[AmplitudeA];
        }

        /// <summary>
        /// Gets the effective amplitude of Channel B
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for</param>
        /// <returns>Sample value</returns>
        public float GetAmplitudeB(long tact)
        {
            return UseEnvelopeB ? GetEnvelopeValue(tact) : s_Amplitudes[AmplitudeB];
        }

        /// <summary>
        /// Gets the effective amplitude of Channel C
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for</param>
        /// <returns>Sample value</returns>
        public float GetAmplitudeC(long tact)
        {
            return UseEnvelopeC ? GetEnvelopeValue(tact) : s_Amplitudes[AmplitudeC];
        }

        /// <summary>
        /// Gets the current value of envelope multiplier
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for</param>
        /// <returns>Envelope aplitude</returns>
        public float GetEnvelopeValue(long tact)
        {
            if (EnvelopePeriod == 0) return 0.0f;

            // --- Lenght of a period in CPU tacts
            var periodLength = EnvelopePeriod << 9;

            // --- #of envelope period we're in
            var periodCount = (tact - EnvelopePeriodModified) / periodLength;

            // --- index of amplitude (0-15) within the current period
            var periodPhase = (tact - EnvelopePeriodModified) % periodLength * 16 / periodLength;

            // --- We're in the very first period
            if (periodCount == 0)
            {
                return AttackFlag ? s_Amplitudes[periodPhase] : s_Amplitudes[15 - periodPhase];
            }

            // --- Process the shape
            if (Register13 <= 7 || Register13 == 9 || Register13 == 15) return 0.0f;
            if (Register13 == 11 || Register13 == 13) return 1.0f;
            if (Register13 == 12) return s_Amplitudes[periodPhase];
            if (Register13 == 8) return s_Amplitudes[15 - periodPhase];
            return Register13 == 14
                ? (periodCount % 2 == 0 ? s_Amplitudes[periodPhase] : s_Amplitudes[15 - periodPhase])
                : (periodCount % 2 == 1 ? s_Amplitudes[periodPhase] : s_Amplitudes[15 - periodPhase]);
        }

        /// <summary>
        /// Gest the state of the PSG
        /// </summary>
        /// <returns></returns>
        public (PsgRegister[] regs, int noiseSeed, ushort lastNoiseIndex) GetState() 
            => (_regs,_noiseSeed, _lastNoiseIndex);

        /// <summary>
        /// Sets the state of the PSG
        /// </summary>
        /// <param name="regs"></param>
        /// <param name="noiseSeed"></param>
        /// <param name="lastNoiseIndex"></param>
        public void SetState(PsgRegister[] regs, int noiseSeed, ushort lastNoiseIndex)
        {
            _regs = regs;
            _noiseSeed = noiseSeed;
            _lastNoiseIndex = lastNoiseIndex;
        }

        /// <summary>
        /// This structure defines the information about a PSG register
        /// </summary>
        public struct PsgRegister
        {
            public byte Value;
            public long ModifiedTact;
        }
    }
}