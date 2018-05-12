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
                    ld (ix+[MySymbol+1]),h", 0xDD, 0x74, 0x01);
            CodeEmitWorks(@"
                MySymbol: 
                    ld (ix+MySymbol+1),h", 0xDD, 0x74, 0x01);
            CodeEmitWorks(@"
                MySymbol: 
                    ld (ix+(MySymbol+1)),h", 0xDD, 0x74, 0x01);
        }

        [TestMethod]
        [DataRow("ld a,hreg(bc)", 0x78)]
        [DataRow("ld a,hreg(de)", 0x7A)]
        [DataRow("ld a,hreg(hl)", 0x7C)]
        [DataRow("ld a,lreg(bc)", 0x79)]
        [DataRow("ld a,lreg(de)", 0x7B)]
        [DataRow("ld a,lreg(hl)", 0x7D)]
        [DataRow("ld a,HREG(bc)", 0x78)]
        [DataRow("ld a,HREG(de)", 0x7A)]
        [DataRow("ld a,HREG(hl)", 0x7C)]
        [DataRow("ld a,LREG(bc)", 0x79)]
        [DataRow("ld a,LREG(de)", 0x7B)]
        [DataRow("ld a,LREG(hl)", 0x7D)]
        public void LregHregWorksWithReg16AsExpected(string source, int expected)
        {
            CodeEmitWorks(source, (byte)expected);
        }

        [TestMethod]
        [DataRow("ld a,hreg(ix)", 0xDD, 0x7C)]
        [DataRow("ld a,lreg(ix)", 0xDD, 0x7D)]
        [DataRow("ld a,hreg(iy)", 0xFD, 0x7C)]
        [DataRow("ld a,lreg(iy)", 0xFD, 0x7D)]
        [DataRow("ld a,HREG(ix)", 0xDD, 0x7C)]
        [DataRow("ld a,LREG(ix)", 0xDD, 0x7D)]
        [DataRow("ld a,HREG(iy)", 0xFD, 0x7C)]
        [DataRow("ld a,LREG(iy)", 0xFD, 0x7D)]
        public void LregHregWorksWithReg16IdxAsExpected(string source, int exp1, int exp2)
        {
            CodeEmitWorks(source, (byte)exp1, (byte)exp2);
        }

    }
}
