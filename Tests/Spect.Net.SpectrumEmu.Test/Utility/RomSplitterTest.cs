using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spect.Net.SpectrumEmu.Test.Utility
{
    [TestClass]
    public class RomSplitterTest
    {
        public const string SOURCEFILE =
            @"C:\Users\dotne\source\repos\spectnetide\TBBlue\enNextOS.rom";

        [TestMethod]
        public void SplitRomFile()
        {
            var fullRom = File.ReadAllBytes(SOURCEFILE);
            for (var i = 0; i < 4; i++)
            {
                var rom = fullRom.Skip(i * 0x4000).Take(0x4000).ToArray();
                var fileName = $"C:\\Temp\\Next-{i}.rom";
                File.WriteAllBytes(fileName, rom);
            }
        }
    }
}
