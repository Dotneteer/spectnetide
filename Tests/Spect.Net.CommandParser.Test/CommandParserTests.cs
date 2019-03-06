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
        [DataRow("G :1234", 1234, null)]
        [DataRow("G:1234", 1234, null)]
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
        [DataRow("r 0", 0)]
        [DataRow("R 7", 7)]
        [DataRow("r :0", 0)]
        [DataRow("R :7", 7)]
        [DataRow("r:0", 0)]
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
        [DataRow("b :0", 0)]
        [DataRow("B :7", 7)]
        [DataRow("b:0", 0)]
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
        [DataRow("L :1234", 1234, null)]
        [DataRow("l 1234 MySymbol", 4660, "MySymbol")]
        [DataRow("L 1234 MySymbol", 4660, "MySymbol")]
        [DataRow("L:1234", 1234, null)]
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
        [DataRow("C :1234", 1234, null)]
        [DataRow("c 1234 My Comments", 4660, "My Comments")]
        [DataRow("C 1234 My Symbols", 4660, "My Symbols")]
        [DataRow("C:1234", 1234, null)]
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
        [DataRow("c MySymbol", "MySymbol", null)]
        [DataRow("c MySymbol Comment", "MySymbol", "Comment")]
        public void CommentParsingWithSymbolWorks(string source, string symbol, string comment)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as CommentToolCommand;
            command.ShouldNotBeNull();
            command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
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
        [DataRow("P :1234", 1234, null)]
        [DataRow("p 1234 My Comments", 4660, "My Comments")]
        [DataRow("P 1234 My Symbols", 4660, "My Symbols")]
        [DataRow("P:1234", 1234, null)]
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
        [DataRow("p MySymbol", "MySymbol", null)]
        [DataRow("p MySymbol Comment", "MySymbol", "Comment")]
        public void PrefixCommentParsingWithSymbolWorks(string source, string symbol, string comment)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as PrefixCommentToolCommand;
            command.ShouldNotBeNull();
            command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
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
        [DataRow("SB :1234 H>3 C HL==5", 1234, ">", 3, "HL==5")]
        [DataRow("SB:1234 H>3 C HL==5", 1234, ">", 3, "HL==5")]
        [DataRow("sb 12AC H>3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("SB 12AC H>3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("Sb 12AC H>3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("sB 12AC H>3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("sb 12AC H >3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("sb 12AC H > 3 C HL==5", 4780, ">", 3, "HL==5")]
        [DataRow("sb 12AC C HL==5 & B != C", 4780, null, 0, "HL==5 & B != C")]
        [DataRow("SB :1234 H>=3 C HL==5", 1234, ">=", 3, "HL==5")]
        [DataRow("SB:1234 H>=3 C HL==5", 1234, ">=", 3, "HL==5")]
        [DataRow("sb 12AC H>=3 C HL==5", 4780, ">=", 3, "HL==5")]
        [DataRow("SB 12AC H>=3 C HL==5", 4780, ">=", 3, "HL==5")]
        [DataRow("Sb 12AC H>=3 C HL==5", 4780, ">=", 3, "HL==5")]
        [DataRow("sB 12AC H>=3 C HL==5", 4780, ">=", 3, "HL==5")]
        [DataRow("sb 12AC H >=3 C HL==5", 4780, ">=", 3, "HL==5")]
        [DataRow("sb 12AC H >= 3 C HL==5", 4780, ">=", 3, "HL==5")]
        [DataRow("SB :1234 H=3 C HL==5", 1234, "=", 3, "HL==5")]
        [DataRow("SB:1234 H=3 C HL==5", 1234, "=", 3, "HL==5")]
        [DataRow("sb 12AC H=3 C HL==5", 4780, "=", 3, "HL==5")]
        [DataRow("SB 12AC H=3 C HL==5", 4780, "=", 3, "HL==5")]
        [DataRow("Sb 12AC H=3 C HL==5", 4780, "=", 3, "HL==5")]
        [DataRow("sB 12AC H=3 C HL==5", 4780, "=", 3, "HL==5")]
        [DataRow("sb 12AC H =3 C HL==5", 4780, "=", 3, "HL==5")]
        [DataRow("sb 12AC H = 3 C HL==5", 4780, "=", 3, "HL==5")]
        [DataRow("SB :1234 H<=3 C HL==5", 1234, "<=", 3, "HL==5")]
        [DataRow("SB:1234 H<=3 C HL==5", 1234, "<=", 3, "HL==5")]
        [DataRow("sb 12AC H<=3 C HL==5", 4780, "<=", 3, "HL==5")]
        [DataRow("SB 12AC H<=3 C HL==5", 4780, "<=", 3, "HL==5")]
        [DataRow("Sb 12AC H<=3 C HL==5", 4780, "<=", 3, "HL==5")]
        [DataRow("sB 12AC H<=3 C HL==5", 4780, "<=", 3, "HL==5")]
        [DataRow("sb 12AC H <=3 C HL==5", 4780, "<=", 3, "HL==5")]
        [DataRow("sb 12AC H <= 3 C HL==5", 4780, "<=", 3, "HL==5")]
        [DataRow("SB :1234 H<3 C HL==5", 1234, "<", 3, "HL==5")]
        [DataRow("SB:1234 H<3 C HL==5", 1234, "<", 3, "HL==5")]
        [DataRow("sb 12AC H<3 C HL==5", 4780, "<", 3, "HL==5")]
        [DataRow("SB 12AC H<3 C HL==5", 4780, "<", 3, "HL==5")]
        [DataRow("Sb 12AC H<3 C HL==5", 4780, "<", 3, "HL==5")]
        [DataRow("sB 12AC H<3 C HL==5", 4780, "<", 3, "HL==5")]
        [DataRow("sb 12AC H <3 C HL==5", 4780, "<", 3, "HL==5")]
        [DataRow("sb 12AC H < 3 C HL==5", 4780, "<", 3, "HL==5")]
        [DataRow("SB :1234 H*3 C HL==5", 1234, "*", 3, "HL==5")]
        [DataRow("SB:1234 H*3 C HL==5", 1234, "*", 3, "HL==5")]
        [DataRow("sb 12AC H*3 C HL==5", 4780, "*", 3, "HL==5")]
        [DataRow("SB 12AC H*3 C HL==5", 4780, "*", 3, "HL==5")]
        [DataRow("Sb 12AC H*3 C HL==5", 4780, "*", 3, "HL==5")]
        [DataRow("sB 12AC H*3 C HL==5", 4780, "*", 3, "HL==5")]
        [DataRow("sb 12AC H *3 C HL==5", 4780, "*", 3, "HL==5")]
        [DataRow("sb 12AC H * 3 C HL==5", 4780, "*", 3, "HL==5")]
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
        [DataRow("SB Symbol H>3 C HL==5", "SYMBOL", ">", 3, "HL==5")]
        [DataRow("Sb Symbol H>3 C HL==5", "SYMBOL", ">", 3, "HL==5")]
        [DataRow("sb Symbol H >3 C HL==5", "SYMBOL", ">", 3, "HL==5")]
        [DataRow("sb Symbol H > 3 C HL==5", "SYMBOL", ">", 3, "HL==5")]
        [DataRow("sb Symbol C HL==5 & B != C", "SYMBOL", null, 0, "HL==5 & B != C")]
        public void SetBreakPointParsingWithSymbolWorks(string source, string symbol, string hitType, int hitValue, string condition)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as SetBreakpointToolCommand;
            command.ShouldNotBeNull();
            command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
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
        [DataRow("tb :1234", 1234)]
        [DataRow("tb:1234", 1234)]
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
        [DataRow("tb Symbol", "Symbol")]
        [DataRow("Tb Symbol", "Symbol")]
        [DataRow("tB Symbol", "Symbol")]
        [DataRow("TB Symbol", "Symbol")]
        public void ToggleBreakpointParsingWithSymbolWorks(string source, string symbol)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as ToggleBreakpointToolCommand;
            command.ShouldNotBeNull();
            command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
        }

        [TestMethod]
        [DataRow("rb 1234", 4660)]
        [DataRow("Rb 1234", 4660)]
        [DataRow("rB 1234", 4660)]
        [DataRow("RB 1234", 4660)]
        [DataRow("rB :1234", 1234)]
        [DataRow("rB:1234", 1234)]
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
        [DataRow("rb Symbol", "Symbol")]
        [DataRow("Rb Symbol", "Symbol")]
        [DataRow("rB Symbol", "Symbol")]
        [DataRow("RB Symbol", "Symbol")]
        public void RemoveBreakpointParsingWithSymbolWorks(string source, string symbol)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as RemoveBreakpointToolCommand;
            command.ShouldNotBeNull();
            command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
        }

        [TestMethod]
        [DataRow("ub 1234", 4660)]
        [DataRow("Ub 1234", 4660)]
        [DataRow("uB 1234", 4660)]
        [DataRow("UB 1234", 4660)]
        [DataRow("UB :1234", 1234)]
        [DataRow("UB:1234", 1234)]
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
        [DataRow("ub Symbol", "Symbol")]
        [DataRow("Ub Symbol", "Symbol")]
        [DataRow("uB Symbol", "Symbol")]
        [DataRow("UB Symbol", "Symbol")]
        public void UpdateBreakpointParsingWithSymbolWorks(string source, string symbol)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as UpdateBreakpointToolCommand;
            command.ShouldNotBeNull();
            command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
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
        [DataRow("rl :1234", 1234, "L")]
        [DataRow("rl:1234", 1234, "L")]
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
        [DataRow("rl MySymbol", "MySymbol", "L")]
        [DataRow("rc MySymbol", "MySymbol", "C")]
        [DataRow("rp MySymbol", "MySymbol", "P")]
        public void RetrieveParsingWithSymbolWorks(string source, string symbol, string type)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as RetrieveToolCommand;
            command.ShouldNotBeNull();
            command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
            command.Type.ShouldBe(type);
        }

        [TestMethod]
        [DataRow("d 1234 MySymbol", 4660, "MYSYMBOL", false)]
        [DataRow("D 1234 MySymbol", 4660, "MYSYMBOL", false)]
        [DataRow("d :1234 MySymbol", 1234, "MYSYMBOL", false)]
        [DataRow("D 1234", 4660, null, false)]
        [DataRow("D 1234 #", 4660, null, true)]
        [DataRow("D :1234 #", 1234, null, true)]
        [DataRow("D:1234 #", 1234, null, true)]
        public void LiteralParsingWorks(string source, int address, string literalName, bool isAuto)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as LiteralToolCommand;
            command.ShouldNotBeNull();
            command.Address.ShouldBe((ushort)address);
            if (literalName != null)
            {
                command.LiteralName.ToUpper().ShouldBe(literalName.ToUpper());
            }
            command.IsAuto.ShouldBe(isAuto);
        }

        [TestMethod]
        [DataRow("d Symbol MySymbol", "SYMBOL", "MYSYMBOL", false)]
        [DataRow("D Symbol", "SYMBOL", null, false)]
        [DataRow("D Symbol #", "SYMBOL", null, true)]
        public void LiteralParsingWithSymbolWorks(string source, string symbol, string literalName, bool isAuto)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as LiteralToolCommand;
            command.ShouldNotBeNull();
            command.Symbol.ToUpper().ShouldBe(symbol.ToUpper());
            if (literalName != null)
            {
                command.LiteralName.ToUpper().ShouldBe(literalName.ToUpper());
            }
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
        [DataRow("j :1234", 1234, null)]
        [DataRow("j:1234", 1234, null)]
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

        [TestMethod]
        [DataRow("mb 1234 234C", 4660, 9036, null, null, "B")]
        [DataRow("mB 1234 234C", 4660, 9036, null, null, "B")]
        [DataRow("Mb 1234 234C", 4660, 9036, null, null, "B")]
        [DataRow("MB 1234 234C", 4660, 9036, null, null, "B")]
        [DataRow("mb start 234C", 0, 9036, "START", null, "B")]
        [DataRow("mb 1234 end", 4660, 0, null, "END", "B")]
        [DataRow("mb start end", 0, 0, "START", "END", "B")]
        [DataRow("md 1234 234C", 4660, 9036, null, null, "D")]
        [DataRow("mD 1234 234C", 4660, 9036, null, null, "D")]
        [DataRow("Md 1234 234C", 4660, 9036, null, null, "D")]
        [DataRow("MD 1234 234C", 4660, 9036, null, null, "D")]
        [DataRow("md start 234C", 0, 9036, "START", null, "D")]
        [DataRow("md 1234 end", 4660, 0, null, "END", "D")]
        [DataRow("md start end", 0, 0, "START", "END", "D")]
        [DataRow("mw 1234 234C", 4660, 9036, null, null, "W")]
        [DataRow("mW 1234 234C", 4660, 9036, null, null, "W")]
        [DataRow("Mw 1234 234C", 4660, 9036, null, null, "W")]
        [DataRow("MW 1234 234C", 4660, 9036, null, null, "W")]
        [DataRow("mw start 234C", 0, 9036, "START", null, "W")]
        [DataRow("mw 1234 end", 4660, 0, null, "END", "W")]
        [DataRow("mw start end", 0, 0, "START", "END", "W")]
        [DataRow("ms 1234 234C", 4660, 9036, null, null, "S")]
        [DataRow("mS 1234 234C", 4660, 9036, null, null, "S")]
        [DataRow("Ms 1234 234C", 4660, 9036, null, null, "S")]
        [DataRow("MS 1234 234C", 4660, 9036, null, null, "S")]
        [DataRow("ms start 234C", 0, 9036, "START", null, "S")]
        [DataRow("ms 1234 end", 4660, 0, null, "END", "S")]
        [DataRow("ms start end", 0, 0, "START", "END", "S")]
        [DataRow("mc 1234 234C", 4660, 9036, null, null, "C")]
        [DataRow("mC 1234 234C", 4660, 9036, null, null, "C")]
        [DataRow("Mc 1234 234C", 4660, 9036, null, null, "C")]
        [DataRow("MC 1234 234C", 4660, 9036, null, null, "C")]
        [DataRow("mc start 234C", 0, 9036, "START", null, "C")]
        [DataRow("mc 1234 end", 4660, 0, null, "END", "C")]
        [DataRow("mc start end", 0, 0, "START", "END", "C")]
        [DataRow("mc :1234 end", 1234, 0, null, "END", "C")]
        [DataRow("mc:1234 end", 1234, 0, null, "END", "C")]
        [DataRow("mg 1234 234C", 4660, 9036, null, null, "G")]
        [DataRow("mG 1234 234C", 4660, 9036, null, null, "G")]
        [DataRow("Mg 1234 234C", 4660, 9036, null, null, "G")]
        [DataRow("MG 1234 234C", 4660, 9036, null, null, "G")]
        [DataRow("mg start 234C", 0, 9036, "START", null, "G")]
        [DataRow("mg 1234 end", 4660, 0, null, "END", "G")]
        [DataRow("mg start end", 0, 0, "START", "END", "G")]
        [DataRow("mg1 1234 234C", 4660, 9036, null, null, "G1")]
        [DataRow("mG1 1234 234C", 4660, 9036, null, null, "G1")]
        [DataRow("Mg1 1234 234C", 4660, 9036, null, null, "G1")]
        [DataRow("MG1 1234 234C", 4660, 9036, null, null, "G1")]
        [DataRow("mg1 start 234C", 0, 9036, "START", null, "G1")]
        [DataRow("mg1 1234 end", 4660, 0, null, "END", "G1")]
        [DataRow("mg1 start end", 0, 0, "START", "END", "G1")]
        [DataRow("mg2 1234 234C", 4660, 9036, null, null, "G2")]
        [DataRow("mG2 1234 234C", 4660, 9036, null, null, "G2")]
        [DataRow("Mg2 1234 234C", 4660, 9036, null, null, "G2")]
        [DataRow("MG2 1234 234C", 4660, 9036, null, null, "G2")]
        [DataRow("mg2 start 234C", 0, 9036, "START", null, "G2")]
        [DataRow("mg2 1234 end", 4660, 0, null, "END", "G2")]
        [DataRow("mg2 start end", 0, 0, "START", "END", "G2")]
        [DataRow("mg3 1234 234C", 4660, 9036, null, null, "G3")]
        [DataRow("mG3 1234 234C", 4660, 9036, null, null, "G3")]
        [DataRow("Mg3 1234 234C", 4660, 9036, null, null, "G3")]
        [DataRow("MG3 1234 234C", 4660, 9036, null, null, "G3")]
        [DataRow("mg3 start 234C", 0, 9036, "START", null, "G3")]
        [DataRow("mg3 1234 end", 4660, 0, null, "END", "G3")]
        [DataRow("mg3 start end", 0, 0, "START", "END", "G3")]
        [DataRow("mg4 1234 234C", 4660, 9036, null, null, "G4")]
        [DataRow("mG4 1234 234C", 4660, 9036, null, null, "G4")]
        [DataRow("Mg4 1234 234C", 4660, 9036, null, null, "G4")]
        [DataRow("MG4 1234 234C", 4660, 9036, null, null, "G4")]
        [DataRow("mg4 start 234C", 0, 9036, "START", null, "G4")]
        [DataRow("mg4 1234 end", 4660, 0, null, "END", "G4")]
        [DataRow("mg4 start end", 0, 0, "START", "END", "G4")]
        public void SectionParsingWorks(string source, int startAddress, int endAddress, 
            string startSymbol, string endSymbol, string type)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as SectionToolCommand;
            command.ShouldNotBeNull();
            if (startSymbol != null)
            {
                command.StartSymbol.ToUpper().ShouldBe(startSymbol.ToUpper());
            }
            else
            {
                command.StartAddress.ShouldBe((ushort)startAddress);
            }
            if (endSymbol != null)
            {
                command.EndSymbol.ToUpper().ShouldBe(endSymbol.ToUpper());
            }
            else
            {
                command.EndAddress.ShouldBe((ushort)endAddress);
            }
            command.Type.ShouldBe(type);
        }

        [TestMethod]
        [DataRow("+HL==5", "HL==5")]
        [DataRow("+ HL==5", "HL==5")]
        public void AddWatchParsingWorks(string source, string watch)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as AddWatchToolCommand;
            command.ShouldNotBeNull();
            command.Condition.ShouldBe(watch);
        }

        [TestMethod]
        [DataRow("-1234", 4660)]
        [DataRow("-:1234", 1234)]
        public void RemoveWatchParsingWorks(string source, int index)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as RemoveWatchToolCommand;
            command.ShouldNotBeNull();
            command.Index.ShouldBe((ushort)index);
        }

        [TestMethod]
        [DataRow("* 1234 HL==5", 4660, "HL==5")]
        [DataRow("* :1234 HL==5", 1234, "HL==5")]
        [DataRow("*:1234 HL==5", 1234, "HL==5")]
        public void UpdateWatchParsingWorks(string source, int index, string watch)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as UpdateWatchToolCommand;
            command.ShouldNotBeNull();
            command.Index.ShouldBe((ushort)index);
            command.Condition.ShouldBe(watch);
        }

        [TestMethod]
        [DataRow("lw 1234", 4660)]
        [DataRow("LW :1234", 1234)]
        [DataRow("Lw:1234", 1234)]
        public void LabelWidthParsingWorks(string source, int width)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as LabelWidthToolCommand;
            command.ShouldNotBeNull();
            command.Width.ShouldBe((ushort)width);
        }

        [TestMethod]
        [DataRow("xw 1234 :1234", 4660, 1234)]
        [DataRow("xw:1234 1234", 1234, 4660)]
        public void ExchangeWatchParsingWorks(string source, int from, int to)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as ExchangeWatchToolCommand;
            command.ShouldNotBeNull();
            command.From.ShouldBe((ushort)from);
            command.To.ShouldBe((ushort)to);
        }

        [TestMethod]
        [DataRow("ew")]
        [DataRow("eW")]
        [DataRow("Ew")]
        [DataRow("EW")]
        public void EraseAllWatchParsingWorks(string source)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as EraseAllWatchToolCommand;
            command.ShouldNotBeNull();
        }


        [TestMethod]
        [DataRow("g1234", "G 1234")]
        [DataRow("GY1234", null)]
        [DataRow("R1234", "R 1234")]
        [DataRow("RY1234", null)]
        [DataRow("b1234", "B 1234")]
        [DataRow("bY1234", null)]
        [DataRow("lw1234", "LW 1234")]
        [DataRow("lwY1234", null)]
        [DataRow("l1234", "L 1234")]
        [DataRow("lY1234", null)]
        [DataRow("c1234 comment", "C 1234 comment")]
        [DataRow("CY1234 comment", null)]
        [DataRow("d1234 #", "D 1234 #")]
        [DataRow("DY1234 #", null)]
        [DataRow("p1234 comment", "P 1234 comment")]
        [DataRow("PY1234 comment", null)]
        [DataRow("j1234", "J 1234")]
        [DataRow("JY1234", null)]
        [DataRow("mb1234 2345", "MB 1234 2345")]
        [DataRow("mbY1234 2345", null)]
        [DataRow("mD1234 2345", "MD 1234 2345")]
        [DataRow("mdY1234 2345", null)]
        [DataRow("mw1234 2345", "MW 1234 2345")]
        [DataRow("mwY1234 2345", null)]
        [DataRow("ms1234 2345", "MS 1234 2345")]
        [DataRow("msY1234 2345", null)]
        [DataRow("mc1234 2345", "MC 1234 2345")]
        [DataRow("mCY1234 2345", null)]
        [DataRow("sb1234 H>3 C HL==5", "SB 1234 H>3 C HL==5")]
        [DataRow("sby1234 H>3 C HL==5", null)]
        [DataRow("ub1234", "UB 1234")]
        [DataRow("uby1234", null)]
        [DataRow("tb1234", "TB 1234")]
        [DataRow("tby1234", null)]
        [DataRow("rb1234", "RB 1234")]
        [DataRow("rby1234", null)]
        [DataRow("t48", "T 48")]
        [DataRow("ty48", null)]
        [DataRow("rl1234", "RL 1234")]
        [DataRow("rly1234", null)]
        [DataRow("rc1234", "RC 1234")]
        [DataRow("rcy1234", null)]
        [DataRow("rp1234", "RP 1234")]
        [DataRow("rpy1234", null)]
        [DataRow("rb1234", "RB 1234")]
        [DataRow("rby1234", null)]
        [DataRow("xw1234 :2345", "XW 1234 :2345")]
        [DataRow("xwy1234 :2345", null)]
        public void CompactParsingWorks(string source, string expected)
        {
            // --- Act
            var parsed = ParseCommand(source);

            // --- Assert

            var command = parsed as CompactToolCommand;
            command.ShouldNotBeNull();
            if (expected != null)
            {
                command.CommandText.ShouldBe(expected);
            }
            else
            {
                command.HasSemanticError.ShouldBeTrue();
            }
        }


    }
}
