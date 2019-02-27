using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
// ReSharper disable StringLiteralTypo

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

        [TestMethod]
        public void SimpleDefmInvocationWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defm ""ABC""
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, (byte)'B', (byte)'C');
        }

        [TestMethod]
        public void SimpleDefmInvocationWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defm ""AB""
                        .defm ""CD""
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, (byte)'B', (byte)'C', (byte)'D');
        }

        [TestMethod]
        public void SimpleDefmInvocationWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defm ""AB""
                    field1:
                        .defm ""CD""
                    .ends

                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, (byte)'A', (byte)'B', 0xA5, (byte)'D');
        }

        [TestMethod]
        public void SimpleDefmInvocationWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defm ""AB""
                    field1:
                        .defm ""CD""
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, (byte)'A', (byte)'B', 0xA5, (byte)'D');
        }

        [TestMethod]
        public void SimpleDefmInvocationWithNoInstructionLineWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defm ""AB""
                    .ends

                    MyStruct()
                    ; This is a comment
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, (byte)'B');
        }

        [TestMethod]
        public void OversizeDefmInvocationIsCaught()
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
                    -> .defm ""Hi""
                Next:
                ";
            // --- Act/Assert
            CodeRaisesError(SOURCE, Errors.Z0442);
        }

        [TestMethod]
        public void SimpleDefnInvocationWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defn ""ABC""
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, (byte)'B', (byte)'C', 0x00);
        }

        [TestMethod]
        public void SimpleDefnInvocationWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defn ""AB""
                        .defn ""CD""
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, (byte)'B', 0x00, (byte)'C', (byte)'D', 0x00);
        }

        [TestMethod]
        public void SimpleDefnInvocationWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defn ""AB""
                    field1:
                        .defn ""CD""
                    .ends

                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, (byte)'A', (byte)'B', 0x00, 0xA5, (byte)'D', 0x00);
        }

        [TestMethod]
        public void SimpleDefnInvocationWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defn ""AB""
                    field1:
                        .defn ""CD""
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, (byte)'A', (byte)'B', 0x00, 0xA5, (byte)'D', 0x00);
        }

        [TestMethod]
        public void SimpleDefnInvocationWithNoInstructionLineWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defn ""AB""
                    .ends

                    MyStruct()
                    ; This is a comment
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, (byte)'B', 0x00);
        }

        [TestMethod]
        public void OversizeDefnInvocationIsCaught()
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
                    -> .defn ""Hi""
                Next:
                ";
            // --- Act/Assert
            CodeRaisesError(SOURCE, Errors.Z0442);
        }

        [TestMethod]
        public void SimpleDefcInvocationWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defc ""ABC""
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, (byte)'B', (byte)('C'|0x80));
        }

        [TestMethod]
        public void SimpleDefcInvocationWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defc ""AB""
                        .defc ""CD""
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, (byte)('B'|0x80), (byte)'C', (byte)('D'|0x80));
        }

        [TestMethod]
        public void SimpleDefcInvocationWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defc ""AB""
                    field1:
                        .defc ""CD""
                    .ends

                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, (byte)'A', (byte)('B'|0x80), 0xA5, (byte)('D'|0x80));
        }

        [TestMethod]
        public void SimpleDefcInvocationWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defc ""AB""
                    field1:
                        .defc ""CD""
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, (byte)'A', (byte)('B'|0x80), 0xA5, (byte)('D'|0x80));
        }

        [TestMethod]
        public void SimpleDefcInvocationWithNoInstructionLineWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defc ""AB""
                    .ends

                    MyStruct()
                    ; This is a comment
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 'B'|0x80);
        }

        [TestMethod]
        public void OversizeDefcInvocationIsCaught()
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
                    -> .defc ""Hi""
                Next:
                ";
            // --- Act/Assert
            CodeRaisesError(SOURCE, Errors.Z0442);
        }

        [TestMethod]
        public void SimpleDefhInvocationWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defh ""12AC""
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xAC);
        }

        [TestMethod]
        public void SimpleDefhInvocationWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defh ""AB""
                        .defh ""CD""
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xCD);
        }

        [TestMethod]
        public void SimpleDefhInvocationWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defh ""AB""
                    field1:
                        .defh ""CD""
                    .ends

                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xAB, 0xA5);
        }

        [TestMethod]
        public void SimpleDefhInvocationWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defh ""AB""
                    field1:
                        .defh ""CD""
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0xAB, 0xA5);
        }

        [TestMethod]
        public void SimpleDefhInvocationWithNoInstructionLineWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defh ""AB""
                    .ends

                    MyStruct()
                    ; This is a comment
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5);
        }

        [TestMethod]
        public void OversizeDefhInvocationIsCaught()
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
                    -> .defh ""E4""
                Next:
                ";
            // --- Act/Assert
            CodeRaisesError(SOURCE, Errors.Z0442);
        }

        [TestMethod]
        public void SimpleDefsInvocationWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defs 3
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0x00, 0x00);
        }

        [TestMethod]
        public void SimpleDefsInvocationWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defs 1
                        .defs 2
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0x00, 0x00);
        }

        [TestMethod]
        public void SimpleDefsInvocationWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defs 1
                    field1:
                        .defs 2
                    .ends

                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00, 0xA5, 0x00);
        }

        [TestMethod]
        public void SimpleDefsInvocationWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defs 1
                    field1:
                        .defs 2
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x00, 0xA5, 0x00);
        }

        [TestMethod]
        public void SimpleDefsInvocationWithNoInstructionLineWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defs 3
                    .ends

                    MyStruct()
                    ; This is a comment
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0x00, 0x00);
        }

        [TestMethod]
        public void OversizeDefsInvocationIsCaught()
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
                    -> .defs 4
                Next:
                ";
            // --- Act/Assert
            CodeRaisesError(SOURCE, Errors.Z0442);
        }

        [TestMethod]
        public void SimpleFillbInvocationWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillb 3, #D9
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xD9, 0xD9);
        }

        [TestMethod]
        public void SimpleFillbInvocationWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillb 1, #D9
                        .fillb 2, #D9
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xD9, 0xD9);
        }

        [TestMethod]
        public void SimpleFillbInvocationWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillb 1, #D7
                    field1:
                        .fillb 2, #D9
                    .ends

                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xD7, 0xA5, 0xD9);
        }

        [TestMethod]
        public void SimpleFillbInvocationWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillb 1, #D7
                    field1:
                        .fillb 2, #D9
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0xD7, 0xA5, 0xD9);
        }

        [TestMethod]
        public void SimpleFillbInvocationWithNoInstructionLineWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillb 3, #D9
                    .ends

                    MyStruct()
                    ; This is a comment
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xD9, 0xD9);
        }

        [TestMethod]
        public void OversizeFillbInvocationIsCaught()
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
                    -> .fillb 4, #13
                Next:
                ";
            // --- Act/Assert
            CodeRaisesError(SOURCE, Errors.Z0442);
        }

        [TestMethod]
        public void SimpleFillwInvocationWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillw 2, #D924
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xD9, 0x24, 0xD9);
        }

        [TestMethod]
        public void SimpleFillwInvocationWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillw 1, #D924
                        .fillw 2, #D924
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xD9, 0x24, 0xD9, 0x24, 0xD9);
        }

        [TestMethod]
        public void SimpleFillwInvocationWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillw 1, #D724
                    field1:
                        .fillw 2, #D924
                    .ends

                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x24, 0xD7, 0xA5, 0xD9, 0x24, 0xD9);
        }

        [TestMethod]
        public void SimpleFillwInvocationWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillw 1, #D724
                    field1:
                        .fillw 2, #D924
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x24, 0xD7, 0xA5, 0xD9, 0x24, 0xD9);
        }

        [TestMethod]
        public void SimpleFillwInvocationWithNoInstructionLineWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .fillw 2, #D924
                    .ends

                    MyStruct()
                    ; This is a comment
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xD9, 0x24, 0xD9);
        }

        [TestMethod]
        public void OversizeFillwInvocationIsCaught()
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
                    -> .fillw 4, #13
                Next:
                ";
            // --- Act/Assert
            CodeRaisesError(SOURCE, Errors.Z0442);
        }

        [TestMethod]
        public void SimpleDefgInvocationWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defg ----OOOO xxxx....
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xF0);
        }

        [TestMethod]
        public void SimpleDefgInvocationWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defg ----OOOO xxxx....
                        .defg x.x.x.x. .o.o.o.o
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xF0, 0xAA, 0x55);
        }

        [TestMethod]
        public void SimpleDefgInvocationWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defg ----OOOO xxxx....
                    field1:
                        .defg x.x.x.x. .o.o.o.o
                    .ends

                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x0F, 0xF0, 0xA5, 0x55);
        }

        [TestMethod]
        public void SimpleDefgInvocationWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defg ----OOOO xxxx....
                    field1:
                        .defg x.x.x.x. .o.o.o.o
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x0F, 0xF0, 0xA5, 0x55);
        }

        [TestMethod]
        public void SimpleDefgInvocationWithNoInstructionLineWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defg ----OOOO xxxx....
                    .ends

                    MyStruct()
                    ; This is a comment
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xF0);
        }

        [TestMethod]
        public void OversizeDefgInvocationIsCaught()
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
                    -> .defg ----OOOO xxxx....
                Next:
                ";
            // --- Act/Assert
            CodeRaisesError(SOURCE, Errors.Z0442);
        }

        [TestMethod]
        public void SimpleDefgxInvocationWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defgx ""----OOOO xxxx....""
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xF0);
        }

        [TestMethod]
        public void SimpleDefgxInvocationWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defgx ""----OOOO xxxx....""
                        .defgx ""x.x.x.x. .o.o.o.o""
                    .ends

                    MyStruct()
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xF0, 0xAA, 0x55);
        }

        [TestMethod]
        public void SimpleDefgxInvocationWorks3()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defgx ""----OOOO xxxx....""
                    field1:
                        .defgx ""x.x.x.x. .o.o.o.o""
                    .ends

                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x0F, 0xF0, 0xA5, 0x55);
        }

        [TestMethod]
        public void SimpleDefgxInvocationWorks4()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defgx ""----OOOO xxxx....""
                    field1:
                        .defgx ""x.x.x.x. .o.o.o.o""
                    .ends
                Start:
                    ld a,b
                    MyStruct()
                    field1 -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x78, 0x0F, 0xF0, 0xA5, 0x55);
        }

        [TestMethod]
        public void SimpleDefgxInvocationWithNoInstructionLineWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct
                    .struct
                        .defgx ""----OOOO xxxx....""
                    .ends

                    MyStruct()
                    ; This is a comment
                    -> .defb #A5
                ";
            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0xA5, 0xF0);
        }

        [TestMethod]
        public void OversizeDefgxInvocationIsCaught()
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
                    -> .defgx ""----OOOO xxxx....""
                Next:
                ";
            // --- Act/Assert
            CodeRaisesError(SOURCE, Errors.Z0442);
        }

        [TestMethod]
        public void FieldInvocationDoesNotRedefineLabels()
        {
            // --- Arrange
            const string SOURCE = @"
                Object2D: .struct
                    X: .defw 0
                    Y: .defw 0
                    DX: .defb 1
                    DY: .defb 1
                .ends

                Object2d()

                Apple: Object2D()
                    X -> .defw 100
                    Y -> .defw 100
                ";
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(SOURCE);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void StructAllowsMultipleNamedFields()
        {
            // --- Arrange
            const string SOURCE = @"
                Object2D: .struct
                    XCoord:
                    X: .defw 0
                    Y: .defw 0
                    DX: .defb 1
                    DY: .defb 1
                .ends

                Object2d()

                Apple: Object2D()
                    X -> .defw 100
                    Y -> .defw 100
                ";
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(SOURCE);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }
    }
}
