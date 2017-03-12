using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Tape;
using Spect.Net.SpectrumEmu.Tape.Tzx;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Tape
{
    [TestClass]
    public class TzxPlayerTests
    {
        [TestMethod]
        public void TzxFileCanBeReadSuccessfully()
        {
            // --- Arrange
            var tzxReader = TzxHelper.GetResourceReader("JetSetWilly.tzx");
            var player = new TzxPlayer(tzxReader);

            // --- Act
            player.ReadContent();

            // --- Assert
            player.DataBlocks.Count.ShouldBe(9);
            player.DataBlocks[0].ShouldBeOfType<TzxTextDescriptionDataBlock>();
            for (var i = 1; i < 9; i++)
            {
                player.DataBlocks[i].ShouldBeOfType<TzxStandardSpeedDataBlock>();
            }
        }

        [TestMethod]
        public void InitPlayWorksAsExpected()
        {
            // --- Arrange
            var tzxReader = TzxHelper.GetResourceReader("JetSetWilly.tzx");
            var player = new TzxPlayer(tzxReader);
            player.ReadContent();

            // --- Act
            player.InitPlay(100ul);

            // --- Assert
            player.PlayPhase.ShouldBe(PlayPhase.None);
            player.StartTact.ShouldBe(100ul);
            player.CurrentBlockIndex.ShouldBe(1);
            player.CurrentBlock.ShouldBeOfType<TzxStandardSpeedDataBlock>();
        }

        [TestMethod]
        public void InitPlayInitializesTheFirstDataBlock()
        {
            // --- Arrange
            var tzxReader = TzxHelper.GetResourceReader("JetSetWilly.tzx");
            var player = new TzxPlayer(tzxReader);
            player.ReadContent();

            // --- Act
            player.InitPlay(100ul);

            // --- Assert
            var currentBlock = player.CurrentBlock as TzxStandardSpeedDataBlock;
            currentBlock.ShouldNotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            currentBlock.PlayPhase.ShouldBe(PlayPhase.Pilot);
            currentBlock.StartTact.ShouldBe(player.StartTact);
            currentBlock.ByteIndex.ShouldBe(0);
            currentBlock.BitMask.ShouldBe((byte)0x80);
        }
    }
}
