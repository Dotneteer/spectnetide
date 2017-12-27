using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Devices.Beeper
{
    [TestClass]
    public class NewBeeperDeviceTests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        /// <summary>
        /// VM to test the beeper
        /// </summary>
        private class SpectrumBeepTestMachine : SpectrumAdvancedTestMachine
        {
            public void SetCurrentCpuTact(long tacts)
            {
                (Cpu as IZ80CpuTestSupport)?.SetTacts(tacts);
            }
        }

    }
}
