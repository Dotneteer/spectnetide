using AntlrZ80Asm.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class TrivialInstructionTests : ParserTestBed
    {
        protected void InstructionWorksAsExpected(string instruction)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as TrivialInstruction;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(instruction.ToUpper());
        }

        [TestMethod]
        public void NopWorksAsExpected()
        {
            InstructionWorksAsExpected("nop");
            InstructionWorksAsExpected("NOP");
        }

        [TestMethod]
        public void RlcaWorksAsExpected()
        {
            InstructionWorksAsExpected("rlca");
            InstructionWorksAsExpected("RLCA");
        }

        [TestMethod]
        public void RrcaWorksAsExpected()
        {
            InstructionWorksAsExpected("rrca");
            InstructionWorksAsExpected("RRCA");
        }

        [TestMethod]
        public void RlaWorksAsExpected()
        {
            InstructionWorksAsExpected("rla");
            InstructionWorksAsExpected("RLA");
        }

        [TestMethod]
        public void RraWorksAsExpected()
        {
            InstructionWorksAsExpected("rra");
            InstructionWorksAsExpected("RRA");
        }

        [TestMethod]
        public void DaaWorksAsExpected()
        {
            InstructionWorksAsExpected("daa");
            InstructionWorksAsExpected("DAA");
        }

        [TestMethod]
        public void CplWorksAsExpected()
        {
            InstructionWorksAsExpected("cpl");
            InstructionWorksAsExpected("CPL");
        }

        [TestMethod]
        public void ScfWorksAsExpected()
        {
            InstructionWorksAsExpected("scf");
            InstructionWorksAsExpected("SCF");
        }

        [TestMethod]
        public void CcfWorksAsExpected()
        {
            InstructionWorksAsExpected("ccf");
            InstructionWorksAsExpected("CCF");
        }

        [TestMethod]
        public void RetWorksAsExpected()
        {
            InstructionWorksAsExpected("ret");
            InstructionWorksAsExpected("RET");
        }

        [TestMethod]
        public void ExxWorksAsExpected()
        {
            InstructionWorksAsExpected("exx");
            InstructionWorksAsExpected("EXX");
        }

        [TestMethod]
        public void DiWorksAsExpected()
        {
            InstructionWorksAsExpected("di");
            InstructionWorksAsExpected("DI");
        }

        [TestMethod]
        public void EiWorksAsExpected()
        {
            InstructionWorksAsExpected("ei");
            InstructionWorksAsExpected("EI");
        }

        [TestMethod]
        public void NegWorksAsExpected()
        {
            InstructionWorksAsExpected("neg");
            InstructionWorksAsExpected("NEG");
        }

        [TestMethod]
        public void RetnWorksAsExpected()
        {
            InstructionWorksAsExpected("retn");
            InstructionWorksAsExpected("RETN");
        }

        [TestMethod]
        public void RetiWorksAsExpected()
        {
            InstructionWorksAsExpected("reti");
            InstructionWorksAsExpected("RETI");
        }

        [TestMethod]
        public void RldWorksAsExpected()
        {
            InstructionWorksAsExpected("rld");
            InstructionWorksAsExpected("RLD");
        }

        [TestMethod]
        public void RrdWorksAsExpected()
        {
            InstructionWorksAsExpected("rrd");
            InstructionWorksAsExpected("RRD");
        }

        [TestMethod]
        public void LdiWorksAsExpected()
        {
            InstructionWorksAsExpected("ldi");
            InstructionWorksAsExpected("LDI");
        }

        [TestMethod]
        public void CpiWorksAsExpected()
        {
            InstructionWorksAsExpected("cpi");
            InstructionWorksAsExpected("CPI");
        }

        [TestMethod]
        public void IniWorksAsExpected()
        {
            InstructionWorksAsExpected("ini");
            InstructionWorksAsExpected("INI");
        }

        [TestMethod]
        public void OutiWorksAsExpected()
        {
            InstructionWorksAsExpected("outi");
            InstructionWorksAsExpected("OUTI");
        }

        [TestMethod]
        public void LddWorksAsExpected()
        {
            InstructionWorksAsExpected("ldd");
            InstructionWorksAsExpected("LDD");
        }

        [TestMethod]
        public void CpdWorksAsExpected()
        {
            InstructionWorksAsExpected("cpd");
            InstructionWorksAsExpected("CPD");
        }

        [TestMethod]
        public void IndWorksAsExpected()
        {
            InstructionWorksAsExpected("ind");
            InstructionWorksAsExpected("IND");
        }

        [TestMethod]
        public void OutdWorksAsExpected()
        {
            InstructionWorksAsExpected("outd");
            InstructionWorksAsExpected("OUTD");
        }

        [TestMethod]
        public void LdirWorksAsExpected()
        {
            InstructionWorksAsExpected("ldir");
            InstructionWorksAsExpected("LDIR");
        }

        [TestMethod]
        public void CpirWorksAsExpected()
        {
            InstructionWorksAsExpected("cpir");
            InstructionWorksAsExpected("CPIR");
        }

        [TestMethod]
        public void InirWorksAsExpected()
        {
            InstructionWorksAsExpected("inir");
            InstructionWorksAsExpected("INIR");
        }

        [TestMethod]
        public void OtirWorksAsExpected()
        {
            InstructionWorksAsExpected("otir");
            InstructionWorksAsExpected("OTIR");
        }

        [TestMethod]
        public void LddrWorksAsExpected()
        {
            InstructionWorksAsExpected("lddr");
            InstructionWorksAsExpected("LDDR");
        }

        [TestMethod]
        public void CpdrWorksAsExpected()
        {
            InstructionWorksAsExpected("cpdr");
            InstructionWorksAsExpected("CPDR");
        }

        [TestMethod]
        public void IndrWorksAsExpected()
        {
            InstructionWorksAsExpected("indr");
            InstructionWorksAsExpected("INDR");
        }

        [TestMethod]
        public void OtdrWorksAsExpected()
        {
            InstructionWorksAsExpected("otdr");
            InstructionWorksAsExpected("OTDR");
        }


    }
}
