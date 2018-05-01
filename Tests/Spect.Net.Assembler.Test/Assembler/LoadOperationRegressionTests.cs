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

        [TestMethod]
        public void LoadWithIndexedAddressWorks()
        {
            CodeEmitWorks(@"
                MySymbol: 
                    ld (ix+MySymbol+1),h", 0xDD, 0x74, 0x01);
            CodeEmitWorks(@"
                MySymbol: 
                    ld (ix+(MySymbol+1)),h", 0xDD, 0x74, 0x01);
            CodeEmitWorks(@"
                MySymbol: 
                    ld (ix+[MySymbol+1]),h", 0xDD, 0x74, 0x01);
        }
    }
}
