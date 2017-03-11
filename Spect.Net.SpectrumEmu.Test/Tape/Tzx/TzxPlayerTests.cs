using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Tape.Tzx;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Tape.Tzx
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
    }
}
