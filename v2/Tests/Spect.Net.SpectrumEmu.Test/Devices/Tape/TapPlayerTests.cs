using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tap;

// ReSharper disable PossibleNullReferenceException

namespace Spect.Net.SpectrumEmu.Test.Devices.Tape
{
    [TestClass]
    public class TapPlayerTests
    {
        private const string TAPESET = "Pinball.tap";

        [TestMethod]
        public void TapFileCanBeReadSuccessfully()
        {
            // --- Act
            var player = TapPlayerHelper.CreatePlayer(TAPESET);

            // --- Assert
            player.DataBlocks.Count.ShouldBe(4);
            player.Eof.ShouldBeFalse();
        }

        [TestMethod]
        public void InitPlayWorksAsExpected()
        {
            // --- Arrange
            var player = TapPlayerHelper.CreatePlayer(TAPESET);

            // --- Act
            player.InitPlay(100);

            // --- Assert
            player.PlayPhase.ShouldBe(PlayPhase.None);
            player.StartTact.ShouldBe(100);
            player.CurrentBlockIndex.ShouldBe(0);
        }

        [TestMethod]
        public void InitPlayInitializesTheFirstDataBlock()
        {
            // --- Arrange
            var player = TapPlayerHelper.CreatePlayer(TAPESET);

            // --- Act
            player.InitPlay(100);

            // --- Assert
            var currentBlock = player.CurrentBlock as TapDataBlock;
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
            var player = TapPlayerHelper.CreatePlayer(TAPESET);
            player.InitPlay(100);
            var currentBlock = player.CurrentBlock as TapDataBlock;

            // --- Act
            var indexBefore = player.CurrentBlockIndex;
            currentBlock.CompleteBlock();
            var lastTact = currentBlock.LastTact;
            player.GetEarBit(lastTact);
            var indexAfter = player.CurrentBlockIndex;

            // --- Assert
            indexBefore.ShouldBe(0);
            indexAfter.ShouldBe(1);
            currentBlock = player.CurrentBlock as TapDataBlock;
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
            var player = TapPlayerHelper.CreatePlayer(TAPESET);
            player.InitPlay(100);
            for (var i = 0; i < 4; i++) // Block 4 is the last
            {
                var currentBlock = player.CurrentBlock as TapDataBlock;
                currentBlock.CompleteBlock();
                var lastTact = currentBlock.LastTact;
                player.GetEarBit(lastTact);
            }

            // --- Act
            var lastBlock = player.CurrentBlock as TapDataBlock;
            var lastPos = lastBlock.ReadUntilPause();
            player.GetEarBit(lastPos);
            
            // --- Assert
            player.Eof.ShouldBeTrue();
        }

        [TestMethod]
        public void PlayDoesNotSetEofUntilEnd()
        {
            // --- Arrange
            var player = TapPlayerHelper.CreatePlayer(TAPESET);
            player.InitPlay(100);
            for (var i = 0; i < 3; i++) // Block 3 is a middle block
            {
                var currentBlock = player.CurrentBlock as TapDataBlock;
                currentBlock.CompleteBlock();
                var lastTact = currentBlock.LastTact;
                player.GetEarBit(lastTact);
            }

            // --- Act
            var lastBlock = player.CurrentBlock as TapDataBlock;
            lastBlock.ReadUntilPause();

            // --- Assert
            player.Eof.ShouldBeFalse();
        }
    }
}