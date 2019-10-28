﻿using System;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tap;

namespace Spect.Net.SpectrumEmu.Test.Devices.Tape
{
    /// <summary>
    /// This class provides helper functions for testing TzxPlayer
    /// </summary>
    public static class TapPlayerHelper
    {
        /// <summary>
        /// Folder for the test TAP files
        /// </summary>
        private const string RESOURCE_FOLDER = "TapResources";

        /// <summary>
        /// Creates a new player for the specified resource
        /// </summary>
        /// <param name="tapResource"></param>
        /// <returns></returns>
        public static TapPlayer CreatePlayer(string tapResource)
        {
            var tzxReader = GetResourceReader(tapResource);
            var player = new TapPlayer(tzxReader);
            player.ReadContent();
            return player;
        }

        /// <summary>
        /// Reads the data byte from the current playback position
        /// </summary>
        /// <param name="block">Block to play back</param>
        /// <returns>Byte read</returns>
        public static byte ReadNextByte(this TapDataBlock block)
        {
            const int BIT_0_PL = TapeDataBlockPlayer.BIT_0_PL;
            const int BIT_1_PL = TapeDataBlockPlayer.BIT_1_PL;
            const int STEP = 50;

            var nextTact = block.LastTact + STEP;
            byte bits = 0x00;
            for (var i = 0; i < 8; i++)
            {
                // --- Wait for the high EAR bit
                var samplesLow = 0;
                while (!block.GetEarBit(nextTact))
                {
                    samplesLow++;
                    nextTact += STEP;
                }

                // --- Wait for the low EAR bit
                var samplesHigh = 0;
                while (block.GetEarBit(nextTact) && samplesHigh < BIT_1_PL / STEP + 2)
                {
                    samplesHigh++;
                    nextTact += STEP;
                }

                bits <<= 1;
                if (samplesLow > (BIT_0_PL + BIT_1_PL) / 2 / STEP
                    && samplesHigh > (BIT_0_PL + BIT_1_PL) / 2 / STEP)
                {
                    bits++;
                }
            }
            return bits;
        }

        /// <summary>
        /// Playes back a standard speed data block entirely
        /// </summary>
        /// <param name="block">Data block to play back</param>
        public static void CompleteBlock(this TapDataBlock block)
        {
            const int PILOT_PL = TapeDataBlockPlayer.PILOT_PL;
            const int HEADER_PILOT_COUNT = TapeDataBlockPlayer.HEADER_PILOT_COUNT;
            const int SYNC_1_PL = TapeDataBlockPlayer.SYNC_1_PL;
            const int SYNC_2_PL = TapeDataBlockPlayer.SYNC_2_PL;
            const long PILOT_END = PILOT_PL * HEADER_PILOT_COUNT;
            const int TERM_SYNC = TapeDataBlockPlayer.TERM_SYNC;
            const int PAUSE_MS = TapeDataBlockPlayer.PAUSE_MS;

            var start = block.StartTact;
            // --- Skip all pilot pulses + the first sync pulse
            for (long pos = 0; pos < PILOT_END + SYNC_1_PL; pos += 50)
            {
                block.GetEarBit(start + pos);
            }
            
            // --- Skip the second sync pulse
            for (var pos = PILOT_END + SYNC_1_PL + 50; pos < PILOT_END + SYNC_1_PL + SYNC_2_PL; pos += 50)
            {
                block.GetEarBit(start + pos);
            }

            // --- Play back the data
            for (var i = 0; i < block.Data.Length; i++)
            {
                ReadNextByte(block);
            }

            // --- Play back the terminating sync
            var nextTact = block.LastTact;
            for (var pos = nextTact; pos < nextTact + TERM_SYNC + 50; pos += 50)
            {
                block.GetEarBit(pos);
            }

            // --- Play back the pause
            nextTact = block.LastTact;
            for (var pos = nextTact; pos < nextTact + PAUSE_MS * block.PauseAfter + 100; pos += 50)
            {
                block.GetEarBit(pos);
            }
        }

        /// <summary>
        /// Playes back a standard speed data block entirely
        /// </summary>
        /// <param name="block">Data block to play back</param>
        /// <remarks>Last tact position</remarks>
        public static long ReadUntilPause(this TapDataBlock block)
        {
            const int PILOT_PL = TapeDataBlockPlayer.PILOT_PL;
            const int HEADER_PILOT_COUNT = TapeDataBlockPlayer.HEADER_PILOT_COUNT;
            const int SYNC_1_PL = TapeDataBlockPlayer.SYNC_1_PL;
            const int SYNC_2_PL = TapeDataBlockPlayer.SYNC_2_PL;
            const long PILOT_END = PILOT_PL * HEADER_PILOT_COUNT;
            const int TERM_SYNC = TapeDataBlockPlayer.TERM_SYNC;
            const int PAUSE_MS = TapeDataBlockPlayer.PAUSE_MS;

            var start = block.StartTact;
            // --- Skip all pilot pulses + the first sync pulse
            for (long pos = 0; pos < PILOT_END + SYNC_1_PL; pos += 50)
            {
                block.GetEarBit(start + pos);
            }

            // --- Skip the second sync pulse
            for (var pos = PILOT_END + SYNC_1_PL + 50; pos < PILOT_END + SYNC_1_PL + SYNC_2_PL; pos += 50)
            {
                block.GetEarBit(start + pos);
            }

            // --- Play back the data
            for (var i = 0; i < block.Data.Length; i++)
            {
                ReadNextByte(block);
            }

            // --- Play back the terminating sync
            var nextTact = block.LastTact;
            for (var pos = nextTact; pos < nextTact + TERM_SYNC + 50; pos += 50)
            {
                block.GetEarBit(pos);
            }

            // --- Play back the pause
            var lastPos = block.LastTact + PAUSE_MS * block.PauseAfter + 100;
            block.GetEarBit(lastPos);
            return lastPos;
        }

        /// <summary>
        /// Obtains the specified resource stream
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        private static BinaryReader GetResourceReader(string resourceName)
        {
            var callingAsm = Assembly.GetCallingAssembly();
            var resMan = GetFileResource(callingAsm, resourceName);
            if (resMan == null)
            {
                throw new InvalidOperationException($"Input stream {resourceName} not found.");
            }
            var reader = new BinaryReader(resMan);
            return reader;
        }

        /// <summary>
        /// Obtains the specified resource stream ot the given assembly
        /// </summary>
        /// <param name="asm">Assembly to get the resource stream from</param>
        /// <param name="resourceName">Resource name</param>
        private static Stream GetFileResource(Assembly asm, string resourceName)
        {
            var resourceFullName = $"{asm.GetName().Name}.{RESOURCE_FOLDER}.{resourceName}";
            return asm.GetManifestResourceStream(resourceFullName);
        }
    }
}