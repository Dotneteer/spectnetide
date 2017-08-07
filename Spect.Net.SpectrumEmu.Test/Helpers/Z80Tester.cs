using Shouldly;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public static class Z80Tester
    {
        public static void Test(string expected, params byte[] opCodes)
        {
            var project = new DisassembyAnnotations();
            project.SetZ80Binary(opCodes);

            project.Disassemble();
            var output = project.Output;
            output.OutputItems.Count.ShouldBe(1);
            output.OutputItems[0].Instruction.ToLower().ShouldBe(expected.ToLower());
        }
    }
}