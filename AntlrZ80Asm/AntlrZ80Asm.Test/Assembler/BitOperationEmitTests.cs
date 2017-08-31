using AntlrZ80Asm.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Assembler
{
    [TestClass]
    public class BitOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void BitOpsWorkAsExpected()
        {
            CodeEmitWorks("bit 0,b", 0x40);
            CodeEmitWorks("bit 0,c", 0x41);
            CodeEmitWorks("bit 0,d", 0x42);
            CodeEmitWorks("bit 0,e", 0x43);
            CodeEmitWorks("bit 0,h", 0x44);
            CodeEmitWorks("bit 0,l", 0x45);
            CodeEmitWorks("bit 0,(hl)", 0x46);
            CodeEmitWorks("bit 0,a", 0x47);

            CodeEmitWorks("bit 1,b", 0x48);
            CodeEmitWorks("bit 1,c", 0x49);
            CodeEmitWorks("bit 1,d", 0x4A);
            CodeEmitWorks("bit 1,e", 0x4B);
            CodeEmitWorks("bit 1,h", 0x4C);
            CodeEmitWorks("bit 1,l", 0x4D);
            CodeEmitWorks("bit 1,(hl)", 0x4E);
            CodeEmitWorks("bit 1,a", 0x4F);

            CodeEmitWorks("bit 2,b", 0x50);
            CodeEmitWorks("bit 2,c", 0x51);
            CodeEmitWorks("bit 2,d", 0x52);
            CodeEmitWorks("bit 2,e", 0x53);
            CodeEmitWorks("bit 2,h", 0x54);
            CodeEmitWorks("bit 2,l", 0x55);
            CodeEmitWorks("bit 2,(hl)", 0x56);
            CodeEmitWorks("bit 2,a", 0x57);

            CodeEmitWorks("bit 3,b", 0x58);
            CodeEmitWorks("bit 3,c", 0x59);
            CodeEmitWorks("bit 3,d", 0x5A);
            CodeEmitWorks("bit 3,e", 0x5B);
            CodeEmitWorks("bit 3,h", 0x5C);
            CodeEmitWorks("bit 3,l", 0x5D);
            CodeEmitWorks("bit 3,(hl)", 0x5E);
            CodeEmitWorks("bit 3,a", 0x5F);

            CodeEmitWorks("bit 4,b", 0x60);
            CodeEmitWorks("bit 4,c", 0x61);
            CodeEmitWorks("bit 4,d", 0x62);
            CodeEmitWorks("bit 4,e", 0x63);
            CodeEmitWorks("bit 4,h", 0x64);
            CodeEmitWorks("bit 4,l", 0x65);
            CodeEmitWorks("bit 4,(hl)", 0x66);
            CodeEmitWorks("bit 4,a", 0x67);

            CodeEmitWorks("bit 5,b", 0x68);
            CodeEmitWorks("bit 5,c", 0x69);
            CodeEmitWorks("bit 5,d", 0x6A);
            CodeEmitWorks("bit 5,e", 0x6B);
            CodeEmitWorks("bit 5,h", 0x6C);
            CodeEmitWorks("bit 5,l", 0x6D);
            CodeEmitWorks("bit 5,(hl)", 0x6E);
            CodeEmitWorks("bit 5,a", 0x6F);

            CodeEmitWorks("bit 6,b", 0x70);
            CodeEmitWorks("bit 6,c", 0x71);
            CodeEmitWorks("bit 6,d", 0x72);
            CodeEmitWorks("bit 6,e", 0x73);
            CodeEmitWorks("bit 6,h", 0x74);
            CodeEmitWorks("bit 6,l", 0x75);
            CodeEmitWorks("bit 6,(hl)", 0x76);
            CodeEmitWorks("bit 6,a", 0x77);

            CodeEmitWorks("bit 7,b", 0x78);
            CodeEmitWorks("bit 7,c", 0x79);
            CodeEmitWorks("bit 7,d", 0x7A);
            CodeEmitWorks("bit 7,e", 0x7B);
            CodeEmitWorks("bit 7,h", 0x7C);
            CodeEmitWorks("bit 7,l", 0x7D);
            CodeEmitWorks("bit 7,(hl)", 0x7E);
            CodeEmitWorks("bit 7,a", 0x7F);
        }

        [TestMethod]
        public void ResOpsWorkAsExpected()
        {
            CodeEmitWorks("res 0,b", 0x80);
            CodeEmitWorks("res 0,c", 0x81);
            CodeEmitWorks("res 0,d", 0x82);
            CodeEmitWorks("res 0,e", 0x83);
            CodeEmitWorks("res 0,h", 0x84);
            CodeEmitWorks("res 0,l", 0x85);
            CodeEmitWorks("res 0,(hl)", 0x86);
            CodeEmitWorks("res 0,a", 0x87);

            CodeEmitWorks("res 1,b", 0x88);
            CodeEmitWorks("res 1,c", 0x89);
            CodeEmitWorks("res 1,d", 0x8A);
            CodeEmitWorks("res 1,e", 0x8B);
            CodeEmitWorks("res 1,h", 0x8C);
            CodeEmitWorks("res 1,l", 0x8D);
            CodeEmitWorks("res 1,(hl)", 0x8E);
            CodeEmitWorks("res 1,a", 0x8F);

            CodeEmitWorks("res 2,b", 0x90);
            CodeEmitWorks("res 2,c", 0x91);
            CodeEmitWorks("res 2,d", 0x92);
            CodeEmitWorks("res 2,e", 0x93);
            CodeEmitWorks("res 2,h", 0x94);
            CodeEmitWorks("res 2,l", 0x95);
            CodeEmitWorks("res 2,(hl)", 0x96);
            CodeEmitWorks("res 2,a", 0x97);

            CodeEmitWorks("res 3,b", 0x98);
            CodeEmitWorks("res 3,c", 0x99);
            CodeEmitWorks("res 3,d", 0x9A);
            CodeEmitWorks("res 3,e", 0x9B);
            CodeEmitWorks("res 3,h", 0x9C);
            CodeEmitWorks("res 3,l", 0x9D);
            CodeEmitWorks("res 3,(hl)", 0x9E);
            CodeEmitWorks("res 3,a", 0x9F);

            CodeEmitWorks("res 4,b", 0xA0);
            CodeEmitWorks("res 4,c", 0xA1);
            CodeEmitWorks("res 4,d", 0xA2);
            CodeEmitWorks("res 4,e", 0xA3);
            CodeEmitWorks("res 4,h", 0xA4);
            CodeEmitWorks("res 4,l", 0xA5);
            CodeEmitWorks("res 4,(hl)", 0xA6);
            CodeEmitWorks("res 4,a", 0xA7);

            CodeEmitWorks("res 5,b", 0xA8);
            CodeEmitWorks("res 5,c", 0xA9);
            CodeEmitWorks("res 5,d", 0xAA);
            CodeEmitWorks("res 5,e", 0xAB);
            CodeEmitWorks("res 5,h", 0xAC);
            CodeEmitWorks("res 5,l", 0xAD);
            CodeEmitWorks("res 5,(hl)", 0xAE);
            CodeEmitWorks("res 5,a", 0xAF);

            CodeEmitWorks("res 6,b", 0xB0);
            CodeEmitWorks("res 6,c", 0xB1);
            CodeEmitWorks("res 6,d", 0xB2);
            CodeEmitWorks("res 6,e", 0xB3);
            CodeEmitWorks("res 6,h", 0xB4);
            CodeEmitWorks("res 6,l", 0xB5);
            CodeEmitWorks("res 6,(hl)", 0xB6);
            CodeEmitWorks("res 6,a", 0xB7);

            CodeEmitWorks("res 7,b", 0xB8);
            CodeEmitWorks("res 7,c", 0xB9);
            CodeEmitWorks("res 7,d", 0xBA);
            CodeEmitWorks("res 7,e", 0xBB);
            CodeEmitWorks("res 7,h", 0xBC);
            CodeEmitWorks("res 7,l", 0xBD);
            CodeEmitWorks("res 7,(hl)", 0xBE);
            CodeEmitWorks("res 7,a", 0xBF);
        }

        [TestMethod]
        public void SetOpsWorkAsExpected()
        {
            CodeEmitWorks("set 0,b", 0xC0);
            CodeEmitWorks("set 0,c", 0xC1);
            CodeEmitWorks("set 0,d", 0xC2);
            CodeEmitWorks("set 0,e", 0xC3);
            CodeEmitWorks("set 0,h", 0xC4);
            CodeEmitWorks("set 0,l", 0xC5);
            CodeEmitWorks("set 0,(hl)", 0xC6);
            CodeEmitWorks("set 0,a", 0xC7);

            CodeEmitWorks("set 1,b", 0xC8);
            CodeEmitWorks("set 1,c", 0xC9);
            CodeEmitWorks("set 1,d", 0xCA);
            CodeEmitWorks("set 1,e", 0xCB);
            CodeEmitWorks("set 1,h", 0xCC);
            CodeEmitWorks("set 1,l", 0xCD);
            CodeEmitWorks("set 1,(hl)", 0xCE);
            CodeEmitWorks("set 1,a", 0xCF);

            CodeEmitWorks("set 2,b", 0xD0);
            CodeEmitWorks("set 2,c", 0xD1);
            CodeEmitWorks("set 2,d", 0xD2);
            CodeEmitWorks("set 2,e", 0xD3);
            CodeEmitWorks("set 2,h", 0xD4);
            CodeEmitWorks("set 2,l", 0xD5);
            CodeEmitWorks("set 2,(hl)", 0xD6);
            CodeEmitWorks("set 2,a", 0xD7);

            CodeEmitWorks("set 3,b", 0xD8);
            CodeEmitWorks("set 3,c", 0xD9);
            CodeEmitWorks("set 3,d", 0xDA);
            CodeEmitWorks("set 3,e", 0xDB);
            CodeEmitWorks("set 3,h", 0xDC);
            CodeEmitWorks("set 3,l", 0xDD);
            CodeEmitWorks("set 3,(hl)", 0xDE);
            CodeEmitWorks("set 3,a", 0xDF);

            CodeEmitWorks("set 4,b", 0xE0);
            CodeEmitWorks("set 4,c", 0xE1);
            CodeEmitWorks("set 4,d", 0xE2);
            CodeEmitWorks("set 4,e", 0xE3);
            CodeEmitWorks("set 4,h", 0xE4);
            CodeEmitWorks("set 4,l", 0xE5);
            CodeEmitWorks("set 4,(hl)", 0xE6);
            CodeEmitWorks("set 4,a", 0xE7);

            CodeEmitWorks("set 5,b", 0xE8);
            CodeEmitWorks("set 5,c", 0xE9);
            CodeEmitWorks("set 5,d", 0xEA);
            CodeEmitWorks("set 5,e", 0xEB);
            CodeEmitWorks("set 5,h", 0xEC);
            CodeEmitWorks("set 5,l", 0xED);
            CodeEmitWorks("set 5,(hl)", 0xEE);
            CodeEmitWorks("set 5,a", 0xEF);

            CodeEmitWorks("set 6,b", 0xF0);
            CodeEmitWorks("set 6,c", 0xF1);
            CodeEmitWorks("set 6,d", 0xF2);
            CodeEmitWorks("set 6,e", 0xF3);
            CodeEmitWorks("set 6,h", 0xF4);
            CodeEmitWorks("set 6,l", 0xF5);
            CodeEmitWorks("set 6,(hl)", 0xF6);
            CodeEmitWorks("set 6,a", 0xF7);

            CodeEmitWorks("set 7,b", 0xF8);
            CodeEmitWorks("set 7,c", 0xF9);
            CodeEmitWorks("set 7,d", 0xFA);
            CodeEmitWorks("set 7,e", 0xFB);
            CodeEmitWorks("set 7,h", 0xFC);
            CodeEmitWorks("set 7,l", 0xFD);
            CodeEmitWorks("set 7,(hl)", 0xFE);
            CodeEmitWorks("set 7,a", 0xFF);
        }

        [TestMethod]
        public void BitWithInvalidBitIndexRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile("bit 12,a");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ShouldBeOfType<InvalidArgumentError>();
        }

        [TestMethod]
        public void ResWithInvalidBitIndexRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile("res -2,a");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ShouldBeOfType<InvalidArgumentError>();
        }

        [TestMethod]
        public void SetWithInvalidBitIndexRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile("set 2+6,a");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ShouldBeOfType<InvalidArgumentError>();
        }

        [TestMethod]
        public void OpsAcceptBitIndexExpression()
        {
            CodeEmitWorks("bit [3+4]*0,b", 0x40);
            CodeEmitWorks("res [3+4]*1,b", 0xB8);
            CodeEmitWorks("set [3+4]*2/3,c", 0xE1);
        }

        [TestMethod]
        public void BitWithUnresolvedBitIndexRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile("bit BitIndex,a");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ShouldBeOfType<InvalidArgumentError>();
        }

        [TestMethod]
        public void ResWithUnresolvedBitIndexRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile("res BitIndex,a");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ShouldBeOfType<InvalidArgumentError>();
        }

        [TestMethod]
        public void SetWithUnresolvedBitIndexRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile("set BitIndex,a");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ShouldBeOfType<InvalidArgumentError>();
        }

        [TestMethod]
        public void IxIndexedBitOpsWorkAsExpected()
        {
            CodeEmitWorks("bit 0,(ix)", 0xDD, 0x40, 0x00);
            CodeEmitWorks("bit 0,(ix+6)", 0xDD, 0x40, 0x06);
            CodeEmitWorks("bit 0,(ix-#0c)", 0xDD, 0x40, 0xF4);
            CodeEmitWorks("bit 1,(ix)", 0xDD, 0x48, 0x00);
            CodeEmitWorks("bit 1,(ix+6)", 0xDD, 0x48, 0x06);
            CodeEmitWorks("bit 1,(ix-#0c)", 0xDD, 0x48, 0xF4);
            CodeEmitWorks("bit 2,(ix)", 0xDD, 0x50, 0x00);
            CodeEmitWorks("bit 2,(ix+6)", 0xDD, 0x50, 0x06);
            CodeEmitWorks("bit 2,(ix-#0c)", 0xDD, 0x50, 0xF4);
            CodeEmitWorks("bit 3,(ix)", 0xDD, 0x58, 0x00);
            CodeEmitWorks("bit 3,(ix+6)", 0xDD, 0x58, 0x06);
            CodeEmitWorks("bit 3,(ix-#0c)", 0xDD, 0x58, 0xF4);
            CodeEmitWorks("bit 4,(ix)", 0xDD, 0x60, 0x00);
            CodeEmitWorks("bit 4,(ix+6)", 0xDD, 0x60, 0x06);
            CodeEmitWorks("bit 4,(ix-#0c)", 0xDD, 0x60, 0xF4);
            CodeEmitWorks("bit 5,(ix)", 0xDD, 0x68, 0x00);
            CodeEmitWorks("bit 5,(ix+6)", 0xDD, 0x68, 0x06);
            CodeEmitWorks("bit 5,(ix-#0c)", 0xDD, 0x68, 0xF4);
            CodeEmitWorks("bit 6,(ix)", 0xDD, 0x70, 0x00);
            CodeEmitWorks("bit 6,(ix+6)", 0xDD, 0x70, 0x06);
            CodeEmitWorks("bit 6,(ix-#0c)", 0xDD, 0x70, 0xF4);
            CodeEmitWorks("bit 7,(ix)", 0xDD, 0x78, 0x00);
            CodeEmitWorks("bit 7,(ix+6)", 0xDD, 0x78, 0x06);
            CodeEmitWorks("bit 7,(ix-#0c)", 0xDD, 0x78, 0xF4);
        }

        [TestMethod]
        public void IyIndexedBitOpsWorkAsExpected()
        {
            CodeEmitWorks("bit 0,(iy)", 0xFD, 0x40, 0x00);
            CodeEmitWorks("bit 0,(iy+6)", 0xFD, 0x40, 0x06);
            CodeEmitWorks("bit 0,(iy-#0c)", 0xFD, 0x40, 0xF4);
            CodeEmitWorks("bit 1,(iy)", 0xFD, 0x48, 0x00);
            CodeEmitWorks("bit 1,(iy+6)", 0xFD, 0x48, 0x06);
            CodeEmitWorks("bit 1,(iy-#0c)", 0xFD, 0x48, 0xF4);
            CodeEmitWorks("bit 2,(iy)", 0xFD, 0x50, 0x00);
            CodeEmitWorks("bit 2,(iy+6)", 0xFD, 0x50, 0x06);
            CodeEmitWorks("bit 2,(iy-#0c)", 0xFD, 0x50, 0xF4);
            CodeEmitWorks("bit 3,(iy)", 0xFD, 0x58, 0x00);
            CodeEmitWorks("bit 3,(iy+6)", 0xFD, 0x58, 0x06);
            CodeEmitWorks("bit 3,(iy-#0c)", 0xFD, 0x58, 0xF4);
            CodeEmitWorks("bit 4,(iy)", 0xFD, 0x60, 0x00);
            CodeEmitWorks("bit 4,(iy+6)", 0xFD, 0x60, 0x06);
            CodeEmitWorks("bit 4,(iy-#0c)", 0xFD, 0x60, 0xF4);
            CodeEmitWorks("bit 5,(iy)", 0xFD, 0x68, 0x00);
            CodeEmitWorks("bit 5,(iy+6)", 0xFD, 0x68, 0x06);
            CodeEmitWorks("bit 5,(iy-#0c)", 0xFD, 0x68, 0xF4);
            CodeEmitWorks("bit 6,(iy)", 0xFD, 0x70, 0x00);
            CodeEmitWorks("bit 6,(iy+6)", 0xFD, 0x70, 0x06);
            CodeEmitWorks("bit 6,(iy-#0c)", 0xFD, 0x70, 0xF4);
            CodeEmitWorks("bit 7,(iy)", 0xFD, 0x78, 0x00);
            CodeEmitWorks("bit 7,(iy+6)", 0xFD, 0x78, 0x06);
            CodeEmitWorks("bit 7,(iy-#0c)", 0xFD, 0x78, 0xF4);
        }

        [TestMethod]
        public void IxIndexedResOpsWorkAsExpected()
        {
            CodeEmitWorks("res 0,(ix),b", 0xDD, 0x80, 0x00);
            CodeEmitWorks("res 0,(ix+6),b", 0xDD, 0x80, 0x06);
            CodeEmitWorks("res 0,(ix-#0c),b", 0xDD, 0x80, 0xF4);
            CodeEmitWorks("res 0,(ix),c", 0xDD, 0x81, 0x00);
            CodeEmitWorks("res 0,(ix+6),c", 0xDD, 0x81, 0x06);
            CodeEmitWorks("res 0,(ix-#0c),c", 0xDD, 0x81, 0xF4);
            CodeEmitWorks("res 0,(ix),d", 0xDD, 0x82, 0x00);
            CodeEmitWorks("res 0,(ix+6),d", 0xDD, 0x82, 0x06);
            CodeEmitWorks("res 0,(ix-#0c),d", 0xDD, 0x82, 0xF4);
            CodeEmitWorks("res 0,(ix),e", 0xDD, 0x83, 0x00);
            CodeEmitWorks("res 0,(ix+6),e", 0xDD, 0x83, 0x06);
            CodeEmitWorks("res 0,(ix-#0c),e", 0xDD, 0x83, 0xF4);
            CodeEmitWorks("res 0,(ix),h", 0xDD, 0x84, 0x00);
            CodeEmitWorks("res 0,(ix+6),h", 0xDD, 0x84, 0x06);
            CodeEmitWorks("res 0,(ix-#0c),h", 0xDD, 0x84, 0xF4);
            CodeEmitWorks("res 0,(ix),l", 0xDD, 0x85, 0x00);
            CodeEmitWorks("res 0,(ix+6),l", 0xDD, 0x85, 0x06);
            CodeEmitWorks("res 0,(ix-#0c),l", 0xDD, 0x85, 0xF4);
            CodeEmitWorks("res 0,(ix)", 0xDD, 0x86, 0x00);
            CodeEmitWorks("res 0,(ix+6)", 0xDD, 0x86, 0x06);
            CodeEmitWorks("res 0,(ix-#0c)", 0xDD, 0x86, 0xF4);
            CodeEmitWorks("res 0,(ix),a", 0xDD, 0x87, 0x00);
            CodeEmitWorks("res 0,(ix+6),a", 0xDD, 0x87, 0x06);
            CodeEmitWorks("res 0,(ix-#0c),a", 0xDD, 0x87, 0xF4);

            // TODO: Add other bit tests
        }

    }
}