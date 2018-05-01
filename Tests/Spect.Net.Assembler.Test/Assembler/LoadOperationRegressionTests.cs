using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class LoadOperationRegressionTests : AssemblerTestBed
    {
        [TestMethod]
        public void LoadWithCharLiteralWorks()
        {
            CodeEmitWorks("ld a,\'0'", 0x3E, 0x30);
            CodeEmitWorks("ld a,'0'", 0x3E, 0x30);
        }

        [TestMethod]
        public void LoadWith16BitRaisesError()
        {
            CodeRaisesError("ld sp,bc", Errors.Z0021);
            CodeRaisesError("ld sp,de", Errors.Z0021);
        }
    }
}
