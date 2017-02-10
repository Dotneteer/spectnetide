using Shouldly;

namespace Spect.Net.Z80DisAsm.Test.Helpers
{
    public static class Z80Tester
    {
        public static void Test(string expected, params byte[] opCodes)
        {
            var project = new Z80DisAsmProject(opCodes);
            var disasm = new Z80Disassembler(project);

            var output = disasm.Disassemble();
            output.OutputItems.Count.ShouldBe(1);
            output.OutputItems[0].Instruction.ToLower().ShouldBe(expected.ToLower());
        }
    }
}