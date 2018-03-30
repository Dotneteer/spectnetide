using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spect.Net.SpectrumEmu.Test.Utility
{
    [TestClass]
    public class RomSplitterTest
    {
        [TestMethod]
        [DeploymentItem("enNextOS.rom")]
        public void SplitRomFile()
        {
            var fullRom = File.ReadAllBytes("enNextOS.rom");
            for (var i = 0; i < 4; i++)
            {
                var rom = fullRom.Skip(i * 0x4000).Take(0x4000).ToArray();
                var fileName = $"C:\\Temp\\Next-{i}.rom";
                File.WriteAllBytes(fileName, rom);
            }
        }

        [TestMethod]
        [DeploymentItem("enNextOS.rom")]
        public void SearchForBadMmc()
        {
            //var pattern = new byte[] {0x6D, 0x6D, 0x63, 0x2E, 0x72};
            var pattern = new byte[] { 0xC8, 0x3D };
            var matchIdx = 0;
            var startAddr = 0;
            var fullRom = File.ReadAllBytes("enNextOS.rom");
            for (var i = 0; i < fullRom.Length - 10; i++)
            {
                if (pattern[matchIdx] != fullRom[i])
                {
                    matchIdx = 0;
                    continue;
                }
                if (matchIdx == 0)
                {
                    startAddr = i;
                }
                matchIdx++;
                if (matchIdx >= pattern.Length) break;
            }
            Console.WriteLine($"address: {startAddr}");
        }

    }
}
