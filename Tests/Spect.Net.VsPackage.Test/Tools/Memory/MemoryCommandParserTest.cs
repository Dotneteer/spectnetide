using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.VsPackage.ToolWindows.Memory;

namespace Spect.Net.VsPackage.Test.Tools.Memory
{
    [TestClass]
    public class MemoryCommandParserTest
    {
        [TestMethod]
        public void ParserRecognizesEmptyCommand()
        {
            // --- Act
            var p1 = new MemoryCommandParser(null);
            var p2 = new MemoryCommandParser("    ");

            // --- Assert
            p1.Command.ShouldBe(MemoryCommandType.None);
            p2.Command.ShouldBe(MemoryCommandType.None);
        }

        [TestMethod]
        public void ParserRecognizesGotoCommand1()
        {
            // --- Act
            var p = new MemoryCommandParser("G45BF");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.Goto);
            p.Address.ShouldBe((ushort)0x45BF);
        }

        [TestMethod]
        public void ParserRecognizesGotoCommand2()
        {
            // --- Act
            var p = new MemoryCommandParser("g45BF");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.Goto);
            p.Address.ShouldBe((ushort)0x45BF);
        }

        [TestMethod]
        public void ParserRecognizesGotoCommand3()
        {
            // --- Act
            var p = new MemoryCommandParser("G   45BF");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.Goto);
            p.Address.ShouldBe((ushort)0x45BF);
        }

        [TestMethod]
        public void ParserRefusesInvalidGotoCommand()
        {
            // --- Act
            var p = new MemoryCommandParser("45BQ");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.Invalid);
        }

        [TestMethod]
        public void ParserRecognizesRomCommand1()
        {
            // --- Act
            var p = new MemoryCommandParser("R0");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.SetRomPage);
            p.Address.ShouldBe((ushort)0x00);
        }

        [TestMethod]
        public void ParserRecognizesRomCommand2()
        {
            // --- Act
            var p = new MemoryCommandParser("R1");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.SetRomPage);
            p.Address.ShouldBe((ushort)0x01);
        }

        [TestMethod]
        public void ParserRecognizesRomCommand3()
        {
            // --- Act
            var p = new MemoryCommandParser("R2");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.SetRomPage);
            p.Address.ShouldBe((ushort)0x02);
        }

        [TestMethod]
        public void ParserRecognizesRomCommand4()
        {
            // --- Act
            var p = new MemoryCommandParser("R3");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.SetRomPage);
            p.Address.ShouldBe((ushort)0x03);
        }

        [TestMethod]
        public void ParserRecognizesRomCommand5()
        {
            // --- Act
            var p = new MemoryCommandParser("r3");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.SetRomPage);
            p.Address.ShouldBe((ushort)0x03);
        }

        [TestMethod]
        public void ParserRefusesInvalidRomCommand1()
        {
            // --- Act
            var p = new MemoryCommandParser("R5");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.Invalid);
        }

        [TestMethod]
        public void ParserRefusesInvalidRomCommand2()
        {
            // --- Act
            var p = new MemoryCommandParser("RQ");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.Invalid);
        }

        [TestMethod]
        public void ParserRecognizesRamBankCommand1()
        {
            // --- Act
            var p = new MemoryCommandParser("B0");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.SetRamBank);
            p.Address.ShouldBe((ushort)0x00);
        }

        [TestMethod]
        public void ParserRecognizesRamBankCommand2()
        {
            // --- Act
            var p = new MemoryCommandParser("b1");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.SetRamBank);
            p.Address.ShouldBe((ushort)0x01);
        }

        [TestMethod]
        public void ParserRecognizesRamBankCommand3()
        {
            // --- Act
            var p = new MemoryCommandParser("B7");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.SetRamBank);
            p.Address.ShouldBe((ushort)0x07);
        }

        [TestMethod]
        public void ParserRefusesInvalidRamBankCommand1()
        {
            // --- Act
            var p = new MemoryCommandParser("SQ");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.Invalid);
        }

        [TestMethod]
        public void ParserRefusesInvalidRamBankCommand2()
        {
            // --- Act
            var p = new MemoryCommandParser("S8");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.Invalid);
        }

        [TestMethod]
        public void ParserRecognizesMemoryModeCommand1()
        {
            // --- Act
            var p = new MemoryCommandParser("M");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.MemoryMode);
        }

        [TestMethod]
        public void ParserRecognizesMemoryModeCommand2()
        {
            // --- Act
            var p = new MemoryCommandParser("m");

            // --- Assert
            p.Command.ShouldBe(MemoryCommandType.MemoryMode);
        }
    }
}
