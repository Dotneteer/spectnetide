using System;
using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Tape;
using Spect.Net.SpectrumEmu.Tape.Tzx;

namespace Spect.Net.SpectrumEmu.Test.Tape
{
    /// <summary>
    /// This class provides helper functions for testing TzxPlayer
    /// </summary>
    public static class TzxPlayerHelper
    {
        /// <summary>
        /// Folder for the test TZX files
        /// </summary>
        private const string RESOURCE_FOLDER = "TzxResources";

        /// <summary>
        /// Creates a new player for the specified resource
        /// </summary>
        /// <param name="tzxResource"></param>
        /// <returns></returns>
        public static TzxPlayer CreatePlayer(string tzxResource)
        {
            var tzxReader = GetResourceReader("JetSetWilly.tzx");
            var player = new TzxPlayer(tzxReader);
            player.ReadContent();
            return player;
        }

        /// <summary>
        /// Reads the data byte from the current playback position
        /// </summary>
        /// <param name="block">Block to play back</param>
        /// <returns>Byte read</returns>
        public static byte ReadNextByte(this TzxStandardSpeedDataBlock block)
        {
            const int BIT_0_PL = TzxStandardSpeedDataBlock.BIT_0_PL;
            const int BIT_1_PL = TzxStandardSpeedDataBlock.BIT_1_PL;
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
        public static void CompleteBlock(this TzxStandardSpeedDataBlock block)
        {
            const int PILOT_PL = TzxStandardSpeedDataBlock.PILOT_PL;
            const int HEADER_PILOT_COUNT = TzxStandardSpeedDataBlock.HEADER_PILOT_COUNT;
            const int SYNC_1_PL = TzxStandardSpeedDataBlock.SYNC_1_PL;
            const int SYNC_2_PL = TzxStandardSpeedDataBlock.SYNC_2_PL;
            const ulong PILOT_END = PILOT_PL * HEADER_PILOT_COUNT;
            const int PAUSE_MS = TzxStandardSpeedDataBlock.PAUSE_MS;

            var start = block.StartTact;
            // --- Skip all pilot pulses + the first sync pulse
            for (ulong pos = 0; pos < PILOT_END + SYNC_1_PL; pos += 50)
            {
                block.GetEarBit(start + pos);
            }
            
            // --- Skip the second sync pulse
            for (var pos = PILOT_END + SYNC_1_PL + 50; pos < PILOT_END + SYNC_1_PL + SYNC_2_PL; pos += 50)
            {
                block.GetEarBit(start + pos);
            }

            // --- Play back the data
            for (var i = 0; i < block.DataLenght; i++)
            {
                block.ReadNextByte();
            }

            // --- Play back the pause
            var nextTact = block.LastTact;
            for (var pos = nextTact; pos < nextTact + (ulong)PAUSE_MS * block.PauseAfter + 100; pos += 50)
            {
                block.GetEarBit(pos);
            }
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