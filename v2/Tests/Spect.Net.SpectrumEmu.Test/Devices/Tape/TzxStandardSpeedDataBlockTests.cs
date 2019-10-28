using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;

// ReSharper disable PossibleNullReferenceException

namespace Spect.Net.SpectrumEmu.Test.Devices.Tape
{
    [TestClass]
    public class TzxStandardSpeedDataBlockTests
    {
        [TestMethod]
        public void FirstPilotPulseGenerationWorks()
        {
            // --- Arrange
            const int PILOT_PL = TapeDataBlockPlayer.PILOT_PL;

            const long START = 123456789L;
            var player = TzxPlayerHelper.CreatePlayer("JetSetWilly.tzx");
            player.InitPlay(START);

            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;

            // --- Act/Assert
            for (long pos = 0; pos < PILOT_PL; pos += 50)
            {
                var earBit = block.GetEarBit(START + pos);
                earBit.ShouldBeTrue();
                block.PlayPhase.ShouldBe(PlayPhase.Pilot);
            }
        }

        [TestMethod]
        public void SecondPilotPulseGenerationWorks()
        {
            // --- Arrange
            const int PILOT_PL = TapeDataBlockPlayer.PILOT_PL;

            const long START = 123456789L;
            var player = TzxPlayerHelper.CreatePlayer("JetSetWilly.tzx");
            player.InitPlay(START);

            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;

            // --- Skip EAR bits of the first pluse
            for (long pos = 0; pos < PILOT_PL; pos += 50)
            {
                block.GetEarBit(START + pos);
            }

            // --- Act/Assert
            for (long pos = PILOT_PL; pos < 2 * PILOT_PL; pos += 50)
            {
                var earBit = block.GetEarBit(START + pos);
                earBit.ShouldBeFalse();
                block.PlayPhase.ShouldBe(PlayPhase.Pilot);
            }
        }

        [TestMethod]
        public void InternalPilotPulseGenerationWorksAsExpected()
        {
            TestPilotPulsePlayback(872);
            TestPilotPulsePlayback(1011);
            TestPilotPulsePlayback(3024);
            TestPilotPulsePlayback(8001);
        }

        [TestMethod]
        public void LastPilotPulseGenerationWorksAsExpected()
        {
            const int HEADER_PILOT_COUNT = TapeDataBlockPlayer.HEADER_PILOT_COUNT;
            TestPilotPulsePlayback(HEADER_PILOT_COUNT);
        }

        [TestMethod]
        public void FirstSyncPulseGenerationWorksAsExpected()
        {
            // --- Arrange
            const int PILOT_PL = TapeDataBlockPlayer.PILOT_PL;
            const int HEADER_PILOT_COUNT = TapeDataBlockPlayer.HEADER_PILOT_COUNT;
            const int SYNC_1_PL = TapeDataBlockPlayer.SYNC_1_PL;
            const long PILOT_END = PILOT_PL * HEADER_PILOT_COUNT;
            const long START = 123456789L;

            var player = TzxPlayerHelper.CreatePlayer("JetSetWilly.tzx");
            player.InitPlay(START);

            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;

            // --- Skip all pilot pulses
            for (long pos = 0; pos < PILOT_END; pos += 50)
            {
                block.GetEarBit(START + pos);
            }

            // --- Act/Assert
            for (var pos = PILOT_END + 50; pos < PILOT_END + SYNC_1_PL; pos += 50)
            {
                var earBit = block.GetEarBit(START + pos);
                earBit.ShouldBeFalse();
                block.PlayPhase.ShouldBe(PlayPhase.Sync);
            }
        }

        [TestMethod]
        public void SecondSyncPulseGenerationWorksAsExpected()
        {
            // --- Arrange
            const int PILOT_PL = TapeDataBlockPlayer.PILOT_PL;
            const int HEADER_PILOT_COUNT = TapeDataBlockPlayer.HEADER_PILOT_COUNT;
            const int SYNC_1_PL = TapeDataBlockPlayer.SYNC_1_PL;
            const int SYNC_2_PL = TapeDataBlockPlayer.SYNC_2_PL;
            const long PILOT_END = PILOT_PL * HEADER_PILOT_COUNT;
            const long START = 123456789L;

            var player = TzxPlayerHelper.CreatePlayer("JetSetWilly.tzx");
            player.InitPlay(START);

            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;

            // --- Skip all pilot pulses + the first sync pulse
            for (long pos = 0; pos < PILOT_END + SYNC_1_PL; pos += 50)
            {
                block.GetEarBit(START + pos);
            }

            // --- Act/Assert
            for (var pos = PILOT_END + SYNC_1_PL + 50; pos < PILOT_END + SYNC_1_PL + SYNC_2_PL; pos += 50)
            {
                var earBit = block.GetEarBit(START + pos);
                earBit.ShouldBeTrue();
                block.PlayPhase.ShouldBe(PlayPhase.Sync);
            }
        }

        [TestMethod]
        public void SecondSyncPulseGenerationMovesToData()
        {
            // --- Arrange
            const int PILOT_PL = TapeDataBlockPlayer.PILOT_PL;
            const int HEADER_PILOT_COUNT = TapeDataBlockPlayer.HEADER_PILOT_COUNT;
            const int SYNC_1_PL = TapeDataBlockPlayer.SYNC_1_PL;
            const int SYNC_2_PL = TapeDataBlockPlayer.SYNC_2_PL;
            const long PILOT_END = PILOT_PL * HEADER_PILOT_COUNT;
            const long START = 123456789L;
            var block = ReadAndPositionToDataSection();

            // --- Act
            var earBit = block.GetEarBit(START + PILOT_END + SYNC_1_PL + SYNC_2_PL + 50);

            // --- Assert
            earBit.ShouldBeFalse();
            block.PlayPhase.ShouldBe(PlayPhase.Data);
        }

        [TestMethod]
        public void HeaderBytePulseGenerationWorksAsExpected()
        {
            // --- Arrange
            const long START = 123456789L;
            var block = ReadAndPositionToByte(START, 0);
            
            // --- Act
            var byte0 = block.ReadNextByte();

            // --- Assert
            byte0.ShouldBe((byte)0);
        }

        [TestMethod]
        public void AllHeaderBytesPlaybackWorksAsExpected()
        {
            // --- Arrange
            const long START = 123456789L;
            var block = ReadAndPositionToByte(START, 0);

            // --- Act/Assert
            for (var i = 0; i < block.DataLength; i++)
            {
                var dataByte = block.ReadNextByte();
                dataByte.ShouldBe(block.Data[i]);
            }
            block.PlayPhase.ShouldBe(PlayPhase.TermSync);
        }

        [TestMethod]
        public void BlockPausePlaybackWorksAsExpected()
        {
            // --- Arrange
            const int PAUSE_MS = TapeDataBlockPlayer.PAUSE_MS;
            const int TERM_SYNC = TapeDataBlockPlayer.TERM_SYNC;
            const long START = 123456789L;

            var block = ReadAndPositionToByte(START, 0);
            for (var i = 0; i < block.DataLength; i++)
            {
                block.ReadNextByte();
            }
            var nextTact = block.LastTact;
            for (var pos = nextTact; pos < nextTact + TERM_SYNC; pos += 50)
            {
                block.GetEarBit(pos).ShouldBeFalse();
            }
            nextTact = block.LastTact + 50;

            // --- Act/Assert
            for (var pos = nextTact; pos < nextTact + PAUSE_MS * block.PauseAfter; pos += 50)
            {
                block.GetEarBit(pos).ShouldBeTrue();
            }
            block.GetEarBit(block.LastTact + 100).ShouldBeTrue();
            block.PlayPhase.ShouldBe(PlayPhase.Completed);
        }

        #region Helper methods

        /// <summary>
        /// Tests a pilot pulse
        /// </summary>
        /// <param name="pulseIndex">The index of the pilor pulse</param>
        private void TestPilotPulsePlayback(int pulseIndex)
        {
            // --- Arrange
            const int PILOT_PL = TapeDataBlockPlayer.PILOT_PL;
            const long START = 123456789L;

            var player = TzxPlayerHelper.CreatePlayer("JetSetWilly.tzx");
            player.InitPlay(START);

            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;
            var prevPulseEnd = PILOT_PL * (pulseIndex - 1);

            // --- Skip EAR bits of the previous pulses
            for (long pos = 0; pos < prevPulseEnd; pos += 50)
            {
                block.GetEarBit(START + pos);
            }

            // --- Act/Assert
            for (var pos = prevPulseEnd + 50; pos < prevPulseEnd + PILOT_PL; pos += 50)
            {
                var earBit = block.GetEarBit(START + pos);
                earBit.ShouldBe(pulseIndex % 2 == 1);
                block.PlayPhase.ShouldBe(PlayPhase.Pilot);
            }
        }

        /// <summary>
        /// Reads nad positions a standard speed data block to its data area
        /// </summary>
        /// <returns>Standard speed data block</returns>
        private static TzxStandardSpeedDataBlock ReadAndPositionToDataSection()
        {
            const int PILOT_PL = TapeDataBlockPlayer.PILOT_PL;
            const int HEADER_PILOT_COUNT = TapeDataBlockPlayer.HEADER_PILOT_COUNT;
            const int SYNC_1_PL = TapeDataBlockPlayer.SYNC_1_PL;
            const int SYNC_2_PL = TapeDataBlockPlayer.SYNC_2_PL;
            const long PILOT_END = PILOT_PL * HEADER_PILOT_COUNT;
            const long START = 123456789L;

            var player = TzxPlayerHelper.CreatePlayer("JetSetWilly.tzx");
            player.InitPlay(START);

            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;

            // --- Skip all pilot pulses + the first sync pulse
            for (long pos = 0; pos < PILOT_END + SYNC_1_PL; pos += 50)
            {
                block.GetEarBit(START + pos);
            }
            // --- Skip the second sync pulse
            for (var pos = PILOT_END + SYNC_1_PL + 50; pos < PILOT_END + SYNC_1_PL + SYNC_2_PL; pos += 50)
            {
                block.GetEarBit(START + pos);
            }
            return block;
        }

        /// <summary>
        /// Reads and positions a standard speed data block to the specified byte index in its data area
        /// </summary>
        /// <param name="start">Start tact</param>
        /// <param name="byteIndex">Byte index to position</param>
        /// <returns>Standard speed data block</returns>
        private static TzxStandardSpeedDataBlock ReadAndPositionToByte(long start, int byteIndex)
        {
            const int PILOT_PL = TapeDataBlockPlayer.PILOT_PL;
            const int HEADER_PILOT_COUNT = TapeDataBlockPlayer.HEADER_PILOT_COUNT;
            const int SYNC_1_PL = TapeDataBlockPlayer.SYNC_1_PL;
            const int SYNC_2_PL = TapeDataBlockPlayer.SYNC_2_PL;
            const long PILOT_END = PILOT_PL * HEADER_PILOT_COUNT;
            const int BIT_0_PL = TapeDataBlockPlayer.BIT_0_PL;
            const int BIT_1_PL = TapeDataBlockPlayer.BIT_1_PL;
            const long DATA_STARTS = PILOT_END + SYNC_1_PL + SYNC_2_PL;

            var block = ReadAndPositionToDataSection();
            var length = 0;
            for (var i = 0; i < byteIndex; i++)
            {
                var bits = block.Data[i];
                for (var j = 0; j < 8; j++)
                {
                    length += 2 * ((bits & (1 << j)) == 0 ? BIT_0_PL : BIT_1_PL);
                }
            }
            for (var pos = DATA_STARTS; pos < DATA_STARTS + length; pos += 50)
            {
                block.GetEarBit(start + pos);
            }
            return block;
        }

        #endregion
    }
}
