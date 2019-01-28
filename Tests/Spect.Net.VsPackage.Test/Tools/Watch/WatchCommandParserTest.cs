using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.VsPackage.ToolWindows.Watch;

namespace Spect.Net.VsPackage.Test.Tools.Watch
{
    [TestClass]
    public class WatchCommandParserTest
    {
        [TestMethod]
        public void ParserRecognizesEmptyCommand()
        {
            // --- Act
            var p1 = new WatchCommandParser(null);
            var p2 = new WatchCommandParser("    ");

            // --- Assert
            p1.Command.ShouldBe(WatchCommandType.None);
            p2.Command.ShouldBe(WatchCommandType.None);
        }

        [TestMethod]
        public void ParserRecognizesEraseAllCommand()
        {
            // --- Act
            var p1 = new WatchCommandParser("e");
            var p2 = new WatchCommandParser("E  ");

            // --- Assert
            p1.Command.ShouldBe(WatchCommandType.EraseAll);
            p2.Command.ShouldBe(WatchCommandType.EraseAll);
        }

        [TestMethod]
        public void ParserRecognizesAddWatchCommand()
        {
            // --- Act
            var p1 = new WatchCommandParser("+ This is a command");
            var p2 = new WatchCommandParser("+ This is another");

            // --- Assert
            p1.Command.ShouldBe(WatchCommandType.AddWatch);
            p1.Arg1.ShouldBe("This is a command");
            p2.Command.ShouldBe(WatchCommandType.AddWatch);
            p2.Arg1.ShouldBe("This is another");
        }

        [TestMethod]
        public void ParserRecognizesRemoveWatchCommand()
        {
            // --- Act
            var p1 = new WatchCommandParser("- 12");
            var p2 = new WatchCommandParser("- 123  ");

            // --- Assert
            p1.Command.ShouldBe(WatchCommandType.RemoveWatch);
            p1.Address.ShouldBe((ushort)12);
            p2.Command.ShouldBe(WatchCommandType.RemoveWatch);
            p2.Address.ShouldBe((ushort)123);
        }

        [TestMethod]
        public void ParserRecognizesUpdateWatchCommand()
        {
            // --- Act
            var p1 = new WatchCommandParser("* 23 This is a command");
            var p2 = new WatchCommandParser("* 234 This is another");

            // --- Assert
            p1.Command.ShouldBe(WatchCommandType.UpdateWatch);
            p1.Address.ShouldBe((ushort)23);
            p1.Arg1.ShouldBe("This is a command");
            p2.Command.ShouldBe(WatchCommandType.UpdateWatch);
            p2.Address.ShouldBe((ushort)234);
            p2.Arg1.ShouldBe("This is another");
        }


        [TestMethod]
        public void ParserRefusesInvalidRemoveWatchCommand()
        {
            // --- Act
            var p1 = new WatchCommandParser("- 1q2");
            var p2 = new WatchCommandParser("- sdf");

            // --- Assert
            p1.Command.ShouldBe(WatchCommandType.Invalid);
            p2.Command.ShouldBe(WatchCommandType.Invalid);
        }

        [TestMethod]
        public void ParserRecognizesLabelWidthCommand()
        {
            // --- Act
            var p1 = new WatchCommandParser("l 12");
            var p2 = new WatchCommandParser("L 123  ");

            // --- Assert
            p1.Command.ShouldBe(WatchCommandType.ChangeLabelWidth);
            p1.Address.ShouldBe((ushort)12);
            p2.Command.ShouldBe(WatchCommandType.ChangeLabelWidth);
            p2.Address.ShouldBe((ushort)123);
        }

        [TestMethod]
        public void ParserRefusesInvalidLabelWidthCommand()
        {
            // --- Act
            var p1 = new WatchCommandParser("l 12q");
            var p2 = new WatchCommandParser("L sdf  ");

            // --- Assert
            p1.Command.ShouldBe(WatchCommandType.Invalid);
            p2.Command.ShouldBe(WatchCommandType.Invalid);
        }

        [TestMethod]
        public void ParserRecognizesMoveCommand()
        {
            // --- Act
            var p1 = new WatchCommandParser("m 12 23");
            var p2 = new WatchCommandParser("M 123 4 ");

            // --- Assert
            p1.Command.ShouldBe(WatchCommandType.MoveItem);
            p1.Address.ShouldBe((ushort)12);
            p1.Address2.ShouldBe((ushort)23);
            p2.Command.ShouldBe(WatchCommandType.MoveItem);
            p2.Address.ShouldBe((ushort)123);
            p2.Address2.ShouldBe((ushort)4);
        }

        [TestMethod]
        public void ParserRefusesInvalidMoveCommand()
        {
            // --- Act
            var p1 = new WatchCommandParser("m 12q");
            var p2 = new WatchCommandParser("M sdf  ");

            // --- Assert
            p1.Command.ShouldBe(WatchCommandType.Invalid);
            p2.Command.ShouldBe(WatchCommandType.Invalid);
        }
    }
}
