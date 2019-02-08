using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.CommandParser.SyntaxTree;
// ReSharper disable StringLiteralTypo

namespace Spect.Net.CommandParser.Test
{
    [TestClass]
    public class CommandParserTests: ParserTestBed
    {
        [TestMethod]
        [DataRow("g 1234", 4660, null)]
        [DataRow("G 1234", 4660, null)]
        [DataRow("g MySymbol", 0, "MySymbol")]
        public void GotoParsingWorks(string source, int address, string symbol)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as GotoToolCommand;
            command.ShouldNotBeNull();
            if (symbol != null)
            {
                command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
            }
            else
            {
                command.Address.ShouldBe((ushort)address);
            }
        }

        [TestMethod]
        [DataRow("g 0", 0)]
        [DataRow("g 1", 1)]
        [DataRow("g 2", 2)]
        [DataRow("g 3", 3)]
        [DataRow("g 4", 4)]
        [DataRow("g 5", 5)]
        [DataRow("g 6", 6)]
        [DataRow("g 7", 7)]
        [DataRow("g 8", 8)]
        [DataRow("g 9", 9)]
        [DataRow("g 0A", 10)]
        [DataRow("g 0B", 11)]
        [DataRow("g 0C", 12)]
        [DataRow("g 0D", 13)]
        [DataRow("g 0E", 14)]
        [DataRow("g 0F", 15)]
        public void HexNumberParsingWorks(string source, int address)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as GotoToolCommand;
            command.ShouldNotBeNull();
            command.Address.ShouldBe((ushort)address);
        }

        [TestMethod]
        [DataRow("gs MySymbol", "MySymbol")]
        [DataRow("gS MySymbol", "MySymbol")]
        [DataRow("Gs MySymbol", "MySymbol")]
        [DataRow("GS MySymbol", "MySymbol")]
        public void GotoSymbolParsingWorks(string source, string symbol)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as GotoSymbolToolCommand;
            command.ShouldNotBeNull();
            command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
        }

        [TestMethod]
        [DataRow("r 0", 0)]
        [DataRow("R 7", 7)]
        public void RomPageParsingWorks(string source, int page)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as RomPageToolCommand;
            command.ShouldNotBeNull();
            command.Page.ShouldBe((ushort)page);
        }

        [TestMethod]
        [DataRow("b 0", 0)]
        [DataRow("B 7", 7)]
        public void BankPageParsingWorks(string source, int page)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as BankPageToolCommand;
            command.ShouldNotBeNull();
            command.Page.ShouldBe((ushort)page);
        }

        [TestMethod]
        [DataRow("m")]
        [DataRow("M")]
        public void MemoryModeParsingWorks(string source)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as MemoryModeToolCommand;
            command.ShouldNotBeNull();
        }

        [TestMethod]
        [DataRow("l 1234", 4660, null)]
        [DataRow("L 1234", 4660, null)]
        [DataRow("l 1234 MySymbol", 4660, "MySymbol")]
        [DataRow("L 1234 MySymbol", 4660, "MySymbol")]
        public void LabelParsingWorks(string source, int address, string symbol)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as LabelToolCommand;
            command.ShouldNotBeNull();
            command.Address.ShouldBe((ushort)address);
            if (symbol == null)
            {
                command.Symbol.ShouldBeNull();
            }
            else
            {
                command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
            }
        }

        [TestMethod]
        [DataRow("c 1234", 4660, null)]
        [DataRow("C 1234", 4660, null)]
        [DataRow("c 1234 My Comments", 4660, "My Comments")]
        [DataRow("C 1234 My Symbols", 4660, "My Symbols")]
        public void CommentParsingWorks(string source, int address, string comment)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as CommentToolCommand;
            command.ShouldNotBeNull();
            command.Address.ShouldBe((ushort)address);
            if (comment == null)
            {
                command.Text.ShouldBeNull();
            }
            else
            {
                command.Text.ToUpper().ShouldBe(comment.ToUpper());
            }
        }

        [TestMethod]
        [DataRow("p 1234", 4660, null)]
        [DataRow("P 1234", 4660, null)]
        [DataRow("p 1234 My Comments", 4660, "My Comments")]
        [DataRow("P 1234 My Symbols", 4660, "My Symbols")]
        public void PrefixCommentParsingWorks(string source, int address, string comment)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as PrefixCommentToolCommand;
            command.ShouldNotBeNull();
            command.Address.ShouldBe((ushort)address);
            if (comment == null)
            {
                command.Text.ShouldBeNull();
            }
            else
            {
                command.Text.ToUpper().ShouldBe(comment.ToUpper());
            }
        }

        [TestMethod]
        [DataRow("sb 12AC H>3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("SB 12AC H>3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("Sb 12AC H>3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("sB 12AC H>3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("sb 12AC H >3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("sb 12AC H > 3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("sb 12AC C HL==5 & B != C", 4780, null, 0, "HL==5 & B != C")]
        public void SetBreakPointParsingWorks(string source, int address, string hitType, int hitValue, string condition)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as SetBreakpointToolCommand;
            command.ShouldNotBeNull();
            command.Address.ShouldBe((ushort)address);
            if (hitType != null)
            {
                command.HitConditionType.ShouldBe(hitType);
                command.HitConditionValue.ShouldBe((ushort)hitValue);
            }

            if (condition != null)
            {
                command.Condition.ShouldBe(condition);
            }
        }

        [TestMethod]
        [DataRow("tb 1234", 4660)]
        [DataRow("Tb 1234", 4660)]
        [DataRow("tB 1234", 4660)]
        [DataRow("TB 1234", 4660)]
        public void ToggleBreakpointParsingWorks(string source, int address)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as ToggleBreakpointToolCommand;
            command.ShouldNotBeNull();
            command.Address.ShouldBe((ushort)address);
        }

        [TestMethod]
        [DataRow("rb 1234", 4660)]
        [DataRow("Rb 1234", 4660)]
        [DataRow("rB 1234", 4660)]
        [DataRow("RB 1234", 4660)]
        public void RemoveBreakpointParsingWorks(string source, int address)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as RemoveBreakpointToolCommand;
            command.ShouldNotBeNull();
            command.Address.ShouldBe((ushort)address);
        }

        [TestMethod]
        [DataRow("ub 1234", 4660)]
        [DataRow("Ub 1234", 4660)]
        [DataRow("uB 1234", 4660)]
        [DataRow("UB 1234", 4660)]
        public void UpdateBreakpointParsingWorks(string source, int address)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as UpdateBreakpointToolCommand;
            command.ShouldNotBeNull();
            command.Address.ShouldBe((ushort)address);
        }

        [TestMethod]
        [DataRow("eb")]
        [DataRow("eB")]
        [DataRow("Eb")]
        [DataRow("EB")]
        public void EraseAllBreakpointParsingWorks(string source)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as EraseAllBreakpointsToolCommand;
            command.ShouldNotBeNull();
        }

        [TestMethod]
        [DataRow("rl 1234", 4660, "L")]
        [DataRow("rL 1234", 4660, "L")]
        [DataRow("Rl 1234", 4660, "L")]
        [DataRow("RL 1234", 4660, "L")]
        [DataRow("rc 1234", 4660, "C")]
        [DataRow("rC 1234", 4660, "C")]
        [DataRow("Rc 1234", 4660, "C")]
        [DataRow("RC 1234", 4660, "C")]
        [DataRow("rp 1234", 4660, "P")]
        [DataRow("rP 1234", 4660, "P")]
        [DataRow("Rp 1234", 4660, "P")]
        [DataRow("RP 1234", 4660, "P")]
        public void RetrieveParsingWorks(string source, int address, string type)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as RetrieveToolCommand;
            command.ShouldNotBeNull();
            command.Address.ShouldBe((ushort)address);
            command.Type.ShouldBe(type);
        }

        [TestMethod]
        [DataRow("d 1234 MySymbol", 4660, "MYSYMBOL", false)]
        [DataRow("D 1234 MySymbol", 4660, "MYSYMBOL", false)]
        [DataRow("D 1234", 4660, null, false)]
        [DataRow("D 1234 #", 4660, null, true)]
        public void RetrieveParsingWorks(string source, int address, string symbol, bool isAuto)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as LiteralToolCommand;
            command.ShouldNotBeNull();
            command.Address.ShouldBe((ushort)address);
            command.Symbol.ShouldBe(symbol);
            command.IsAuto.ShouldBe(isAuto);
        }

        [TestMethod]
        [DataRow("t 48", "48")]
        [DataRow("T 128", "128")]
        [DataRow("t p3", "P3")]
        [DataRow("T next", "NEXT")]
        public void DisassemblyTypeParsingWorks(string source, string type)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as DisassemblyTypeToolCommand;
            command.ShouldNotBeNull();
            command.Type.ShouldBe(type);
        }

        [TestMethod]
        [DataRow("rd")]
        [DataRow("rD")]
        [DataRow("Rd")]
        [DataRow("RD")]
        public void ReDisassemblyParsingWorks(string source)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as ReDisassemblyToolCommand;
            command.ShouldNotBeNull();
        }

        [TestMethod]
        [DataRow("j 1234", 4660, null)]
        [DataRow("J 1234", 4660, null)]
        [DataRow("j MySymbol", 0, "MySymbol")]
        public void JumpParsingWorks(string source, int address, string symbol)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as JumpToolCommand;
            command.ShouldNotBeNull();
            if (symbol != null)
            {
                command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
            }
            else
            {
                command.Address.ShouldBe((ushort)address);
            }
        }

    }
}
