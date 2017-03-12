using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Tape;
using Spect.Net.SpectrumEmu.Tape.Tzx;
using Spect.Net.SpectrumEmu.Test.Helpers;
// ReSharper disable PossibleNullReferenceException

namespace Spect.Net.SpectrumEmu.Test.Tape
{
    [TestClass]
    public class TzxStandardSpeedDataBlockTests
    {
        [TestMethod]
        public void FirstPilotPulseGenerationWorks()
        {
            // --- Arrange
            const int PILOT_PL = TzxStandardSpeedDataBlock.PILOT_PL;

            const ulong START = 123456789ul;
            var tzxReader = TzxHelper.GetResourceReader("JetSetWilly.tzx");
            var player = new TzxPlayer(tzxReader);
            player.ReadContent();
            player.InitPlay(START);
            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;

            // --- Act/Assert
            for (ulong pos = 0; pos < PILOT_PL; pos += 50)
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
            const int PILOT_PL = TzxStandardSpeedDataBlock.PILOT_PL;

            const ulong START = 123456789ul;
            var tzxReader = TzxHelper.GetResourceReader("JetSetWilly.tzx");
            var player = new TzxPlayer(tzxReader);
            player.ReadContent();
            player.InitPlay(START);

            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;

            // --- Skip EAR bits of the first pluse
            for (ulong pos = 0; pos < PILOT_PL; pos += 50)
            {
                block.GetEarBit(START + pos);
            }

            // --- Act/Assert
            for (ulong pos = PILOT_PL; pos < 2 * PILOT_PL; pos += 50)
            {
                var earBit = block.GetEarBit(START + pos);
                earBit.ShouldBeFalse();
                block.PlayPhase.ShouldBe(PlayPhase.Pilot);
            }
        }

        [TestMethod]
        public void InternalPilotPulseGenerationWorksAsExpected()
        {
            TestPilotPulse(872);
            TestPilotPulse(1011);
            TestPilotPulse(3024);
            TestPilotPulse(8001);
        }

        [TestMethod]
        public void LastPilotPulseGenerationWorksAsExpected()
        {
            const int HEADER_PILOT_COUNT = TzxStandardSpeedDataBlock.HEADER_PILOT_COUNT;
            TestPilotPulse(HEADER_PILOT_COUNT);
        }

        [TestMethod]
        public void FirstSyncPulseGenerationWorksAsExpected()
        {
            // --- Arrange
            const int PILOT_PL = TzxStandardSpeedDataBlock.PILOT_PL;
            const int HEADER_PILOT_COUNT = TzxStandardSpeedDataBlock.HEADER_PILOT_COUNT;
            const int SYNC_1_PL = TzxStandardSpeedDataBlock.SYNC_1_PL;
            const ulong PILOT_END = (ulong)(PILOT_PL * HEADER_PILOT_COUNT);
            const ulong START = 123456789ul;

            var tzxReader = TzxHelper.GetResourceReader("JetSetWilly.tzx");
            var player = new TzxPlayer(tzxReader);
            player.ReadContent();
            player.InitPlay(START);

            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;

            // --- Skip all pilot pulses
            for (ulong pos = 0; pos < PILOT_END; pos += 50)
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
            const int PILOT_PL = TzxStandardSpeedDataBlock.PILOT_PL;
            const int HEADER_PILOT_COUNT = TzxStandardSpeedDataBlock.HEADER_PILOT_COUNT;
            const int SYNC_1_PL = TzxStandardSpeedDataBlock.SYNC_1_PL;
            const int SYNC_2_PL = TzxStandardSpeedDataBlock.SYNC_2_PL;
            const ulong PILOT_END = (ulong)(PILOT_PL * HEADER_PILOT_COUNT);
            const ulong START = 123456789ul;

            var tzxReader = TzxHelper.GetResourceReader("JetSetWilly.tzx");
            var player = new TzxPlayer(tzxReader);
            player.ReadContent();
            player.InitPlay(START);

            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;

            // --- Skip all pilot pulses + the first sync pulse
            for (ulong pos = 0; pos < PILOT_END + SYNC_1_PL; pos += 50)
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
            const int PILOT_PL = TzxStandardSpeedDataBlock.PILOT_PL;
            const int HEADER_PILOT_COUNT = TzxStandardSpeedDataBlock.HEADER_PILOT_COUNT;
            const int SYNC_1_PL = TzxStandardSpeedDataBlock.SYNC_1_PL;
            const int SYNC_2_PL = TzxStandardSpeedDataBlock.SYNC_2_PL;
            const ulong PILOT_END = (ulong)(PILOT_PL * HEADER_PILOT_COUNT);
            const ulong START = 123456789ul;

            var tzxReader = TzxHelper.GetResourceReader("JetSetWilly.tzx");
            var player = new TzxPlayer(tzxReader);
            player.ReadContent();
            player.InitPlay(START);

            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;

            // --- Skip all pilot pulses + the first sync pulse
            for (ulong pos = 0; pos < PILOT_END + SYNC_1_PL; pos += 50)
            {
                block.GetEarBit(START + pos);
            }
            // --- Skip the second sync pulse
            for (var pos = PILOT_END + SYNC_1_PL + 50; pos < PILOT_END + SYNC_1_PL + SYNC_2_PL; pos += 50)
            {
                block.GetEarBit(START + pos);
            }

            // --- Act
            var earBit = block.GetEarBit(START + PILOT_END + SYNC_1_PL + SYNC_2_PL + 50);

            // --- Assert
            earBit.ShouldBeFalse();
            block.PlayPhase.ShouldBe(PlayPhase.Data);
        }

        #region Helper methods

        private void TestPilotPulse(int pulseIndex)
        {
            // --- Arrange
            const int PILOT_PL = TzxStandardSpeedDataBlock.PILOT_PL;
            const ulong START = 123456789ul;

            var tzxReader = TzxHelper.GetResourceReader("JetSetWilly.tzx");
            var player = new TzxPlayer(tzxReader);
            player.ReadContent();
            player.InitPlay(START);

            // --- This is a standard ROM header data block
            var block = player.CurrentBlock as TzxStandardSpeedDataBlock;
            var prevPulseEnd = (ulong) (PILOT_PL * (pulseIndex - 1));

            // --- Skip EAR bits of the previous pulses
            for (ulong pos = 0; pos < prevPulseEnd; pos += 50)
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

        #endregion
    }
}
