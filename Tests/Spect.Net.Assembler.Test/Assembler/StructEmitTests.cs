using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable StringLiteralTypo

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class StructEmitTests: AssemblerTestBed
    {
        [TestMethod]
        public void EmptyStructEmitsNoBytes()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE);
        }

        [TestMethod]
        [DataRow(".defb 0x00")]
        [DataRow(".defb 0x00, 0x23")]
        [DataRow(".defw 0x1234")]
        [DataRow(".defw 12345")]
        [DataRow(".defm \"Hello\"")]
        [DataRow(".defn \"Hello\"")]
        [DataRow(".defc \"Hello\"")]
        [DataRow(".defs 3")]
        [DataRow(".fillb 3,#aa")]
        [DataRow(".fillw 3,#aa55")]
        [DataRow(".defgx \"----OOOO\"")]
        [DataRow(".defg ----OOOO")]
        public void NoStructInvocationEmitsNoBytes(string body)
        {
            // --- Arrange
            var source = $@"
                MyStruct
                    .struct
                        {body}
                    .ends
                ";
            // --- Act/Assert
            CodeEmitWorks(source);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefb1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb 0x00
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefb2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb 0x00, 0x23, #a4, 0xc3, 12
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00, 0x23, 0xA4, 0xC3, 0x0C);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefw1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defw 0x00
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00, 0x00);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefw2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defw 0x1234, #FEDC
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x34, 0x12, 0xDC, 0xFE);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefm()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defm ""ABCD""
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, (byte)'A', (byte)'B', (byte)'C', (byte)'D');
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefn()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defn ""ABCD""
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, (byte)'A', (byte)'B', (byte)'C', (byte)'D', 0x00);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefc()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defc ""ABCD""
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, (byte)'A', (byte)'B', (byte)'C', (byte)('D' | 0x80));
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefs()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defs 3
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00, 0x00, 0x00);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithFillb()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillb 3, #A5
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xA5, 0xA5);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithFillw()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillw 2, #12A5
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0x12, 0xA5, 0x12);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefgx()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defgx ""----OOOO xxxx....""
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x0F, 0xF0);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefg()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defg ----OOOO xxxx....
                    .ends

                    MyStruct()
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x0F, 0xF0);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefbFixup1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb MyAddr
                    .ends

                    MyStruct()
                MyAddr
                    ld a,b
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x01, 0x78);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefbFixup2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb #a4, MyAddr
                    .ends

                    MyStruct()
                MyAddr
                    ld a,b
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA4, 0x02, 0x78);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefbFixup3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb MyAddr, #A4
                    .ends

                    MyStruct()
                MyAddr
                    ld a,b
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x02, 0xA4, 0x78);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefwFixup1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defw MyAddr
                    .ends

                    MyStruct()
                MyAddr
                    ld a,b
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x02, 0x80, 0x78);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefwFixup2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defw #1234, MyAddr
                    .ends

                    MyStruct()
                MyAddr
                    ld a,b
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x34, 0x12, 0x04, 0x80, 0x78);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithDefwFixup3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defw MyAddr, #1234
                    .ends

                    MyStruct()
                MyAddr
                    ld a,b
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x04, 0x80, 0x34, 0x12, 0x78);
        }

        [TestMethod]
        public void SimpleStructInvocationWorksWithMultipleFixups()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defw MyAddr, #1234
                        .defb 0xe4, MyAddr1
                        .defn ""ABCD""
                    .ends

                    MyStruct()
                MyAddr
                    ld a,b
                MyAddr1
                    ld a,b
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x0B, 0x80, 0x34, 0x12, 0xE4, 0x0C, (byte)'A', (byte)'B', (byte)'C', (byte)'D', 0x00, 0x78, 0x78);
        }
    }
}
