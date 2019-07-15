using Spect.Net.SpectrumEmu.Devices.Sound;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices.Sound
{
    /// <summary>
    /// Represents the state of the PSG (Programmable Sound Generator).
    /// </summary>
    public interface IPsgState
    {
        /// <summary>
        /// Channel A Fine Tune Register.
        /// </summary>
        byte Register0 { get; }

        /// <summary>
        /// Channel A Coarse Tune Register.
        /// </summary>
        byte Register1 { get; }

        /// <summary>
        /// Channel A Value.
        /// </summary>
        ushort ChannelA { get; }

        /// <summary>
        /// CPU tact when Channel A value was last time modified.
        /// </summary>
        long ChannelAModified { get; }

        /// <summary>
        /// Channel B Fine Tune Register.
        /// </summary>
        byte Register2 { get; }

        /// <summary>
        /// Channel B Coarse Tune Register.
        /// </summary>
        byte Register3 { get; }

        /// <summary>
        /// Channel B Value.
        /// </summary>
        ushort ChannelB { get; }

        /// <summary>
        /// CPU tact when Channel B value was last time modified.
        /// </summary>
        long ChannelBModified { get; }

        /// <summary>
        /// Channel C Fine Tune Register.
        /// </summary>
        byte Register4 { get; }

        /// <summary>
        /// Channel C Coarse Tune Register.
        /// </summary>
        byte Register5 { get; }

        /// <summary>
        /// Channel C Value
        /// </summary>
        ushort ChannelC { get; }

        /// <summary>
        /// CPU tact when Channel C value was last time modified.
        /// </summary>
        long ChannelCModified { get; }

        /// <summary>
        /// Noise Period Register
        /// </summary>
        byte Register6 { get; }

        /// <summary>
        /// CPU tact when Noise Period value was last time modified.
        /// </summary>
        long NoisePeriodModified { get; }

        /// <summary>
        /// Mixer Control-I/O Enable Register.
        /// </summary>
        byte Register7 { get; }

        /// <summary>
        /// CPU tact when Mixer register value was last time modified.
        /// </summary>
        long MixerModified { get; }

        /// <summary>
        /// Input is enabled in Register 7.
        /// </summary>
        bool InputEnabled { get; }

        /// <summary>
        /// Tone A is enabled in Register 7.
        /// </summary>
        bool ToneAEnabled { get; }

        /// <summary>
        /// Tone B is enabled in Register 7.
        /// </summary>
        bool ToneBEnabled { get; }

        /// <summary>
        /// Tone C is enabled in Register 7.
        /// </summary>
        bool ToneCEnabled { get; }

        /// <summary>
        /// Noise A is enabled in Register 7.
        /// </summary>
        bool NoiseAEnabled { get; }

        /// <summary>
        /// Noise B is enabled in Register 7.
        /// </summary>
        bool NoiseBEnabled { get; }

        /// <summary>
        /// Noise C is enabled in Register 7.
        /// </summary>
        bool NoiseCEnabled { get; }

        /// <summary>
        /// Amplitude Control A Register.
        /// </summary>
        byte Register8 { get; }

        /// <summary>
        /// Gets the amplitude level of Channel A.
        /// </summary>
        byte AmplitudeA { get; }

        /// <summary>
        /// CPU tact when Amplitude A register value was last time modified.
        /// </summary>
        long AmplitudeAModified { get; }

        /// <summary>
        /// Indicates if envelope mode should be used for Channel A.
        /// </summary>
        bool UseEnvelopeA { get; }

        /// <summary>
        /// Amplitude Control B Register.
        /// </summary>
        byte Register9 { get; }

        /// <summary>
        /// Gets the amplitude level of Channel B.
        /// </summary>
        byte AmplitudeB { get; }

        /// <summary>
        /// CPU tact when Amplitude B register value was last time modified.
        /// </summary>
        long AmplitudeBModified { get; }

        /// <summary>
        /// Indicates if envelope mode should be used for Channel B.
        /// </summary>
        bool UseEnvelopeB { get; }

        /// <summary>
        /// Amplitude Control C Register.
        /// </summary>
        byte Register10 { get; }

        /// <summary>
        /// Gets the amplitude level of Channel C.
        /// </summary>
        byte AmplitudeC { get; }

        /// <summary>
        /// CPU tact when Amplitude C register value was last time modified.
        /// </summary>
        long AmplitudeCModified { get; }

        /// <summary>
        /// Indicates if envelope mode should be used for Channel C.
        /// </summary>
        bool UseEnvelopeC { get; }

        /// <summary>
        /// Envelope Period LSB Register.
        /// </summary>
        byte Register11 { get; }

        /// <summary>
        /// Envelope Period MSB Register.
        /// </summary>
        byte Register12 { get; }

        /// <summary>
        /// CPU tact when Envelope Period value was last time modified.
        /// </summary>
        long EnvelopePeriodModified { get; }

        /// <summary>
        /// Envelope period value.
        /// </summary>
        ushort EnvelopePeriod { get; }

        /// <summary>
        /// Envelope shape register.
        /// </summary>
        byte Register13 { get; }

        /// <summary>
        /// CPU tact when Envelope Shape register value was last time modified.
        /// </summary>
        long EnvelopeShapeModified { get; }

        /// <summary>
        /// Hold flag of the envelope.
        /// </summary>
        bool HoldFlag { get; }

        /// <summary>
        /// Alternate flag of the envelope .
        /// </summary>
        bool AlternateFlag { get; }

        /// <summary>
        /// Attack flag of the envelope.
        /// </summary>
        bool AttackFlag { get; }

        /// <summary>
        /// Continue flag of the envelope.
        /// </summary>
        bool ContinueFlag { get; }

        /// <summary>
        /// I/O Port register A.
        /// </summary>
        byte Register14 { get; }

        /// <summary>
        /// I/O Port register B.
        /// </summary>
        byte Register15 { get; set; }

        /// <summary>
        /// Gets a register by its index.
        /// </summary>
        /// <param name="index">
        /// Register index (should be between 0 and 14).
        /// </param>
        /// <returns>Register value.</returns>
        byte this[int index] { get; }

        /// <summary>
        /// Gets the effective value of Channel A.
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for.</param>
        /// <returns>Sample value.</returns>
        float GetChannelASample(long tact);

        /// <summary>
        /// Gets the effective value of Channel B.
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for.</param>
        /// <returns>Sample value.</returns>
        float GetChannelBSample(long tact);

        /// <summary>
        /// Gets the effective value of Channel C.
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for.</param>
        /// <returns>Sample value.</returns>
        float GetChannelCSample(long tact);

        /// <summary>
        /// Gets the effective value of the Noise Generator.
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for.</param>
        /// <returns>Sample value.</returns>
        float GetNoiseSample(long tact);

        /// <summary>
        /// Gets the effective amplitude of Channel A.
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for.</param>
        /// <returns>Sample value.</returns>
        float GetAmplitudeA(long tact);

        /// <summary>
        /// Gets the effective amplitude of Channel B.
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for.</param>
        /// <returns>Sample value.</returns>
        float GetAmplitudeB(long tact);

        /// <summary>
        /// Gets the effective amplitude of Channel C.
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for.</param>
        /// <returns>Sample value.</returns>
        float GetAmplitudeC(long tact);

        /// <summary>
        /// Gets the current value of envelope multiplier.
        /// </summary>
        /// <param name="tact">CPU tact to get the sample for.</param>
        /// <returns>Envelope amplitude.</returns>
        float GetEnvelopeValue(long tact);

        /// <summary>
        /// Gets the state of the PSG.
        /// </summary>
        /// <returns>The state of the PSG</returns>
        (PsgState.PsgRegister[] regs, int noiseSeed, ushort lastNoiseIndex) GetState();

        /// <summary>
        /// Sets the state of the PSG.
        /// </summary>
        /// <param name="regs">PSG registers</param>
        /// <param name="noiseSeed">Noise seed value</param>
        /// <param name="lastNoiseIndex">Last noise index value</param>
        void SetState(PsgState.PsgRegister[] regs, int noiseSeed, ushort lastNoiseIndex);
    }
}