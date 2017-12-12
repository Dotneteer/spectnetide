using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.VsPackage.CustomEditors.RomEditor;

namespace Spect.Net.VsPackage.Test.CustomEditors.RomEditor
{
    [TestClass]
    public class RomEditorCommandParserTests
    {
        [TestMethod]
        public void ParserRecognizesEmptyCommand()
        {
            // --- Act
            var p1 = new RomEditorCommandParser(null);
            var p2 = new RomEditorCommandParser("    ");

            // --- Assert
            p1.Command.ShouldBe(RomEditorCommandType.None);
            p2.Command.ShouldBe(RomEditorCommandType.None);
        }

        [TestMethod]
        public void ParserRecognizesGotoCommand()
        {
            // --- Act
            var p = new RomEditorCommandParser("45BF");

            // --- Assert
            p.Command.ShouldBe(RomEditorCommandType.Goto);
            p.Address.ShouldBe((ushort)0x45BF);
        }

        [TestMethod]
        public void ParserRefusesInvalidGotoCommand()
        {
            // --- Act
            var p = new RomEditorCommandParser("45BQ");

            // --- Assert
            p.Command.ShouldBe(RomEditorCommandType.Invalid);
        }

        [TestMethod]
        public void ParserRecognizesDisassemblyCommand1()
        {
            // --- Act
            var p = new RomEditorCommandParser("#45BF");

            // --- Assert
            p.Command.ShouldBe(RomEditorCommandType.Disassemble);
            p.Address.ShouldBe((ushort)0x45BF);
        }

        [TestMethod]
        public void ParserRecognizesDisassemblyCommand2()
        {
            // --- Act
            var p = new RomEditorCommandParser("# 45BF");

            // --- Assert
            p.Command.ShouldBe(RomEditorCommandType.Disassemble);
            p.Address.ShouldBe((ushort)0x45BF);
        }

        [TestMethod]
        public void ParserRefusesInvalidDisassemblyCommand()
        {
            // --- Act
            var p = new RomEditorCommandParser("#45BQ");

            // --- Assert
            p.Command.ShouldBe(RomEditorCommandType.Invalid);
        }

        [TestMethod]
        public void ParserRecognizesExitDisassemblyCommand1()
        {
            // --- Act
            var p = new RomEditorCommandParser("x ");

            // --- Assert
            p.Command.ShouldBe(RomEditorCommandType.ExitDisass);
        }

        [TestMethod]
        public void ParserRecognizesExitDisassemblyCommand2()
        {
            // --- Act
            var p = new RomEditorCommandParser("X");

            // --- Assert
            p.Command.ShouldBe(RomEditorCommandType.ExitDisass);
        }

        [TestMethod]
        public void ParserRefusesInvalidExitDisassemblyCommand()
        {
            // --- Act
            var p = new RomEditorCommandParser("x 45BQ");

            // --- Assert
            p.Command.ShouldBe(RomEditorCommandType.Invalid);
        }
    }
}
