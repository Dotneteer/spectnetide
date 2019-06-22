using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.EvalParser.SyntaxTree;

namespace Spect.Net.EvalParser.Test.Parser
{
    [TestClass]
    public class Z80SpecificParserTest: ParserTestBed
    {
        [TestMethod]
        [DataRow("a", "A")]
        [DataRow("A", "A")]
        [DataRow("b", "B")]
        [DataRow("B", "B")]
        [DataRow("c", "C")]
        [DataRow("C", "C")]
        [DataRow("d", "D")]
        [DataRow("D", "D")]
        [DataRow("e", "E")]
        [DataRow("E", "E")]
        [DataRow("h", "H")]
        [DataRow("H", "H")]
        [DataRow("l", "L")]
        [DataRow("L", "L")]
        [DataRow("f", "F")]
        [DataRow("F", "F")]
        [DataRow("i", "I")]
        [DataRow("I", "I")]
        [DataRow("r", "R")]
        [DataRow("R", "R")]
        [DataRow("xl", "XL")]
        [DataRow("XL", "XL")]
        [DataRow("ixl", "IXL")]
        [DataRow("IXL", "IXL")]
        [DataRow("xh", "XH")]
        [DataRow("XH", "XH")]
        [DataRow("ixh", "IXH")]
        [DataRow("IXH", "IXH")]
        [DataRow("yl", "YL")]
        [DataRow("YL", "YL")]
        [DataRow("iyl", "IYL")]
        [DataRow("IYL", "IYL")]
        [DataRow("yh", "YH")]
        [DataRow("YH", "YH")]
        [DataRow("iyh", "IYH")]
        [DataRow("IYH", "IYH")]
        public void Register8ParsingWorks(string reg, string expected)
        {
            // --- Act
            var expr = ParseExpr(reg);

            // --- Assert
            var literal = expr as Z80RegisterNode;
            literal.ShouldNotBeNull();
            literal.Register.ShouldBe(expected);
        }

        [TestMethod]
        [DataRow("af", "AF")]
        [DataRow("AF", "AF")]
        [DataRow("bc", "BC")]
        [DataRow("BC", "BC")]
        [DataRow("de", "DE")]
        [DataRow("DE", "DE")]
        [DataRow("hl", "HL")]
        [DataRow("HL", "HL")]
        [DataRow("af'", "AF'")]
        [DataRow("AF'", "AF'")]
        [DataRow("bc'", "BC'")]
        [DataRow("BC'", "BC'")]
        [DataRow("de'", "DE'")]
        [DataRow("DE'", "DE'")]
        [DataRow("hl'", "HL'")]
        [DataRow("HL'", "HL'")]
        [DataRow("ix", "IX")]
        [DataRow("IX", "IX")]
        [DataRow("iy", "IY")]
        [DataRow("IY", "IY")]
        [DataRow("sp", "SP")]
        [DataRow("SP", "SP")]
        [DataRow("pc", "PC")]
        [DataRow("PC", "PC")]
        [DataRow("wz", "WZ")]
        [DataRow("WZ", "WZ")]
        public void Register16ParsingWorks(string reg, string expected)
        {
            // --- Act
            var expr = ParseExpr(reg);

            // --- Assert
            var literal = expr as Z80RegisterNode;
            literal.ShouldNotBeNull();
            literal.Register.ShouldBe(expected);
        }

        [TestMethod]
        [DataRow("`z", "`Z")]
        [DataRow("`Z", "`Z")]
        [DataRow("`nz", "`NZ")]
        [DataRow("`NZ", "`NZ")]
        [DataRow("`c", "`C")]
        [DataRow("`C", "`C")]
        [DataRow("`nc", "`NC")]
        [DataRow("`NC", "`NC")]
        [DataRow("`po", "`PO")]
        [DataRow("`PO", "`PO")]
        [DataRow("`pe", "`PE")]
        [DataRow("`pe", "`PE")]
        [DataRow("`p", "`P")]
        [DataRow("`P", "`P")]
        [DataRow("`m", "`M")]
        [DataRow("`M", "`M")]
        [DataRow("`h", "`H")]
        [DataRow("`H", "`H")]
        [DataRow("`nh", "`NH")]
        [DataRow("`NH", "`NH")]
        [DataRow("`n", "`N")]
        [DataRow("`N", "`N")]
        [DataRow("`nn", "`NN")]
        [DataRow("`NN", "`NN")]
        [DataRow("`3", "`3")]
        [DataRow("`n3", "`N3")]
        [DataRow("`N3", "`N3")]
        [DataRow("`5", "`5")]
        [DataRow("`n5", "`N5")]
        [DataRow("`N5", "`N5")]
        public void FlagParsingWorks(string flag, string expected)
        {
            // --- Act
            var expr = ParseExpr(flag);

            // --- Assert
            var literal = expr as Z80FlagNode;
            literal.ShouldNotBeNull();
            literal.Flag.ShouldBe(expected);
        }

    }
}
