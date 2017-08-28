using AntlrZ80Asm.SyntaxTree;
using AntlrZ80Asm.SyntaxTree.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class LoadOperationTests : ParserTestBed
    {
        [TestMethod]
        public void Load8BitRegTo8BitRegWorksAsExpected()
        {
            Load8BitTo8BitWorks("ld b, b", "B", "B");
            Load8BitTo8BitWorks("ld b, c", "C", "B");
            Load8BitTo8BitWorks("ld b, d", "D", "B");
            Load8BitTo8BitWorks("ld b, e", "E", "B");
            Load8BitTo8BitWorks("ld b, h", "H", "B");
            Load8BitTo8BitWorks("ld b, l", "L", "B");
            Load8BitTo8BitWorks("ld b, (hl)", "(HL)", "B");
            Load8BitTo8BitWorks("ld b, ( HL )", "(HL)", "B");
            Load8BitTo8BitWorks("ld b, a", "A", "B");

            Load8BitTo8BitWorks("ld c, b", "B", "C");
            Load8BitTo8BitWorks("ld c, c", "C", "C");
            Load8BitTo8BitWorks("ld c, d", "D", "C");
            Load8BitTo8BitWorks("ld c, e", "E", "C");
            Load8BitTo8BitWorks("ld c, h", "H", "C");
            Load8BitTo8BitWorks("ld c, l", "L", "C");
            Load8BitTo8BitWorks("ld c, (hl)", "(HL)", "C");
            Load8BitTo8BitWorks("ld c, a", "A", "C");

            Load8BitTo8BitWorks("ld d, b", "B", "D");
            Load8BitTo8BitWorks("ld d, c", "C", "D");
            Load8BitTo8BitWorks("ld d, d", "D", "D");
            Load8BitTo8BitWorks("ld d, e", "E", "D");
            Load8BitTo8BitWorks("ld d, h", "H", "D");
            Load8BitTo8BitWorks("ld d, l", "L", "D");
            Load8BitTo8BitWorks("ld d, (hl)", "(HL)", "D");
            Load8BitTo8BitWorks("ld d, a", "A", "D");

            Load8BitTo8BitWorks("ld e, b", "B", "E");
            Load8BitTo8BitWorks("ld e, c", "C", "E");
            Load8BitTo8BitWorks("ld e, d", "D", "E");
            Load8BitTo8BitWorks("ld e, e", "E", "E");
            Load8BitTo8BitWorks("ld e, h", "H", "E");
            Load8BitTo8BitWorks("ld e, l", "L", "E");
            Load8BitTo8BitWorks("ld e, (hl)", "(HL)", "E");
            Load8BitTo8BitWorks("ld e, a", "A", "E");

            Load8BitTo8BitWorks("ld h, b", "B", "H");
            Load8BitTo8BitWorks("ld h, c", "C", "H");
            Load8BitTo8BitWorks("ld h, d", "D", "H");
            Load8BitTo8BitWorks("ld h, e", "E", "H");
            Load8BitTo8BitWorks("ld h, h", "H", "H");
            Load8BitTo8BitWorks("ld h, l", "L", "H");
            Load8BitTo8BitWorks("ld h, (hl)", "(HL)", "H");
            Load8BitTo8BitWorks("ld h, a", "A", "H");

            Load8BitTo8BitWorks("ld l, b", "B", "L");
            Load8BitTo8BitWorks("ld l, c", "C", "L");
            Load8BitTo8BitWorks("ld l, d", "D", "L");
            Load8BitTo8BitWorks("ld l, e", "E", "L");
            Load8BitTo8BitWorks("ld l, h", "H", "L");
            Load8BitTo8BitWorks("ld l, l", "L", "L");
            Load8BitTo8BitWorks("ld l, (hl)", "(HL)", "L");
            Load8BitTo8BitWorks("ld l, a", "A", "L");

            Load8BitTo8BitWorks("ld (hl), b", "B", "(HL)");
            Load8BitTo8BitWorks("ld (hl), c", "C", "(HL)");
            Load8BitTo8BitWorks("ld (hl), d", "D", "(HL)");
            Load8BitTo8BitWorks("ld (hl), e", "E", "(HL)");
            Load8BitTo8BitWorks("ld (hl), h", "H", "(HL)");
            Load8BitTo8BitWorks("ld (hl), l", "L", "(HL)");
            Load8BitTo8BitWorks("ld (hl), (hl)", "(HL)", "(HL)"); // --- This is syntactically correct, though semantically not
            Load8BitTo8BitWorks("ld (hl), a", "A", "(HL)");

            Load8BitTo8BitWorks("ld a, i", "I", "A");
            Load8BitTo8BitWorks("ld i, a", "A", "I");
            Load8BitTo8BitWorks("ld a, r", "R", "A");
            Load8BitTo8BitWorks("ld r, a", "A", "R");

            Load8BitTo8BitWorks("ld b, xl", "XL", "B");
            Load8BitTo8BitWorks("ld c, xl", "XL", "C");
            Load8BitTo8BitWorks("ld d, xl", "XL", "D");
            Load8BitTo8BitWorks("ld e, xl", "XL", "E");
            Load8BitTo8BitWorks("ld h, xl", "XL", "H");
            Load8BitTo8BitWorks("ld l, xl", "XL", "L");
            Load8BitTo8BitWorks("ld a, xl", "XL", "A");

            Load8BitTo8BitWorks("ld b, xh", "XH", "B");
            Load8BitTo8BitWorks("ld c, xh", "XH", "C");
            Load8BitTo8BitWorks("ld d, xh", "XH", "D");
            Load8BitTo8BitWorks("ld e, xh", "XH", "E");
            Load8BitTo8BitWorks("ld h, xh", "XH", "H");
            Load8BitTo8BitWorks("ld l, xh", "XH", "L");
            Load8BitTo8BitWorks("ld a, xh", "XH", "A");

            Load8BitTo8BitWorks("ld b, yl", "YL", "B");
            Load8BitTo8BitWorks("ld c, yl", "YL", "C");
            Load8BitTo8BitWorks("ld d, yl", "YL", "D");
            Load8BitTo8BitWorks("ld e, yl", "YL", "E");
            Load8BitTo8BitWorks("ld h, yl", "YL", "H");
            Load8BitTo8BitWorks("ld l, yl", "YL", "L");
            Load8BitTo8BitWorks("ld a, yl", "YL", "A");

            Load8BitTo8BitWorks("ld b, yh", "YH", "B");
            Load8BitTo8BitWorks("ld c, yh", "YH", "C");
            Load8BitTo8BitWorks("ld d, yh", "YH", "D");
            Load8BitTo8BitWorks("ld e, yh", "YH", "E");
            Load8BitTo8BitWorks("ld h, yh", "YH", "H");
            Load8BitTo8BitWorks("ld l, yh", "YH", "L");
            Load8BitTo8BitWorks("ld a, yh", "YH", "A");
        }

        [TestMethod]
        public void Load8BitFromReg16AddrWorksAsExpected()
        {
            Load8BitFromReg16AddrWorks("ld a,(bc)", "BC", "A");
            Load8BitFromReg16AddrWorks("ld a,(de)", "DE", "A");
        }

        [TestMethod]
        public void Load8BitFromIndexedAddrWorksAsExpected()
        {
            Load8BitFromIndexedAddrWorks("ld a,(ix)", "A", "IX", null);
            Load8BitFromIndexedAddrWorks("ld a,(ix+#2)", "A", "IX", "+");
            Load8BitFromIndexedAddrWorks("ld a,(ix-#2)", "A", "IX", "-");
            Load8BitFromIndexedAddrWorks("ld a,(iy)", "A", "IY", null);
            Load8BitFromIndexedAddrWorks("ld a,(iy+#2)", "A", "IY", "+");
            Load8BitFromIndexedAddrWorks("ld a,(iy-#2)", "A", "IY", "-");
        }

        [TestMethod]
        public void Load8BitToReg16AddrWorksAsExpected()
        {
            Load8BitToReg16AddrWorks("ld (bc), a", "A", "BC");
            Load8BitToReg16AddrWorks("ld (de), a", "A", "DE");
        }

        [TestMethod]
        public void Load8BitToIndexedAddrWorksAsExpected()
        {
            Load8BitToIndexedAddrWorks("ld (ix), a", "A", "IX", null);
            Load8BitToIndexedAddrWorks("ld (ix+#02), a", "A", "IX", "+");
            Load8BitToIndexedAddrWorks("ld (ix - #02), a", "A", "IX", "-");
            Load8BitToIndexedAddrWorks("ld (iy), a", "A", "IY", null);
            Load8BitToIndexedAddrWorks("ld (iy+#02), a", "A", "IY", "+");
            Load8BitToIndexedAddrWorks("ld (iy - #02), a", "A", "IY", "-");
        }

        [TestMethod]
        public void LoadRegFromMemWorksAsExpected()
        {
            LoadRegFromMemWorks("ld a,(#1234)", "A");
            LoadRegFromMemWorks("ld bc,(#1234)", "BC");
            LoadRegFromMemWorks("ld de,(#1234)", "DE");
            LoadRegFromMemWorks("ld hl,(#1234)", "HL");
            LoadRegFromMemWorks("ld ix,(#1234)", "IX");
            LoadRegFromMemWorks("ld iy,(#1234)", "IY");
        }

        [TestMethod]
        public void LoadRegToMemWorksAsExpected()
        {
            LoadRegToMemWorks("ld (#1234),a", "A");
            LoadRegToMemWorks("ld (#1234),bc", "BC");
            LoadRegToMemWorks("ld (#1234),de", "DE");
            LoadRegToMemWorks("ld (#1234),hl", "HL");
            LoadRegToMemWorks("ld (#1234),ix", "IX");
            LoadRegToMemWorks("ld (#1234),iy", "IY");
        }

        [TestMethod]
        public void LoadValueTo8BitRegWorksAsExpected()
        {
            LoadValueTo8BitRegWorks("ld b, 12345", "B");
            LoadValueTo8BitRegWorks("ld c, 12345", "C");
            LoadValueTo8BitRegWorks("ld d, 12345", "D");
            LoadValueTo8BitRegWorks("ld e, 12345", "E");
            LoadValueTo8BitRegWorks("ld h, 12345", "H");
            LoadValueTo8BitRegWorks("ld l, 12345", "L");
            LoadValueTo8BitRegWorks("ld (hl), 12345", "(HL)");
            LoadValueTo8BitRegWorks("ld a, 12345", "A");

            LoadValueTo8BitRegWorks("ld bc, 12345", "BC");
            LoadValueTo8BitRegWorks("ld de, 12345", "DE");
            LoadValueTo8BitRegWorks("ld hl, 12345", "HL");
            LoadValueTo8BitRegWorks("ld sp, 12345", "SP");
            LoadValueTo8BitRegWorks("ld ix, 12345", "IX");
            LoadValueTo8BitRegWorks("ld iy, 12345", "IY");
        }

        protected void Load8BitTo8BitWorks(string instruction, string source, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadOperation;
            line.ShouldNotBeNull();
            line.SourceOperand.AddressingType.ShouldBe(AddressingType.Register);
            line.SourceOperand.Register.ShouldBe(source);
            line.DestinationOperand.AddressingType.ShouldBe(AddressingType.Register);
            line.DestinationOperand.Register.ShouldBe(dest);
        }

        protected void Load8BitFromReg16AddrWorks(string instruction, string source, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadOperation;
            line.ShouldNotBeNull();
            line.SourceOperand.AddressingType.ShouldBe(AddressingType.RegisterIndirection);
            line.SourceOperand.Register.ShouldBe(source);
            line.DestinationOperand.AddressingType.ShouldBe(AddressingType.Register);
            line.DestinationOperand.Register.ShouldBe(dest);
        }

        protected void Load8BitFromIndexedAddrWorks(string instruction, string dest, string reg, string sign)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadOperation;
            line.ShouldNotBeNull();
            line.DestinationOperand.AddressingType.ShouldBe(AddressingType.Register);
            line.DestinationOperand.Register.ShouldBe(dest);
            line.SourceOperand.AddressingType.ShouldBe(AddressingType.IndexedAddress);
            line.SourceOperand.Register.ShouldBe(reg);
            line.SourceOperand.Sign.ShouldBe(sign);
            if (sign == null)
            {
                line.SourceOperand.Expression.ShouldBeNull();
            }
            else
            {
                line.SourceOperand.Expression.ShouldNotBeNull();
            }
        }

        protected void Load8BitToReg16AddrWorks(string instruction, string source, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadOperation;
            line.ShouldNotBeNull();
            line.SourceOperand.AddressingType.ShouldBe(AddressingType.Register);
            line.SourceOperand.Register.ShouldBe(source);
            line.DestinationOperand.AddressingType.ShouldBe(AddressingType.RegisterIndirection);
            line.DestinationOperand.Register.ShouldBe(dest);
        }

        protected void Load8BitToIndexedAddrWorks(string instruction, string source, string reg, string sign)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadOperation;
            line.ShouldNotBeNull();
            line.SourceOperand.AddressingType.ShouldBe(AddressingType.Register);
            line.SourceOperand.Register.ShouldBe(source);
            line.DestinationOperand.AddressingType.ShouldBe(AddressingType.IndexedAddress);
            line.DestinationOperand.Register.ShouldBe(reg);
            line.DestinationOperand.Sign.ShouldBe(sign);
            if (sign == null)
            {
                line.DestinationOperand.Expression.ShouldBeNull();
            }
            else
            {
                line.DestinationOperand.Expression.ShouldNotBeNull();
            }
        }

        protected void LoadRegFromMemWorks(string instruction, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadOperation;
            line.ShouldNotBeNull();
            line.SourceOperand.AddressingType.ShouldBe(AddressingType.AddressIndirection);
            line.SourceOperand.Expression.ShouldNotBeNull();
            line.DestinationOperand.AddressingType.ShouldBe(AddressingType.Register);
            line.DestinationOperand.Register.ShouldBe(dest);
        }

        protected void LoadRegToMemWorks(string instruction, string source)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadOperation;
            line.ShouldNotBeNull();
            line.SourceOperand.AddressingType.ShouldBe(AddressingType.Register);
            line.SourceOperand.Register.ShouldBe(source);
            line.DestinationOperand.AddressingType.ShouldBe(AddressingType.AddressIndirection);
            line.DestinationOperand.Expression.ShouldNotBeNull();
        }

        protected void LoadValueTo8BitRegWorks(string instruction, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadOperation;
            line.ShouldNotBeNull();
            line.SourceOperand.AddressingType.ShouldBe(AddressingType.Expression);
            line.SourceOperand.Expression.ShouldNotBeNull();
            line.DestinationOperand.AddressingType.ShouldBe(AddressingType.Register);
            line.DestinationOperand.Register.ShouldBe(dest);
        }
    }
}