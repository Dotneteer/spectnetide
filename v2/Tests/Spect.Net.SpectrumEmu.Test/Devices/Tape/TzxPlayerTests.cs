using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;

// ReSharper disable PossibleNullReferenceException

namespace Spect.Net.SpectrumEmu.Test.Devices.Tape
{
    [TestClass]
    public class TzxPlayerTests
    {
        private const string TAPESET = "JetSetWilly.tzx";

        [TestMethod]
        public void TzxFileCanBeReadSuccessfully()
        {
            // --- Act
            var player = TzxPlayerHelper.CreatePlayer(TAPESET);

            // --- Assert
            player.DataBlocks.Count.ShouldBe(9);
            player.DataBlocks[0].ShouldBeOfType<TzxTextDescriptionDataBlock>();
            for (var i = 1; i < 9; i++)
            {
                player.DataBlocks[i].ShouldBeOfType<TzxStandardSpeedDataBlock>();
            }
            player.Eof.ShouldBeFalse();
        }

        [TestMethod]
        public void InitPlayWorksAsExpected()
        {
            // --- Arrange
            var player = TzxPlayerHelper.CreatePlayer(TAPESET);

            // --- Act
            player.InitPlay(100);

            // --- Assert
            player.PlayPhase.ShouldBe(PlayPhase.None);
            player.StartTact.ShouldBe(100);
            player.CurrentBlockIndex.ShouldBe(0);
            player.CurrentBlock.ShouldBeOfType<TzxStandardSpeedDataBlock>();
        }

        [TestMethod]
        public void InitPlayInitializesTheFirstDataBlock()
        {
            // --- Arrange
            var player = TzxPlayerHelper.CreatePlayer(TAPESET);

            // --- Act
            player.InitPlay(100);

            // --- Assert
            var currentBlock = player.CurrentBlock as TzxStandardSpeedDataBlock;
            currentBlock.ShouldNotBeNull();
            currentBlock.PlayPhase.ShouldBe(PlayPhase.Pilot);
            currentBlock.StartTact.ShouldBe(player.StartTact);
            currentBlock.ByteIndex.ShouldBe(0);
            currentBlock.BitMask.ShouldBe((byte)0x80);
        }

        [TestMethod]
        public void PlayMovesToNextBlock()
        {
            // --- Arrange
            var player = TzxPlayerHelper.CreatePlayer(TAPESET);
            player.InitPlay(100);
            var currentBlock = player.CurrentBlock as TzxStandardSpeedDataBlock;

            // --- Act
            var indexBefore = player.CurrentBlockIndex;
            currentBlock.CompleteBlock();
            var lastTact = currentBlock.LastTact;
            player.GetEarBit(lastTact);
            var indexAfter = player.CurrentBlockIndex;

            // --- Assert
            indexBefore.ShouldBe(0);
            indexAfter.ShouldBe(1);
            currentBlock = player.CurrentBlock as TzxStandardSpeedDataBlock;
            currentBlock.ShouldNotBeNull();
            currentBlock.PlayPhase.ShouldBe(PlayPhase.Pilot);
            currentBlock.StartTact.ShouldBe(lastTact);
            currentBlock.ByteIndex.ShouldBe(0);
            currentBlock.BitMask.ShouldBe((byte)0x80);
        }

        [TestMethod]
        public void PlaySetsEofAtTheLastPlayableBlock()
        {
            // --- Arrange
            var player = TzxPlayerHelper.CreatePlayer(TAPESET);
            player.InitPlay(100);
            for (var i = 0; i < 8; i++) // Block 7 is the last
            {
                var currentBlock = player.CurrentBlock as TzxStandardSpeedDataBlock;
                currentBlock.CompleteBlock();
                var lastTact = currentBlock.LastTact;
                player.GetEarBit(lastTact);
            }

            // --- Act
            var lastBlock = player.CurrentBlock as TzxStandardSpeedDataBlock;
            var lastPos = lastBlock.ReadUntilPause();
            player.GetEarBit(lastPos);
            
            // --- Assert
            player.Eof.ShouldBeTrue();
        }

        [TestMethod]
        public void PlayDoesNotSetEofUntilEnd()
        {
            // --- Arrange
            var player = TzxPlayerHelper.CreatePlayer(TAPESET);
            player.InitPlay(100);
            while (player.CurrentBlockIndex < 6) // Block 6 is a middle block
            {
                var currentBlock = player.CurrentBlock as TzxStandardSpeedDataBlock;
                currentBlock.CompleteBlock();
                var lastTact = currentBlock.LastTact;
                player.GetEarBit(lastTact);
            }

            // --- Act
            var lastBlock = player.CurrentBlock as TzxStandardSpeedDataBlock;
            lastBlock.ReadUntilPause();

            // --- Assert
            player.Eof.ShouldBeFalse();
        }
    }
}
