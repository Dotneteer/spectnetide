using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class MacroArgumentEmitTests: AssemblerTestBed
    {
        [TestMethod]
        public void MacroArgumentInGlobalScopeFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                {{MyParam}}
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0420);
        }

        [TestMethod]
        public void MacroArgumentInLocalScopeFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .loop 3
                {{MyParam}}
                .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0420);
        }

        [TestMethod]
        public void MacroArgumentInArgumentlessMacroDeclarationFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro()
                {{MyParam}}
                .endm
                MyMacro()
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(2);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0403);
        }

        [TestMethod]
        public void SingleMacroArgumentWithNumberWorks()
        {
            const string SOURCE = @"
                MyMacro: .macro(MyArg)
                ld a,{{MyArg}}
                .endm
                MyMacro(#A3)";

            CodeEmitWorks(SOURCE, 0x3E, 0xA3);
        }

        [TestMethod]
        [DataRow("a", 0x7F)]
        [DataRow("b", 0x78)]
        [DataRow("c", 0x79)]
        [DataRow("d", 0x7A)]
        [DataRow("e", 0x7B)]
        [DataRow("h", 0x7C)]
        [DataRow("l", 0x7D)]
        public void SingleMacroArgumentWith8BitRegisterWorks(string reg, int expected)
        {
            var source = @"
                MyMacro: .macro(MyArg)
                ld a,{{MyArg}}
                .endm
                MyMacro(" + reg + @")";

            CodeEmitWorks(source, (byte)expected);
        }

        [TestMethod]
        [DataRow("xl", 0xDD, 0x6F)]
        [DataRow("ixl", 0xDD, 0x6F)]
        [DataRow("xh", 0xDD, 0x67)]
        [DataRow("ixh", 0xDD, 0x67)]
        [DataRow("yl", 0xFD, 0x6F)]
        [DataRow("iyl", 0xFD, 0x6F)]
        [DataRow("yh", 0xFD, 0x67)]
        [DataRow("iyh", 0xFD, 0x67)]
        public void SingleMacroArgumentWith8BitIndexRegisterWorks(string reg, int exp1, int exp2)
        {
            var source = @"
                MyMacro: .macro(MyArg)
                ld {{MyArg}},a
                .endm
                MyMacro(" + reg + @")";

            CodeEmitWorks(source, (byte)exp1, (byte)exp2);
        }

        [TestMethod]
        [DataRow("i", 0x47)]
        [DataRow("r", 0x4F)]
        public void SingleMacroArgumentWith8BitSpecRegisterWorks(string reg, int expected)
        {
            var source = @"
                MyMacro: .macro(MyArg)
                ld {{MyArg}},a
                .endm
                MyMacro(" + reg + @")";

            CodeEmitWorks(source, 0xED, (byte)expected);
        }

        [TestMethod]
        [DataRow("bc", 0x03)]
        [DataRow("de", 0x13)]
        [DataRow("hl", 0x23)]
        [DataRow("sp", 0x33)]
        public void SingleMacroArgumentWith16BitRegisterWorks(string reg, int expected)
        {
            var source = @"
                MyMacro: .macro(MyArg)
                inc {{MyArg}}
                .endm
                MyMacro(" + reg + @")";

            CodeEmitWorks(source, (byte)expected);
        }

        [TestMethod]
        [DataRow("push {{MyArg}}", "af", 0xF5)]
        [DataRow("ex af,{{MyArg}}", "af'", 0x08)]
        public void SingleMacroArgumentWith16BitSpecRegisterWorks(string instr, string reg, int expected)
        {
            var source = @"
                MyMacro: .macro(MyArg)
                " + instr + @"
                .endm
                MyMacro(" + reg + @")";

            CodeEmitWorks(source, (byte)expected);
        }

        [TestMethod]
        [DataRow("(bc)", 0x0A)]
        [DataRow("(de)", 0x1A)]
        [DataRow("(hl)", 0x7E)]
        public void SingleMacroArgumentWith16BitRegIndirectWorks(string reg, int expected)
        {
            var source = @"
                MyMacro: .macro(MyArg)
                ld a,{{MyArg}}
                .endm
                MyMacro(" + reg + @")";

            CodeEmitWorks(source, (byte)expected);
        }

        [TestMethod]
        public void SingleMacroArgumentWithCPortWorks()
        {
            const string SOURCE = @"
                MyMacro: .macro(MyArg)
                out {{MyArg}},a
                .endm
                MyMacro((c))";

            CodeEmitWorks(SOURCE, 0xED, 0x79);
        }

        [TestMethod]
        [DataRow("(#1000)", 0x00, 0x10)]
        [DataRow("(#1000+#0800)", 0x00, 0x18)]
        public void SingleMacroArgumentWithMemIndirectWorks(string memind, int exp1, int exp2)
        {
            var source = @"
                MyMacro: .macro(MyArg)
                ld a,{{MyArg}}
                .endm
                MyMacro(" + memind + @")";

            CodeEmitWorks(source, 0x3A, (byte)exp1, (byte)exp2);
        }

        [TestMethod]
        [DataRow("(ix+#12)", 0xDD, 0x7E, 0x12)]
        [DataRow("(ix-#12)", 0xDD, 0x7E, 0xEE)]
        [DataRow("(iy+#12)", 0xFD, 0x7E, 0x12)]
        [DataRow("(iy-#12)", 0xFD, 0x7E, 0xEE)]
        [DataRow("(ix)", 0xDD, 0x7E, 0x00)]
        [DataRow("(iy)", 0xFD, 0x7E, 0x00)]
        public void SingleMacroArgumentWithIndexedAddrWorks(string memind, int exp1, int exp2, int exp3)
        {
            var source = @"
                MyMacro: .macro(MyArg)
                ld a,{{MyArg}}
                .endm
                MyMacro(" + memind + @")";

            CodeEmitWorks(source, (byte)exp1, (byte)exp2, (byte)exp3);
        }

        [TestMethod]
        [DataRow("z", 0xCC)]
        public void SingleMacroArgumentWithConditionWorks(string cond, int expected)
        {
            var source = @"
                MyMacro: .macro(MyArg)
                call {{MyArg}},#1000
                .endm
                MyMacro(" + cond + @")";

            CodeEmitWorks(source, (byte)expected, 0x00, 0x10);
        }


    }
}
