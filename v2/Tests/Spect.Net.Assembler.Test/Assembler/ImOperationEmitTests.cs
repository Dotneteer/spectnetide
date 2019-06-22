﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class ImOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void ImOpsWorkAsExpected()
        {
            CodeEmitWorks("im 0", 0xED, 0x46);
            CodeEmitWorks("im 1", 0xED, 0x56);
            CodeEmitWorks("im 2", 0xED, 0x5E);
        }

        [TestMethod]
        public void InvalidImModeRaisesError()
        {
            CodeRaisesError("im 3", Errors.Z0020);
        }
    }
}