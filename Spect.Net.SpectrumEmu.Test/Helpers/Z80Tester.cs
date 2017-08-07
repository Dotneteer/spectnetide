using Shouldly;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public static class Z80Tester
    {
        public static void Test(string expected, params byte[] opCodes)
        {
            var map = new MemoryMap
            {
                new MemorySection(0x0000, (ushort) opCodes.Length)
            };
            var annotations = new DisassembyAnnotations(map);
            var disassembler = new Z80Disassembler(annotations, opCodes);
            var output = disassembler.Disassemble();
            output.OutputItems.Count.ShouldBe(1);
            output.OutputItems[0].Instruction.ToLower().ShouldBe(expected.ToLower());
        }
    }
}