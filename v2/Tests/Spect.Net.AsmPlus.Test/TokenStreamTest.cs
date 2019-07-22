using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Spect.Net.AsmPlus.Test
{
    [TestClass]
    public class TokenStreamTest
    {
        [TestMethod]
        [DataRow(".IF", new[] { TokenType.IfStmt }, new[] {".IF"})]
        public void ExperimentalWorks(string source, TokenType[] tokens, string[] texts)
        {
            // --- Arrange
            var ts = new TokenStream(source);

            // --- Act/Assert
            for (var i = 0; i < tokens.Length; i++)
            {
                var next = ts.GetNext(out var tokenText);
                next.ShouldBe(tokens[i]);
                tokenText.ShouldBe(texts[i]);
            }
        }

        [TestMethod]
        [DataRow(":", new [] {TokenType.Colon, TokenType.Eof}, new[] {":", ""})]
        [DataRow("::", new[] { TokenType.Dcolon, TokenType.Eof }, new[] { "::", "" })]
        [DataRow(":=", new[] { TokenType.VarPrag, TokenType.Eof }, new[] { ":=", "" })]
        [DataRow("/", new[] { TokenType.DivOp, TokenType.Eof }, new[] { "/", "" })]
        [DataRow(",", new[] { TokenType.Comma, TokenType.Eof }, new[] { ",", "" })]
        [DataRow("=", new[] { TokenType.Assign, TokenType.Eof }, new[] { "=", "" })]
        [DataRow("==", new[] { TokenType.EqOp, TokenType.Eof }, new[] { "==", "" })]
        [DataRow("===", new[] { TokenType.CiEqOp, TokenType.Eof }, new[] { "===", "" })]
        [DataRow("(", new[] { TokenType.Lpar, TokenType.Eof }, new[] { "(", "" })]
        [DataRow(")", new[] { TokenType.Rpar, TokenType.Eof }, new[] { ")", "" })]
        [DataRow("[", new[] { TokenType.Lsbrac, TokenType.Eof }, new[] { "[", "" })]
        [DataRow("]", new[] { TokenType.Rsbrac, TokenType.Eof }, new[] { "]", "" })]
        [DataRow("?", new[] { TokenType.Qmark, TokenType.Eof }, new[] { "?", "" })]
        [DataRow("+", new[] { TokenType.Plus, TokenType.Eof }, new[] { "+", "" })]
        [DataRow("-", new[] { TokenType.Minus, TokenType.Eof }, new[] { "-", "" })]
        [DataRow("->", new[] { TokenType.GoesTo, TokenType.Eof }, new[] { "->", "" })]
        [DataRow("|", new[] { TokenType.VBar, TokenType.Eof }, new[] { "|", "" })]
        [DataRow("^", new[] { TokenType.UpArr, TokenType.Eof }, new[] { "^", "" })]
        [DataRow("&", new[] { TokenType.Amp, TokenType.Eof }, new[] { "&", "" })]
        [DataRow("!", new[] { TokenType.Exclm, TokenType.Eof }, new[] { "!", "" })]
        [DataRow("!=", new[] { TokenType.NeqOp, TokenType.Eof }, new[] { "!=", "" })]
        [DataRow("!==", new[] { TokenType.CiNeqOp, TokenType.Eof }, new[] { "!==", "" })]
        [DataRow("<", new[] { TokenType.LtOp, TokenType.Eof }, new[] { "<", "" })]
        [DataRow("<=", new[] { TokenType.LteOp, TokenType.Eof }, new[] { "<=", "" })]
        [DataRow("<<", new[] { TokenType.LshOp, TokenType.Eof }, new[] { "<<", "" })]
        [DataRow("<?", new[] { TokenType.MinOp, TokenType.Eof }, new[] { "<?", "" })]
        [DataRow(">", new[] { TokenType.GtOp, TokenType.Eof }, new[] { ">", "" })]
        [DataRow(">=", new[] { TokenType.GteOp, TokenType.Eof }, new[] { ">=", "" })]
        [DataRow(">>", new[] { TokenType.RshOp, TokenType.Eof }, new[] { ">>", "" })]
        [DataRow(">?", new[] { TokenType.MaxOp, TokenType.Eof }, new[] { ">?", "" })]
        [DataRow("*", new[] { TokenType.MulOp, TokenType.Eof }, new[] { "*", "" })]
        [DataRow("~", new[] { TokenType.Tilde, TokenType.Eof }, new[] { "~", "" })]
        [DataRow("%", new[] { TokenType.ModOp, TokenType.Eof }, new[] { "%", "" })]
        [DataRow("{{", new[] { TokenType.LdBrac, TokenType.Eof }, new[] { "{{", "" })]
        [DataRow("}}", new[] { TokenType.RdBrac, TokenType.Eof }, new[] { "}}", "" })]
        [DataRow("{-", new[] { TokenType.Error }, new[] { "{" })]
        [DataRow("}-", new[] { TokenType.Error }, new[] { "}" })]
        [DataRow(".", new[] { TokenType.Dot, TokenType.Eof }, new[] { ".", "" })]
        public void SimpleTokensWork(string source, TokenType[] tokens, string[] texts)
        {
            // --- Arrange
            var ts = new TokenStream(source);

            // --- Act/Assert
            for (var i = 0; i < tokens.Length; i++)
            {
                var next = ts.GetNext(out var tokenText);
                next.ShouldBe(tokens[i]);
                tokenText.ShouldBe(texts[i]);
            }
        }

        [TestMethod]
        [DataRow(":::", new[] { TokenType.Dcolon, TokenType.Colon, TokenType.Eof }, new[] { "::", ":", "" })]
        [DataRow(":/", new[] { TokenType.Colon, TokenType.DivOp, TokenType.Eof }, new[] { ":", "/", "" })]
        [DataRow("/::", new[] { TokenType.DivOp, TokenType.Dcolon, TokenType.Eof }, new[] { "/", "::", "" })]
        [DataRow(",,", new[] { TokenType.Comma, TokenType.Comma, TokenType.Eof }, new[] { ",", ",", "" })]
        [DataRow("=,", new[] { TokenType.Assign, TokenType.Comma, TokenType.Eof }, new[] { "=", ",", "" })]
        [DataRow("==,", new[] { TokenType.EqOp, TokenType.Comma, TokenType.Eof }, new[] { "==", ",", "" })]
        [DataRow("===,", new[] { TokenType.CiEqOp, TokenType.Comma, TokenType.Eof }, new[] { "===", ",", "" })]
        [DataRow("=(", new[] { TokenType.Assign, TokenType.Lpar, TokenType.Eof }, new[] { "=", "(", "" })]
        [DataRow("==)", new[] { TokenType.EqOp, TokenType.Rpar, TokenType.Eof }, new[] { "==", ")", "" })]
        [DataRow("(/", new[] { TokenType.Lpar, TokenType.DivOp, TokenType.Eof }, new[] { "(", "/", "" })]
        [DataRow(")::", new[] { TokenType.Rpar, TokenType.Dcolon, TokenType.Eof }, new[] { ")", "::", "" })]
        [DataRow("[/", new[] { TokenType.Lsbrac, TokenType.DivOp, TokenType.Eof }, new[] { "[", "/", "" })]
        [DataRow("]::", new[] { TokenType.Rsbrac, TokenType.Dcolon, TokenType.Eof }, new[] { "]", "::", "" })]
        [DataRow(":?", new[] { TokenType.Colon, TokenType.Qmark, TokenType.Eof }, new[] { ":", "?", "" })]
        [DataRow("+?", new[] { TokenType.Plus, TokenType.Qmark, TokenType.Eof }, new[] { "+", "?", "" })]
        [DataRow("-?", new[] { TokenType.Minus, TokenType.Qmark, TokenType.Eof }, new[] { "-", "?", "" })]
        [DataRow("->?", new[] { TokenType.GoesTo, TokenType.Qmark, TokenType.Eof }, new[] { "->", "?", "" })]
        [DataRow("[|", new[] { TokenType.Lsbrac, TokenType.VBar, TokenType.Eof }, new[] { "[", "|", "" })]
        [DataRow("^|", new[] { TokenType.UpArr, TokenType.VBar, TokenType.Eof }, new[] { "^", "|", "" })]
        [DataRow("^&", new[] { TokenType.UpArr, TokenType.Amp, TokenType.Eof }, new[] { "^", "&", "" })]
        [DataRow("!,", new[] { TokenType.Exclm, TokenType.Comma, TokenType.Eof }, new[] { "!", ",", "" })]
        [DataRow("!=,", new[] { TokenType.NeqOp, TokenType.Comma, TokenType.Eof }, new[] { "!=", ",", "" })]
        [DataRow("!==,", new[] { TokenType.CiNeqOp, TokenType.Comma, TokenType.Eof }, new[] { "!==", ",", "" })]
        [DataRow("<,", new[] { TokenType.LtOp, TokenType.Comma, TokenType.Eof }, new[] { "<", ",", "" })]
        [DataRow("<=,", new[] { TokenType.LteOp, TokenType.Comma, TokenType.Eof }, new[] { "<=", ",", "" })]
        [DataRow("<<,", new[] { TokenType.LshOp, TokenType.Comma, TokenType.Eof }, new[] { "<<", ",", "" })]
        [DataRow("<?,", new[] { TokenType.MinOp, TokenType.Comma, TokenType.Eof }, new[] { "<?", ",", "" })]
        [DataRow(">,", new[] { TokenType.GtOp, TokenType.Comma, TokenType.Eof }, new[] { ">", ",", "" })]
        [DataRow(">=,", new[] { TokenType.GteOp, TokenType.Comma, TokenType.Eof }, new[] { ">=", ",", "" })]
        [DataRow(">>,", new[] { TokenType.RshOp, TokenType.Comma, TokenType.Eof }, new[] { ">>", ",", "" })]
        [DataRow(">?,", new[] { TokenType.MaxOp, TokenType.Comma, TokenType.Eof }, new[] { ">?", ",", "" })]
        [DataRow(":*", new[] { TokenType.Colon, TokenType.MulOp, TokenType.Eof }, new[] { ":", "*", "" })]
        [DataRow("~*", new[] { TokenType.Tilde, TokenType.MulOp, TokenType.Eof }, new[] { "~", "*", "" })]
        [DataRow("~%", new[] { TokenType.Tilde, TokenType.ModOp, TokenType.Eof }, new[] { "~", "%", "" })]
        [DataRow("{{%", new[] { TokenType.LdBrac, TokenType.ModOp, TokenType.Eof }, new[] { "{{", "%", "" })]
        [DataRow("}}%", new[] { TokenType.RdBrac, TokenType.ModOp, TokenType.Eof }, new[] { "}}", "%", "" })]
        [DataRow("}}.", new[] { TokenType.RdBrac, TokenType.Dot, TokenType.Eof }, new[] { "}}", ".", "" })]
        public void SimpleTokenCombinationsWork(string source, TokenType[] tokens, string[] texts)
        {
            // --- Arrange
            var ts = new TokenStream(source);

            // --- Act/Assert
            for (var i = 0; i < tokens.Length; i++)
            {
                var next = ts.GetNext(out var tokenText);
                next.ShouldBe(tokens[i]);
                tokenText.ShouldBe(texts[i]);
            }
        }

        [TestMethod]
        [DataRow("; Comm", new[] { TokenType.Comment, TokenType.Eof }, new[] { "; Comm", "" })]
        [DataRow("; Comm\r", new[] { TokenType.Comment }, new[] { "; Comm" })]
        [DataRow("; Comm\n", new[] { TokenType.Comment }, new[] { "; Comm" })]
        [DataRow(":; Comm\n", new[] { TokenType.Colon, TokenType.Comment }, new[] { ":", "; Comm" })]
        public void EolCommentWorks(string source, TokenType[] tokens, string[] texts)
        {
            // --- Arrange
            var ts = new TokenStream(source);

            // --- Act/Assert
            for (var i = 0; i < tokens.Length; i++)
            {
                var next = ts.GetNext(out var tokenText);
                next.ShouldBe(tokens[i]);
                tokenText.ShouldBe(texts[i]);
            }
        }

        [TestMethod]
        [DataRow("/* Block */", new[] { TokenType.Comment, TokenType.Eof }, new[] { "/* Block */", "" })]
        [DataRow("/* Block ***/", new[] { TokenType.Comment, TokenType.Eof }, new[] { "/* Block ***/", "" })]
        [DataRow("/* Block \r", new[] { TokenType.Error}, new[] { "/* Block "})]
        [DataRow("/* Block \n", new[] { TokenType.Error }, new[] { "/* Block " })]
        [DataRow("/* Block *\r", new[] { TokenType.Error }, new[] { "/* Block *" })]
        [DataRow("/* Block *\n", new[] { TokenType.Error }, new[] { "/* Block *" })]
        public void BlockCommentWorks(string source, TokenType[] tokens, string[] texts)
        {
            // --- Arrange
            var ts = new TokenStream(source);

            // --- Act/Assert
            for (var i = 0; i < tokens.Length; i++)
            {
                var next = ts.GetNext(out var tokenText);
                next.ShouldBe(tokens[i]);
                tokenText.ShouldBe(texts[i]);
            }
        }

        [TestMethod]
        [DataRow("nop", TokenType.Nop)]
        [DataRow("NOP", TokenType.Nop)]
        [DataRow("rlca", TokenType.Rlca)]
        [DataRow("RLCA", TokenType.Rlca)]
        [DataRow("rrca", TokenType.Rrca)]
        [DataRow("RRCA", TokenType.Rrca)]
        [DataRow("rla", TokenType.Rla)]
        [DataRow("RLA", TokenType.Rla)]
        [DataRow("rra", TokenType.Rra)]
        [DataRow("RRA", TokenType.Rra)]
        [DataRow("daa", TokenType.Daa)]
        [DataRow("DAA", TokenType.Daa)]
        [DataRow("cpl", TokenType.Cpl)]
        [DataRow("CPL", TokenType.Cpl)]
        [DataRow("scf", TokenType.Scf)]
        [DataRow("SCF", TokenType.Scf)]
        [DataRow("ccf", TokenType.Ccf)]
        [DataRow("CCF", TokenType.Ccf)]
        [DataRow("halt", TokenType.Halt)]
        [DataRow("HALT", TokenType.Halt)]
        [DataRow("exx", TokenType.Exx)]
        [DataRow("EXX", TokenType.Exx)]
        [DataRow("di", TokenType.Di)]
        [DataRow("DI", TokenType.Di)]
        [DataRow("ei", TokenType.Ei)]
        [DataRow("EI", TokenType.Ei)]
        [DataRow("neg", TokenType.Neg)]
        [DataRow("NEG", TokenType.Neg)]
        [DataRow("retn", TokenType.Retn)]
        [DataRow("RETN", TokenType.Retn)]
        [DataRow("reti", TokenType.Reti)]
        [DataRow("RETI", TokenType.Reti)]
        [DataRow("rld", TokenType.Rld)]
        [DataRow("RLD", TokenType.Rld)]
        [DataRow("rrd", TokenType.Rrd)]
        [DataRow("RRD", TokenType.Rrd)]
        [DataRow("ldi", TokenType.Ldi)]
        [DataRow("LDI", TokenType.Ldi)]
        [DataRow("cpi", TokenType.Cpi)]
        [DataRow("CPI", TokenType.Cpi)]
        [DataRow("ini", TokenType.Ini)]
        [DataRow("INI", TokenType.Ini)]
        [DataRow("outi", TokenType.Outi)]
        [DataRow("OUTI", TokenType.Outi)]
        [DataRow("ldd", TokenType.Ldd)]
        [DataRow("LDD", TokenType.Ldd)]
        [DataRow("cpd", TokenType.Cpd)]
        [DataRow("CPD", TokenType.Cpd)]
        [DataRow("ind", TokenType.Ind)]
        [DataRow("IND", TokenType.Ind)]
        [DataRow("outd", TokenType.Outd)]
        [DataRow("OUTD", TokenType.Outd)]
        [DataRow("ldir", TokenType.Ldir)]
        [DataRow("LDIR", TokenType.Ldir)]
        [DataRow("cpir", TokenType.Cpir)]
        [DataRow("CPIR", TokenType.Cpir)]
        [DataRow("inir", TokenType.Inir)]
        [DataRow("INIR", TokenType.Inir)]
        [DataRow("otir", TokenType.Otir)]
        [DataRow("OTIR", TokenType.Otir)]
        [DataRow("lddr", TokenType.Lddr)]
        [DataRow("LDDR", TokenType.Lddr)]
        [DataRow("cpdr", TokenType.Cpdr)]
        [DataRow("CPDR", TokenType.Cpdr)]
        [DataRow("indr", TokenType.Indr)]
        [DataRow("INDR", TokenType.Indr)]
        [DataRow("otdr", TokenType.Otdr)]
        [DataRow("OTDR", TokenType.Otdr)]
        [DataRow("ld", TokenType.Ld)]
        [DataRow("LD", TokenType.Ld)]
        [DataRow("inc", TokenType.Inc)]
        [DataRow("INC", TokenType.Inc)]
        [DataRow("dec", TokenType.Dec)]
        [DataRow("DEC", TokenType.Dec)]
        [DataRow("ex", TokenType.Ex)]
        [DataRow("EX", TokenType.Ex)]
        [DataRow("add", TokenType.Add)]
        [DataRow("ADD", TokenType.Add)]
        [DataRow("adc", TokenType.Adc)]
        [DataRow("ADC", TokenType.Adc)]
        [DataRow("sub", TokenType.Sub)]
        [DataRow("SUB", TokenType.Sub)]
        [DataRow("sbc", TokenType.Sbc)]
        [DataRow("SBC", TokenType.Sbc)]
        [DataRow("and", TokenType.And)]
        [DataRow("AND", TokenType.And)]
        [DataRow("xor", TokenType.Xor)]
        [DataRow("XOR", TokenType.Xor)]
        [DataRow("or", TokenType.Or)]
        [DataRow("OR", TokenType.Or)]
        [DataRow("cp", TokenType.Cp)]
        [DataRow("CP", TokenType.Cp)]
        [DataRow("djnz", TokenType.Djnz)]
        [DataRow("DJNZ", TokenType.Djnz)]
        [DataRow("jr", TokenType.Jr)]
        [DataRow("JR", TokenType.Jr)]
        [DataRow("jp", TokenType.Jp)]
        [DataRow("JP", TokenType.Jp)]
        [DataRow("call", TokenType.Call)]
        [DataRow("CALL", TokenType.Call)]
        [DataRow("ret", TokenType.Ret)]
        [DataRow("RET", TokenType.Ret)]
        [DataRow("rst", TokenType.Rst)]
        [DataRow("RST", TokenType.Rst)]
        [DataRow("push", TokenType.Push)]
        [DataRow("PUSH", TokenType.Push)]
        [DataRow("pop", TokenType.Pop)]
        [DataRow("POP", TokenType.Pop)]
        [DataRow("in", TokenType.In)]
        [DataRow("IN", TokenType.In)]
        [DataRow("out", TokenType.Out)]
        [DataRow("OUT", TokenType.Out)]
        [DataRow("im", TokenType.Im)]
        [DataRow("IM", TokenType.Im)]
        [DataRow("rlc", TokenType.Rlc)]
        [DataRow("RLC", TokenType.Rlc)]
        [DataRow("rrc", TokenType.Rrc)]
        [DataRow("RRC", TokenType.Rrc)]
        [DataRow("rl", TokenType.Rl)]
        [DataRow("RL", TokenType.Rl)]
        [DataRow("rr", TokenType.Rr)]
        [DataRow("RR", TokenType.Rr)]
        [DataRow("sla", TokenType.Sla)]
        [DataRow("SLA", TokenType.Sla)]
        [DataRow("sra", TokenType.Sra)]
        [DataRow("SRA", TokenType.Sra)]
        [DataRow("sll", TokenType.Sll)]
        [DataRow("SLL", TokenType.Sll)]
        [DataRow("srl", TokenType.Srl)]
        [DataRow("SRL", TokenType.Srl)]
        [DataRow("bit", TokenType.Bit)]
        [DataRow("BIT", TokenType.Bit)]
        [DataRow("res", TokenType.Res)]
        [DataRow("RES", TokenType.Res)]
        [DataRow("set", TokenType.Set)]
        [DataRow("SET", TokenType.Set)]
        [DataRow("mirror", TokenType.Mirror)]
        [DataRow("MIRROR", TokenType.Mirror)]
        [DataRow("test", TokenType.Test)]
        [DataRow("TEST", TokenType.Test)]
        [DataRow("nextreg", TokenType.NextReg)]
        [DataRow("NEXTREG", TokenType.NextReg)]
        [DataRow("swapnib", TokenType.SwapNib)]
        [DataRow("SWAPNIB", TokenType.SwapNib)]
        [DataRow("mul", TokenType.Mul)]
        [DataRow("MUL", TokenType.Mul)]
        [DataRow("outinb", TokenType.Outinb)]
        [DataRow("OUTINB", TokenType.Outinb)]
        [DataRow("ldix", TokenType.Ldix)]
        [DataRow("LDIX", TokenType.Ldix)]
        [DataRow("ldirx", TokenType.Ldirx)]
        [DataRow("LDIRX", TokenType.Ldirx)]
        [DataRow("lddx", TokenType.Lddx)]
        [DataRow("LDDX", TokenType.Lddx)]
        [DataRow("lddrx", TokenType.Lddrx)]
        [DataRow("LDDRX", TokenType.Lddrx)]
        [DataRow("pixeldn", TokenType.Pixeldn)]
        [DataRow("PIXELDN", TokenType.Pixeldn)]
        [DataRow("pixelad", TokenType.Pixelad)]
        [DataRow("PIXELAD", TokenType.Pixelad)]
        [DataRow("setae", TokenType.Setae)]
        [DataRow("SETAE", TokenType.Setae)]
        [DataRow("ldpirx", TokenType.Ldpirx)]
        [DataRow("LDPIRX", TokenType.Ldpirx)]
        [DataRow("ldirscale", TokenType.Ldirscale)]
        [DataRow("LDIRSCALE", TokenType.Ldirscale)]
        public void MnemonicsWork(string source, TokenType token)
        {
            // --- Arrange
            var ts = new TokenStream(source);

            // --- Act/Assert
            var next = ts.GetNext(out var tokenText);
            next.ShouldBe(token);
            tokenText.ShouldBe(source);
            next = ts.GetNext(out _);
            next.ShouldBe(TokenType.Eof);
        }

        [TestMethod]
        [DataRow("a", TokenType.A)]
        [DataRow("A", TokenType.A)]
        [DataRow("b", TokenType.B)]
        [DataRow("B", TokenType.B)]
        [DataRow("c", TokenType.C)]
        [DataRow("C", TokenType.C)]
        [DataRow("d", TokenType.D)]
        [DataRow("D", TokenType.D)]
        [DataRow("e", TokenType.E)]
        [DataRow("E", TokenType.E)]
        [DataRow("h", TokenType.H)]
        [DataRow("H", TokenType.H)]
        [DataRow("l", TokenType.L)]
        [DataRow("L", TokenType.L)]
        [DataRow("i", TokenType.I)]
        [DataRow("I", TokenType.I)]
        [DataRow("r", TokenType.R)]
        [DataRow("R", TokenType.R)]
        [DataRow("xl", TokenType.Xl)]
        [DataRow("XL", TokenType.Xl)]
        [DataRow("xh", TokenType.Xh)]
        [DataRow("XH", TokenType.Xh)]
        [DataRow("yl", TokenType.Yl)]
        [DataRow("YL", TokenType.Yl)]
        [DataRow("yh", TokenType.Yh)]
        [DataRow("YH", TokenType.Yh)]
        [DataRow("ixl", TokenType.Xl)]
        [DataRow("IXL", TokenType.Xl)]
        [DataRow("IXl", TokenType.Xl)]
        [DataRow("ixh", TokenType.Xh)]
        [DataRow("IXH", TokenType.Xh)]
        [DataRow("IXh", TokenType.Xh)]
        [DataRow("iyl", TokenType.Yl)]
        [DataRow("IYL", TokenType.Yl)]
        [DataRow("IYl", TokenType.Yl)]
        [DataRow("iyh", TokenType.Yh)]
        [DataRow("IYH", TokenType.Yh)]
        [DataRow("IYh", TokenType.Yh)]
        [DataRow("bc", TokenType.Bc)]
        [DataRow("BC", TokenType.Bc)]
        [DataRow("de", TokenType.De)]
        [DataRow("DE", TokenType.De)]
        [DataRow("hl", TokenType.Hl)]
        [DataRow("HL", TokenType.Hl)]
        [DataRow("sp", TokenType.Sp)]
        [DataRow("SP", TokenType.Sp)]
        [DataRow("ix", TokenType.Ix)]
        [DataRow("IX", TokenType.Ix)]
        [DataRow("iy", TokenType.Iy)]
        [DataRow("IY", TokenType.Iy)]
        [DataRow("af", TokenType.Af)]
        [DataRow("AF", TokenType.Af)]
        [DataRow("af'", TokenType.Af_)]
        [DataRow("AF'", TokenType.Af_)]
        [DataRow("z", TokenType.Z)]
        [DataRow("Z", TokenType.Z)]
        [DataRow("nz", TokenType.Nz)]
        [DataRow("NZ", TokenType.Nz)]
        [DataRow("nc", TokenType.Nc)]
        [DataRow("NC", TokenType.Nc)]
        [DataRow("po", TokenType.Po)]
        [DataRow("PO", TokenType.Po)]
        [DataRow("pe", TokenType.Pe)]
        [DataRow("PE", TokenType.Pe)]
        [DataRow("p", TokenType.P)]
        [DataRow("P", TokenType.P)]
        [DataRow("m", TokenType.M)]
        [DataRow("M", TokenType.M)]
        public void RegsWork(string source, TokenType token)
        {
            // --- Arrange
            var ts = new TokenStream(source);

            // --- Act/Assert
            var next = ts.GetNext(out var tokenText);
            next.ShouldBe(token);
            tokenText.ShouldBe(source);
            next = ts.GetNext(out _);
            next.ShouldBe(TokenType.Eof);
        }

        [TestMethod]
        [DataRow("macro", TokenType.Macro)]
        [DataRow("MACRO", TokenType.Macro)]
        [DataRow(".macro", TokenType.Macro)]
        [DataRow(".MACRO", TokenType.Macro)]
        [DataRow("endm", TokenType.EndMacro)]
        [DataRow("ENDM", TokenType.EndMacro)]
        [DataRow(".endm", TokenType.EndMacro)]
        [DataRow(".ENDM", TokenType.EndMacro)]
        [DataRow(".proc", TokenType.Proc)]
        [DataRow(".PROC", TokenType.Proc)]
        [DataRow(".endp", TokenType.EndProc)]
        [DataRow(".ENDP", TokenType.EndProc)]
        [DataRow(".pend", TokenType.EndProc)]
        [DataRow(".PEND", TokenType.EndProc)]
        [DataRow(".loop", TokenType.Loop)]
        [DataRow(".LOOP", TokenType.Loop)]
        [DataRow(".endl", TokenType.EndLoop)]
        [DataRow(".ENDL", TokenType.EndLoop)]
        [DataRow(".lend", TokenType.EndLoop)]
        [DataRow(".LEND", TokenType.EndLoop)]
        [DataRow(".repeat", TokenType.Repeat)]
        [DataRow(".REPEAT", TokenType.Repeat)]
        [DataRow(".until", TokenType.Until)]
        [DataRow(".UNTIL", TokenType.Until)]
        [DataRow(".while", TokenType.While)]
        [DataRow(".WHILE", TokenType.While)]
        [DataRow(".endw", TokenType.EndWhile)]
        [DataRow(".ENDW", TokenType.EndWhile)]
        [DataRow(".wend", TokenType.EndWhile)]
        [DataRow(".WEND", TokenType.EndWhile)]
        [DataRow("if", TokenType.IfStmt)]
        [DataRow("IF", TokenType.IfStmt)]
        [DataRow(".if", TokenType.IfStmt)]
        [DataRow(".IF", TokenType.IfStmt)]
        [DataRow(".ifused", TokenType.IfUsed)]
        [DataRow(".IFUSED", TokenType.IfUsed)]
        [DataRow(".ifnused", TokenType.IfNused)]
        [DataRow(".IFNUSED", TokenType.IfNused)]
        [DataRow(".elif", TokenType.Elif)]
        [DataRow(".ELIF", TokenType.Elif)]
        [DataRow(".else", TokenType.ElseStmt)]
        [DataRow(".ELSE", TokenType.ElseStmt)]
        [DataRow(".endif", TokenType.EndIfStmt)]
        [DataRow(".ENDIF", TokenType.EndIfStmt)]
        [DataRow(".for", TokenType.For)]
        [DataRow(".FOR", TokenType.For)]
        [DataRow("for", TokenType.For)]
        [DataRow("FOR", TokenType.For)]
        [DataRow(".to", TokenType.To)]
        [DataRow(".TO", TokenType.To)]
        [DataRow("to", TokenType.To)]
        [DataRow("TO", TokenType.To)]
        [DataRow(".step", TokenType.Step)]
        [DataRow(".STEP", TokenType.Step)]
        [DataRow("step", TokenType.Step)]
        [DataRow("STEP", TokenType.Step)]
        [DataRow(".next", TokenType.ForNext)]
        [DataRow(".NEXT", TokenType.ForNext)]
        [DataRow("next", TokenType.ForNext)]
        [DataRow("NEXT", TokenType.ForNext)]
        [DataRow(".break", TokenType.Break)]
        [DataRow(".BREAK", TokenType.Break)]
        [DataRow("break", TokenType.Break)]
        [DataRow("BREAK", TokenType.Break)]
        [DataRow(".continue", TokenType.Continue)]
        [DataRow(".CONTINUE", TokenType.Continue)]
        [DataRow("continue", TokenType.Continue)]
        [DataRow("CONTINUE", TokenType.Continue)]
        public void StatementsWork(string source, TokenType token)
        {
            // --- Arrange
            var ts = new TokenStream(source);

            // --- Act/Assert
            var next = ts.GetNext(out var tokenText);
            next.ShouldBe(token);
            tokenText.ShouldBe(source);
            next = ts.GetNext(out _);
            next.ShouldBe(TokenType.Eof);
        }

    }
}
