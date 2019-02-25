using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
// ReSharper disable StringLiteralTypo

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class StructEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void StructWithNoLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .struct
                .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0432);
        }

        [TestMethod]
        public void StructWithLocalLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                `local .struct
                .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0433);
        }

        [TestMethod]
        public void StructWithEndLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct .struct
                MyEnd: .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0436);
        }

        [TestMethod]
        public void StructWithHangingLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct
                    .struct
                MyEnd:
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0436);
        }

        [TestMethod]
        public void StructWithLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: .struct
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Section.FirstLine.ShouldBe(0);
            def.Section.LastLine.ShouldBe(1);
            def.StructName.ShouldBe("MYSTRUCT");
        }

        [TestMethod]
        public void StructWithExistingLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: nop
                MyStruct: .struct
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0434);
        }

        [TestMethod]
        public void StructWithHangingLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Section.FirstLine.ShouldBe(1);
            def.Section.LastLine.ShouldBe(2);
            def.StructName.ShouldBe("MYSTRUCT");
        }

        [TestMethod]
        public void StructWithNoEndFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: .struct
                    ld a,b
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0401);
        }

        [TestMethod]
        [DataRow(".ends")]
        [DataRow("ends")]
        [DataRow(".ENDS")]
        [DataRow("ENDS")]
        public void EndStructWithoutOpenTagFails(string source)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(source);

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0405);
        }

        [TestMethod]
        [DataRow("ld a,b")]
        [DataRow("jp #1000")]
        public void StructWithInvalidInstructionFails(string stmt)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile($@"
                MyStruct .struct
                  {stmt}
                .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0435);
        }

        [TestMethod]
        [DataRow("; this is comment")]
        [DataRow("MyField")]
        [DataRow("MyField:")]
        [DataRow(".defb 0x80")]
        [DataRow(".defw 0x8078")]
        [DataRow(".defc \"Hello\"")]
        [DataRow(".defm \"Hello\"")]
        [DataRow(".defn \"Hello\"")]
        [DataRow(".defh \"e345\"")]
        [DataRow(".defs 100")]
        [DataRow(".fillb 10,#ff")]
        [DataRow(".fillw 10,#ffe3")]
        [DataRow(".defgx \"....OOOO\"")]
        [DataRow(".defg \"....OOOO\"")]
        public void StructWithValidPragmaWorks(string stmt)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile($@"
                MyStruct .struct
                  {stmt}
                  .defb 0x80
                .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }
    }
}