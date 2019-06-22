using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Operations;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class AluOperationTests : ParserTestBed
    {
        [TestMethod]
        public void AddInstructionWorksAsExpected()
        {
            AluOpWorksAsExpected("add a,b", "ADD", "B", "A");
            AluOpWorksAsExpected("add a,c", "ADD", "C", "A");
            AluOpWorksAsExpected("add a,d", "ADD", "D", "A");
            AluOpWorksAsExpected("add a,e", "ADD", "E", "A");
            AluOpWorksAsExpected("add a,h", "ADD", "H", "A");
            AluOpWorksAsExpected("add a,l", "ADD", "L", "A");
            AluOpWorksAsExpected("add a,(hl)", "ADD", "(HL)", "A");
            AluOpWorksAsExpected("add a,a", "ADD", "A", "A");

            AluOpWorksAsExpected("add a,xh", "ADD", "XH", "A");
            AluOpWorksAsExpected("add a,xl", "ADD", "XL", "A");
            AluOpWorksAsExpected("add a,yh", "ADD", "YH", "A");
            AluOpWorksAsExpected("add a,yl", "ADD", "YL", "A");

            AluOpWorksAsExpected("add hl,bc", "ADD", "BC", "HL");
            AluOpWorksAsExpected("add hl,de", "ADD", "DE", "HL");
            AluOpWorksAsExpected("add hl,hl", "ADD", "HL", "HL");
            AluOpWorksAsExpected("add hl,sp", "ADD", "SP", "HL");

            AluOpWorksAsExpected("add ix,bc", "ADD", "BC", "IX");
            AluOpWorksAsExpected("add ix,de", "ADD", "DE", "IX");
            AluOpWorksAsExpected("add ix,ix", "ADD", "IX", "IX");
            AluOpWorksAsExpected("add ix,sp", "ADD", "SP", "IX");

            AluOpWorksAsExpected("add iy,bc", "ADD", "BC", "IY");
            AluOpWorksAsExpected("add iy,de", "ADD", "DE", "IY");
            AluOpWorksAsExpected("add iy,iy", "ADD", "IY", "IY");
            AluOpWorksAsExpected("add iy,sp", "ADD", "SP", "IY");

            AluOpWorksAsExpected("add a,(ix)", "ADD", "A", "IX", null);
            AluOpWorksAsExpected("add a,(ix+#04)", "ADD", "A", "IX", "+");
            AluOpWorksAsExpected("add a,(ix-#04)", "ADD", "A", "IX", "-");
            AluOpWorksAsExpected("add a,(iy)", "ADD", "A", "IY", null);
            AluOpWorksAsExpected("add a,(iy+#04)", "ADD", "A", "IY", "+");
            AluOpWorksAsExpected("add a,(iy-#04)", "ADD", "A", "IY", "-");

            AluOpWorksAsExpected("add a,#12", "ADD", "A");
            AluOpWorksAsExpected("add a,Some", "ADD", "A");
        }

        [TestMethod]
        public void AdcInstructionWorksAsExpected()
        {
            AluOpWorksAsExpected("adc hl,bc", "ADC", "BC", "HL");
            AluOpWorksAsExpected("adc hl,de", "ADC", "DE", "HL");
            AluOpWorksAsExpected("adc hl,hl", "ADC", "HL", "HL");
            AluOpWorksAsExpected("adc hl,sp", "ADC", "SP", "HL");

            AluOpWorksAsExpected("adc a,b", "ADC", "B", "A");
            AluOpWorksAsExpected("adc a,c", "ADC", "C", "A");
            AluOpWorksAsExpected("adc a,d", "ADC", "D", "A");
            AluOpWorksAsExpected("adc a,e", "ADC", "E", "A");
            AluOpWorksAsExpected("adc a,h", "ADC", "H", "A");
            AluOpWorksAsExpected("adc a,l", "ADC", "L", "A");
            AluOpWorksAsExpected("adc a,(hl)", "ADC", "(HL)", "A");
            AluOpWorksAsExpected("adc a,a", "ADC", "A", "A");

            AluOpWorksAsExpected("adc a,xh", "ADC", "XH", "A");
            AluOpWorksAsExpected("adc a,xl", "ADC", "XL", "A");
            AluOpWorksAsExpected("adc a,yh", "ADC", "YH", "A");
            AluOpWorksAsExpected("adc a,yl", "ADC", "YL", "A");

            AluOpWorksAsExpected("adc a,(ix)", "ADC", "A", "IX", null);
            AluOpWorksAsExpected("adc a,(ix+#04)", "ADC", "A", "IX", "+");
            AluOpWorksAsExpected("adc a,(ix-#04)", "ADC", "A", "IX", "-");
            AluOpWorksAsExpected("adc a,(iy)", "ADC", "A", "IY", null);
            AluOpWorksAsExpected("adc a,(iy+#04)", "ADC", "A", "IY", "+");
            AluOpWorksAsExpected("adc a,(iy-#04)", "ADC", "A", "IY", "-");

            AluOpWorksAsExpected("adc a,#12", "ADC", "A");
            AluOpWorksAsExpected("adc a,Some", "ADC", "A");
        }

        [TestMethod]
        public void SubInstructionWorksAsExpected()
        {
            AluOpWorksAsExpected2("sub b", "SUB", "B");
            AluOpWorksAsExpected2("sub c", "SUB", "C");
            AluOpWorksAsExpected2("sub d", "SUB", "D");
            AluOpWorksAsExpected2("sub e", "SUB", "E");
            AluOpWorksAsExpected2("sub h", "SUB", "H");
            AluOpWorksAsExpected2("sub l", "SUB", "L");
            AluOpWorksAsExpected2("sub (hl)", "SUB", "(HL)");
            AluOpWorksAsExpected2("sub a", "SUB", "A");

            AluOpWorksAsExpected2("sub xh", "SUB", "XH");
            AluOpWorksAsExpected2("sub xl", "SUB", "XL");
            AluOpWorksAsExpected2("sub yh", "SUB", "YH");
            AluOpWorksAsExpected2("sub yl", "SUB", "YL");

            AluOpWorksAsExpected2("sub (ix)", "SUB", null, "IX", null);
            AluOpWorksAsExpected2("sub (ix+#04)", "SUB", null, "IX", "+");
            AluOpWorksAsExpected2("sub (ix-#04)", "SUB", null, "IX", "-");
            AluOpWorksAsExpected2("sub (iy)", "SUB", null, "IY", null);
            AluOpWorksAsExpected2("sub (iy+#04)", "SUB", null, "IY", "+");
            AluOpWorksAsExpected2("sub (iy-#04)", "SUB", null, "IY", "-");

            AluOpWorksAsExpected2("sub #12", "SUB");
            AluOpWorksAsExpected2("sub Some", "SUB");
        }

        [TestMethod]
        public void SubInstructionWorksWithAlternateSyntaxAsExpected()
        {
            AluOpWorksAsExpected("sub a,b", "SUB", "B", "A");
            AluOpWorksAsExpected("sub a,c", "SUB", "C", "A");
            AluOpWorksAsExpected("sub a,d", "SUB", "D", "A");
            AluOpWorksAsExpected("sub a,e", "SUB", "E", "A");
            AluOpWorksAsExpected("sub a,h", "SUB", "H", "A");
            AluOpWorksAsExpected("sub a,l", "SUB", "L", "A");
            AluOpWorksAsExpected("sub a,(hl)", "SUB", "(HL)", "A");
            AluOpWorksAsExpected("sub a,a", "SUB", "A", "A");

            AluOpWorksAsExpected("sub a,xh", "SUB", "XH", "A");
            AluOpWorksAsExpected("sub a,xl", "SUB", "XL", "A");
            AluOpWorksAsExpected("sub a,yh", "SUB", "YH", "A");
            AluOpWorksAsExpected("sub a,yl", "SUB", "YL", "A");

            AluOpWorksAsExpected3("sub a,(ix)", "SUB", "A", "IX", null);
            AluOpWorksAsExpected3("sub a,(ix+#04)", "SUB", "A", "IX", "+");
            AluOpWorksAsExpected3("sub a,(ix-#04)", "SUB", "A", "IX", "-");
            AluOpWorksAsExpected3("sub a,(iy)", "SUB", "A", "IY", null);
            AluOpWorksAsExpected3("sub a,(iy+#04)", "SUB", "A", "IY", "+");
            AluOpWorksAsExpected3("sub a,(iy-#04)", "SUB", "A", "IY", "-");

            AluOpWorksAsExpected3("sub a,#12", "SUB", "A");
            AluOpWorksAsExpected3("sub a,Some", "SUB", "A");
        }

        [TestMethod]
        public void SbcInstructionWorksAsExpected()
        {
            AluOpWorksAsExpected("sbc hl,bc", "SBC", "BC", "HL");
            AluOpWorksAsExpected("sbc hl,de", "SBC", "DE", "HL");
            AluOpWorksAsExpected("sbc hl,hl", "SBC", "HL", "HL");
            AluOpWorksAsExpected("sbc hl,sp", "SBC", "SP", "HL");

            AluOpWorksAsExpected("sbc a,b", "SBC", "B", "A");
            AluOpWorksAsExpected("sbc a,c", "SBC", "C", "A");
            AluOpWorksAsExpected("sbc a,d", "SBC", "D", "A");
            AluOpWorksAsExpected("sbc a,e", "SBC", "E", "A");
            AluOpWorksAsExpected("sbc a,h", "SBC", "H", "A");
            AluOpWorksAsExpected("sbc a,l", "SBC", "L", "A");
            AluOpWorksAsExpected("sbc a,(hl)", "SBC", "(HL)", "A");
            AluOpWorksAsExpected("sbc a,a", "SBC", "A", "A");

            AluOpWorksAsExpected("sbc a,xh", "SBC", "XH", "A");
            AluOpWorksAsExpected("sbc a,xl", "SBC", "XL", "A");
            AluOpWorksAsExpected("sbc a,yh", "SBC", "YH", "A");
            AluOpWorksAsExpected("sbc a,yl", "SBC", "YL", "A");

            AluOpWorksAsExpected("sbc a,(ix)", "SBC", "A", "IX", null);
            AluOpWorksAsExpected("sbc a,(ix+#04)", "SBC", "A", "IX", "+");
            AluOpWorksAsExpected("sbc a,(ix-#04)", "SBC", "A", "IX", "-");
            AluOpWorksAsExpected("sbc a,(iy)", "SBC", "A", "IY", null);
            AluOpWorksAsExpected("sbc a,(iy+#04)", "SBC", "A", "IY", "+");
            AluOpWorksAsExpected("sbc a,(iy-#04)", "SBC", "A", "IY", "-");

            AluOpWorksAsExpected("sbc a,#12", "SBC", "A");
            AluOpWorksAsExpected("sbc a,Some", "SBC", "A");
        }

        [TestMethod]
        public void AndInstructionWorksAsExpected()
        {
            AluOpWorksAsExpected2("and b", "AND", "B");
            AluOpWorksAsExpected2("and c", "AND", "C");
            AluOpWorksAsExpected2("and d", "AND", "D");
            AluOpWorksAsExpected2("and e", "AND", "E");
            AluOpWorksAsExpected2("and h", "AND", "H");
            AluOpWorksAsExpected2("and l", "AND", "L");
            AluOpWorksAsExpected2("and (hl)", "AND", "(HL)");
            AluOpWorksAsExpected2("and a", "AND", "A");

            AluOpWorksAsExpected2("and xh", "AND", "XH");
            AluOpWorksAsExpected2("and xl", "AND", "XL");
            AluOpWorksAsExpected2("and yh", "AND", "YH");
            AluOpWorksAsExpected2("and yl", "AND", "YL");

            AluOpWorksAsExpected2("and (ix)", "AND", null, "IX", null);
            AluOpWorksAsExpected2("and (ix+#04)", "AND", null, "IX", "+");
            AluOpWorksAsExpected2("and (ix-#04)", "AND", null, "IX", "-");
            AluOpWorksAsExpected2("and (iy)", "AND", null, "IY", null);
            AluOpWorksAsExpected2("and (iy+#04)", "AND", null, "IY", "+");
            AluOpWorksAsExpected2("and (iy-#04)", "AND", null, "IY", "-");

            AluOpWorksAsExpected2("and #12", "AND");
            AluOpWorksAsExpected2("and Some", "AND");
        }

        [TestMethod]
        public void AndInstructionWorksWithAlternateSyntaxAsExpected()
        {
            AluOpWorksAsExpected("and a,b", "AND", "B", "A");
            AluOpWorksAsExpected("and a,c", "AND", "C", "A");
            AluOpWorksAsExpected("and a,d", "AND", "D", "A");
            AluOpWorksAsExpected("and a,e", "AND", "E", "A");
            AluOpWorksAsExpected("and a,h", "AND", "H", "A");
            AluOpWorksAsExpected("and a,l", "AND", "L", "A");
            AluOpWorksAsExpected("and a,(hl)", "AND", "(HL)", "A");
            AluOpWorksAsExpected("and a,a", "AND", "A", "A");

            AluOpWorksAsExpected("and a,xh", "AND", "XH", "A");
            AluOpWorksAsExpected("and a,xl", "AND", "XL", "A");
            AluOpWorksAsExpected("and a,yh", "AND", "YH", "A");
            AluOpWorksAsExpected("and a,yl", "AND", "YL", "A");

            AluOpWorksAsExpected3("and a,(ix)", "AND", "A", "IX", null);
            AluOpWorksAsExpected3("and a,(ix+#04)", "AND", "A", "IX", "+");
            AluOpWorksAsExpected3("and a,(ix-#04)", "AND", "A", "IX", "-");
            AluOpWorksAsExpected3("and a,(iy)", "AND", "A", "IY", null);
            AluOpWorksAsExpected3("and a,(iy+#04)", "AND", "A", "IY", "+");
            AluOpWorksAsExpected3("and a,(iy-#04)", "AND", "A", "IY", "-");

            AluOpWorksAsExpected3("and a,#12", "AND", "A");
            AluOpWorksAsExpected3("and a,Some", "AND", "A");
        }

        [TestMethod]
        public void XorInstructionWorksAsExpected()
        {
            AluOpWorksAsExpected2("xor b", "XOR", "B");
            AluOpWorksAsExpected2("xor c", "XOR", "C");
            AluOpWorksAsExpected2("xor d", "XOR", "D");
            AluOpWorksAsExpected2("xor e", "XOR", "E");
            AluOpWorksAsExpected2("xor h", "XOR", "H");
            AluOpWorksAsExpected2("xor l", "XOR", "L");
            AluOpWorksAsExpected2("xor (hl)", "XOR", "(HL)");
            AluOpWorksAsExpected2("xor a", "XOR", "A");

            AluOpWorksAsExpected2("xor xh", "XOR", "XH");
            AluOpWorksAsExpected2("xor xl", "XOR", "XL");
            AluOpWorksAsExpected2("xor yh", "XOR", "YH");
            AluOpWorksAsExpected2("xor yl", "XOR", "YL");

            AluOpWorksAsExpected2("xor (ix)", "XOR", null, "IX", null);
            AluOpWorksAsExpected2("xor (ix+#04)", "XOR", null, "IX", "+");
            AluOpWorksAsExpected2("xor (ix-#04)", "XOR", null, "IX", "-");
            AluOpWorksAsExpected2("xor (iy)", "XOR", null, "IY", null);
            AluOpWorksAsExpected2("xor (iy+#04)", "XOR", null, "IY", "+");
            AluOpWorksAsExpected2("xor (iy-#04)", "XOR", null, "IY", "-");

            AluOpWorksAsExpected2("xor #12", "XOR");
            AluOpWorksAsExpected2("xor Some", "XOR");
        }

        [TestMethod]
        public void XorInstructionWorksWithAlternateSyntaxAsExpected()
        {
            AluOpWorksAsExpected("xor a,b", "XOR", "B", "A");
            AluOpWorksAsExpected("xor a,c", "XOR", "C", "A");
            AluOpWorksAsExpected("xor a,d", "XOR", "D", "A");
            AluOpWorksAsExpected("xor a,e", "XOR", "E", "A");
            AluOpWorksAsExpected("xor a,h", "XOR", "H", "A");
            AluOpWorksAsExpected("xor a,l", "XOR", "L", "A");
            AluOpWorksAsExpected("xor a,(hl)", "XOR", "(HL)", "A");
            AluOpWorksAsExpected("xor a,a", "XOR", "A", "A");

            AluOpWorksAsExpected("xor a,xh", "XOR", "XH", "A");
            AluOpWorksAsExpected("xor a,xl", "XOR", "XL", "A");
            AluOpWorksAsExpected("xor a,yh", "XOR", "YH", "A");
            AluOpWorksAsExpected("xor a,yl", "XOR", "YL", "A");

            AluOpWorksAsExpected3("xor a,(ix)", "XOR", "A", "IX", null);
            AluOpWorksAsExpected3("xor a,(ix+#04)", "XOR", "A", "IX", "+");
            AluOpWorksAsExpected3("xor a,(ix-#04)", "XOR", "A", "IX", "-");
            AluOpWorksAsExpected3("xor a,(iy)", "XOR", "A", "IY", null);
            AluOpWorksAsExpected3("xor a,(iy+#04)", "XOR", "A", "IY", "+");
            AluOpWorksAsExpected3("xor a,(iy-#04)", "XOR", "A", "IY", "-");

            AluOpWorksAsExpected3("xor a,#12", "XOR", "A");
            AluOpWorksAsExpected3("xor a,Some", "XOR", "A");
        }

        [TestMethod]
        public void OrInstructionWorksAsExpected()
        {
            AluOpWorksAsExpected2("or b", "OR", "B");
            AluOpWorksAsExpected2("or c", "OR", "C");
            AluOpWorksAsExpected2("or d", "OR", "D");
            AluOpWorksAsExpected2("or e", "OR", "E");
            AluOpWorksAsExpected2("or h", "OR", "H");
            AluOpWorksAsExpected2("or l", "OR", "L");
            AluOpWorksAsExpected2("or (hl)", "OR", "(HL)");
            AluOpWorksAsExpected2("or a", "OR", "A");

            AluOpWorksAsExpected2("or xh", "OR", "XH");
            AluOpWorksAsExpected2("or xl", "OR", "XL");
            AluOpWorksAsExpected2("or yh", "OR", "YH");
            AluOpWorksAsExpected2("or yl", "OR", "YL");

            AluOpWorksAsExpected2("or (ix)", "OR", null, "IX", null);
            AluOpWorksAsExpected2("or (ix+#04)", "OR", null, "IX", "+");
            AluOpWorksAsExpected2("or (ix-#04)", "OR", null, "IX", "-");
            AluOpWorksAsExpected2("or (iy)", "OR", null, "IY", null);
            AluOpWorksAsExpected2("or (iy+#04)", "OR", null, "IY", "+");
            AluOpWorksAsExpected2("or (iy-#04)", "OR", null, "IY", "-");

            AluOpWorksAsExpected2("or #12", "OR");
            AluOpWorksAsExpected2("or Some", "OR");
        }

        [TestMethod]
        public void OrInstructionWorksWithAlternateSyntaxAsExpected()
        {
            AluOpWorksAsExpected("or a,b", "OR", "B", "A");
            AluOpWorksAsExpected("or a,c", "OR", "C", "A");
            AluOpWorksAsExpected("or a,d", "OR", "D", "A");
            AluOpWorksAsExpected("or a,e", "OR", "E", "A");
            AluOpWorksAsExpected("or a,h", "OR", "H", "A");
            AluOpWorksAsExpected("or a,l", "OR", "L", "A");
            AluOpWorksAsExpected("or a,(hl)", "OR", "(HL)", "A");
            AluOpWorksAsExpected("or a,a", "OR", "A", "A");

            AluOpWorksAsExpected("or a,xh", "OR", "XH", "A");
            AluOpWorksAsExpected("or a,xl", "OR", "XL", "A");
            AluOpWorksAsExpected("or a,yh", "OR", "YH", "A");
            AluOpWorksAsExpected("or a,yl", "OR", "YL", "A");

            AluOpWorksAsExpected3("or a,(ix)", "OR", "A", "IX", null);
            AluOpWorksAsExpected3("or a,(ix+#04)", "OR", "A", "IX", "+");
            AluOpWorksAsExpected3("or a,(ix-#04)", "OR", "A", "IX", "-");
            AluOpWorksAsExpected3("or a,(iy)", "OR", "A", "IY", null);
            AluOpWorksAsExpected3("or a,(iy+#04)", "OR", "A", "IY", "+");
            AluOpWorksAsExpected3("or a,(iy-#04)", "OR", "A", "IY", "-");

            AluOpWorksAsExpected3("or a,#12", "OR", "A");
            AluOpWorksAsExpected3("or a,Some", "OR", "A");
        }

        [TestMethod]
        public void CpInstructionWorksAsExpected()
        {
            AluOpWorksAsExpected2("cp b", "CP", "B");
            AluOpWorksAsExpected2("cp c", "CP", "C");
            AluOpWorksAsExpected2("cp d", "CP", "D");
            AluOpWorksAsExpected2("cp e", "CP", "E");
            AluOpWorksAsExpected2("cp h", "CP", "H");
            AluOpWorksAsExpected2("cp l", "CP", "L");
            AluOpWorksAsExpected2("cp (hl)", "CP", "(HL)");
            AluOpWorksAsExpected2("cp a", "CP", "A");

            AluOpWorksAsExpected2("cp xh", "CP", "XH");
            AluOpWorksAsExpected2("cp xl", "CP", "XL");
            AluOpWorksAsExpected2("cp yh", "CP", "YH");
            AluOpWorksAsExpected2("cp yl", "CP", "YL");

            AluOpWorksAsExpected2("cp (ix)", "CP", null, "IX", null);
            AluOpWorksAsExpected2("cp (ix+#04)", "CP", null, "IX", "+");
            AluOpWorksAsExpected2("cp (ix-#04)", "CP", null, "IX", "-");
            AluOpWorksAsExpected2("cp (iy)", "CP", null, "IY", null);
            AluOpWorksAsExpected2("cp (iy+#04)", "CP", null, "IY", "+");
            AluOpWorksAsExpected2("cp (iy-#04)", "CP", null, "IY", "-");

            AluOpWorksAsExpected2("cp #12", "CP");
            AluOpWorksAsExpected2("cp Some", "CP");
        }

        [TestMethod]
        public void CpInstructionWorksWithAlternateSyntaxAsExpected()
        {
            AluOpWorksAsExpected("cp a,b", "CP", "B", "A");
            AluOpWorksAsExpected("cp a,c", "CP", "C", "A");
            AluOpWorksAsExpected("cp a,d", "CP", "D", "A");
            AluOpWorksAsExpected("cp a,e", "CP", "E", "A");
            AluOpWorksAsExpected("cp a,h", "CP", "H", "A");
            AluOpWorksAsExpected("cp a,l", "CP", "L", "A");
            AluOpWorksAsExpected("cp a,(hl)", "CP", "(HL)", "A");
            AluOpWorksAsExpected("cp a,a", "CP", "A", "A");

            AluOpWorksAsExpected("cp a,xh", "CP", "XH", "A");
            AluOpWorksAsExpected("cp a,xl", "CP", "XL", "A");
            AluOpWorksAsExpected("cp a,yh", "CP", "YH", "A");
            AluOpWorksAsExpected("cp a,yl", "CP", "YL", "A");

            AluOpWorksAsExpected3("cp a,(ix)", "CP", "A", "IX", null);
            AluOpWorksAsExpected3("cp a,(ix+#04)", "CP", "A", "IX", "+");
            AluOpWorksAsExpected3("cp a,(ix-#04)", "CP", "A", "IX", "-");
            AluOpWorksAsExpected3("cp a,(iy)", "CP", "A", "IY", null);
            AluOpWorksAsExpected3("cp a,(iy+#04)", "CP", "A", "IY", "+");
            AluOpWorksAsExpected3("cp a,(iy-#04)", "CP", "A", "IY", "-");

            AluOpWorksAsExpected3("cp a,#12", "CP", "A");
            AluOpWorksAsExpected3("cp a,Some", "CP", "A");
        }


        protected void AluOpWorksAsExpected(string instruction, string type, string source, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand.Register.ShouldBe(dest);
            line.Operand2.Register.ShouldBe(source);
            line.Operand2.Sign.ShouldBeNull();
            line.Operand2.Expression.ShouldBeNull();
        }

        protected void AluOpWorksAsExpected2(string instruction, string type, string source)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand.Register.ShouldBe(source);
            line.Operand.Sign.ShouldBeNull();
            line.Operand.Expression.ShouldBeNull();
        }

        protected void AluOpWorksAsExpected(string instruction, string type, string dest, string reg, string sign)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand2.Type.ShouldBe(OperandType.IndexedAddress);
            line.Operand2.Register.ShouldBe(reg);
            line.Operand2.Sign.ShouldBe(sign);
            if (line.Operand2.Sign == null)
            {
                line.Operand2.Expression.ShouldBeNull();
            }
            else
            {
                line.Operand2.Expression.ShouldNotBeNull();
            }
        }

        protected void AluOpWorksAsExpected2(string instruction, string type, string dest, string reg, string sign)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand.Type.ShouldBe(OperandType.IndexedAddress);
            line.Operand.Register.ShouldBe(reg);
            line.Operand.Sign.ShouldBe(sign);
            if (line.Operand.Sign == null)
            {
                line.Operand.Expression.ShouldBeNull();
            }
            else
            {
                line.Operand.Expression.ShouldNotBeNull();
            }
        }

        protected void AluOpWorksAsExpected(string instruction, string type, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand2.Type.ShouldBe(OperandType.Expr);
            line.Operand2.Register.ShouldBeNull();
            line.Operand2.Sign.ShouldBeNull();
            line.Operand2.Expression.ShouldNotBeNull();
        }

        protected void AluOpWorksAsExpected2(string instruction, string type)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand.Type.ShouldBe(OperandType.Expr);
            line.Operand.Register.ShouldBeNull();
            line.Operand.Sign.ShouldBeNull();
            line.Operand.Expression.ShouldNotBeNull();
        }

        protected void AluOpWorksAsExpected3(string instruction, string type, string dest, string reg, string sign)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand.Register.ShouldBe(dest);
            line.Operand2.Type.ShouldBe(OperandType.IndexedAddress);
            line.Operand2.Register.ShouldBe(reg);
            line.Operand2.Sign.ShouldBe(sign);
            if (line.Operand2.Sign == null)
            {
                line.Operand2.Expression.ShouldBeNull();
            }
            else
            {
                line.Operand2.Expression.ShouldNotBeNull();
            }
        }

        protected void AluOpWorksAsExpected3(string instruction, string type, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand.Register.ShouldBe(dest);
            line.Operand2.Type.ShouldBe(OperandType.Expr);
            line.Operand2.Register.ShouldBeNull();
            line.Operand2.Sign.ShouldBeNull();
            line.Operand2.Expression.ShouldNotBeNull();
        }
    }
}