using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Scripting;

namespace Spect.Net.SpectrumEmu.Test.Scripting
{
    [TestClass]
    public class CpuTests
    {
        private const string STATE_FOLDER = @"C:\Temp\SavedState";

        [TestMethod]
        public async Task InterruptExecutingIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var counter = 0;

            // --- Act
            sm.Cpu.InterruptExecuting += (s, e) => { counter++; };
            var entryAddress = sm.InjectCode(@"
                .org #8000
                ld a,2
                out (#fe),a
                halt
                halt
                halt
                ret
            ");
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            counter.ShouldBe(4);
        }
    }
}
