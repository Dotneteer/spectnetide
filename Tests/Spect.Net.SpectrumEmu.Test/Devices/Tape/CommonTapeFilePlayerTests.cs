using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.Tape;

namespace Spect.Net.SpectrumEmu.Test.Devices.Tape
{
    [TestClass]
    public class CommonTapeFilePlayerTests
    {
        private const string TAPESET1 = "TapResources.Pinball.tap";
        private const string TAPESET2 = "TzxResources.JetSetWilly.tzx";

        [TestMethod]
        public void TapFileCanBeReadSuccessfully1()
        {
            // --- Act
            var player = CommonTapeFilePlayerHelper.CreatePlayer(TAPESET1);

            // --- Assert
            player.DataBlocks.Count.ShouldBe(4);
            player.Eof.ShouldBeFalse();
        }

        [TestMethod]
        public void InitPlayWorksAsExpected1()
        {
            // --- Arrange
            var player = CommonTapeFilePlayerHelper.CreatePlayer(TAPESET1);

            // --- Act
            player.InitPlay(100);

            // --- Assert
            player.PlayPhase.ShouldBe(PlayPhase.None);
            player.StartTact.ShouldBe(100);
            player.CurrentBlockIndex.ShouldBe(0);
        }

        [TestMethod]
        public void InitPlayInitializesTheFirstDataBlock1()
        {
            // --- Arrange
            var player = CommonTapeFilePlayerHelper.CreatePlayer(TAPESET1);

            // --- Act
            player.InitPlay(100);

            // --- Assert
            var currentBlock = player.CurrentBlock;
            currentBlock.ShouldNotBeNull();
            currentBlock.PlayPhase.ShouldBe(PlayPhase.Pilot);
            currentBlock.StartTact.ShouldBe(player.StartTact);
        }

        [TestMethod]
        public void TapFileCanBeReadSuccessfully2()
        {
            // --- Act
            var player = CommonTapeFilePlayerHelper.CreatePlayer(TAPESET2);

            // --- Assert
            player.DataBlocks.Count.ShouldBe(8);
            player.Eof.ShouldBeFalse();
        }

        [TestMethod]
        public void InitPlayWorksAsExpected2()
        {
            // --- Arrange
            var player = CommonTapeFilePlayerHelper.CreatePlayer(TAPESET2);

            // --- Act
            player.InitPlay(100);

            // --- Assert
            player.PlayPhase.ShouldBe(PlayPhase.None);
            player.StartTact.ShouldBe(100);
            player.CurrentBlockIndex.ShouldBe(0);
        }

        [TestMethod]
        public void InitPlayInitializesTheFirstDataBlock2()
        {
            // --- Arrange
            var player = CommonTapeFilePlayerHelper.CreatePlayer(TAPESET2);

            // --- Act
            player.InitPlay(100);

            // --- Assert
            var currentBlock = player.CurrentBlock;
            currentBlock.ShouldNotBeNull();
            currentBlock.PlayPhase.ShouldBe(PlayPhase.Pilot);
            currentBlock.StartTact.ShouldBe(player.StartTact);
        }
    }
}