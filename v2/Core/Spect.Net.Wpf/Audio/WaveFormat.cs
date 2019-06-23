﻿using System;
using System.Runtime.InteropServices;

namespace Spect.Net.Wpf.Audio
{
    /// <summary>
    /// Represents a Wave file format
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    public class WaveFormat
    {
        /// <summary>format type</summary>
        protected WaveFormatEncoding waveFormatTag;
        /// <summary>number of channels</summary>
        protected short channels;
        /// <summary>sample rate</summary>
        protected int sampleRate;
        /// <summary>for buffer estimation</summary>
        protected int averageBytesPerSecond;
        /// <summary>block size of data</summary>
        protected short blockAlign;
        /// <summary>number of bits per sample of mono data</summary>
        protected short bitsPerSample;
        /// <summary>number of following bytes</summary>
        protected short extraSize;

        /// <summary>
        /// Creates a new PCM 44.1Khz stereo 16 bit format
        /// </summary>
        public WaveFormat() : this(44100, 16, 2)
        {
        }

        /// <summary>
        /// Creates a new 16 bit wave format with the specified sample
        /// rate and channel count
        /// </summary>
        /// <param name="sampleRate">Sample Rate</param>
        /// <param name="channels">Number of channels</param>
        public WaveFormat(int sampleRate, int channels)
            : this(sampleRate, 16, channels)
        {
        }

        /// <summary>
        /// Gets the size of a wave buffer equivalent to the latency in milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns></returns>
        public int ConvertLatencyToByteSize(int milliseconds)
        {
            var bytes = (int)((AverageBytesPerSecond / 1000.0) * milliseconds);
            if ((bytes % BlockAlign) != 0)
            {
                // Return the upper BlockAligned
                bytes = bytes + BlockAlign - (bytes % BlockAlign);
            }
            return bytes;
        }

        /// <summary>
        /// Creates a new PCM format with the specified sample rate, bit depth and channels
        /// </summary>
        public WaveFormat(int rate, int bits, int channels)
        {
            if (channels < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(channels), @"Channels must be 1 or greater");
            }
            // minimum 16 bytes, sometimes 18 for PCM
            waveFormatTag = WaveFormatEncoding.Pcm;
            this.channels = (short)channels;
            sampleRate = rate;
            bitsPerSample = (short)bits;
            extraSize = 0;

            blockAlign = (short)(channels * (bits / 8));
            averageBytesPerSecond = sampleRate * blockAlign;
        }

        /// <summary>
        /// Creates a new 32 bit IEEE floating point wave format
        /// </summary>
        /// <param name="sampleRate">sample rate</param>
        /// <param name="channels">number of channels</param>
        public static WaveFormat CreateIeeeFloatWaveFormat(int sampleRate, int channels)
        {
            var wf = new WaveFormat
            {
                waveFormatTag = WaveFormatEncoding.IeeeFloat,
                channels = (short) channels,
                bitsPerSample = 32,
                sampleRate = sampleRate,
                blockAlign = (short) (4 * channels)
            };
            wf.averageBytesPerSecond = sampleRate * wf.blockAlign;
            wf.extraSize = 0;
            return wf;
        }

        /// <summary>
        /// Reports this WaveFormat as a string
        /// </summary>
        /// <returns>String describing the wave format</returns>
        public override string ToString()
        {
            switch (waveFormatTag)
            {
                case WaveFormatEncoding.Pcm:
                case WaveFormatEncoding.Extensible:
                    // extensible just has some extra bits after the PCM header
                    return $"{bitsPerSample} bit PCM: {sampleRate / 1000}kHz {channels} channels";
                default:
                    return waveFormatTag.ToString();
            }
        }

        /// <summary>
        /// Compares with another WaveFormat object
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if the objects are the same</returns>
        public override bool Equals(object obj)
        {
            var other = obj as WaveFormat;
            if (other != null)
            {
                return waveFormatTag == other.waveFormatTag &&
                       channels == other.channels &&
                       sampleRate == other.sampleRate &&
                       averageBytesPerSecond == other.averageBytesPerSecond &&
                       blockAlign == other.blockAlign &&
                       bitsPerSample == other.bitsPerSample;
            }
            return false;
        }

        /// <summary>
        /// Provides a Hashcode for this WaveFormat
        /// </summary>
        /// <returns>A hashcode</returns>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return (int)waveFormatTag ^
                   channels ^
                   sampleRate ^
                   averageBytesPerSecond ^
                   blockAlign ^
                   bitsPerSample;
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        /// <summary>
        /// Returns the encoding type used
        /// </summary>
        public WaveFormatEncoding Encoding => waveFormatTag;

        /// <summary>
        /// Returns the number of channels (1=mono,2=stereo etc)
        /// </summary>
        public int Channels => channels;

        /// <summary>
        /// Returns the sample rate (samples per second)
        /// </summary>
        public int SampleRate => sampleRate;

        /// <summary>
        /// Returns the average number of bytes used per second
        /// </summary>
        public int AverageBytesPerSecond => averageBytesPerSecond;

        /// <summary>
        /// Returns the block alignment
        /// </summary>
        public virtual int BlockAlign => blockAlign;

        /// <summary>
        /// Returns the number of bits per sample (usually 16 or 32, sometimes 24 or 8)
        /// Can be 0 for some codecs
        /// </summary>
        public int BitsPerSample => bitsPerSample;

        /// <summary>
        /// Returns the number of extra bytes used by this waveformat. Often 0,
        /// except for compressed formats which store extra data after the WAVEFORMATEX header
        /// </summary>
        public int ExtraSize => extraSize;
    }
}