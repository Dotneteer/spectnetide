using System;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Scripting;

namespace $safeprojectname$
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Creating ZX Spectrum 48K");
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();

            Console.WriteLine("Running to breakpoint 0x0001");
            sm.Breakpoints.AddBreakpoint(0x0001);
            sm.StartDebug();
            await sm.CompletionTask;

            Console.WriteLine($"Paused at breakpoint 0x{sm.Cpu.PC:X4}");
        }
    }
}
