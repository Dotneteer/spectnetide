using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class MacroEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void MacroWithNoLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .macro()
                .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0400);
        }

        [TestMethod]
        public void MacroWithLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro()
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Macros.ContainsKey("MyMacro").ShouldBeTrue();
            var def = output.Macros["MyMacro"];
            def.ShouldNotBeNull();
            def.MacroDefLine.ShouldBe(0);
            def.MacroEndLine.ShouldBe(1);
            def.MacroName.ShouldBe("MYMACRO");
        }

        [TestMethod]
        public void MacroWithHangingLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: 
                    .macro()
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Macros.ContainsKey("MyMacro").ShouldBeTrue();
            var def = output.Macros["MyMacro"];
            def.ShouldNotBeNull();
            def.MacroDefLine.ShouldBe(1);
            def.MacroEndLine.ShouldBe(2);
            def.MacroName.ShouldBe("MYMACRO");
        }

    }
}
