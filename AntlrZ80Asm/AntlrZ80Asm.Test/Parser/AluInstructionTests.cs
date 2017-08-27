using AntlrZ80Asm.SyntaxTree;
using AntlrZ80Asm.SyntaxTree.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class AluInstructionTests : ParserTestBed
    {
        [TestMethod]
        public void AddInstructionWorksAsExpected()
        {
            AluInstructionWorksAsExpected("add hl,bc", "ADD", "BC", "HL");
            AluInstructionWorksAsExpected("add hl,de", "ADD", "DE", "HL");
            AluInstructionWorksAsExpected("add hl,hl", "ADD", "HL", "HL");
            AluInstructionWorksAsExpected("add hl,sp", "ADD", "SP", "HL");

            AluInstructionWorksAsExpected("add ix,bc", "ADD", "BC", "IX");
            AluInstructionWorksAsExpected("add ix,de", "ADD", "DE", "IX");
            AluInstructionWorksAsExpected("add ix,ix", "ADD", "IX", "IX");
            AluInstructionWorksAsExpected("add ix,sp", "ADD", "SP", "IX");

            AluInstructionWorksAsExpected("add iy,bc", "ADD", "BC", "IY");
            AluInstructionWorksAsExpected("add iy,de", "ADD", "DE", "IY");
            AluInstructionWorksAsExpected("add iy,iy", "ADD", "IY", "IY");
            AluInstructionWorksAsExpected("add iy,sp", "ADD", "SP", "IY");

            AluInstructionWorksAsExpected("add a,b", "ADD", "B", "A");
            AluInstructionWorksAsExpected("add a,c", "ADD", "C", "A");
            AluInstructionWorksAsExpected("add a,d", "ADD", "D", "A");
            AluInstructionWorksAsExpected("add a,e", "ADD", "E", "A");
            AluInstructionWorksAsExpected("add a,h", "ADD", "H", "A");
            AluInstructionWorksAsExpected("add a,l", "ADD", "L", "A");
            AluInstructionWorksAsExpected("add a,(hl)", "ADD", "(HL)", "A");
            AluInstructionWorksAsExpected("add a,a", "ADD", "A", "A");

            AluInstructionWorksAsExpected("add a,xh", "ADD", "XH", "A");
            AluInstructionWorksAsExpected("add a,xl", "ADD", "XL", "A");
            AluInstructionWorksAsExpected("add a,yh", "ADD", "YH", "A");
            AluInstructionWorksAsExpected("add a,yl", "ADD", "YL", "A");

            AluInstructionWorksAsExpected("add a,(ix)", "ADD", "A", "IX", null);
            AluInstructionWorksAsExpected("add a,(ix+#04)", "ADD", "A", "IX", "+");
            AluInstructionWorksAsExpected("add a,(ix-#04)", "ADD", "A", "IX", "-");
            AluInstructionWorksAsExpected("add a,(iy)", "ADD", "A", "IY", null);
            AluInstructionWorksAsExpected("add a,(iy+#04)", "ADD", "A", "IY", "+");
            AluInstructionWorksAsExpected("add a,(iy-#04)", "ADD", "A", "IY", "-");

            AluInstructionWorksAsExpected("add a,#12", "ADD", "A");
            AluInstructionWorksAsExpected("add a,Some", "ADD", "A");
        }

        [TestMethod]
        public void AdcInstructionWorksAsExpected()
        {
            AluInstructionWorksAsExpected("adc hl,bc", "ADC", "BC", "HL");
            AluInstructionWorksAsExpected("adc hl,de", "ADC", "DE", "HL");
            AluInstructionWorksAsExpected("adc hl,hl", "ADC", "HL", "HL");
            AluInstructionWorksAsExpected("adc hl,sp", "ADC", "SP", "HL");

            AluInstructionWorksAsExpected("adc a,b", "ADC", "B", "A");
            AluInstructionWorksAsExpected("adc a,c", "ADC", "C", "A");
            AluInstructionWorksAsExpected("adc a,d", "ADC", "D", "A");
            AluInstructionWorksAsExpected("adc a,e", "ADC", "E", "A");
            AluInstructionWorksAsExpected("adc a,h", "ADC", "H", "A");
            AluInstructionWorksAsExpected("adc a,l", "ADC", "L", "A");
            AluInstructionWorksAsExpected("adc a,(hl)", "ADC", "(HL)", "A");
            AluInstructionWorksAsExpected("adc a,a", "ADC", "A", "A");

            AluInstructionWorksAsExpected("adc a,xh", "ADC", "XH", "A");
            AluInstructionWorksAsExpected("adc a,xl", "ADC", "XL", "A");
            AluInstructionWorksAsExpected("adc a,yh", "ADC", "YH", "A");
            AluInstructionWorksAsExpected("adc a,yl", "ADC", "YL", "A");

            AluInstructionWorksAsExpected("adc a,(ix)", "ADC", "A", "IX", null);
            AluInstructionWorksAsExpected("adc a,(ix+#04)", "ADC", "A", "IX", "+");
            AluInstructionWorksAsExpected("adc a,(ix-#04)", "ADC", "A", "IX", "-");
            AluInstructionWorksAsExpected("adc a,(iy)", "ADC", "A", "IY", null);
            AluInstructionWorksAsExpected("adc a,(iy+#04)", "ADC", "A", "IY", "+");
            AluInstructionWorksAsExpected("adc a,(iy-#04)", "ADC", "A", "IY", "-");

            AluInstructionWorksAsExpected("adc a,#12", "ADC", "A");
            AluInstructionWorksAsExpected("adc a,Some", "ADC", "A");
        }

        [TestMethod]
        public void SubInstructionWorksAsExpected()
        {
            AluInstructionWorksAsExpected("sub b", "SUB", "B", null);
            AluInstructionWorksAsExpected("sub c", "SUB", "C", null);
            AluInstructionWorksAsExpected("sub d", "SUB", "D", null);
            AluInstructionWorksAsExpected("sub e", "SUB", "E", null);
            AluInstructionWorksAsExpected("sub h", "SUB", "H", null);
            AluInstructionWorksAsExpected("sub l", "SUB", "L", null);
            AluInstructionWorksAsExpected("sub (hl)", "SUB", "(HL)", null);
            AluInstructionWorksAsExpected("sub a", "SUB", "A", null);

            AluInstructionWorksAsExpected("sub xh", "SUB", "XH", null);
            AluInstructionWorksAsExpected("sub xl", "SUB", "XL", null);
            AluInstructionWorksAsExpected("sub yh", "SUB", "YH", null);
            AluInstructionWorksAsExpected("sub yl", "SUB", "YL", null);

            AluInstructionWorksAsExpected("sub (ix)", "SUB", null, "IX", null);
            AluInstructionWorksAsExpected("sub (ix+#04)", "SUB", null, "IX", "+");
            AluInstructionWorksAsExpected("sub (ix-#04)", "SUB", null, "IX", "-");
            AluInstructionWorksAsExpected("sub (iy)", "SUB", null, "IY", null);
            AluInstructionWorksAsExpected("sub (iy+#04)", "SUB", null, "IY", "+");
            AluInstructionWorksAsExpected("sub (iy-#04)", "SUB", null, "IY", "-");

            AluInstructionWorksAsExpected("sub #12", "SUB", null);
            AluInstructionWorksAsExpected("sub Some", "SUB", null);
        }

        [TestMethod]
        public void SbcInstructionWorksAsExpected()
        {
            AluInstructionWorksAsExpected("sbc hl,bc", "SBC", "BC", "HL");
            AluInstructionWorksAsExpected("sbc hl,de", "SBC", "DE", "HL");
            AluInstructionWorksAsExpected("sbc hl,hl", "SBC", "HL", "HL");
            AluInstructionWorksAsExpected("sbc hl,sp", "SBC", "SP", "HL");

            AluInstructionWorksAsExpected("sbc a,b", "SBC", "B", "A");
            AluInstructionWorksAsExpected("sbc a,c", "SBC", "C", "A");
            AluInstructionWorksAsExpected("sbc a,d", "SBC", "D", "A");
            AluInstructionWorksAsExpected("sbc a,e", "SBC", "E", "A");
            AluInstructionWorksAsExpected("sbc a,h", "SBC", "H", "A");
            AluInstructionWorksAsExpected("sbc a,l", "SBC", "L", "A");
            AluInstructionWorksAsExpected("sbc a,(hl)", "SBC", "(HL)", "A");
            AluInstructionWorksAsExpected("sbc a,a", "SBC", "A", "A");

            AluInstructionWorksAsExpected("sbc a,xh", "SBC", "XH", "A");
            AluInstructionWorksAsExpected("sbc a,xl", "SBC", "XL", "A");
            AluInstructionWorksAsExpected("sbc a,yh", "SBC", "YH", "A");
            AluInstructionWorksAsExpected("sbc a,yl", "SBC", "YL", "A");

            AluInstructionWorksAsExpected("sbc a,(ix)", "SBC", "A", "IX", null);
            AluInstructionWorksAsExpected("sbc a,(ix+#04)", "SBC", "A", "IX", "+");
            AluInstructionWorksAsExpected("sbc a,(ix-#04)", "SBC", "A", "IX", "-");
            AluInstructionWorksAsExpected("sbc a,(iy)", "SBC", "A", "IY", null);
            AluInstructionWorksAsExpected("sbc a,(iy+#04)", "SBC", "A", "IY", "+");
            AluInstructionWorksAsExpected("sbc a,(iy-#04)", "SBC", "A", "IY", "-");

            AluInstructionWorksAsExpected("sbc a,#12", "SBC", "A");
            AluInstructionWorksAsExpected("sbc a,Some", "SBC", "A");
        }

        [TestMethod]
        public void AndInstructionWorksAsExpected()
        {
            AluInstructionWorksAsExpected("and b", "AND", "B", null);
            AluInstructionWorksAsExpected("and c", "AND", "C", null);
            AluInstructionWorksAsExpected("and d", "AND", "D", null);
            AluInstructionWorksAsExpected("and e", "AND", "E", null);
            AluInstructionWorksAsExpected("and h", "AND", "H", null);
            AluInstructionWorksAsExpected("and l", "AND", "L", null);
            AluInstructionWorksAsExpected("and (hl)", "AND", "(HL)", null);
            AluInstructionWorksAsExpected("and a", "AND", "A", null);

            AluInstructionWorksAsExpected("and xh", "AND", "XH", null);
            AluInstructionWorksAsExpected("and xl", "AND", "XL", null);
            AluInstructionWorksAsExpected("and yh", "AND", "YH", null);
            AluInstructionWorksAsExpected("and yl", "AND", "YL", null);

            AluInstructionWorksAsExpected("and (ix)", "AND", null, "IX", null);
            AluInstructionWorksAsExpected("and (ix+#04)", "AND", null, "IX", "+");
            AluInstructionWorksAsExpected("and (ix-#04)", "AND", null, "IX", "-");
            AluInstructionWorksAsExpected("and (iy)", "AND", null, "IY", null);
            AluInstructionWorksAsExpected("and (iy+#04)", "AND", null, "IY", "+");
            AluInstructionWorksAsExpected("and (iy-#04)", "AND", null, "IY", "-");

            AluInstructionWorksAsExpected("and #12", "AND", null);
            AluInstructionWorksAsExpected("and Some", "AND", null);
        }

        [TestMethod]
        public void XorInstructionWorksAsExpected()
        {
            AluInstructionWorksAsExpected("xor b", "XOR", "B", null);
            AluInstructionWorksAsExpected("xor c", "XOR", "C", null);
            AluInstructionWorksAsExpected("xor d", "XOR", "D", null);
            AluInstructionWorksAsExpected("xor e", "XOR", "E", null);
            AluInstructionWorksAsExpected("xor h", "XOR", "H", null);
            AluInstructionWorksAsExpected("xor l", "XOR", "L", null);
            AluInstructionWorksAsExpected("xor (hl)", "XOR", "(HL)", null);
            AluInstructionWorksAsExpected("xor a", "XOR", "A", null);

            AluInstructionWorksAsExpected("xor xh", "XOR", "XH", null);
            AluInstructionWorksAsExpected("xor xl", "XOR", "XL", null);
            AluInstructionWorksAsExpected("xor yh", "XOR", "YH", null);
            AluInstructionWorksAsExpected("xor yl", "XOR", "YL", null);

            AluInstructionWorksAsExpected("xor (ix)", "XOR", null, "IX", null);
            AluInstructionWorksAsExpected("xor (ix+#04)", "XOR", null, "IX", "+");
            AluInstructionWorksAsExpected("xor (ix-#04)", "XOR", null, "IX", "-");
            AluInstructionWorksAsExpected("xor (iy)", "XOR", null, "IY", null);
            AluInstructionWorksAsExpected("xor (iy+#04)", "XOR", null, "IY", "+");
            AluInstructionWorksAsExpected("xor (iy-#04)", "XOR", null, "IY", "-");

            AluInstructionWorksAsExpected("xor #12", "XOR", null);
            AluInstructionWorksAsExpected("xor Some", "XOR", null);
        }

        [TestMethod]
        public void OrInstructionWorksAsExpected()
        {
            AluInstructionWorksAsExpected("or b", "OR", "B", null);
            AluInstructionWorksAsExpected("or c", "OR", "C", null);
            AluInstructionWorksAsExpected("or d", "OR", "D", null);
            AluInstructionWorksAsExpected("or e", "OR", "E", null);
            AluInstructionWorksAsExpected("or h", "OR", "H", null);
            AluInstructionWorksAsExpected("or l", "OR", "L", null);
            AluInstructionWorksAsExpected("or (hl)", "OR", "(HL)", null);
            AluInstructionWorksAsExpected("or a", "OR", "A", null);

            AluInstructionWorksAsExpected("or xh", "OR", "XH", null);
            AluInstructionWorksAsExpected("or xl", "OR", "XL", null);
            AluInstructionWorksAsExpected("or yh", "OR", "YH", null);
            AluInstructionWorksAsExpected("or yl", "OR", "YL", null);

            AluInstructionWorksAsExpected("or (ix)", "OR", null, "IX", null);
            AluInstructionWorksAsExpected("or (ix+#04)", "OR", null, "IX", "+");
            AluInstructionWorksAsExpected("or (ix-#04)", "OR", null, "IX", "-");
            AluInstructionWorksAsExpected("or (iy)", "OR", null, "IY", null);
            AluInstructionWorksAsExpected("or (iy+#04)", "OR", null, "IY", "+");
            AluInstructionWorksAsExpected("or (iy-#04)", "OR", null, "IY", "-");

            AluInstructionWorksAsExpected("or #12", "OR", null);
            AluInstructionWorksAsExpected("or Some", "OR", null);
        }

        [TestMethod]
        public void CpInstructionWorksAsExpected()
        {
            AluInstructionWorksAsExpected("cp b", "CP", "B", null);
            AluInstructionWorksAsExpected("cp c", "CP", "C", null);
            AluInstructionWorksAsExpected("cp d", "CP", "D", null);
            AluInstructionWorksAsExpected("cp e", "CP", "E", null);
            AluInstructionWorksAsExpected("cp h", "CP", "H", null);
            AluInstructionWorksAsExpected("cp l", "CP", "L", null);
            AluInstructionWorksAsExpected("cp (hl)", "CP", "(HL)", null);
            AluInstructionWorksAsExpected("cp a", "CP", "A", null);

            AluInstructionWorksAsExpected("cp xh", "CP", "XH", null);
            AluInstructionWorksAsExpected("cp xl", "CP", "XL", null);
            AluInstructionWorksAsExpected("cp yh", "CP", "YH", null);
            AluInstructionWorksAsExpected("cp yl", "CP", "YL", null);

            AluInstructionWorksAsExpected("cp (ix)", "CP", null, "IX", null);
            AluInstructionWorksAsExpected("cp (ix+#04)", "CP", null, "IX", "+");
            AluInstructionWorksAsExpected("cp (ix-#04)", "CP", null, "IX", "-");
            AluInstructionWorksAsExpected("cp (iy)", "CP", null, "IY", null);
            AluInstructionWorksAsExpected("cp (iy+#04)", "CP", null, "IY", "+");
            AluInstructionWorksAsExpected("cp (iy-#04)", "CP", null, "IY", "-");

            AluInstructionWorksAsExpected("cp #12", "CP", null);
            AluInstructionWorksAsExpected("cp Some", "CP", null);
        }

        protected void AluInstructionWorksAsExpected(string instruction, string type, string source, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as AluOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            if (dest == "A")
            {
                line.Register.ShouldBeNull();
            }
            else
            {
                line.Register.ShouldBe(dest);
            }
            line.Operand.AddressingType.ShouldBe(AddressingType.Register);
            line.Operand.Register.ShouldBe(source);
            line.Operand.Sign.ShouldBeNull();
            line.Operand.Expression.ShouldBeNull();
        }

        protected void AluInstructionWorksAsExpected(string instruction, string type, string dest, string reg, string sign)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as AluOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Register.ShouldBeNull();
            line.Operand.AddressingType.ShouldBe(AddressingType.IndexedAddress);
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

        protected void AluInstructionWorksAsExpected(string instruction, string type, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as AluOperation;
            line.ShouldNotBeNull();
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Register.ShouldBeNull();
            line.Operand.AddressingType.ShouldBe(AddressingType.Expression);
            line.Operand.Register.ShouldBeNull();
            line.Operand.Sign.ShouldBeNull();
            line.Operand.Expression.ShouldNotBeNull();
        }
    }
}