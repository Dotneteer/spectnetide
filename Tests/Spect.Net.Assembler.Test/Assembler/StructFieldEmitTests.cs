using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class StructFieldEmitTests: AssemblerTestBed
    {
        [TestMethod]
        public void SimpleDefbInvocationWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb #23
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5);
        }

        [TestMethod]
        public void SimpleDefbInvocationWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb #23, #34
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0x34);
        }

        [TestMethod]
        public void SimpleDefbInvocationWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb #23
                    field1:
                        .defb #34
                    .ends

                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x23, 0xA5);
        }

        [TestMethod]
        public void SimpleDefbInvocationWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb #23
                    field1:
                        .defb #34
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x23, 0xA5);
        }

        [TestMethod]
        public void SimpleDefbInvocationWithNoInstructionLineWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb #23
                    .ends

                    MyStruct()
                    ; This is a comment
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5);
        }

        [TestMethod]
        public void DefbInvocationWithSingleFixupWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defb #23, Start
                    .ends
                    
                Start:
                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0x00);
        }

        [TestMethod]
        public void DefbInvocationWithSingleFixupWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defb Start, #23
                    .ends
                    
                Start:
                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0x23);
        }

        [TestMethod]
        public void DefbInvocationWithSingleFixupWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defb Start
                    field1
                        .defb #23
                    .ends
                    
                Start:
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00, 0xA5);
        }

        [TestMethod]
        public void DefbInvocationWithSingleFixupWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defb Start
                    field1
                        .defb #23
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x01, 0xA5);
        }

        [TestMethod]
        public void DefbInvocationWithDoubleFixupWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defb Start
                    field1
                        .defb #23
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    field1 -> .defb Next
                Next:
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x01, 0x03);
        }

        [TestMethod]
        public void DefbInvocationWithDoubleFixupWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defb Start
                    field1
                        .defb #23
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    -> .defb Next
                Next:
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x03, 0x23);
        }

        [TestMethod]
        public void DefbInvocationWithDoubleFixupWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defb Start
                    field1
                        .defb #23
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    -> .defb Next
                    field1 -> .defb #a5
                Next:
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x03, 0xA5);
        }

        [TestMethod]
        public void DefbInvocationWithDoubleFixupWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defb Start
                    field1
                        .defb #23
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    -> .defb #a5
                    field1 -> .defb Next
                Next:
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0xA5, 0x03);
        }

        [TestMethod]
        public void OversizeDefbInvocationIsCaught1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defb Start
                    field1
                        .defb #23
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    -> .defb #a5
                    field1 -> .defb Next
                    -> .defb #00
                Next:
                ";
            // --- Act/Assert
            CodeRaisesError(SOURCE, Errors.Z0442);
        }

        [TestMethod]
        public void SimpleDefwInvocationWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defw #234C
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0x23);
        }

        [TestMethod]
        public void SimpleDefwInvocationWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defw #234C
                    .ends

                    MyStruct()
                    -> .defw #A516
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x16, 0xA5);
        }

        [TestMethod]
        public void SimpleDefwInvocationWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defw #234C, #3456
                    .ends

                    MyStruct()
                    -> .defw #A516
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x16, 0xA5, 0x56, 0x34);
        }

        [TestMethod]
        public void SimpleDefwInvocationWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defw #234C, #3456
                    .ends

                    MyStruct()
                    -> .defb #01
                    -> .defw #A516
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x01, 0x16, 0xA5, 0x34);
        }

        [TestMethod]
        public void SimpleDefwInvocationWorks5()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb #23
                    field1:
                        .defw #3429
                    .ends

                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x23, 0xA5, 0x34);
        }

        [TestMethod]
        public void SimpleDefwInvocationWorks6()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb #23
                    field1:
                        .defw #3429
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x23, 0xA5, 0x34);
        }

        [TestMethod]
        public void SimpleDefwInvocationWorks7()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defb #23
                    field1:
                        .defw #3429
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defw #A5C4
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x23, 0xC4, 0xA5);
        }

        [TestMethod]
        public void SimpleDefwInvocationWithNoInstructionLineWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defw #231E
                    .ends

                    MyStruct()
                    ; This is a comment
                    -> .defw #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0x00);
        }

        [TestMethod]
        public void DefwInvocationWithSingleFixupWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defw #231A, Start
                    .ends
                    
                Start:
                    MyStruct()
                    -> .defw #A503
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x03, 0xA5, 0x00, 0x80);
        }

        [TestMethod]
        public void DefwInvocationWithSingleFixupWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defw Start, #231A
                    .ends
                    
                Start:
                    MyStruct()
                    -> .defw #A503
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x03, 0xA5, 0x1A, 0x23);
        }

        [TestMethod]
        public void DefwInvocationWithSingleFixupWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defw Start
                    field1
                        .defw #2315
                    .ends
                    
                Start:
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00, 0x80, 0xA5, 0x23);
        }

        [TestMethod]
        public void DefwInvocationWithSingleFixupWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defw Start
                    field1
                        .defw #23
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    field1 -> .defw #A512
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x01, 0x80, 0x12, 0xA5);
        }

        [TestMethod]
        public void DefwInvocationWithDoubleFixupWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defw Start
                    field1
                        .defw #232C
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    field1 -> .defb Next
                Next:
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x01, 0x80, 0x05, 0x23);
        }

        [TestMethod]
        public void DefwInvocationWithDoubleFixupWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defw Start
                    field1
                        .defw #232C
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    -> .defw Next
                Next:
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x05, 0x80, 0x2C, 0x23);
        }

        [TestMethod]
        public void DefwInvocationWithDoubleFixupWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defw Start
                    field1
                        .defw #232C
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    -> .defw Next
                    field1 -> .defb #a5
                Next:
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x05, 0x80, 0xA5, 0x23);
        }

        [TestMethod]
        public void DefwInvocationWithDoubleFixupWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defw Start
                    field1
                        .defw #232C
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    -> .defb #a5
                    field1 -> .defw Next
                Next:
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0xA5, 0x80, 0x05, 0x80);
        }

        [TestMethod]
        public void DefwInvocationWithDoubleFixupWorks5()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defw Start
                    field1
                        .defw #232C
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    -> .defb #a5
                    field1 -> .defb Next
                Next:
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0xA5, 0x80, 0x05, 0x23);
        }

        [TestMethod]
        public void OversizeDefwInvocationIsCaught1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct:
                    .struct
                        .defw Start
                    field1
                        .defw #23
                    .ends

                    ld a,b
                Start:
                    MyStruct()
                    -> .defw #a515
                    field1 -> .defw Next
                    -> .defb #00
                Next:
                ";
            // --- Act/Assert
            CodeRaisesError(SOURCE, Errors.Z0442);
        }


    }
}
